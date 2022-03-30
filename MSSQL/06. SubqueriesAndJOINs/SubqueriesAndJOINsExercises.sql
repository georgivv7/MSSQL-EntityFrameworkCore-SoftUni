-- Problem 1. Employee Address
SELECT TOP(5) e.EmployeeID, e.JobTitle, a.AddressID, a.AddressText
FROM Employees AS e
JOIN Addresses AS a
ON a.AddressID = e.AddressID
ORDER BY a.AddressID

-- Problem 2. Addresses with Towns
SELECT TOP(50) e.FirstName, e.LastName, t.[Name], a.AddressText 
  FROM Employees AS e
  JOIN Addresses AS a 
    ON a.AddressID = e.AddressID
  JOIN Towns AS t
    ON t.TownID = a.TownID
ORDER BY e.FirstName, e.LastName

-- Problem 3. Sales Employees
SELECT emp.EmployeeID, emp.FirstName, emp.LastName, dep.[Name] AS DepartmentName
  FROM Employees AS emp
  JOIN Departments AS dep 
    ON (dep.DepartmentID = emp.DepartmentID AND dep.Name = 'Sales')
ORDER BY emp.EmployeeID  

-- Problem 4. Employee Departments  
SELECT TOP(5) emp.EmployeeID, emp.FirstName,emp.Salary, dep.[Name] AS DepartmentName
  FROM Employees AS emp
  JOIN Departments AS dep 
    ON (dep.DepartmentID = emp.DepartmentID AND emp.Salary > 15000)
ORDER BY dep.DepartmentID 

-- Problem 5. Employees Without Projects
SELECT TOP(3) e.EmployeeID, e.FirstName 
  FROM Employees AS e
	   LEFT OUTER JOIN EmployeesProjects AS ep
	ON ep.EmployeeID = e.EmployeeID
 WHERE ep.ProjectID IS NULL
 ORDER BY e.EmployeeID

-- Problem 6. Employees Hired After
SELECT emp.FirstName, emp.LastName, emp.HireDate, dept.[Name] AS DeptName
  FROM Employees AS emp
  JOIN Departments AS dept 
    ON (dept.DepartmentID = emp.DepartmentID
   AND emp.HireDate > '01/01/1999'
   AND dept.[Name] IN ('Sales','Finance'))
 ORDER BY emp.HireDate

-- Problem 7. Employees with Project
SELECT TOP(5) e.EmployeeID, e.FirstName, p.[Name] AS ProjectName 
  FROM Employees AS e
       JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
       JOIN Projects AS p ON p.ProjectID = ep.ProjectID	
 WHERE p.StartDate > '2002-08-13'
   AND p.EndDate IS NULL
 ORDER BY e.EmployeeID

-- Problem 8. Employee 24
SELECT e.EmployeeID,
	   e.FirstName,
	   CASE
			WHEN YEAR(p.StartDate) >= 2005 THEN NULL
			ELSE p.[Name] 
			END AS ProjectName
 FROM Employees AS e
	  JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
      JOIN Projects AS p ON p.ProjectID = ep.ProjectID
 WHERE e.EmployeeID = 24

-- Problem 9. Employee Manager
SELECT e.EmployeeID, e.FirstName, e.ManagerID, m.FirstName AS ManagerName
FROM Employees AS e
JOIN Employees AS m ON m.EmployeeID = e.ManagerID
AND e.ManagerID IN (3,7)
ORDER BY e.EmployeeID

-- Problem 10. Employee Summary
SELECT TOP (50) 
	   e.EmployeeID, 
	   e.FirstName + ' ' + e.LastName as EmployeeName, 
	   m.FirstName + ' ' + m.LastName AS ManagerName,
	   d.[Name] AS DepartmentName  
FROM Employees AS e
JOIN Employees AS m ON m.EmployeeID = e.ManagerID
JOIN Departments AS d ON d.DepartmentID = e.DepartmentID 
ORDER BY e.EmployeeID
  
-- Problem 11. Min Average Salary
SELECT MIN(a.AverageSalary) AS MinAverageSalary 
FROM
   (
	SELECT AVG(Salary) AS AverageSalary
    FROM Employees AS e
    GROUP BY e.DepartmentID
	) AS a
