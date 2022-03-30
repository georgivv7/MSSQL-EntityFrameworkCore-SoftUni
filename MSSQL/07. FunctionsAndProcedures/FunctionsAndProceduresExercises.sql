--Problem 1. Employees with Salary Above 35000
CREATE PROCEDURE usp_GetEmployeesSalaryAbove35000(@minSalary DECIMAL = 35000)
AS
	SELECT e.FirstName, e.LastName
	  FROM Employees AS e
	 WHERE e.Salary > @minSalary
GO

EXEC usp_GetEmployeesSalaryAbove35000

--Problem 2. Employees with Salary Above Number
CREATE PROCEDURE usp_GetEmployeesSalaryAboveNumber (@minSalary DECIMAL(18,4))
AS
	SELECT e.FirstName, e.LastName
	  FROM Employees AS e
	 WHERE e.Salary >= @minSalary
GO

EXEC usp_GetEmployeesSalaryAboveNumber 48100

--Problem 3. Town Names Starting With
CREATE PROC usp_GetTownsStartingWith(@firstTownCharacter NVARCHAR(MAX))
AS
	SELECT t.[Name] AS [Town]
	  FROM Towns AS t
	  WHERE t.NamE LIKE @firstTownCharacter + '%';
GO

EXEC usp_GetTownsStartingWith 'b'

-- Problem 4. Employees from Town
CREATE PROC usp_GetEmployeesFromTown (@townName NVARCHAR(MAX))
AS
	SELECT e.FirstName, e.LastName
	  FROM Employees AS e
	  JOIN Addresses AS a ON a.AddressID = e.AddressID
	  JOIN Towns AS t ON t.TownID = a.TownID
	 WHERE t.Name = @townName
GO

EXEC usp_GetEmployeesFromTown 'Sofia'

-- Problem 5. Salary Level Function
CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS VARCHAR(10)
AS
BEGIN
	DECLARE @level VARCHAR(10);
	IF(@salary < 30000)
		SET @level = 'Low';
	ELSE IF(@salary >= 30000 AND @salary <= 50000)
		SET @level = 'Average';
	ELSE
		SET @level = 'High'
RETURN @level;	
END
GO
SELECT e.Salary, 
	   [dbo].ufn_GetSalaryLevel(e.Salary) AS [Salary Level]
  FROM Employees AS e

-- Problem 6. Employees by Salary Level
CREATE PROC usp_EmployeesBySalaryLevel (@salaryLevel VARCHAR(10))
AS
	SELECT FirstName AS [First Name], 
		   LastName  AS [Last Name]
	  FROM Employees
	  WHERE @salaryLevel = dbo.ufn_GetSalaryLevel(Salary);
GO

EXEC usp_EmployeesBySalaryLevel 'Low'

--Problem 7. Define Function
CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(MAX), @word VARCHAR(MAX))
RETURNS BIT
 BEGIN
 DECLARE @index INT = 1;
 DECLARE @currentChar CHAR(1);
 DECLARE @result INT;
	WHILE(@index <= LEN(@word))
	BEGIN
		SET @currentChar = SUBSTRING(@word,@index,1)
		SET @result = CHARINDEX(@currentChar,@setOfLetters)
		IF(@result = 0)
		BEGIN
		 RETURN 0;
		END
		SET @index += 1;
	END
	RETURN 1;
 END

 SELECT dbo.ufn_IsWordComprised ('oistmiahf','Sofia')


--Problem 8. * Delete Employees and Departments
CREATE PROC usp_DeleteEmployeesFromDepartment(@departmentId INT)
AS
	BEGIN
		DELETE 
		  FROM EmployeesProjects
		  WHERE EmployeeID IN(SELECT EmployeeID
								FROM Employees
								WHERE DepartmentID = @departmentId)
		UPDATE Employees
		SET ManagerID = NULL
		WHERE ManagerID IN (SELECT EmployeeID
							  FROM Employees
							  WHERE DepartmentID = @departmentId)
		
		ALTER TABLE Departments
		ALTER COLUMN ManagerID INT;

		UPDATE Departments
		SET ManagerID = NULL
		WHERE ManagerID IN (SELECT EmployeeID
							  FROM Employees
							  WHERE DepartmentID = @departmentId)
		
		DELETE FROM Employees
		WHERE DepartmentID = @departmentId

		DELETE FROM Departments
		WHERE DepartmentID = @departmentId

		SELECT COUNT(*)
		  FROM Employees
		  WHERE DepartmentID = @departmentId
	END
	
