using FluentMigrator;

namespace BibeCatalogue.Migrations;

[Migration(20241001001)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Email").AsString(255).NotNullable().Unique()
            .WithColumn("Password").AsString(255).NotNullable()
            .WithColumn("FirstName").AsString(100).NotNullable()
            .WithColumn("LastName").AsString(100).NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
        Delete.Table("Users");
    }
}

[Migration(20241001002)]
public class CreateCoursesTable : Migration
{
    public override void Up()
    {
        Create.Table("Courses")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Title").AsString(200).NotNullable()
            .WithColumn("StartDate").AsDateTime().NotNullable()
            .WithColumn("EndDate").AsDateTime().NotNullable()
            .WithColumn("Result").AsInt32().NotNullable()
            .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_Courses_Users", "Users", "Id")
                .OnDelete(System.Data.Rule.Cascade);

        Create.Index("IX_Courses_UserId").OnTable("Courses").OnColumn("UserId");
        Create.Index("IX_Courses_Title").OnTable("Courses").OnColumn("Title");
    }

    public override void Down()
    {
        Delete.Table("Courses");
    }
}

[Migration(20241001003)]
public class SeedInitialData : Migration
{
    public override void Up()
    {
        // Inserimento utente di test
        Insert.IntoTable("Users").Row(new
        {
            Email = "test@example.com",
            Password = "password123", // In produzione usare hash sicuro
            FirstName = "Test",
            LastName = "User"
        });

        // Inserimento studenti
        Insert.IntoTable("Users").Row(new
        {
            Email = "d.adorni@studenti.unibs.it",
            Password = "d4v1d3",
            FirstName = "Davide",
            LastName = "Adorni"
        });
        Insert.IntoTable("Users").Row(new
        {
            Email = "d.bisoli@studenti.unibs.it",
            Password = "d1m1tr1", // In produzione usare hash sicuro
            FirstName = "Dimitri",
            LastName = "Bisoli"
        });

        // Inserimento corsi di esempio
        Execute.Sql(@"
            DECLARE @UserId INT = (SELECT Id FROM Users WHERE Email = 'test@example.com');
            
            INSERT INTO Courses (Title, StartDate, EndDate, Result, UserId) VALUES 
            ('Corso di Programmazione C#', '2024-01-15', '2024-03-15', 28, @UserId),
            ('Corso di Database Design', '2024-02-01', '2024-04-01', 25, @UserId),
            ('Corso di Web Development', '2024-03-01', '2024-05-01', 30, @UserId),
            ('Corso di Azure Fundamentals', '2024-04-01', '2024-06-01', 27, @UserId);
        ");

        // Inserimento corsi
        Execute.Sql(@"
            DECLARE @UserId INT = (SELECT Id FROM Users WHERE Email = 'd.adorni@studenti.unibs.it');
            
            INSERT INTO Courses (Title, StartDate, EndDate, Result, UserId) VALUES 
            ('Basi di dati e linguaggi di programmazione', '2025-09-16', '2025-12-16', 24, @UserId);
        ");
        Execute.Sql(@"
            DECLARE @UserId INT = (SELECT Id FROM Users WHERE Email = 'd.bisoli@studenti.unibs.it');
            
            INSERT INTO Courses (Title, StartDate, EndDate, Result, UserId) VALUES 
            ('Basi di dati e linguaggi di programmazione', '2025-09-16', '2025-12-16', 28, @UserId);
        ");
    }

    public override void Down()
    {
        Delete.FromTable("Courses").AllRows();
        Delete.FromTable("Users").Row(new { Email = "test@example.com" });
    }
}