using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;

namespace BibeCatalogue.Services;

public class DatabaseMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService> _logger;
    private readonly string _connectionString;

    public DatabaseMigrationService(
        IServiceProvider serviceProvider, 
        ILogger<DatabaseMigrationService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task MigrateAsync()
    {
        try
        {
            _logger.LogInformation("Starting database migration...");

            // Crea il database se non esiste
            await EnsureDatabaseExistsAsync();

            // Esegue le migrazioni
            using var scope = _serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();

            _logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database migration");
            throw;
        }
    }

    private async Task EnsureDatabaseExistsAsync()
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            var sql = $@"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
                BEGIN
                    CREATE DATABASE [{databaseName}]
                END";

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("Database {DatabaseName} ensured to exist", databaseName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring database exists");
            throw;
        }
    }
}