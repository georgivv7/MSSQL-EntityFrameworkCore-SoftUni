--Problem 1. Create Database
CREATE DATABASE Minions

-- Problem 2. Create Tables
CREATE TABLE Minions(
    Id INT PRIMARY KEY NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    Age INT
)

CREATE TABLE Towns(
    Id INT PRIMARY KEY NOT NULL,
    [Name] NVARCHAR(50) NOT NULL
)
-- Problem 3. Alter Minions Table
ALTER TABLE Minions
ADD TownId INT FOREIGN KEY REFERENCES Towns(Id)

-- Problem 4. Insert Records in Both Tables

INSERT INTO Towns (Id,[Name]) VALUES
(1,'Sofia'),
(2,'Plovdiv'),
(3,'Varna')

INSERT INTO Minions (Id,[Name], Age, TownId) VALUES
(1,'Kevin', 22, 1),
(2,'Bob', 15, 3),
(3,'Steward',NULL ,2)

--Problem 5. Truncate Table Minions
TRUNCATE TABLE Minions

-- Problem 6. Drop All Tables
DROP TABLE Minions
DROP TABLE Towns

--Problem 7. Create Table People

CREATE TABLE People(
	Id INT PRIMARY KEY IDENTITY,	
	[Name] NVARCHAR(200) NOT NULL,
	Picture IMAGE,
	Height DECIMAL(5,2),
	[Weight] DECIMAL(5,2),
	Gender CHAR(1) NOT NULL,
	Birthdate DATE NOT NULL,
	Biography NVARCHAR(MAX)
)

INSERT INTO People([Name],Height,[Weight],Gender,Birthdate,Biography) VALUES
('IVAN',180.75,NULL,'m','1990/12/20','Teacher'),
('DRAGAN',190,NULL,'m','1966/10/21','Trainer'),
('PETKAN',187.5,NULL,'m','1989/12/10','Footballer'),
('VIKTORIA',165.6,NULL,'f','1999/07/20', 'Athlete'),
('ROSEN',185,NULL,'m','1998/05/03','Tennis player')

-- Problem 8. Create Table Users

CREATE TABLE Users(
	Id BIGINT IDENTITY,	
	Username VARCHAR(30) NOT NULL,
	[Password] VARCHAR(26) NOT NULL,
	ProfilePicture VARBINARY(MAX)
	CHECK(DATALENGTH(ProfilePicture)<=900*1024),
	LastLoginTime DATETIME2,
	IsDeleted BIT NOT NULL
)

ALTER TABLE Users
ADD CONSTRAINT PK_Id
PRIMARY KEY(Id)

INSERT INTO Users(Username,Password,ProfilePicture,LastLoginTime,IsDeleted) VALUES
				('Kicke','password',NULL,'08.15.2021',0),
				('Kicke1','SOMEpassword',NULL,'08.15.2021',1),
				('Kicke2','password123',NULL,'08.15.2021',0),
				('Kicke3','passwordsF',NULL,'08.15.2021',1),
				('Kicke4','passwordT321',NULL,'08.15.2021',0)

ALTER TABLE Users
DROP CONSTRAINT PK_Id

-- Problem 9. Change Primary Key
ALTER TABLE Users
ADD CONSTRAINT PK_Users_CompositeIdUsername
PRIMARY KEY(Id,Username)

-- Problem 10. Add Check Constraint
ALTER TABLE Users
ADD CONSTRAINT CH_PasswordLength
CHECK (DATALENGTH([Password]) >= 5)

-- Problem 11. Set Default Value of a Field
ALTER TABLE Users
ADD DEFAULT GETDATE() FOR LastLoginTime

-- Problem 12. Set Unique Field
ALTER TABLE Users
DROP CONSTRAINT PK_Users_CompositeIdUsername

ALTER TABLE Users
ADD CONSTRAINT PK_Id 
PRIMARY KEY(Id)

ALTER TABLE Users
ADD CONSTRAINT CHK_UsernameLength
CHECK (DATALENGTH(Username) >=3)

-- Problem 13. Movies Database
CREATE DATABASE Movies

CREATE TABLE DIRECTORS(
	Id INT PRIMARY KEY IDENTITY,
	DirectorName VARCHAR(30) NOT NULL UNIQUE,
	Notes VARCHAR(30)
)

