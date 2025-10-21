USE master;
GO

IF DB_ID('BibeCatalogueLedgerDB') IS NOT NULL
BEGIN

    ALTER DATABASE BibeCatalogueLedgerDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE BibeCatalogueLedgerDB;

END;
GO

CREATE DATABASE BibeCatalogueLedgerDB WITH LEDGER = ON;
GO

USE BibeCatalogueLedgerDB;
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
) ON [PRIMARY]
WITH (
    SYSTEM_VERSIONING = ON (
        HISTORY_TABLE = dbo.Users_History
    ),
    LEDGER = ON (
        LEDGER_VIEW = dbo.Users_LedgerView
    )
);
GO

ALTER TABLE dbo.Users
ADD
    CONSTRAINT DF_Users_CreatedAt
    DEFAULT (GETDATE ()) FOR CreatedAt;
GO

-- Transazione #1
INSERT INTO dbo.Users (
    Email,
    Password,
    FirstName,
    LastName,
    CreatedAt
)
VALUES (N'test@example.com', N'changeme', N'Test', N'User', GETDATE ()),
    (N'd.adorni@studenti.unibs.it', N'changeme', N'Davide', N'Adorni', GETDATE ());
GO

-- Transazione #2
INSERT INTO dbo.Users (
    Email,
    Password,
    FirstName,
    LastName,
    CreatedAt
)
VALUES (N'd.bisoli@studenti.unibs.it', N'changeme', N'Dimitri', N'Bisoli', GETDATE ());
GO

-- Transazione #3
UPDATE dbo.Users
SET Password = N'd4v1d3'
WHERE Email = N'd.adorni@studenti.unibs.it';
GO

-- Transazione #4
UPDATE dbo.Users
SET Password = N'd1m1tr1'
WHERE Email = N'd.bisoli@studenti.unibs.it';
GO

SELECT * FROM dbo.Users ORDER BY Id;
GO

SELECT * FROM dbo.Users_LedgerView ORDER BY ledger_transaction_id, Id, ledger_operation_type DESC;
GO

SELECT * FROM dbo.Users_History;
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
) ON [PRIMARY]
WITH (
    LEDGER = ON (
        APPEND_ONLY = ON,
        LEDGER_VIEW = dbo.Courses_LedgerView
    )
);
GO

ALTER TABLE dbo.Courses WITH CHECK
ADD
    CONSTRAINT FK_Courses_Users
    FOREIGN KEY (UserId)
    REFERENCES dbo.Users (Id);
    -- ON DELETE CASCADE;
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
