-- Problem 1.	Number of Users for Email Provider

SELECT SUBSTRING(Email,CHARINDEX('@',Email)+ 1,
	   LEN(Email) - CHARINDEX('@',Email)+ 1) AS [Email Provider],
	   COUNT(*) AS [Number of Users]
  FROM Users 
  GROUP BY SUBSTRING(Email,CHARINDEX('@',Email)+ 1,
	   LEN(Email) - CHARINDEX('@',Email)+ 1)
   ORDER BY [Number of Users] DESC, [Email Provider]	

-- Problem 2.	All User in Games
SELECT g.[Name] as [Game],
	   gt.[Name] as [Game Type],
	   u.Username,
	   ug.Level,
	   ug.Cash,
	   c.[Name] AS [Character]

FROM UsersGames AS ug
JOIN Games AS g ON g.Id = ug.GameId
JOIN GameTypes as gt ON gt.Id = g.GameTypeId
JOIN Users AS u ON u.Id = ug.UserId
JOIN Characters AS c ON c.Id = ug.CharacterId
ORDER BY ug.[Level] DESC, u.Username, g.[Name] 

-- Problem 3.	Users in Games with Their Items
SELECT u.Username,
	   g.[Name] AS [Game],
	   COUNT(i.ItemTypeId) AS [Items Count],
	   SUM(i.Price) AS [Items Price]
FROM Users AS u
JOIN UsersGames AS ug ON ug.UserId = u.Id
JOIN Games AS g ON g.Id = ug.GameId
JOIN UserGameItems AS ugi ON ugi.UserGameId = ug.Id
JOIN Items AS i ON i.Id = ugi.ItemId
GROUP BY g.Name,u.Username
HAVING COUNT(i.Id) >= 10
ORDER BY [Items Count] DESC, [Items Price] DESC, u.Username 

--Problem 4.	* User in Games with Their Statistics
SELECT u.Username,
	   g.[Name] AS Game,
	   MAX(c.[Name]) AS [Character],
	   SUM(s3.Strength) + MAX(s2.Strength) + MAX(s.Strength)
AS Strength,
	   SUM(s3.Defence) + MAX(s2.Defence) + MAX(s.Defence)
AS Defence,
        SUM(s3.Speed) + MAX(s2.Speed) + MAX(s.Speed)
AS Speed,
        SUM(s3.Mind) + MAX(s2.Mind) + MAX(s.Mind)
AS Mind,
        SUM(s3.Luck) + MAX(s2.Luck) + MAX(s.Luck)
AS Luck	
FROM Users AS u
JOIN UsersGames AS ug ON ug.UserId = u.Id
JOIN Games AS g ON g.Id = ug.GameId
JOIN GameTypes AS gt ON gt.Id = g.GameTypeId
JOIN [Statistics] AS s ON s.Id = gt.BonusStatsId
	JOIN Characters AS c ON c.Id = ug.CharacterId
	JOIN [Statistics] AS s2 ON s2.Id = c.StatisticId
	JOIN UserGameItems AS ugi ON ugi.UserGameId = ug.Id
	JOIN Items AS i ON i.Id = ugi.ItemId
JOIN [Statistics] AS s3 ON s3.Id = i.StatisticId
GROUP BY u.Username, g.[Name]
ORDER BY Strength desc, Defence DESC, Speed DESC, Mind DESC, Luck DESC

-- Problem 5.	All Items with Greater than Average Statistics
SELECT i.[Name],
       i.Price,
       i.MinLevel,
       s.Strength,
       s.Defence,
       s.Speed,
       s.Luck,
       s.Mind
FROM Items AS i
     JOIN [Statistics] AS s ON s.Id = i.StatisticId
WHERE s.Mind >
(
    SELECT AVG(Mind)
    FROM [Statistics]
)
      AND s.Luck >
(
    SELECT AVG(Luck)
    FROM [Statistics]
)
      AND s.Speed >
(
    SELECT AVG(Speed)
    FROM [Statistics]
)
ORDER BY i.[Name];