INSERT INTO DIRECTORS(DirectorName,Notes) VALUES
			('QUENTIN','MAGICAL QUOTE'),
			('LEONARDO','QUOTE'),
			('DICAPRIO',NULL),
			('ROSS','EVERYBODY..'),
			('TARANTINO','EPIC')

CREATE TABLE Genres(
	Id INT PRIMARY KEY IDENTITY,
	GenreName VARCHAR(30) NOT NULL,
	Notes VARCHAR(30)
)

INSERT INTO Genres(GenreName,Notes) VALUES
			('DRAMA',NULL),
			('ACTION',NULL),
			('THRILLER',NULL),
			('SCI-FI',NULL),
			('HORROR',NULL)

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY,
	CategoryName VARCHAR(30) NOT NULL,
	Notes VARCHAR(30)
)

INSERT INTO Categories(CategoryName,Notes) VALUES
			('category1',NULL),
			('category2',NULL),
			('category3',NULL),
			('category4',NULL),
			('category5',NULL)

CREATE TABLE Movies(
	Id INT PRIMARY KEY IDENTITY,
	Title VARCHAR(30) NOT NULL,
	DirectorId INT REFERENCES DIRECTORS(Id),
	CopyrightYear INT,
	[Length] INT,
	GenreId INT REFERENCES Genres(Id),
	CategoryId INT REFERENCES Categories(Id),
	Rating DECIMAL(5,2) NOT NULL,
	Notes VARCHAR(30)
)	

INSERT INTO Movies(Title,CopyrightYear,[Length],Rating,Notes)
VALUES
			('STARTREK',1976,180,93.5,'MASTERPIECE'),
			('HARRY POTTER',1998,200,95.7,'PARRY HOTTER'),
			('WOLFOFWALLSTREET',1985,205,94.4,'FAKE'),
			('STARWARS',1986,195,97.8,'BEST'),
			('CONJURING',2015,92.9,85,'HORRIBLE')

-- Problem 14. Car Rental Database
CREATE DATABASE CarRental

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY,
	CategoryName NVARCHAR(30) NOT NULL,
	DailyRate DECIMAL(5,2) NOT NULL,
	WeeklyRate DECIMAL(5,2) NOT NULL,
	MonthlyRate DECIMAL(5,2) NOT NULL,
	WeekendRate DECIMAL(5,2) NOT NULL,
)

INSERT INTO Categories(CategoryName,DailyRate,WeeklyRate,MonthlyRate,WeekendRate)
VALUES
			('CATEGORY1',24,30,19,25),
			('CATEGORY2',28,35,26,30),
			('CATEGORY3',29,33,32,31)

CREATE TABLE Cars(
	Id INT PRIMARY KEY IDENTITY,
	PlateNumber NVARCHAR(30) NOT NULL UNIQUE,
	Manufacturer NVARCHAR(30) NOT NULL,
	Model NVARCHAR(30) NOT NULL,
	CarYear INT NOT NULL,
	CategoryId INT REFERENCES Categories(Id),
	Doors INT NOT NULL,
	Picture BINARY(200),
	Condition VARCHAR(5) NOT NULL,
	Available VARCHAR(5) NOT NULL
)
INSERT INTO Cars(PlateNumber,Manufacturer,Model,CarYear,Doors,Picture,Condition,Available)
VALUES 
			('CT2222AK','TOYOTA','RAV4',2010,4,NULL,'USED','YES'),
			('CT3333AP','AUDI','A7',2021,2,NULL,'NEW','YES'),
			('CT4444BM','MERCEDES','E-CLASS',2018,4,NULL,'USED','NO')

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	Title VARCHAR(30) NOT NULL,
	Notes VARCHAR(30)
)
INSERT INTO Employees(FirstName,LastName,Title,Notes)
VALUES
			('GEORGI','IVANOV','MANAGER',NULL),
			('RADKO','PIRATKO','VOL',NULL),
			('IVAN','PETROV','CASHIER',NULL)

CREATE TABLE Customers(
	Id INT PRIMARY KEY IDENTITY,
	DriverLicenceNumber INT NOT NULL,
	FullName VARCHAR(30) NOT NULL UNIQUE,
	[Address] VARCHAR(30) NOT NULL,
	City VARCHAR(10) NOT NULL,
	ZIPCode INT NOT NULL,
	Notes VARCHAR(30)
)

