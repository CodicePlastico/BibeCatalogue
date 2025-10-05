using Dapper;
using BibeCatalogue.Models;
using System.Data;

namespace BibeCatalogue.Services;

public class UserService
{
    private readonly DatabaseConnectionService _dbConnectionService;
    private readonly ILogger<UserService> _logger;

    public UserService(DatabaseConnectionService dbConnectionService, ILogger<UserService> logger)
    {
        _dbConnectionService = dbConnectionService;
        _logger = logger;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            const string sql = @"
                SELECT Id, Email, Password, FirstName, LastName, CreatedAt 
                FROM Users 
                WHERE Email = @Email AND Password = @Password";
            
            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email, Password = password });
            
            if (user != null)
            {
                _logger.LogInformation("User {Email} authenticated successfully", email);
            }
            else
            {
                _logger.LogWarning("Authentication failed for user {Email}", email);
            }
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user {Email}", email);
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            const string sql = @"
                SELECT Id, Email, Password, FirstName, LastName, CreatedAt 
                FROM Users 
                WHERE Id = @Id";
            
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID {Id}", id);
            throw;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            const string sql = @"
                SELECT Id, Email, Password, FirstName, LastName, CreatedAt 
                FROM Users 
                WHERE Email = @Email";
            
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            throw;
        }
    }
}