-- Problem 6.	Display All Items with Information about Forbidden Game Type
SELECT i.[Name] AS [Item],
	   i.Price,
	   i.MinLevel,
	   gt.[Name] AS [Forbidden Game Type]	   
  FROM Items AS i
  LEFT JOIN GameTypeForbiddenItems AS gtfi ON gtfi.ItemId = I.Id
  LEFT JOIN GameTypes AS gt ON gt.Id = gtfi.GameTypeId
ORDER BY [Forbidden Game Type] DESC, i.[Name] 

-- Problem 7.	Buy Items for User in Game
DECLARE @UserID INT = (
					SELECT Id
					  FROM Users
					 WHERE Username = 'Alex'
)
DECLARE @GameID INT = (
					SELECT Id
					  FROM Games
					 WHERE [Name] = 'Edinburgh'
)
DECLARE @UGID INT = (
					SELECT Id
					  FROM UsersGames
					 WHERE UserId = @UserID AND
						   GameId = @GameID
)
DECLARE @ItemsCost DECIMAL(15,2) = (
					SELECT SUM(Price)
					  FROM Items
					  WHERE [Name] IN ('Blackguard', 'Bottomless Potion of Amplification', 
									   'Eye of Etlich (Diablo III)','Gem of Efficacious Toxin', 
									   'Golden Gorget of Leoric', 'Hellfire Amulet'))
UPDATE UsersGames
SET Cash -= (SELECT SUM(Price)
					  FROM Items
					  WHERE [Name] IN ('Blackguard', 'Bottomless Potion of Amplification', 
									   'Eye of Etlich (Diablo III)','Gem of Efficacious Toxin', 
									   'Golden Gorget of Leoric', 'Hellfire Amulet'))
WHERE Id = @UGID;

INSERT INTO UserGameItems(ItemId, UserGameId)
VALUES
       ((SELECT Id FROM Items WHERE Name = 'Blackguard'), @UGID),
 ((SELECT Id FROM Items WHERE Name='Bottomless Potion of Amplification'),@UGID ),
((SELECT Id FROM Items WHERE Name='Eye of Etlich (Diablo III)'),@UGID ),
((SELECT Id FROM Items WHERE Name='Gem of Efficacious Toxin'),@UGID ),
((SELECT Id FROM Items WHERE Name='Golden Gorget of Leoric'),@UGID ),
((SELECT Id FROM Items WHERE Name='Hellfire Amulet'),@UGID )

SELECT u.Username,
	   g.[Name],
	   ug.Cash,
	   i.[Name] AS [Item Name]
  FROM Users AS u
  JOIN UsersGames AS ug ON ug.UserId = u.Id
  JOIN Games AS g ON g.Id = ug.GameId
  JOIN UserGameItems AS ugi ON ugi.UserGameId = ug.Id
  JOIN Items AS i ON i.Id = ugi.ItemId
WHERE g.[Name] = 'Edinburgh'
ORDER BY i.[Name]


-- Problem 8. Peaks and Mountains
SELECT p.PeakName,
	   m.MountainRange AS [Mountain],
	   p.Elevation
FROM Peaks AS p
JOIN Mountains AS m ON M.Id = p.MountainId
ORDER BY p.Elevation DESC, p.PeakName

-- Problem 9. Peaks with Their Mountain, Country and Continent
SELECT p.PeakName,
	   m.MountainRange AS [Mountain],
	   c.CountryName,
	   con.ContinentName
FROM Peaks AS p
JOIN Mountains AS m ON M.Id = p.MountainId
JOIN MountainsCountries AS mc ON mc.MountainId = m.Id
JOIN Countries AS c ON c.CountryCode = mc.CountryCode
JOIN Continents AS con ON con.ContinentCode = c.ContinentCode
ORDER BY p.PeakName, c.CountryName

-- Problem 10.	Rivers by Country
SELECT c.CountryName,
       con.ContinentName,
	   ISNULL(COUNT(r.Id),0) AS [RiversCount],
	   ISNULL(SUM(r.Length),0) AS [TotalLength]
FROM Countries AS c
	 JOIN Continents AS con ON con.ContinentCode = c.ContinentCode
     LEFT JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode     
     LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
GROUP BY c.CountryName, con.ContinentName
ORDER BY RiversCount DESC, TotalLength DESC, c.CountryName

