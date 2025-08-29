using Microsoft.EntityFrameworkCore;

namespace FizzBuzzGame.API.Data;

// This class runs the migrations for the database when the application is started,
// to ensure the correct tables and fields are present.
public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FizzBuzzGameContext>();
        await dbContext.Database.MigrateAsync();
    }    
}