--Problem 9. Find Full Name
USE Bank1

CREATE PROC usp_GetHoldersFullName
AS
	SELECT CONCAT(FirstName,' ',LastName) AS [Full Name]
	  FROM AccountHolders

EXEC usp_GetHoldersFullName

--Problem 10. People with Balance Higher Than
CREATE PROC usp_GetHoldersWithBalanceHigherThan(@balance DECIMAL(18,4))
AS
   SELECT FirstName as [First Name], 
   		  LastName as [Last Name]
     FROM Accounts AS a
     JOIN AccountHolders as ah ON ah.Id = a.AccountHolderId
     GROUP BY FirstName, LastName
    HAVING SUM(a.Balance) > @balance
    ORDER BY FirstName, LastName
GO
	
EXEC usp_GetHoldersWithBalanceHigherThan 300000

--Problem 11. Future Value Function
CREATE FUNCTION ufn_CalculateFutureValue (@sum DECIMAL(18,4),@yearlyInterestRate FLOAT, @years INT)
RETURNS DECIMAL(18,4)
AS
 BEGIN
	DECLARE @result DECIMAL(18,4);
	SET @result = @sum * (POWER	(1+@yearlyInterestRate,@years));
	RETURN @result;
 END

SELECT dbo.ufn_CalculateFutureValue(1000,0.1,5)

--Problem 12. Calculating Interest
CREATE PROC usp_CalculateFutureValueForAccount @accountID INT, @interestRate FLOAT
AS
	DECLARE @years INT = 5;
	SELECT a.Id AS [Account Id],
		   ah.FirstName AS [First Name],
		   ah.LastName AS [Last Name],
		   a.Balance AS [Current Balance],
		   dbo.ufn_CalculateFutureValue(a.Balance, @interestRate, @years) AS [Balance in 5 years]
	  FROM Accounts AS a
	  JOIN AccountHolders AS ah 
	  ON ah.Id = a.AccountHolderId
	  WHERE a.Id = @accountID

EXEC usp_CalculateFutureValueForAccount 1,0.1

-- Problem 13. *Scalar Function: Cash in User Games Odd Rows
CREATE FUNCTION ufn_CashInUsersGames (@gameName VARCHAR(20))
RETURNS TABLE
AS
	RETURN SELECT SUM(Cash) AS SumCash
					FROM (
							 SELECT g.Name,
                                    ug.Cash,
                                    ROW_NUMBER() OVER (PARTITION BY g.Name ORDER BY ug.Cash DESC) AS RowNum
                                   FROM Games AS g
                                   JOIN UsersGames AS ug
                                                 ON g.Id = ug.GameId
                                   WHERE g.Name = @GameName
                               )
                                   AS RowNumQuery
                          WHERE RowNum % 2 <> 0
			 
SELECT *
FROM ufn_CashInUsersGames ('Lily Stargazer')

-- Problem 14. Create Table Logs
CREATE TABLE Logs(
	LogID INT IDENTITY,
	AccountID INT,
	OldSum DECIMAL(15,2),
	NewSum DECIMAL(15,2)
)

CREATE TRIGGER tr_Accounts
ON Accounts
FOR UPDATE
AS
	INSERT INTO Logs(AccountID,OldSum,NewSum)	
	SELECT i.Id,d.Balance,i.Balance
	  FROM inserted AS i
		   JOIN deleted AS d ON d.Id = d.Id
	 WHERE i.Balance != d.Balance
	
-- Problem 15. Create Table Emails
CREATE TABLE NotificationEmails
(
	ID INT PRIMARY KEY IDENTITY,
	Recipient INT REFERENCES Accounts(Id),
	[Subject] NVARCHAR(MAX),
	Body NVARCHAR(MAX)
)

