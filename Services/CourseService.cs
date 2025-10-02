using Dapper;
using BibeCatalogue.Models;
using System.Data;

namespace BibeCatalogue.Services;

public class CourseService
{
    private readonly DatabaseConnectionService _dbConnectionService;
    private readonly ILogger<CourseService> _logger;

    public CourseService(DatabaseConnectionService dbConnectionService, ILogger<CourseService> logger)
    {
        _dbConnectionService = dbConnectionService;
        _logger = logger;
    }

    public async Task<IEnumerable<Course>> GetUserCoursesAsync(int userId, string? searchTerm = null)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            // ATTENZIONE: Questo approccio è vulnerabile a SQL injection e non è sicuro per la produzione
            var sql = $@"
                SELECT Id, Title, StartDate, EndDate, Result, UserId 
                FROM Courses 
                WHERE UserId = {userId}";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                sql += $" AND Title LIKE '%{searchTerm}%'";
            }

            sql += " ORDER BY StartDate DESC";
            
            var courses = await connection.QueryAsync<Course>(sql);
            
            _logger.LogInformation("Retrieved {Count} courses for user {UserId}", courses.Count(), userId);
            
            return courses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses for user {UserId}", userId);
            throw;
        }
    }

    public async Task<Course?> GetCourseByIdAsync(int courseId)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            const string sql = @"
                SELECT Id, Title, StartDate, EndDate, Result, UserId 
                FROM Courses 
                WHERE Id = @CourseId";
            
            return await connection.QueryFirstOrDefaultAsync<Course>(sql, new { CourseId = courseId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course by ID {CourseId}", courseId);
            throw;
        }
    }

    public async Task<int> CreateCourseAsync(Course course)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            const string sql = @"
                INSERT INTO Courses (Title, StartDate, EndDate, Result, UserId)
                OUTPUT INSERTED.Id
                VALUES (@Title, @StartDate, @EndDate, @Result, @UserId)";
            
            var courseId = await connection.QuerySingleAsync<int>(sql, course);
            
            _logger.LogInformation("Created course {CourseId} for user {UserId}", courseId, course.UserId);
            
            return courseId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course for user {UserId}", course.UserId);
            throw;
        }
    }

    public async Task<bool> UpdateCourseAsync(Course course)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            const string sql = @"
                UPDATE Courses 
                SET Title = @Title, StartDate = @StartDate, EndDate = @EndDate, Result = @Result
                WHERE Id = @Id AND UserId = @UserId";
            
            var rowsAffected = await connection.ExecuteAsync(sql, course);
            
            _logger.LogInformation("Updated course {CourseId}, rows affected: {RowsAffected}", course.Id, rowsAffected);
            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course {CourseId}", course.Id);
            throw;
        }
    }

    public async Task<bool> DeleteCourseAsync(int courseId, int userId)
    {
        try
        {
            using var connection = _dbConnectionService.CreateConnection();
            
            const string sql = @"
                DELETE FROM Courses 
                WHERE Id = @CourseId AND UserId = @UserId";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { CourseId = courseId, UserId = userId });
            
            _logger.LogInformation("Deleted course {CourseId}, rows affected: {RowsAffected}", courseId, rowsAffected);
            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course {CourseId}", courseId);
            throw;
        }
    }
}