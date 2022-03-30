-- Problem 2.Find All Department Info
SELECT * 
FROM Departments

-- Problem 3.Find All Department Names
SELECT Name
FROM Departments

-- Problem 4.Find Salary of Each Employee
SELECT FirstName, LastName, Salary
FROM Employees

-- Problem 5.Find Full Name of Each Employee
SELECT FirstName, MiddleName,LastName
FROM Employees

-- Problem 6.Find Email Addresses of Each Employee
SELECT CONCAT(FirstName,'.',LastName,'@softuni.bg')
	AS [Full Email Address] 
	FROM Employees

-- Problem 7.Find All Different Employee's Salaries
SELECT DISTINCT Salary
FROM Employees

-- Problem 8.Find All Info About Employees
SELECT *
FROM Employees
WHERE JobTitle = 'Sales Representative'

-- Problem 9.Find Names of All Employees by Salary in Range
SELECT FirstName,LastName,JobTitle 
FROM Employees
WHERE Salary BETWEEN 20000 AND 30000

-- Problem 10.Find Names of All Employees
SELECT CONCAT(FirstName,' ',MiddleName,' ',LastName) 
	AS [Full Name]
	FROM Employees
	WHERE Salary IN (25000, 14000, 12500,23600)

-- Problem 11.Find All Employees Without Manager
SELECT FirstName, LastName
FROM Employees
WHERE ManagerID IS NULL

-- Problem 12.Find All Employees with Salary More Than
SELECT FirstName, LastName, Salary
FROM Employees
WHERE Salary > 50000
ORDER BY Salary DESC

-- Problem 13.Find 5 Best Paid Employees
SELECT TOP(5) FirstName, LastName
FROM Employees
ORDER BY Salary DESC

-- Problem 14.Find All Employees Except Marketing
SELECT FirstName, LastName
FROM Employees
WHERE DepartmentID <> 4

-- Problem 15.Sort Employees Table
SELECT *
FROM Employees
ORDER BY Salary DESC,FirstName,LastName DESC,MiddleName
GO

-- Problem 16.Create View Employees with Salaries
CREATE VIEW V_EmployeesSalaries AS
SELECT FirstName, LastName, Salary
FROM Employees
GO

SELECT * FROM V_EmployeesSalaries
GO

-- Problem 17.Create View Employees with Job Title
CREATE VIEW V_EmployeeNameJobTitle AS
	SELECT FirstName + ' ' + ISNULL(MiddleName,'') + ' ' + LastName AS 
		[Full Name], JobTitle
		FROM Employees
GO

-- Problem 18.Distinct Job Titles
SELECT DISTINCT JobTitle 
FROM Employees

-- Problem 19.Find First 10 Started Projects
SELECT TOP(10)*
FROM Projects
ORDER BY StartDate, [Name]

-- Problem 20.Last 7 Hired Employees
SELECT TOP(7) FirstName,LastName,HireDate 
	FROM Employees
	ORDER BY HireDate DESC

-- Problem 21.Increase Salaries
UPDATE Employees 
SET Salary *= 1.12
WHERE DepartmentID IN (1,2,4,11)

SELECT Salary
FROM Employees

-- Problem 22.All Mountain Peaks
USE Geography

SELECT PeakName
	FROM Peaks
	ORDER BY PeakName

-- Problem 23.Biggest Countries in Europe
SELECT TOP(30) CountryName,Population
	FROM Countries
	WHERE ContinentCode = 'EU'
	ORDER BY Population DESC, CountryName 

-- Problems 24.Countries and Currencies
SELECT * 
FROM Currencies
GO

SELECT CountryName,CountryCode,
	CASE
		WHEN CurrencyCode = 'EUR' THEN 'Euro'
ELSE 'Not Euro'
 	END AS Currency
	FROM Countries
	ORDER BY CountryName

-- Problem 25.All Diablo Characters
USE Diablo

SELECT [Name]
	FROM Characters
	ORDER BY [Name]