CREATE TRIGGER tr_AddToNotificationEmailOnLogsUpdate
    ON Logs
    FOR INSERT
    AS
    INSERT INTO NotificationEmails(Recipient, Subject, Body)
    SELECT i.AccountID,
           'Balance change for account: ' + CAST(i.AccountID AS nvarchar(20)),
           'On ' + CONVERT(nvarchar(20), GETDATE(), 100) + ' your balance was changed from ' +
           CAST(i.OldSum AS nvarchar(20)) + ' to ' + CAST(i.NewSum AS nvarchar(20)) + '.'
    FROM inserted AS i
exec dbo.usp_TransferMoney 12, 15,1000

SELECT * from Logs
SELECT *
FROM NotificationEmails;

--Problem 16. Deposit Money
CREATE PROC usp_DepositMoney @accountID INT, @moneyAmount DECIMAL(18,4)
AS
	BEGIN
		BEGIN TRAN
		UPDATE Accounts SET Balance = Balance + @moneyAmount
		WHERE Id = @accountID
		IF @moneyAmount < 0
		BEGIN
			ROLLBACK;
			RAISERROR('Amount cannot be negative!',16,1);
			RETURN
		END
		COMMIT
	END

EXEC usp_DepositMoney 1,10
SELECT Id AS [AccountId],
	   AccountHolderId,
	   Balance
  FROM Accounts

-- Problem 17. Withdraw Money
CREATE PROC usp_WithdrawMoney @accountID INT, @moneyAmount DECIMAL(18,4)
AS
	BEGIN
		BEGIN TRAN
		UPDATE Accounts SET Balance = Balance - @moneyAmount;
		IF @moneyAmount < 0
		BEGIN
			ROLLBACK;
			RAISERROR('Amount cannot be negative!',16,1);
			RETURN;
		END
		COMMIT
	END

EXEC usp_WithdrawMoney 5,25

SELECT Id AS [AccountId],
	   AccountHolderId,
	   Balance
  FROM Accounts
  WHERE Id = 5

-- Problem 18. Money Transfer
CREATE PROC usp_TransferMoney @senderID INT, @recieverID INT, @amount DECIMAL(18,4)
AS
	BEGIN
		BEGIN TRAN
		UPDATE Accounts SET Balance = Balance - @amount
		WHERE Id = @senderID
		UPDATE Accounts SET Balance = Balance + @amount
		WHERE Id = @recieverID
		IF @amount < 0
		 BEGIN
			ROLLBACK;
			RAISERROR('Amount cannot be negative!',16,1);
			RETURN;
		 END	
		COMMIT;
	END

EXEC usp_TransferMoney 5,1,5000
-- Problem 20. *Massive Shopping


-- Problem 21. Employees with Three Projects
CREATE PROC usp_AssignProject(@emloyeeId INT, @projectID INT)
AS
	DECLARE @maxAssignedProjects INT = 3;
	DECLARE @projectsCount INT;
	SET @projectsCount = 
	(	
		SELECT COUNT(*)
		  FROM [dbo].[EmployeesProjects] as ep
		  WHERE ep.EmployeeID = @emloyeeId
	) 
	BEGIN TRAN
	 INSERT INTO [dbo].[EmployeesProjects] 
	 (EmployeeID,ProjectID)
	 VALUES (@emloyeeId,@projectID)

	 IF @projectsCount >= @maxAssignedProjects 
	  BEGIN
		ROLLBACK;
		RAISERROR('The employee has too many projects!', 16, 1)
		RETURN;
	  END
	COMMIT;

-- Problem 22. Delete Employees
CREATE TABLE Deleted_Employees(
	EmployeeID INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	MiddleName NVARCHAR(30),
	JobTitle NVARCHAR(30) NOT NULL,
	DepartmentID INT,
	Salary DECIMAL(7,2)
)
CREATE TRIGGER tr_InsertEmployeesOnDelete
ON Employees
AFTER DELETE
AS
	BEGIN
		INSERT INTO Deleted_Employees
		SELECT d.FirstName, d.LastName,d.MiddleName,d.JobTitle,d.DepartmentID,d.Salary
		  FROM deleted as d
 	END

	