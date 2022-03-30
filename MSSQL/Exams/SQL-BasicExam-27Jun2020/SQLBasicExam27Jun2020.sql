-- Section 1. DDL
	-- 1. Database design
CREATE DATABASE WMS
USE WMS
GO

CREATE TABLE Clients
(
	ClientId INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Phone CHAR(12) CHECK(LEN(Phone) = 12) NOT NULL
)
CREATE TABLE Mechanics
(
	MechanicId INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	[Address] VARCHAR(255) NOT NULL
)
CREATE TABLE Models
( 
	ModelId INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) UNIQUE NOT NULL
)
CREATE TABLE Jobs
(
	JobId INT PRIMARY KEY IDENTITY NOT NULL,
	ModelId INT FOREIGN KEY REFERENCES Models(ModelId) NOT NULL,
	[Status] VARCHAR(11) CHECK([Status] IN('Pending', 'In Progress', 'Finished')) DEFAULT 'Pending' NOT NULL,
	ClientId INT FOREIGN KEY REFERENCES Clients(ClientId) NOT NULL,
	MechanicId INT FOREIGN KEY REFERENCES Mechanics(MechanicId),
	IssueDate DATE  NOT NULL,
	FinishDate DATE 
)
CREATE TABLE Orders
(
	OrderId INT	PRIMARY KEY IDENTITY NOT NULL,
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId) NOT NULL,
	IssueDate DATE,
	Delivered BIT DEFAULT 'False' NOT NULL
)
CREATE TABLE Vendors
(
	VendorId INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) UNIQUE NOT NULL
)
CREATE TABLE Parts
(
	PartId INT PRIMARY KEY IDENTITY NOT NULL,
	SerialNumber VARCHAR(50) UNIQUE NOT NULL,
	[Description] VARCHAR(255),
	Price DECIMAL (6,2) CHECK(Price > 0) NOT NULL,
	VendorId INT FOREIGN KEY REFERENCES Vendors(VendorId) NOT NULL,
	StockQty INT DEFAULT 0 CHECK(StockQty >= 0) NOT NULL
)
CREATE TABLE OrderParts
(
	OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
	PartId INT FOREIGN KEY REFERENCES Parts(PartId),
	Quantity INT DEFAULT 1 CHECK(Quantity > 0),
	PRIMARY KEY (OrderId, PartId)
)
CREATE TABLE PartsNeeded
(
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId),
	PartId INT FOREIGN KEY REFERENCES Parts(PartId),
	Quantity INT DEFAULT 1 CHECK(Quantity > 0),
	PRIMARY KEY (JobId, PartId)
)

-- Section 2. DML
	-- 2. Insert
INSERT INTO Clients(FirstName, LastName, Phone)
VALUES ('Teri', 'Ennaco', '570-889-5187'),
	   ('Merlyn', 'Lawler', '201-588-7810'),
	   ('Georgene', 'Montezuma', '925-615-5185'),
	   ('Jettie', 'Mconnell', '908-802-3564'),
	   ('Lemuel', 'Latzke', '631-748-6479'),
	   ('Melodie', 'Knipp', '805-690-1682'),
	   ('Candida', 'Corbley', '908-275-8357')

INSERT INTO Parts(SerialNumber, [Description], Price, VendorId)
VALUES ('WP8182119', 'Door Boot Seal', 117.86, 2),
	   ('W10780048', 'Suspension Rod', 42.81, 1),
	   ('W10841140', 'Silicone Adhesive', 6.77, 4),
	   ('WPY055980', 'High Temperature Adhesive', 13.94, 3)

	-- 3. Update
UPDATE Jobs
SET [Status] = 'In Progress', MechanicId = 3
WHERE [Status] = 'Pending' 
	-- 4. Delete
DELETE OrderParts
WHERE OrderId = 19

DELETE Orders
WHERE OrderId = 19

--Section 3. Querying 
	-- 5. Mechanic Assignments
SELECT m.FirstName + ' ' + m.LastName AS [Mechanic],
	   j.[Status],
	   j.IssueDate 
  FROM Mechanics AS m
  JOIN Jobs AS j ON j.MechanicId = m.MechanicId
  ORDER BY m.MechanicId, j.IssueDate, j.JobId
	-- 6. Current Clients
SELECT c.FirstName + ' ' + c.LastName AS [Client],
	   DATEDIFF(DAY, j.IssueDate,'2017-04-24') AS [Days going],
	   j.[Status]
  FROM Clients AS c
  JOIN Jobs AS j ON j.ClientId = c.ClientId
  WHERE j.[Status] != 'Finished'
  ORDER BY [Days going] DESC, c.ClientId
	-- 7. Mechanic Performance
SELECT m.FirstName + ' ' + m.LastName AS [Mechanic],
	   AVG(DATEDIFF(DAY, j.IssueDate, j.FinishDate)) AS [Average Days]
  FROM Mechanics AS m
  JOIN Jobs AS j ON j.MechanicId = m.MechanicId
  GROUP BY m.FirstName, m.LastName, m.MechanicId
  ORDER BY m.MechanicId

	-- 8. Available Mechanics
