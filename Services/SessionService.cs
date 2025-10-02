using BibeCatalogue.Models;

namespace BibeCatalogue.Services;

public class SessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SessionService> _logger;
    private const string UserSessionKey = "CurrentUser";

    public SessionService(IHttpContextAccessor httpContextAccessor, ILogger<SessionService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public void SetUser(User user)
    {
        try
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetInt32($"{UserSessionKey}_Id", user.Id);
                session.SetString($"{UserSessionKey}_Email", user.Email);
                session.SetString($"{UserSessionKey}_FirstName", user.FirstName);
                session.SetString($"{UserSessionKey}_LastName", user.LastName);
                
                _logger.LogInformation("User {Email} set in session", user.Email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting user in session");
            throw;
        }
    }

    public User? GetUser()
    {
        try
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return null;

            var userId = session.GetInt32($"{UserSessionKey}_Id");
            if (!userId.HasValue) return null;

            var email = session.GetString($"{UserSessionKey}_Email");
            var firstName = session.GetString($"{UserSessionKey}_FirstName");
            var lastName = session.GetString($"{UserSessionKey}_LastName");

            if (string.IsNullOrEmpty(email)) return null;

            return new User
            {
                Id = userId.Value,
                Email = email,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user from session");
            return null;
        }
    }

    public void ClearUser()
    {
        try
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove($"{UserSessionKey}_Id");
                session.Remove($"{UserSessionKey}_Email");
                session.Remove($"{UserSessionKey}_FirstName");
                session.Remove($"{UserSessionKey}_LastName");
                
                _logger.LogInformation("User session cleared");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing user session");
            throw;
        }
    }

    public bool IsUserLoggedIn()
    {
        return GetUser() != null;
    }
}