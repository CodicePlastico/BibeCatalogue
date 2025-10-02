using Microsoft.Data.SqlClient;
using System.Data;

namespace BibeCatalogue.Services;

public class DatabaseConnectionService
{
    private readonly string _connectionString;

    public DatabaseConnectionService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}