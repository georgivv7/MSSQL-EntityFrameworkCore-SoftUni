-- Problem 1. One-To-One Relationship
CREATE TABLE Passports(
	PassportID INT NOT NULL,
	PassportNumber NVARCHAR(30) NOT NULL,

	CONSTRAINT PK_Passports
	PRIMARY KEY(PassportID)
)
CREATE TABLE Persons(
	PersonID INT IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	Salary DECIMAL(7,2) NOT NULL,
	PassportID INT NOT NULL,

	CONSTRAINT PK_Persons
	PRIMARY KEY (PersonID),
	CONSTRAINT FK_Persons_Passports
	FOREIGN KEY (PassportID)
	REFERENCES Passports(PassportID)
)

INSERT INTO Passports
VALUES
(101,'N34FG21B'),
(102,'K65LO4R7'),
(103,'ZE657QP2')

INSERT INTO Persons
VALUES
('Roberto',43300.00,102),
('Tom',56100.00,103),
('Yana',60200.00,101)

-- Problem 2. One-To-Many Relationship
CREATE TABLE Manufacturers(
	ManufacturerID INT IDENTITY,
	[Name] VARCHAR(20) NOT NULL,
	EstablishedOn DATE NOT NULL,

	CONSTRAINT PK_Manufacturers
	PRIMARY KEY (ManufacturerID)
)
CREATE TABLE Models(
	ModelID INT NOT NULL,
	[Name] VARCHAR(20) NOT NULL,
	ManufacturerID INT,
	
	CONSTRAINT PK_Models
	PRIMARY KEY(ModelID),
	CONSTRAINT FK_Models_Manufacturers
	FOREIGN KEY (ManufacturerID)
	REFERENCES Manufacturers(ManufacturerID)
)

INSERT INTO Manufacturers
VALUES
('BMW','07/03/1916'),
('Tesla','01/01/2003'),
('Lada','01/05/1966')

INSERT INTO Models
VALUES
(101,'X1',1),
(102,'I6',1),
(103,'Model S',2),
(104,'Model X',2),
(105,'Model 3',2),
(106,'Nova',3)

-- Problem 3. Many-To-Many Relationship
CREATE TABLE Students(
	StudentID INT IDENTITY,
	[Name] NVARCHAR(30) NOT NULL,

	CONSTRAINT PK_Students
	PRIMARY KEY (StudentID)
)
CREATE TABLE Exams(
	ExamID INT IDENTITY(101,1),
	[Name] NVARCHAR(30) NOT NULL,

	CONSTRAINT PK_Exams
	PRIMARY KEY (ExamID)
)
CREATE TABLE StudentsExams(
	StudentID INT NOT NULL,
	ExamID INT NOT NULL,

	CONSTRAINT PK_StudentsExams
	PRIMARY KEY (StudentID,ExamID),
	CONSTRAINT FK_StudentsExams_Students
	FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
	CONSTRAINT FK_StudentsExams_Exams
	FOREIGN KEY (ExamID) REFERENCES Exams(ExamID)
)

INSERT INTO Students
VALUES
('Mila'),
('Toni'),
('Ron')

INSERT INTO Exams
VALUES
('SpringMVC'),
('Neo4j'),
('Oracle 11g')

INSERT INTO StudentsExams
VALUES
(1,101),
(1,102),
(2,101),
(3,103),
(2,102),
(2,103)

-- Problem 4. Self-Referencing
CREATE TABLE Teachers(
	TeacherID INT IDENTITY(101,1),
	[Name] NVARCHAR(30) NOT NULL,
	ManagerID INT,

	CONSTRAINT PK_Teachers
	PRIMARY KEY (TeacherID),
	CONSTRAINT FK_Teachers_ManagerID
	FOREIGN KEY (ManagerID) REFERENCES Teachers(TeacherID)
)

INSERT INTO Teachers
VALUES
('John',NULL),
('Maya',106),
('Silvia',106),
('Ted',105),
('Mark',101),
('Greta',101)

-- Problem 5. Online Store Database

CREATE TABLE Cities(
	CityID INT,
	[Name] VARCHAR(50),

	CONSTRAINT PK_Cities
	PRIMARY KEY (CityID)
)
CREATE TABLE Customers(
	CustomerID INT,
	[Name] VARCHAR(50),
	Birthday DATE,
	CityID INT,

	CONSTRAINT PK_Customers
	PRIMARY KEY (CustomerID),
	CONSTRAINT FK_Customers_Cities
	FOREIGN KEY (CityID) 
	REFERENCES Cities(CityID)
)
CREATE TABLE Orders(
	OrderID INT,
	CustomerID INT,

	CONSTRAINT PK_Orders
	PRIMARY KEY (OrderID),
	CONSTRAINT FK_Orders_Customers
	FOREIGN KEY (CustomerID)
	REFERENCES Customers(CustomerID)
)
CREATE TABLE ItemTypes(
	ItemTypeID INT,
	[Name] VARCHAR(50),

	CONSTRAINT PK_ItemTypes
	PRIMARY KEY (ItemTypeID)
)
CREATE TABLE Items(
	ItemID INT,
	[Name] VARCHAR(50),
	ItemTypeID INT,

	CONSTRAINT PK_Items
	PRIMARY KEY (ItemID),
	CONSTRAINT FK_Items_ItemTypes
	FOREIGN KEY (ItemTypeID) 
	REFERENCES ItemTypes(ItemTypeID)
)
CREATE TABLE OrderItems(
	OrderID INT,
	ItemID INT,

	CONSTRAINT PK_OrderItems
	PRIMARY KEY (OrderID,ItemID),
	CONSTRAINT FK_OrderItems_Orders
	FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
	CONSTRAINT FK_OrderItems_Items
	FOREIGN KEY (ItemID) REFERENCES Items(ItemID)
)

-- Problem 6. University Database
CREATE TABLE Majors(
	MajorID INT,
	[Name] VARCHAR(50),

	CONSTRAINT PK_Majors
	PRIMARY KEY (MajorID)
)
CREATE TABLE Students(
	StudentID INT,
	StudentNumber INT,
	StudentName VARCHAR(50),
	MajorID INT,

	CONSTRAINT PK_Students
	PRIMARY KEY (StudentID),
	CONSTRAINT FK_Students_Majors
	FOREIGN KEY (MajorID) 
	REFERENCES Majors(MajorID)
)

CREATE TABLE Payments(
	PaymentID INT,
	PaymentDate DATE,
	PaymentAmount DECIMAL,
	StudentID INT,

	CONSTRAINT PK_Payments
	PRIMARY KEY (PaymentID),
	CONSTRAINT FK_Payments_Students
	FOREIGN KEY (StudentID) 
	REFERENCES Students(StudentID)
)
CREATE TABLE Subjects(
	SubjectID INT,
	SubjectName VARCHAR(50),

	CONSTRAINT PK_Subjects
	PRIMARY KEY (SubjectID)
)
CREATE TABLE Agenda(
	StudentID INT,
	SubjectID INT,

	CONSTRAINT PK_Agenda
	PRIMARY KEY (StudentID,SubjectID),
	CONSTRAINT FK_Agenda_Students
	FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
	CONSTRAINT FK_Agenda_Subjects
	FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID)
)

-- Problem 9. Peaks In Rila
SELECT m.MountainRange, p.PeakName,p.Elevation
FROM Mountains AS M
JOIN Peaks AS P ON P.MountainId = M.Id
WHERE m.MountainRange = 'Rila'
ORDER BY P.Elevation DESC