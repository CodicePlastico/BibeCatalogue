using BibeCatalogue.Services;
using FluentMigrator.Runner;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configurazione sessioni
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "BibeCatalogue.Session";
});

// HttpContextAccessor per le sessioni
builder.Services.AddHttpContextAccessor();

// Registrazione servizi personalizzati
builder.Services.AddScoped<DatabaseConnectionService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<DatabaseMigrationService>();

// Configurazione FluentMigrator
builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

// Configurazione logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Non usare HTTPS redirection in Development quando si usa Docker
app.UseStaticFiles();

// Abilitazione sessioni
app.UseSession();

// Disabilita antiforgery per l'applicazione demo
// app.UseAntiforgery();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Home redirect to login
app.MapGet("/", () => Results.Redirect("/login"));

// Login page
app.MapGet("/login", async (HttpContext context) =>
{
    var loginTemplate = await File.ReadAllTextAsync("Templates/login.html");
    var errorMessage = "";
    var emailValue = "";
    
    if (context.Request.Query.ContainsKey("error"))
    {
        errorMessage = "<div class='alert alert-danger'><i class='bi bi-exclamation-triangle'></i> Credenziali non valide. Riprova.</div>";
    }
    
    if (context.Request.Query.ContainsKey("email"))
    {
        emailValue = context.Request.Query["email"].ToString();
    }
    
    var html = loginTemplate
        .Replace("{{ERROR_MESSAGE}}", errorMessage)
        .Replace("{{EMAIL_VALUE}}", emailValue);
    
    return Results.Content(html, "text/html");
});

// Login endpoint
app.MapPost("/login", async (HttpContext context, UserService userService, SessionService sessionService) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    
    var user = await userService.AuthenticateAsync(email, password);
    if (user != null)
    {
        sessionService.SetUser(user);
        return Results.Redirect("/courses");
    }
    
    return Results.Redirect($"/login?error=invalid&email={Uri.EscapeDataString(email)}");
});

// Courses page
app.MapGet("/courses", async (HttpContext context, SessionService sessionService, CourseService courseService) =>
{
    var currentUser = sessionService.GetUser();
    if (currentUser == null)
    {
        return Results.Redirect("/login");
    }
    
    // Get search parameter
    var searchTerm = context.Request.Query["search"].ToString();
    var isSearching = !string.IsNullOrWhiteSpace(searchTerm);
    
    // Get courses (filtered if search term provided)
    var courses = await courseService.GetUserCoursesAsync(currentUser.Id, searchTerm);
    var allCourses = isSearching ? await courseService.GetUserCoursesAsync(currentUser.Id) : courses;
    
    var coursesTemplate = await File.ReadAllTextAsync("Templates/courses.html");
    
    // Build courses HTML
    var coursesHtml = "";
    if (courses.Any())
    {
        coursesHtml = "<div class='row'>";
        foreach (var course in courses)
        {
            var resultClass = course.Result >= 18 ? "success" : "danger";
            var resultIcon = course.Result >= 18 ? "check-circle" : "x-circle";
            
            // Highlight search term in title if searching
            var displayTitle = course.Title;
            if (isSearching && !string.IsNullOrWhiteSpace(searchTerm))
            {
                displayTitle = course.Title.Replace(searchTerm, $"<mark>{searchTerm}</mark>", StringComparison.OrdinalIgnoreCase);
            }
            
            coursesHtml += $@"
                <div class='col-md-6 mb-3'>
                    <div class='card'>
                        <div class='card-body'>
                            <h5 class='card-title'>{displayTitle}</h5>
                            <p class='card-text'>
                                <small class='text-muted'>
                                    <i class='bi bi-calendar'></i> {course.StartDate:dd/MM/yyyy} - {course.EndDate:dd/MM/yyyy}
                                </small>
                            </p>
                            <div class='d-flex justify-content-between align-items-center'>
                                <span class='badge bg-{resultClass}'>
                                    <i class='bi bi-{resultIcon}'></i> Voto: {course.Result}/30
                                </span>
                                <small class='text-muted'>{course.Result} punti</small>
                            </div>
                        </div>
                    </div>
                </div>";
        }
        coursesHtml += "</div>";
    }
    else
    {
        if (isSearching)
        {
            coursesHtml = $@"
                <div class='alert alert-warning'>
                    <i class='bi bi-search'></i> Nessun corso trovato per '<strong>{searchTerm}</strong>'.
                    <br><small>Prova con un termine di ricerca diverso.</small>
                </div>";
        }
        else
        {
            coursesHtml = "<div class='alert alert-info'><i class='bi bi-info-circle'></i> Nessun corso trovato.</div>";
        }
    }
    
    // Build search info
    var searchInfo = "";
    if (isSearching)
    {
        var totalCourses = allCourses.Count();
        var foundCourses = courses.Count();
        searchInfo = $@"
            <div class='alert alert-light border'>
                <i class='bi bi-info-circle'></i> 
                Trovati <strong>{foundCourses}</strong> corsi su <strong>{totalCourses}</strong> per '<strong>{searchTerm}</strong>'
            </div>";
    }
    
    // Build clear button
    var clearButton = "";
    if (isSearching)
    {
        clearButton = @"
            <a href='/courses' class='btn btn-outline-secondary'>
                <i class='bi bi-x-circle'></i> Cancella
            </a>";
    }
    
    // Replace template placeholders
    var html = coursesTemplate
        .Replace("{{USER_NAME}}", $"{currentUser.FirstName} {currentUser.LastName}")
        .Replace("{{COURSES_CONTENT}}", coursesHtml)
        .Replace("{{SEARCH_VALUE}}", searchTerm ?? "")
        .Replace("{{SEARCH_INFO}}", searchInfo)
        .Replace("{{CLEAR_BUTTON}}", clearButton);
    
    return Results.Content(html, "text/html");
});

// Logout endpoint
app.MapGet("/logout", (HttpContext context, SessionService sessionService) =>
{
    sessionService.ClearUser();
    return Results.Redirect("/login");
});

// Keep test page for debugging
app.MapGet("/test", () => Results.File("wwwroot/test.html", "text/html"));

// HTML endpoints only - no Blazor components needed

// Esecuzione migrazioni database all'avvio
try
{
    using (var scope = app.Services.CreateScope())
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<DatabaseMigrationService>();
        await migrationService.MigrateAsync();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error running database migrations during startup");
    
    // In un ambiente di produzione, potresti voler fermare l'applicazione se le migrazioni falliscono
    // throw;
}

app.Run();