-- Problem 11.	Count of Countries by Currency
SELECT c.CurrencyCode AS [CurrencyCode],
	   c.[Description] AS [Currency],
	   COUNT(coun.CountryCode) AS [NumberOfCountries]
FROM Currencies AS c
LEFT JOIN Countries AS coun ON coun.CurrencyCode = c.CurrencyCode
GROUP BY c.CurrencyCode, c.[Description]
ORDER BY NumberOfCountries DESC, c.[Description]

-- Problem 12.	Population and Area by Continent
SELECT c.ContinentName,
	   (SUM(CAST((c2.AreaInSqKm) AS BIGINT))) AS [CountriesArea],
	   (SUM(CAST((c2.[Population]) AS BIGINT))) AS [CountriesPopulation]
FROM Continents AS c
LEFT JOIN Countries AS c2 ON c2.ContinentCode = c.ContinentCode
GROUP BY c.ContinentName
ORDER BY CountriesPopulation DESC

--Problem 13.	Monasteries by Country
CREATE TABLE Monasteries(
	Id INT IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	CountryCode CHAR(2) NOT NULL,

	CONSTRAINT PK_Monasteries
	PRIMARY KEY (Id),
	CONSTRAINT FK_Monasteries_Countries
	FOREIGN KEY (CountryCode)
	REFERENCES Countries(CountryCode)
)
INSERT INTO Monasteries([Name], CountryCode) VALUES
('Rila Monastery “St. Ivan of Rila”', 'BG'), 
('Bachkovo Monastery “Virgin Mary”', 'BG'),
('Troyan Monastery “Holy Mother''s Assumption”', 'BG'),
('Kopan Monastery', 'NP'),
('Thrangu Tashi Yangtse Monastery', 'NP'),
('Shechen Tennyi Dargyeling Monastery', 'NP'),
('Benchen Monastery', 'NP'),
('Southern Shaolin Monastery', 'CN'),
('Dabei Monastery', 'CN'),
('Wa Sau Toi', 'CN'),
('Lhunshigyia Monastery', 'CN'),
('Rakya Monastery', 'CN'),
('Monasteries of Meteora', 'GR'),
('The Holy Monastery of Stavronikita', 'GR'),
('Taung Kalat Monastery', 'MM'),
('Pa-Auk Forest Monastery', 'MM'),
('Taktsang Palphug Monastery', 'BT'),
('S?mela Monastery', 'TR')

--ALTER TABLE Countries
	    --ADD IsDeleted BIT DEFAULT 0

UPDATE Countries
SET IsDeleted = 1
WHERE CountryCode IN 
      (SELECT Q1.CountryCode
       FROM (SELECT C.CountryCode,
                    COUNT(CR.RiverId) AS COUNT
             FROM Countries AS C
                      JOIN CountriesRivers CR on C.CountryCode = CR.CountryCode
             GROUP BY C.CountryCode) AS Q1
       WHERE Q1.COUNT > 3)

SELECT M.[Name] AS [Monastery],
       C.CountryName AS [Country]
FROM Monasteries AS M
JOIN Countries C on M.CountryCode = C.CountryCode
WHERE C.IsDeleted = 0
ORDER BY Monastery

-- Problem 14.	Monasteries by Continents and Countries
UPDATE Countries 
SET CountryName = 'Burma'
WHERE CountryName = 'Myanmar'

INSERT INTO Monasteries
VALUES
('Hanga Abbey', (SELECT Countries.CountryCode
                        FROM Countries
                        WHERE CountryName = 'Tanzania'))

INSERT INTO Monasteries
VALUES
('Myin-Tin-Daik',(SELECT Countries.CountryCode
                    FROM Countries
					WHERE CountryName = 'Myanmar'))

SELECT c.ContinentName,
	   c2.CountryName,
	   ISNULL(COUNT(m.Id),0) AS [MonasteriesCount]
FROM Continents AS c
JOIN Countries AS c2 ON c2.ContinentCode = c.ContinentCode AND
				c2.IsDeleted = 0
LEFT JOIN Monasteries AS m ON m.CountryCode = c2.CountryCode
GROUP BY c.ContinentName, c2.CountryName
ORDER BY MonasteriesCount DESC, c2.CountryName