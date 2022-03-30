-- Section 1. DDL
   -- 1. Database Design
CREATE DATABASE ColonialJourney
USE ColonialJourney

CREATE TABLE Planets(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(30) NOT NULL,
)

CREATE TABLE Spaceports(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	PlanetId INT NOT NULL FOREIGN KEY
	REFERENCES Planets(Id) 
)
CREATE TABLE Spaceships(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	Manufacturer VARCHAR(30) NOT NULL,
	LightSpeedRate INT DEFAULT 0
)

CREATE TABLE Colonists(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(20) NOT NULL, 
	LastName VARCHAR(20) NOT NULL,
	Ucn VARCHAR(10) UNIQUE NOT NULL,
	BirthDate DATE NOT NULL
)

CREATE TABLE Journeys(
	Id INT PRIMARY KEY IDENTITY,
	JourneyStart Datetime NOT NULL,
	JourneyEnd Datetime NOT NULL,
	Purpose VARCHAR(11) CHECK(Purpose IN ('Medical','Technical','Educational','Military')),
	DestinationSpaceportId INT NOT NULL FOREIGN KEY
	REFERENCES Spaceports(Id),
	SpaceshipId INT NOT NULL FOREIGN KEY
	REFERENCES Spaceships(Id) 
)

CREATE TABLE TravelCards(
	Id INT PRIMARY KEY IDENTITY,
	CardNumber CHAR(10) UNIQUE NOT NULL CHECK(LEN(CardNumber)=10), 
	JobDuringJourney VARCHAR(8) CHECK(JobDuringJourney IN ('Pilot','Engineer','Trooper','Cleaner','Cook')),
	ColonistId INT NOT NULL FOREIGN KEY 
	REFERENCES Colonists(Id) ,
	JourneyId INT NOT NULL FOREIGN KEY 
	REFERENCES Journeys(Id)
)

-- Section 2. DML
   -- 2.Insert
INSERT INTO Planets
VALUES
('Mars'),
('Earth'),
('Jupiter'),
('Saturn')

INSERT INTO Spaceships([Name],Manufacturer,LightSpeedRate)
VALUES
('Golf','VW',3),
('WakaWaka','Wakanda',4),
('Falcon9','SpaceX',1),
('Bed','Vidolov',6)

	-- 3.Update
UPDATE Spaceships
SET LightSpeedRate += 1
WHERE Id BETWEEN 8 AND 12

	-- 4.Delete
DELETE FROM TravelCards
WHERE JourneyId BETWEEN 1 AND 3

DELETE FROM Journeys
WHERE Id BETWEEN 1 AND 3
-- Section 3. Querying 
	-- 5.Select all military journeys

SELECT Id,
	   FORMAT(JourneyStart,'dd/MM/yyyy') AS [JourneyStart],
	   FORMAT(JourneyEnd,'dd/MM/yyyy') AS [JourneyEnd]
  FROM Journeys
  WHERE Purpose LIKE 'Military'
  ORDER BY JourneyStart
	-- 6.Select All Pilots
SELECT c.Id AS [id],
	   c.FirstName + ' ' + c.LastName AS [full_name]	
FROM Colonists AS c
JOIN TravelCards AS tc ON tc.ColonistId = c.Id
WHERE tc.JobDuringJourney LIKE 'Pilot'
ORDER BY c.Id 

	-- 7. Count colonists
SELECT COUNT(c.Id) AS [count] 
  FROM Colonists AS c
  JOIN TravelCards AS tc ON tc.ColonistId = c.Id
  JOIN Journeys AS j ON j.Id = tc.JourneyId
 WHERE j.Purpose = 'Technical'
 GROUP BY j.Purpose

	-- 8. Select spaceships with pilots younger than 30 years
SELECT s.[Name],
	   s.Manufacturer	
  FROM Spaceships as s
  JOIN Journeys AS j ON j.SpaceshipId = s.Id
  JOIN TravelCards AS tc ON tc.JourneyId = j.Id
  JOIN Colonists AS c ON c.Id = tc.ColonistId
  WHERE tc.JobDuringJourney LIKE 'Pilot' AND 
		DATEDIFF(YEAR,c.BirthDate,'2019-01-01')<30
  ORDER BY s.[Name] 
	-- 9. Select all planets and their journey count
SELECT p.[Name] AS [PlanetName],
	   COUNT(j.Id) AS [JourneysCount]	    
FROM Planets as p
JOIN Spaceports AS s ON s.PlanetId = p.Id
JOIN Journeys as j ON j.DestinationSpaceportId = s.Id
GROUP BY p.[Name]
ORDER BY JourneysCount DESC, p.[Name] 
	--10. Select Second Oldest Important Colonist
SELECT JobDuringJourney, RANKQUERY.FullName, RANKQUERY.JobRank 
	FROM(SELECT tc.JobDuringJourney  AS [JobDuringJourney],
		 c.FirstName + ' ' + c.LastName AS [FullName],
		 (DENSE_RANK() over (PARTITION BY tc.JobDuringJourney ORDER BY c.BirthDate)) AS [JobRank]			
FROM Colonists AS c
JOIN TravelCards AS tc ON tc.ColonistId = c.Id) AS RANKQUERY
WHERE [JobRank] = 2

--Section 4. Programmability
	--11. Get Colonists Count
CREATE FUNCTION udf_GetColonistsCount(@PlanetName VARCHAR (30))
RETURNS INT
AS 
	BEGIN
	DECLARE @count INT;
	SET @count = (
				  SELECT COUNT(c.Id)
                    FROM Colonists AS c
                         JOIN TravelCards AS tc ON tc.ColonistId = c.Id
                         JOIN Journeys AS j ON j.Id = tc.JourneyId
				         JOIN Spaceports AS s ON s.Id = j.DestinationSpaceportId
				         JOIN Planets AS p ON p.Id = s.PlanetId
				   WHERE p.[Name] LIKE @PlanetName
				 )
	RETURN @count;
	END
	GO

	SELECT dbo.udf_GetColonistsCount('Otroyphus')
	--12. Change Journey Purpose
CREATE PROCEDURE usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose VARCHAR(11))
AS
BEGIN
	IF(@JourneyId NOT IN (
		SELECT Id
		FROM Journeys))
	 BEGIN
	  RAISERROR('The journey does not exist!',16,1);
	  ROLLBACK;
	 END
	IF((SELECT COUNT(*)
		FROM Journeys
		WHERE Id = @JourneyId AND
			  Purpose = @NewPurpose) != 0)
	  BEGIN
	   RAISERROR('You cannot change the purpose!',16,2);
	   ROLLBACK;
	  END

	UPDATE Journeys
	SET Purpose = @NewPurpose
	WHERE Id = @JourneyId
END

EXEC dbo.usp_ChangeJourneyPurpose 196,'Educational'