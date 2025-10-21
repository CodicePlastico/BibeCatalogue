USE master;
GO

CREATE LOGIN bibecatalogueuser WITH PASSWORD = 'YourStrong@Passw0rd';
GO

USE BibeCatalogueDB;
GO

CREATE USER bibecatalogueuser FOR LOGIN bibecatalogueuser;
GO

ALTER ROLE [db_datareader] ADD MEMBER bibecatalogueuser;
GO

--ALTER ROLE [db_datawriter] ADD MEMBER bibecatalogueuser;
GO
