-- Problem 1. Records' Count
SELECT COUNT(*) AS [Count]
  FROM WizzardDeposits

-- Problem 2. Longest Magic Wand
SELECT MAX(MagicWandSize) AS [LongestMagicWand]
  FROM WizzardDeposits

-- Problem 3. Longest Magic Wand Per DepositGroups
SELECT DepositGroup,
		MAX(MagicWandSize) AS [LongestMagicWand]		
  FROM WizzardDeposits
  GROUP BY DepositGroup

-- Problem 4. Smallest Deposit Group per Magic Wand Size
SELECT TOP(2) DepositGroup
  FROM WizzardDeposits
  GROUP BY DepositGroup
  ORDER BY AVG(MagicWandSize)	

-- Problem 5. DepositsSum
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum]
  FROM WizzardDeposits
	GROUP BY DepositGroup

-- Problem 6. DepositsSum for Ollivander Family
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum]
  FROM WizzardDeposits
  WHERE MagicWandCreator = 'Ollivander family'
	GROUP BY DepositGroup

-- Problem 7. DepositsFilter
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum]
  FROM WizzardDeposits
  WHERE MagicWandCreator = 'Ollivander family'
	GROUP BY DepositGroup
	HAVING SUM(DepositAmount) < 150000
	ORDER BY TotalSum DESC

-- Problem 8. Deposit Charge
SELECT DepositGroup, MagicWandCreator, MIN(DepositCharge) AS [MinDepositCharge]
  FROM WizzardDeposits
	GROUP BY DepositGroup, MagicWandCreator
	ORDER BY MagicWandCreator, DepositGroup

-- Problem 9. Age Groups
SELECT AgeRanking AS AgeGroup, COUNT(*) AS WizardCount
FROM (
		SELECT CASE
					WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
					WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
					WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
					WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
					WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
					WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
					ELSE '[61+]'	
					END AS AgeRanking
		FROM WizzardDeposits) AS AgeRankingQuery
GROUP BY AgeRanking

-- Problem 10. FirstLetter
SELECT FirstLetter
FROM   (
		SELECT SUBSTRING(FirstName,1,1) AS FirstLetter
		FROM WizzardDeposits
		WHERE DepositGroup LIKE 'Troll Chest') AS FirstCharQuery
GROUP BY FirstLetter
ORDER BY FirstLetter

-- Problem 11. Average Interest
SELECT DepositGroup, IsDepositExpired,AVG(DepositInterest) AS AverageInterest
  FROM WizzardDeposits
 WHERE DepositStartDate > '1985-01-01'  
 GROUP BY DepositGroup, IsDepositExpired
 ORDER BY DepositGroup DESC, IsDepositExpired

 -- Problem 12. Rich Wizard, Poor Wizard
SELECT SUM([Difference]) AS SumDifference
 FROM(
		SELECT FirstName                                                AS HostWizard,
			   DepositAmount                                            AS HostWizardDeposit,
			   LEAD(FirstName) OVER (ORDER BY Id)                       AS GuestWizard,
			   LEAD(DepositAmount) OVER (ORDER BY Id)                   AS GuestDeposit,
			   (DepositAmount - LEAD(DepositAmount) OVER (ORDER BY Id)) AS [Difference]
	FROM WizzardDeposits) AS DifferenceAmountQuery

-- Problem 13. Departments Total Salaries
USE SoftUni

SELECT DepartmentID, SUM(Salary) AS TotalSalary 
  FROM Employees
  GROUP BY DepartmentID
  ORDER BY DepartmentID

-- Problem 14. Employees Minimum Salaries
SELECT DepartmentID, MIN(Salary) AS MinimumSalary
  FROM Employees
  WHERE HireDate > '2000-01-01' AND DepartmentID IN (2,5,7)
  GROUP BY DepartmentID

-- Problem 15. Employees Average Salaries
SELECT * INTO HIGHESTPAIDEMPLOYEES
FROM Employees
WHERE Salary > 30000

DELETE 
FROM HIGHESTPAIDEMPLOYEES
WHERE ManagerID = 42

UPDATE HIGHESTPAIDEMPLOYEES
SET Salary += 5000
WHERE DepartmentID = 1

SELECT DepartmentID, AVG(Salary) AS AverageSalary
FROM HIGHESTPAIDEMPLOYEES
GROUP BY DepartmentID

-- Problem 16. Employees Maximum Salaries
SELECT DepartmentID, MAX(Salary) AS MaxSalary
  FROM Employees
  GROUP BY DepartmentID
  HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000

-- Problem 17. Employees Count Salaries
SELECT COUNT(Salary) AS Count
  FROM Employees
  WHERE ManagerID IS NULL

-- Problem 18. 3rd Highest Salary
SELECT DepartmentID, MaxSalary
FROM (SELECT e.DepartmentID,
            MAX(e.Salary) AS MaxSalary,
             DENSE_RANK() OVER (PARTITION BY DepartmentID ORDER BY e.Salary DESC)
                 AS SalaryRank
      FROM Employees as e
      GROUP BY e.DepartmentID, e.Salary) AS SalaryRankingQuery
WHERE SalaryRankingQuery.SalaryRank = 3

-- Problem 19. Salary Challenge
SELECT TOP(10) E.FirstName, E.LastName,E.DepartmentID
  FROM Employees AS E
  WHERE E.Salary > 
		(
			SELECT AVG(Salary) AS AverageSalary
			FROM Employees AS EAvarageSalary
			WHERE EAvarageSalary.DepartmentID = E.DepartmentID
			GROUP BY DepartmentID
		)
ORDER BY DepartmentID