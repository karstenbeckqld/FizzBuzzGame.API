using FizzBuzzGame.API.Controllers;
using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.DataManager;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Standard services for Web APIs.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SQLite Database service.
builder.Services.AddDbContext<FizzBuzzGameContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RulesDb")));

// var connectionString = builder.Configuration.GetConnectionString("RulesDb");
// builder.Services.AddSqlite<FizzBuzzGameContext>(connectionString);

// Add HTTP context accessor for session access.
builder.Services.AddHttpContextAccessor();

// Add distributed memory cache for session storage.
builder.Services.AddDistributedMemoryCache();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add logging to the application.
builder.Services.AddLogging();

// Let the app know about the GameManager, RulesManager, ILogger and other required services.
builder.Services.AddScoped<IRuleRepository<Rule, int>, RulesManager>();
builder.Services.AddScoped<IGameRepository, GameManager>();
builder.Services.AddSingleton<ILogger>(options =>
    options
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("DefaultLogger")
);

// Enable CORS to give frontend access to the API. Could be more restricted, but as we still operate in dev environments,
// the frontend URL is not known in advance.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", options =>
    {
        options.AllowAnyOrigin() // Allow all origins (*)
            .AllowAnyMethod() // Allow all HTTP methods: GET, POST, etc.
            .AllowAnyHeader(); // Allow all headers (e.g., Content-Type)
    });
});


var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

// Ensure the app finds the correct routes and controllers for requests.
app.MapControllers();

// Run a database migration when the application starts, to ensure the correct schema is in place.
await app.MigrateDbAsync();

// Seed data programmatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception e)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(e, "Error while seeding database");
        throw;
    }
}

// Enable CORS Middleware
app.UseCors("AllowAllOrigins");

app.Run();