INSERT INTO Customers(DriverLicenceNumber,FullName,[Address],City,ZIPCode,Notes)
VALUES 
			(23456,'BADR HARI','BITKA STREET','LA',2070,NULL),
			(26783,'CONOR MANAFOV','PAVAJ STREET','PARISH',035,NULL),
			(33875,'HARI MAGUIRE','A STREET','MANCHESTER',049,NULL)

CREATE TABLE RentalOrders(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT REFERENCES Employees(Id),
	CustomerId INT REFERENCES Customers(Id),
	CarId INT REFERENCES Cars(Id),
	TankLevel VARCHAR(30) NOT NULL,
	KilometrageStart INT NOT NULL,
	KilometrageEnd INT NOT NULL,
	TotalKilometrage INT NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays INT NOT NULL,
	RateApplied DECIMAL(5,2) NOT NULL,
	TaxRate DECIMAL(5,2) NOT NULL,
	OrderStatus VARCHAR(10) NOT NULL,
	Notes VARCHAR(30)
)

INSERT INTO RentalOrders(TankLevel,KilometrageStart,KilometrageEnd,TotalKilometrage,StartDate,EndDate,TotalDays,
RateApplied,TaxRate,OrderStatus,Notes)
VALUES 
('full',1100,2100,1000,'2021-08-20','2021-08-22',2,5.5,2.3,'Paid',NULL),
('half',10000,12100,2100,'2021-09-02','2021-09-05',3,8.2,4.1,'Paid',NULL),
('empty',2300,3000,700,'2021-08-16','2021-08-17',1,2.7,1.8,'NotPaid',NULL)

-- Problem 15.Hotel Database
CREATE DATABASE Hotel

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	Title VARCHAR(30) NOT NULL,
	Notes VARCHAR(30)
)

INSERT INTO Employees(FirstName,LastName,Title,Notes)
VALUES 
			('GEORGI','KOKOV','MANAGER',NULL),
			('NIKOLA','KOLOV','WORKER',NULL),
			('VANIUSHI','PECHEV','WC',NULL)

CREATE TABLE Customers(
	AccountNumber INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	PhoneNumber INT NOT NULL,
	EmergencyName VARCHAR(30) NOT NULL,
	EmergencyNumber INT NOT NULL,
	Notes VARCHAR(30)
)
INSERT INTO Customers(FirstName,LastName,PhoneNumber,EmergencyName,EmergencyNumber,Notes)
VALUES 
			('ZLATOMIR','VOLCHEV',0895678910,'ZLATKO',359,NULL),
			('DANIEL','VOLCHEV',0895678911,'DANI',358,NULL),
			('STANISLAV','NEDKOV',0895678912,'STUKI',357,NULL)

CREATE TABLE RoomStatus(
	RoomStatus VARCHAR(10) PRIMARY KEY NOT NULL,
	Notes VARCHAR(10)
)

INSERT INTO RoomStatus(RoomStatus,Notes)
VALUES
			('FREE',NULL),
			('BUSY',NULL),
			('EMPTY',NULL)

CREATE TABLE RoomTypes(
	RoomType VARCHAR(30) PRIMARY KEY NOT NULL,
	Notes VARCHAR(10)
)

INSERT INTO RoomTypes(RoomType,Notes)
VALUES
			('ONE-BED',NULL),
			('TWO-BED',NULL),
			('VIP',NULL)


CREATE TABLE BedTypes(
	BedType VARCHAR(10) PRIMARY KEY NOT NULL,
	Notes VARCHAR(10)
)

INSERT INTO BedTypes(BedType,Notes)
VALUES	
			('SMALL',NULL),
			('BIG',NULL),
			('KING',NULL)

CREATE TABLE Rooms(
	RoomNumber INT PRIMARY KEY IDENTITY,
	RoomType VARCHAR(30) REFERENCES RoomTypes(RoomType), 
	BedType VARCHAR(10) REFERENCES BedTypes(BedType),
	Rate DECIMAL(5,2) NOT NULL,
	RoomStatus VARCHAR(10) REFERENCES RoomStatus(RoomStatus),
	Notes VARCHAR(10) 
)

INSERT INTO Rooms(Rate,Notes) VALUES
			(6.6,'NOT GREAT'),
			(9.6, 'PERFECT'),
			(3.3, 'POOR')

