CREATE TABLE Category (
    CategoryId INT NOT NULL IDENTITY(1,1),
    CategoryName VARCHAR(255) NOT NULL,
    PRIMARY KEY (CategoryId)
);

CREATE TABLE Ticket (
    TicketCode VARCHAR(12) NOT NULL,
    TicketName VARCHAR(80) NOT NULL,
    CategoryId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Quota INT DEFAULT 0,
    EventDateMinimal DATETIME NOT NULL,
    EventDateMaximal DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (TicketCode),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);

CREATE TABLE BookedTicketTransaction (
    BookedTicketTransactionId INT NOT NULL IDENTITY(1,1),
    TotalTickets INT NOT NULL,
    SummaryPrice DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (BookedTicketTransactionId)
);

CREATE TABLE BookedTicket (
    BookedTicketId INT NOT NULL IDENTITY(1,1),
    BookedTicketTransactionId INT NOT NULL,
    TicketCode VARCHAR(12) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    BookedDate DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (BookedTicketId),
    FOREIGN KEY (BookedTicketTransactionId) REFERENCES BookedTicketTransaction(BookedTicketTransactionId),
    FOREIGN KEY (TicketCode) REFERENCES Ticket(TicketCode)
);

-- Insert Categories
INSERT INTO Category (CategoryName) VALUES
('Bioskop'),
('Konser'),
('Kereta'),
('Kapal Laut'),
('Pesawat'),
('Hotel');

-- Insert Tickets for each category
INSERT INTO Ticket (TicketCode, TicketName, CategoryId, Price, Quota, EventDateMinimal, EventDateMaximal, CreatedAt) VALUES
('B001', 'Tiket Reguler Bioskop', 1, 50000.00, 100, '2025-03-01 10:00:00', '2025-03-01 22:00:00', GETDATE()),
('B002', 'Tiket VIP Bioskop', 1, 75000.00, 150, '2025-03-02 10:00:00', '2025-03-02 22:00:00', GETDATE()),
('B003', 'Tiket Premiere Bioskop', 1, 100000.00, 200, '2025-03-03 10:00:00', '2025-03-03 22:00:00', GETDATE()),

('K001', 'Konser Silver', 2, 200000.00, 500, '2025-04-10 18:00:00', '2025-04-10 23:59:00', GETDATE()),
('K002', 'Konser Gold', 2, 350000.00, 300, '2025-04-15 18:00:00', '2025-04-15 23:59:00', GETDATE()),
('K003', 'Konser Platinum', 2, 500000.00, 200, '2025-04-20 18:00:00', '2025-04-20 23:59:00', GETDATE()),

('KR001', 'Kereta Ekonomi', 3, 150000.00, 100, '2025-05-01 08:00:00', '2025-05-01 12:00:00', GETDATE()),
('KR002', 'Kereta Bisnis', 3, 250000.00, 120, '2025-05-05 08:00:00', '2025-05-05 12:00:00', GETDATE()),
('KR003', 'Kereta Eksekutif', 3, 300000.00, 150, '2025-05-10 08:00:00', '2025-05-10 12:00:00', GETDATE()),

('KL001', 'Kapal Ekonomi', 4, 100000.00, 50, '2025-06-01 07:00:00', '2025-06-01 17:00:00', GETDATE()),
('KL002', 'Kapal Bisnis', 4, 175000.00, 75, '2025-06-05 07:00:00', '2025-06-05 17:00:00', GETDATE()),
('KL003', 'Kapal Eksekutif', 4, 250000.00, 100, '2025-06-10 07:00:00', '2025-06-10 17:00:00', GETDATE()),

('P001', 'Pesawat Ekonomi', 5, 500000.00, 200, '2025-07-01 06:00:00', '2025-07-01 12:00:00', GETDATE()),
('P002', 'Pesawat Bisnis', 5, 750000.00, 150, '2025-07-05 06:00:00', '2025-07-05 12:00:00', GETDATE()),
('P003', 'Pesawat First Class', 5, 1000000.00, 100, '2025-07-10 06:00:00', '2025-07-10 12:00:00', GETDATE()),

('H001', 'Hotel Standar', 6, 300000.00, 50, '2025-08-01 14:00:00', '2025-08-02 12:00:00', GETDATE()),
('H002', 'Hotel Deluxe', 6, 500000.00, 75, '2025-08-05 14:00:00', '2025-08-06 12:00:00', GETDATE()),
('H003', 'Hotel Suite', 6, 750000.00, 100, '2025-08-10 14:00:00', '2025-08-11 12:00:00', GETDATE());