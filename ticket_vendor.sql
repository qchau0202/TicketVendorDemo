create database TicketVendor
use TicketVendor

-- Bảng Passenger
CREATE TABLE Passengers (
    PassengerID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL, 
    Password NVARCHAR(50) NULL, 
    PhoneNumber NVARCHAR(20) NULL,
    IsGuest BIT NOT NULL DEFAULT 1 -- 1: Guest, 0: Registered
);

-- Bảng Trains
CREATE TABLE Trains (
    TrainID INT PRIMARY KEY IDENTITY(1,1),
    TrainNumber NVARCHAR(10) NOT NULL,
    TrainName NVARCHAR(100) NOT NULL,
    DepartureStation NVARCHAR(100) NOT NULL,
    ArrivalStation NVARCHAR(100) NOT NULL,
    DepartureTime TIME NOT NULL,
    ArrivalTime TIME NOT NULL,
    TravelDate DATE NOT NULL,
    TicketType NVARCHAR(50) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL
);

-- Bảng Tickets
CREATE TABLE Tickets (
    TicketID INT PRIMARY KEY IDENTITY(1,1),
    PassengerID INT NOT NULL,
    TrainID INT NOT NULL,
    SeatNumber NVARCHAR(10) NOT NULL,
    CONSTRAINT FK_Tickets_Passengers FOREIGN KEY (PassengerID) REFERENCES Passengers(PassengerID),
    CONSTRAINT FK_Tickets_Trains FOREIGN KEY (TrainID) REFERENCES Trains(TrainID)
);

-- Bảng TicketPassengers
CREATE TABLE TicketPassengers (
    TicketPassengerID INT PRIMARY KEY IDENTITY(1,1),
    PassengerID INT NOT NULL,
    TicketID INT NOT NULL,
    QRCodeData NVARCHAR(MAX) NOT NULL,
    CONSTRAINT FK_TicketPassengers_Passengers FOREIGN KEY (PassengerID) REFERENCES Passengers(PassengerID),
    CONSTRAINT FK_TicketPassengers_Tickets FOREIGN KEY (TicketID) REFERENCES Tickets(TicketID)
);

-- Dữ liệu mẫu cho Passengers
INSERT INTO Passengers (FullName, Email, Password, PhoneNumber, IsGuest) VALUES
('Nguyen Van A', 'a@example.com', 'password123', '0901234567', 0),
('Tran Thi B', 'b@example.com', 'password456', '0912345678', 0),

-- Dữ liệu mẫu cho Trains
INSERT INTO Trains (TrainNumber, TrainName, DepartureStation, ArrivalStation, DepartureTime, ArrivalTime, TravelDate, TicketType, Price) VALUES
('12630', 'SE15', 'HCM, Ho Chi Minh City', 'DN, Da Nang', '08:00:00', '16:00:00', '2025-04-22', 'Single', 450000.00),
('12730', 'SE8', 'HN, Ha Noi', 'HUE, Hue', '09:30:00', '17:30:00', '2025-04-22', 'Return', 600000.00),
('12830', 'SE10', 'DN, Da Nang', 'HCM, Ho Chi Minh City', '14:00:00', '22:00:00', '2025-04-23', 'Single', 400000.00),
('12930', 'SE7', 'HP, Hai Phong', 'CT, Can Tho', '06:00:00', '14:00:00', '2025-04-23', 'Return', 550000.00),
('13030', 'SE9', 'HUE, Hue', 'HN, Ha Noi', '12:00:00', '20:00:00', '2025-04-24', 'Single', 500000.00),
('13130', 'SE11', 'CT, Can Tho', 'DN, Da Nang', '07:00:00', '15:00:00', '2025-04-24', 'Return', 650000.00),
('13230', 'SE13', 'HCM, Ho Chi Minh City', 'HP, Hai Phong', '10:00:00', '18:00:00', '2025-04-25', 'Single', 700000.00),
('13330', 'SE14', 'HN, Ha Noi', 'CT, Can Tho', '13:00:00', '21:00:00', '2025-04-25', 'Return', 800000.00),
('13430', 'SE16', 'DN, Da Nang', 'HUE, Hue', '15:00:00', '19:00:00', '2025-04-26', 'Single', 300000.00),
('13530', 'SE17', 'HP, Hai Phong', 'HCM, Ho Chi Minh City', '11:00:00', '19:00:00', '2025-04-26', 'Return', 750000.00),
('13630', 'SE18', 'HUE, Hue', 'HP, Hai Phong', '08:30:00', '16:30:00', '2025-04-27', 'Single', 550000.00),
('13730', 'SE19', 'CT, Can Tho', 'HN, Ha Noi', '09:00:00', '17:00:00', '2025-04-27', 'Return', 900000.00);

SELECT * FROM Passengers;
SELECT * FROM Tickets;
SELECT * FROM TicketPassengers;
SELECT * FROM Trains;

DROP TABLE IF EXISTS TicketPassengers;
DROP TABLE IF EXISTS Tickets;
DROP TABLE IF EXISTS Trains;
DROP TABLE IF EXISTS Passengers;