-- Problem 12. Highest Peaks in Bulgaria
USE Geography
GO

SELECT mc.CountryCode, m.MountainRange, p.PeakName, p.Elevation
  FROM MountainsCountries AS mc
  JOIN Mountains AS m ON m.Id = MC.MountainId
  JOIN Peaks AS p ON p.MountainId = m.Id
 WHERE mc.CountryCode = 'BG'AND
	   p.Elevation > 2835
 ORDER BY p.Elevation DESC

-- Problem 13. Count Mountain Ranges
SELECT CountryCode, 
	   COUNT(MountainId) AS MountainRanges
  FROM MountainsCountries
 WHERE CountryCode IN ('BG','RU','US')
 GROUP BY CountryCode

-- Problem 14. Countries With Rivers
SELECT TOP(5) c.CountryName, r.RiverName
  FROM Countries AS c  
  LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
  LEFT JOIN Rivers AS r ON r.Id = cr.RiverId 
 WHERE c.ContinentCode = 'AF'
 ORDER BY c.CountryName

-- Problem 15. Continents And Currencies
SELECT ContinentCode, CurrencyCode, CurrencyCount AS CurrencyUsage
FROM (SELECT ContinentCode,
             CurrencyCode,
             CurrencyCount,
             DENSE_RANK()
                     OVER (PARTITION BY ContinentCode
                         ORDER BY CurrencyCount DESC) AS CurrencyRank
      FROM (SELECT ContinentCode,
                   CurrencyCode,
                   COUNT(*) AS CurrencyCount
            FROM Countries
            GROUP BY ContinentCode, CurrencyCode
           ) AS CurrencyCountQuery
      WHERE CurrencyCount > 1) AS CurrencyRankingQuery
WHERE CurrencyRank=1
ORDER BY ContinentCode

-- Problem 16. Countries Without Any Mountains
SELECT COUNT(*) as CountryCode
FROM(
		SELECT CountryName
		FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc
		ON MC.CountryCode = c.CountryCode
		WHERE mc.MountainId IS NULL
) AS CountriesWihtoutMountains

-- Problem 17. Highest Peak and Longest River by Country
SELECT TOP(5) CountryName,
			  MAX(p.Elevation) AS HighestPeakElevation,
			  MAX(r.Length) AS LongestRiverLength
FROM Countries AS c
     LEFT JOIN MountainsCountries AS mc 
			   ON mc.CountryCode = c.CountryCode
	 LEFT JOIN Mountains AS m 
			   ON m.Id = mc.MountainId
	 LEFT JOIN Peaks AS p 
			   ON P.MountainId = m.Id
	 LEFT JOIN CountriesRivers AS cr 
	           ON cr.CountryCode = c.CountryCode
	 LEFT JOIN Rivers AS r 
	           ON r.Id = cr.RiverId
GROUP BY c.CountryName
ORDER BY HighestPeakElevation DESC,
		 LongestRiverLength DESC,
		 CountryName
-- Problem 18. Highest Peak Name And Elevation By Country

WITH CTE_CountriesInfo (CountryName, PeakName, Elevation, Mountain) AS(
SELECT c.CountryName, p.PeakName, MAX(p.Elevation), m.MountainRange
FROM Countries AS c
      LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
      LEFT JOIN Mountains AS m ON m.Id = mc.MountainId
      LEFT JOIN Peaks AS p ON p.MountainId = m.Id
GROUP BY c.CountryName,p.PeakName,m.MountainRange)

SELECT TOP(5)
	  e.CountryName,
	  ISNULL(cci.PeakName,'(no highest peak)') AS [Highest Peak Name],
	  ISNULL(cci.Elevation,0) AS [Highst Peak Elevation],
	  ISNULL(cci.Mountain,'(no mountain)') AS [Mountain]
FROM(
SELECT CountryName,MAX(Elevation) AS MaxElevation
FROM CTE_CountriesInfo
GROUP BY CountryName) AS e
LEFT JOIN CTE_CountriesInfo AS cci ON cci.CountryName = e.CountryName 
AND cci.Elevation = e.MaxElevation
ORDER BY e.CountryName, cci.PeakName