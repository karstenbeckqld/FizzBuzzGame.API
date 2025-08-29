using FizzBuzzGame.API.Models;
using Microsoft.EntityFrameworkCore;


namespace FizzBuzzGame.API.Data;

public class FizzBuzzGameContext(DbContextOptions<FizzBuzzGameContext> options) : DbContext(options)
{
    public DbSet<Rule> Rules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rule>().HasKey(r => r.Id);
        
        modelBuilder.Entity<Rule>().HasData(
            new { Id = 1, Divisor = 5, Text = "Buzz", IsActive = true },
            new { Id = 2, Divisor = 3, Text = "Fizz", IsActive = false },
            new { Id = 3, Divisor = 15, Text = "FizzBuzz", IsActive = false }
        );

        Console.WriteLine("Seeding Data from OnModelCreating");
    }
}