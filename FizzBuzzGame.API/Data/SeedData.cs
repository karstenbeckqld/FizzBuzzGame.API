using FizzBuzzGame.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FizzBuzzGame.API.Data;

public static class SeedData
{
    private static readonly List<Rule> _rules =
    [
        new() { Id = 1, Divisor = 5, Text = "Buzz", IsActive = true },
        new() { Id = 2, Divisor = 3, Text = "Fizz", IsActive = false },
        new() { Id = 3, Divisor = 15, Text = "FizzBuzz", IsActive = false }
    ];

    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<FizzBuzzGameContext>();

        try
        {
            // Check if data already exists to avoid duplicates
            if (!dbContext.Rules.Any())
            {
                dbContext.Rules.AddRange(_rules);
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Database seeded successfully.");
            }
            else
            {
                Console.WriteLine("Database already contains data, skipping seeding.");
            }
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.LogError(ex, "An error occurred seeding database");
            Console.WriteLine("Error seeding database: " + ex.Message);
        }
    }
}