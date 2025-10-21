USE master;
GO

IF DB_ID('BibeCatalogueDB') IS NOT NULL
BEGIN

    ALTER DATABASE BibeCatalogueDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE BibeCatalogueDB;

END;
GO

CREATE DATABASE BibeCatalogueDB;
GO

USE BibeCatalogueDB;
GO

CREATE TABLE dbo.Users (
    Id INT IDENTITY(1, 1) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT PK_Users
        PRIMARY KEY CLUSTERED (Id)
) ON [PRIMARY];
GO

ALTER TABLE dbo.Users
ADD
    CONSTRAINT DF_Users_CreatedAt
    DEFAULT (GETDATE ()) FOR CreatedAt;
GO

INSERT INTO dbo.Users (
    Email,
    Password,
    FirstName,
    LastName,
    CreatedAt
)
VALUES (N'test@example.com', N'password123', N'Test', N'User', GETDATE ()),
    (N'd.adorni@studenti.unibs.it', N'd4v1d3', N'Davide', N'Adorni', GETDATE ()),
    (N'd.bisoli@studenti.unibs.it', N'd1m1tr1', N'Dimitri', N'Bisoli', GETDATE ());
GO

SELECT * FROM dbo.Users ORDER BY Id;
GO

CREATE TABLE dbo.Courses (
    Id INT IDENTITY(1, 1) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Result INT NOT NULL,
    UserId INT NOT NULL,
    CONSTRAINT PK_Courses
        PRIMARY KEY CLUSTERED (Id ASC)
) ON [PRIMARY];
GO

ALTER TABLE dbo.Courses WITH CHECK
ADD
    CONSTRAINT FK_Courses_Users
    FOREIGN KEY (UserId)
    REFERENCES dbo.Users (Id)
    ON DELETE CASCADE;
GO

ALTER TABLE dbo.Courses CHECK CONSTRAINT FK_Courses_Users;
GO

DECLARE @UserId INT;

SELECT TOP (1) @UserId = Id FROM dbo.Users WHERE Email = 'test@example.com' ORDER BY Id;

INSERT INTO dbo.Courses (Title, StartDate, EndDate, Result, UserId)
VALUES ('Corso di Programmazione C#', '2024-01-15', '2024-03-15', 28, @UserId),
    ('Corso di Database Design', '2024-02-01', '2024-04-01', 25, @UserId),
    ('Corso di Web Development', '2024-03-01', '2024-05-01', 30, @UserId),
    ('Corso di Azure Fundamentals', '2024-04-01', '2024-06-01', 27, @UserId);

SELECT TOP (1) @UserId = Id FROM dbo.Users WHERE Email = 'd.adorni@studenti.unibs.it' ORDER BY Id;

INSERT INTO dbo.Courses (Title, StartDate, EndDate, Result, UserId)
VALUES ('Basi di dati e linguaggi di programmazione', '2025-09-16', '2025-12-16', 24, @UserId);

SELECT TOP (1) @UserId = Id FROM dbo.Users WHERE Email = 'd.bisoli@studenti.unibs.it' ORDER BY Id;

INSERT INTO dbo.Courses (Title, StartDate, EndDate, Result, UserId)
VALUES ('Basi di dati e linguaggi di programmazione', '2025-09-16', '2025-12-16', 28, @UserId);
GO

UPDATE dbo.Courses
SET Result = 30
WHERE UserId = 2;
GO

DELETE FROM dbo.Courses
WHERE UserId = 3;
GO
