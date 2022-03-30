-- Section 1. DDL
	-- 1. Database design
CREATE DATABASE [Service]
USE [Service]
GO

CREATE TABLE Users
(
	Id INT PRIMARY KEY IDENTITY,
	Username NVARCHAR(30) UNIQUE NOT NULL,
	[Password] NVARCHAR(50) NOT NULL,	
	[Name] NVARCHAR(50),
	Birthdate DATETIME2,
	Age INT CHECK(Age BETWEEN 14 AND 110),
	Email NVARCHAR(50) NOT NULL
)
CREATE TABLE Departments
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
)
CREATE TABLE Employees
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(25),
	LastName NVARCHAR(25),
	Birthdate DATETIME2,
	Age INT CHECK(Age BETWEEN 18 AND 110),
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id)
)
CREATE TABLE Categories
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL,
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id) NOT NULL
)
CREATE TABLE [Status]
(
	Id INT PRIMARY KEY IDENTITY,
	[Label] NVARCHAR(30) NOT NULL
)
CREATE TABLE Reports
(
	Id INT PRIMARY KEY IDENTITY,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	StatusId INT FOREIGN KEY REFERENCES [Status](Id) NOT NULL,
	OpenDate DATETIME2 NOT NULL,
	CloseDate DATETIME2,
	[Description] NVARCHAR(200) NOT NULL,
	UserId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id)
)
-- Section 2. DML
	-- 2. Insert
INSERT INTO Employees(FirstName,LastName,Birthdate,DepartmentId)
VALUES ('Marlo', 'O''Malley', '1958-9-21', 1),
	   ('Niki', 'Stanaghan', '1969-11-26', 4),
	   ('Ayrton', 'Senna', '1960-03-21', 9),
	   ('Ronnie', 'Peterson', '1944-02-14', 9),
	   ('Giovanna', 'Amati', '1959-07-20', 5)

INSERT INTO Reports(CategoryId, StatusId, OpenDate, CloseDate, [Description], UserId, EmployeeId)
VALUES (1, 1, '2017-04-13', NULL, 'Stuck Road on Str.133', 6, 2),
	   (6, 3, '2015-09-05', '2015-12-06', 'Charity trail running', 3, 5),
	   (14, 2, '2015-09-07', NULL, 'Falling bricks on Str.58', 5, 2),
	   (4, 3, '2017-07-03', '2017-07-06', 'Cut off streetlight on Str.11', 1, 1)

	-- 3. Update
UPDATE Reports
SET CloseDate = GETDATE()
WHERE CloseDate IS NULL

	-- 4. Delete
DELETE Reports
WHERE StatusId = 4

DELETE [Status]
WHERE Id = 4

	-- 5. Unassigned Reports
SELECT [Description],
	   FORMAT(OpenDate, 'dd-MM-yyyy') AS [OpenDate]
 FROM Reports
 WHERE EmployeeId IS NULL
 ORDER BY YEAR(OpenDate), MONTH(OpenDate), DAY(OpenDate)

	-- 6. Reports & Categories
SELECT [Description],
	   c.[Name] AS [CategoryName]
  FROM Reports AS r
  JOIN Categories AS c ON c.Id = r.CategoryId
  WHERE [Description] IS NOT NULL
  ORDER BY [Description], CategoryName

	-- 7. Most Reported Category
SELECT TOP(5) c.[Name] AS [CategoryName],
	   COUNT(r.Id) AS [ReportsNumber]
  FROM Categories AS c
  JOIN Reports AS r ON r.CategoryId = c.Id
  GROUP BY c.[Name]
  ORDER BY ReportsNumber DESC, CategoryName

	-- 8. Birthday Report
SELECT u.Username,
	   c.[Name] AS [CategoryName]	
  FROM Users AS u
  JOIN Reports AS r ON r.UserId = u.Id
  JOIN Categories AS c ON c.Id = r.CategoryId
  WHERE DAY(r.OpenDate) = DAY(u.Birthdate) AND MONTH(r.OpenDate) = MONTH(u.Birthdate)
  ORDER BY u.Username, CategoryName	

	-- 9. Users per Employee 
SELECT CONCAT(e.FirstName, ' ', e.LastName) AS [FullName],
	   COUNT(r.UserId) AS [UsersCount]
  FROM Employees AS e
		LEFT JOIN Reports AS r ON r.EmployeeId = e.Id
		LEFT JOIN Users AS u ON u.Id = r.UserId
  GROUP BY e.FirstName, e.LastName
  ORDER BY UsersCount DESC, FullName

	-- 10. Full Info 
SELECT CASE WHEN COALESCE(e.FirstName,e.LastName) IS NOT NULL
THEN CONCAT(e.FirstName,' ',e.LastName)
ELSE 'None'
END AS  [Employee],
       ISNULL(d.[Name], 'None') AS [Department],
	   ISNULL(c.[Name], 'None') AS [Category],
	   ISNULL(r.[Description], 'None') AS [Description],
	   ISNULL(FORMAT(r.OpenDate,'dd.MM.yyyy'), 'None') AS [OpenDate],
	   ISNULL(s.[Label], 'None') AS [Status],
	   ISNULL(u.[Name], 'None') AS [User]
  FROM Reports AS r
	   LEFT JOIN Employees AS e ON e.Id = r.EmployeeId 
	   LEFT JOIN Departments AS d ON d.Id = e.DepartmentId
	   LEFT JOIN Categories AS c ON c.Id = r.CategoryId
	   LEFT JOIN Users AS u ON u.Id = r.UserId
	   LEFT JOIN [Status] AS s ON s.Id = r.StatusId

  ORDER BY e.FirstName DESC, e.LastName DESC, 
		   d.[Name], c.[Name], r.[Description], r.OpenDate, s.[Label], u.[Name]

-- Section 4. Programmability
	-- 11. Hours to Complete
CREATE FUNCTION udf_HoursToComplete(@StartDate DATETIME, @EndDate DATETIME) 
RETURNS INT
AS 
BEGIN
	DECLARE @totalHours INT;
		IF(@StartDate IS NULL)
		BEGIN
			RETURN 0;
		END
		IF(@EndDate IS NULL)
		BEGIN
			RETURN 0;
		END
	SET @totalHours = DATEDIFF(HOUR,@StartDate,@EndDate)
	RETURN @totalHours;
END
GO

SELECT dbo.udf_HoursToComplete(OpenDate, CloseDate) AS TotalHours
   FROM Reports

	-- 12. Assign Employee
CREATE PROCEDURE usp_AssignEmployeeToReport(@EmployeeId INT, @ReportId INT)
AS
BEGIN
		DECLARE @departmentEmployee INT = (SELECT DepartmentId
											FROM Employees
											WHERE Id = @EmployeeId);
		DECLARE @departmentReport INT = (SELECT c.DepartmentId
										   FROM Reports AS r
										   JOIN Categories AS c ON c.Id = r.CategoryId
										   WHERE r.Id = @ReportId);
		IF(@departmentEmployee <> @departmentReport)
		BEGIN
			THROW 50001, 'Employee doesn''t belong to the appropriate department!',1;
		END

		UPDATE Reports
		SET Reports.EmployeeId=@EmployeeId
		WHERE Reports.Id = @ReportId
END
GO

EXEC usp_AssignEmployeeToReport 30, 1
EXEC usp_AssignEmployeeToReport 17, 2