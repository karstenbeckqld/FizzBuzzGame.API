using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Models.DataManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add SQLite Database service.
var connectionString = builder.Configuration.GetConnectionString("RulesDb");
builder.Services.AddSqlite<FizzBuzzGameContext>(connectionString);

// Add logging to the application.
builder.Services.AddLogging();

// Let the app know about the GameManager, RulesManager, ILogger and other required services.
builder.Services.AddScoped<RulesManager>();
builder.Services.AddSingleton<ILogger>(options =>
    options
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("DefaultLogger")
);

// Standard services for Web APIs.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()    // Allow all origins (*)
            .AllowAnyMethod()       // Allow all HTTP methods: GET, POST, etc.
            .AllowAnyHeader();      // Allow all headers (e.g., Content-Type)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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