-- Section 1. DDL
CREATE DATABASE TripService
USE TripService

CREATE TABLE Cities
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(20) NOT NULL,
	CountryCode CHAR(2) NOT NULL
)
CREATE TABLE Hotels
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL,
	CityId INT FOREIGN KEY
    REFERENCES Cities(Id) NOT NULL,
	EmployeeCount INT NOT NULL,
	BaseRate DECIMAL(18,2)
)
CREATE TABLE Rooms(
	Id INT PRIMARY KEY IDENTITY,
	Price DECIMAL(18,2) NOT NULL,
	[Type] NVARCHAR(20) NOT NULL,
	Beds INT NOT NULL,
	HotelId INT FOREIGN KEY
    REFERENCES Hotels(Id) NOT NULL
)
CREATE TABLE Trips(
	Id INT PRIMARY KEY IDENTITY,
	RoomId INT FOREIGN KEY
    REFERENCES Rooms(Id) NOT NULL,
	BookDate DATE NOT NULL,
	ArrivalDate DATE NOT NULL,
	ReturnDate DATE NOT NULL,
	CancelDate DATE,
  
  	CONSTRAINT CH_BookDate_ArrivalDate CHECK(BookDate < ArrivalDate), 
    CONSTRAINT CH_ArrivalDate_ReturnDate CHECK(ArrivalDate < ReturnDate)
)
CREATE TABLE Accounts(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	MiddleName NVARCHAR(20),
	LastName NVARCHAR(50) NOT NULL,
	CityId INT FOREIGN KEY 
    REFERENCES Cities(Id) NOT NULL,
	BirthDate DATE NOT NULL,
	Email VARCHAR(100) UNIQUE NOT NULL
)
CREATE TABLE AccountsTrips(
	AccountId INT NOT NULL REFERENCES Accounts(Id),
	TripId INT NOT NULL REFERENCES Trips(Id),
	Luggage INT NOT NULL CHECK(Luggage >=0),
  
  	CONSTRAINT PK_Accounts_Trips
	PRIMARY KEY(AccountID,TripId)
)


-- Section 2. DML
	-- 2. Insert
INSERT INTO Accounts(FirstName,MiddleName,LastName,CityId,BirthDate,Email)
VALUES ('John', 'Smith', 'Smith', 34, '1975-07-21', 'j_smith@gmail.com'),
       ('Gosho', NULL, 'Petrov', 11, '1978-05-16', 'g_petrov@gmail.com'),
       ('Ivan', 'Petrovich', 'Pavlov', 59, '1849-09-26', 'i_pavlov@softuni.bg'),
       ('Friedrich', 'Wilhelm', 'Nietzsche', 2, '1844-10-15', 'f_nietzsche@softuni.bg')


INSERT INTO Trips(RoomId,BookDate,ArrivalDate,ReturnDate,CancelDate)
VALUES (101,'2015-04-12','2015-04-14','2015-04-20','2015-02-02'),
       (102,'2015-07-07','2015-07-15','2015-07-22','2015-04-29'),
       (103,'2013-07-17','2013-07-23','2013-07-24',NULL),
       (104,'2012-03-17','2012-03-31','2012-04-01','2012-01-10'),
       (109,'2017-08-07','2017-08-28','2017-08-29',NULL)

	-- 3.Update
UPDATE Rooms
SET Price *= 1.14
WHERE HotelId IN (5, 7, 9)

	-- 4.Delete
DELETE AccountsTrips
WHERE AccountId = 47
-- Section 3. Querying
	-- 5. EEE-Emails
SELECT a.FirstName,
	   a.LastName,
	   FORMAT(a.BirthDate,'MM-dd-yyyy') AS [BirthDate],
	   c.[Name] AS [Hometown],
	   a.Email
  FROM Accounts AS a
  JOIN Cities AS c ON c.Id = a.CityId
  WHERE a.Email LIKE 'e%'
ORDER BY Hometown