SELECT t.Available
FROM (SELECT m.FirstName + ' ' + m.LastName AS [Available],
		   j.MechanicId
      FROM Mechanics AS m
      LEFT JOIN Jobs AS j ON j.MechanicId = m.MechanicId
      WHERE j.[Status] != 'In Progress'
      GROUP BY j.MechanicId,m.FirstName, m.LastName
	  ) AS t 
ORDER BY t.MechanicId

	-- 9. Past Expenses
SELECT j.JobId,
	   ISNULL(SUM(p.Price * op.Quantity), 0.00) AS [Total]
  FROM Jobs AS j 
       LEFT JOIN Orders AS o ON o.JobId = j.JobId
       LEFT JOIN OrderParts AS op ON op.OrderId = o.OrderId
       LEFT JOIN Parts AS p ON p.PartId = op.PartId
  WHERE j.[Status] LIKE 'Finished'
  GROUP BY j.JobId
  ORDER BY Total DESC, j.JobId 

	-- 10. Missing Parts
SELECT p.PartId,
	   p.[Description],
	   ISNULL(pn.Quantity,0) AS [Required],
	   ISNULL(p.StockQty,0) AS [In Stock],
	   ISNULL(CASE
				WHEN o.Delivered = 0
				THEN op.Quantity
				ELSE 0
			  END,0 ) AS [Ordered]
  FROM Parts AS p
  JOIN PartsNeeded as pn ON pn.PartId = p.PartId
  LEFT JOIN OrderParts AS op ON op.PartId = p.PartId
  JOIN Jobs AS j ON j.JobId = pn.JobId
  LEFT JOIN Orders AS o ON o.JobId = j.JobId
  WHERE j.[Status] != 'Finished' 
  AND  pn.Quantity > p.StockQty + ISNULL(CASE
				WHEN o.Delivered = 0
				THEN op.Quantity
				ELSE 0
			  END,0 )
ORDER BY p.PartId

-- Section 4. Programmability
	-- 11. Place Order
CREATE PROCEDURE usp_PlaceOrder
(
                 @jobId            INT,
                 @partSerialNumber VARCHAR(50),
                 @quantity         INT
)
AS
     BEGIN
	    -- Params validation
         IF(@quantity <= 0)
             BEGIN;
                 THROW 50012, 'Part quantity must be more than zero!', 1;
         END;
         IF(
           (
               SELECT JobId  
               FROM Jobs
               WHERE JobId = @jobId
           ) IS NULL)
             BEGIN;
                 THROW 50013, 'Job not found!', 1;
         END;
         IF(
           (
               SELECT Status
               FROM Jobs
               WHERE JobId = @jobId
           ) = 'Finished')
             BEGIN;
                 THROW 50011, 'This job is not active!', 1;
         END;
         IF(
           (
               SELECT SerialNumber
               FROM Parts
               WHERE SerialNumber = @partSerialNumber
           ) IS NULL)
             BEGIN;
                 THROW 50014, 'Part not found!', 1;
         END;

	    -- Create Order if not exists
         IF(
           (
               SELECT JobId
               FROM Orders
               WHERE JobId = @jobId
                     AND IssueDate IS NULL
           ) IS NULL)
             BEGIN
                 INSERT INTO Orders(JobId,
                                    IssueDate,
                                    Delivered
                                   )
                 VALUES
                 (
                        @jobId,
                        NULL,
                        0
                 );
         END;

	    -- Add part to order if not exists
         DECLARE @orderId INT=
         (
             SELECT TOP 1 OrderId
             FROM Orders
             WHERE JobId = @jobId
                   AND IssueDate IS NULL
         );
         DECLARE @partId INT=
         (
             SELECT PartId
             FROM Parts
             WHERE SerialNumber = @partSerialNumber
         );
         IF(
           (
               SELECT PartId
               FROM OrderParts
               WHERE PartId = @partId
                     AND OrderId = @orderId
           ) IS NULL)
             BEGIN
                 INSERT INTO OrderParts(OrderId,
                                        PartId,
                                        Quantity
                                       )
                 VALUES
                 (
                        @orderId,
                        @partId,
                        @quantity
                 );
         END;
	    -- Part exists in the order - Add quantity
             ELSE
             BEGIN
                 UPDATE OrderParts
                   SET
                       Quantity+=@quantity
                 WHERE PartId = @partId
                       AND OrderId = @orderId;
         END;
     END;

DECLARE @err_msg AS NVARCHAR(MAX);
BEGIN TRY
    EXEC usp_PlaceOrder 1, 'ZeroQuantity', 0
END TRY
BEGIN CATCH
    SET @err_msg = ERROR_MESSAGE();
    SELECT @err_msg
END CATCH

	-- 12. Cost Of Order
CREATE FUNCTION udf_GetCost(@jobId INT)
RETURNS DECIMAL(18,2)
AS
	BEGIN
		DECLARE @totalSum DECIMAL(18,2) = (SELECT SUM(p.Price)
											 FROM Orders AS o
											 JOIN OrderParts AS op ON op.OrderId = o.OrderId
											 JOIN Parts AS p ON p.PartId = op.PartId
											 WHERE o.JobId = @jobId
											 GROUP BY o.JobId)
			IF(@totalSum IS NULL)
				BEGIN
					RETURN 0;
				END

				RETURN @totalSum;
	END

	SELECT dbo.udf_GetCost(1)