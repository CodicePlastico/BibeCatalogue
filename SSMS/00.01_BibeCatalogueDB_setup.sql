DELETE FROM dbo.Courses;
GO

DBCC CHECKIDENT ('dbo.Courses', RESEED, 0);
GO

DELETE FROM dbo.Users;
GO

DBCC CHECKIDENT ('dbo.Users', RESEED, 0);
GO

SET IDENTITY_INSERT dbo.Users ON;

INSERT INTO dbo.Users (
    Id,
    Email,
    Password,
    FirstName,
    LastName,
    CreatedAt
)
VALUES
(
    1,
    N'test@example.com',    -- Email - nvarchar(255)
    N'password123',    -- Password - nvarchar(255)
    N'Test',    -- FirstName - nvarchar(100)
    N'User',    -- LastName - nvarchar(100)
    DEFAULT -- CreatedAt - datetime
),
(2, N'd.adorni@studenti.unibs.it', N'd4v1d3', N'Davide', N'Adorni', DEFAULT),
(3, N'd.bisoli@studenti.unibs.it', N'd1m1tr1', N'Dimitri', N'Bisoli', DEFAULT);
GO

SET IDENTITY_INSERT dbo.Users OFF;
GO

DECLARE @UserId INT = (SELECT Id FROM Users WHERE Email = 'test@example.com');

INSERT INTO Courses (
    Title,
    StartDate,
    EndDate,
    Result,
    UserId
)
VALUES
('Corso di Programmazione C#', '2024-01-15', '2024-03-15', 28, @UserId),
('Corso di Database Design', '2024-02-01', '2024-04-01', 25, @UserId),
('Corso di Web Development', '2024-03-01', '2024-05-01', 30, @UserId),
('Corso di Azure Fundamentals', '2024-04-01', '2024-06-01', 27, @UserId);
GO

DECLARE @UserId INT = (SELECT Id FROM Users WHERE Email = 'd.adorni@studenti.unibs.it');
            
INSERT INTO Courses (Title, StartDate, EndDate, Result, UserId) VALUES 
('Basi di dati e linguaggi di programmazione', '2025-09-16', '2025-12-16', 24, @UserId);
 GO

DECLARE @UserId INT = (SELECT Id FROM Users WHERE Email = 'd.bisoli@studenti.unibs.it');
            
INSERT INTO Courses (Title, StartDate, EndDate, Result, UserId) VALUES 
('Basi di dati e linguaggi di programmazione', '2025-09-16', '2025-12-16', 28, @UserId);
GO