CREATE TABLE Payments(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	PaymentDate DATETIME2 NOT NULL,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber),
	FirstDateOccupied DATETIME2 NOT NULL,
	LastDateOccupied DATETIME2 NOT NULL,
	TotalDays AS DATEDIFF(DAY,FirstDateOccupied,LastDateOccupied),
	AmountCharged DECIMAL(6,2) NOT NULL,
	TaxRate DECIMAL(4,2) NOT NULL,
	TaxAmount AS AmountCharged*TaxRate,
	PaymentTotal DECIMAL(6,2) NOT NULL,
	Notes VARCHAR(30)
)

INSERT INTO Payments(EmployeeId,PaymentDate,AccountNumber,FirstDateOccupied,LastDateOccupied,
AmountCharged,TaxRate,PaymentTotal,Notes) VALUES
					(1,'2021-08-21',1,'2021-08-22','2021-08-28',1200,20,1440,NULL),
					(2,'2021-08-13',2,'2021-08-14','2021-08-16',280.50,20,336.60,NULL),
					(3,'2021-06-30',3,'2021-07-01','2021-07-10',2050,20,2460,NULL)

CREATE TABLE Occupancies(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	DateOccupied DATETIME2,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber),
	RoomNumber INT FOREIGN KEY REFERENCES Rooms(RoomNumber),
	RateApplied DECIMAL(4,2),
	PhoneCharge DECIMAL(5,2),
	Notes VARCHAR(30)
)

INSERT INTO Occupancies(EmployeeId,DateOccupied,AccountNumber,RoomNumber,RateApplied,PhoneCharge,Notes)
VALUES
			(1,'2021-05-21',1,1,20,40,NULL),
			(2,'2021-07-27',2,2,20,40,NULL),
			(3,'2021-08-22',3,3,20,40,NULL)

-- Problem 16. Create Softuni Database
CREATE DATABASE Softuni

CREATE TABLE Towns(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(15) NOT NULL
)
CREATE TABLE Departments(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL
)
CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(15) NOT NULL,
	MiddleName NVARCHAR(15) NOT NULL,
	LastName NVARCHAR(15) NOT NULL,
	JobTitle NVARCHAR(15) NOT NULL,
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id),
	HireDate DATE NOT NULL,
	Salary DECIMAL(8,2) NOT NULL
)
-- Problem 18. Basic Insert

INSERT INTO Towns([Name]) VALUES
('Sofia'),
('Plovdiv'),
('Varna'),
('Burgas')

INSERT INTO Departments([Name]) VALUES
('Engineering'),
('Sales'),
('Marketing'),
('Software Development'),
('Quality Assurance')

INSERT INTO Employees(FirstName,MiddleName,LastName,JobTitle,DepartmentId,HireDate,Salary)
VALUES
			('Ivan','Ivanov','Ivanov','.NET Developer',4,CONVERT(datetime,'01/02/2013',103),3500.00),
			('Petar','Petrov','Petrov','Senior Engineer',1,CONVERT(datetime,'02/03/2004',103),4000.00),
			('Maria','Petrova','Ivanova','Intern',5,CONVERT(datetime,'28/08/2016',103),525.25),
			('Georgi','Teziev','Ivanov','CEO',2,CONVERT(datetime,'09/12/2007',103),3000.00),
			('Ivan','Ivanov','Ivanov','Intern',3,CONVERT(datetime,'28/08/2016',103),599.88)

-- Problem 19. Basic Select All Fields

SELECT * FROM Towns
SELECT * FROM Departments
SELECT * FROM Employees

-- Problem 20. Basic Select All Fields And Order Them

SELECT * FROM Towns
ORDER BY [Name]

SELECT * FROM Departments
ORDER BY [Name]

SELECT * FROM Employees
ORDER BY Salary DESC

-- Problem 21. Basic Select Some Fields

SELECT [Name] FROM Towns
ORDER BY [Name]

SELECT [Name] FROM Departments
ORDER BY [Name]

SELECT FirstName,LastName,JobTitle,Salary FROM Employees
ORDER BY Salary DESC

-- Problem 22. Increase Employees Salary

UPDATE Employees
SET Salary *= 1.1
SELECT Salary FROM Employees

-- Problem 23. Decrease Tax Rate
USE Hotel 

UPDATE Payments
SET TaxRate -= TaxRate*0.03
SELECT TaxRate FROM Payments

-- Problem 24. Delete All Records
USE Hotel

TRUNCATE TABLE Occupancies
SELECT * FROM Occupancies