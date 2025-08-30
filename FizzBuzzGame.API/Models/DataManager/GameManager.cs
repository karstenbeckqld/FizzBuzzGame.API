using FizzBuzzGame.API.Controllers;
using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FizzBuzzGame.API.Models.DataManager;

public class GameManager(FizzBuzzGameContext context) : IGameRepository
{
    public int GetRandomNumber()
    {
        var random = new Random();
        return random.Next(1, 101);
    }

    public async Task<bool> VerifyAnswer(UserInputTransferDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text))
            throw new ArgumentException("Input cannot be null or empty");

        var activeRules = await context.Rules
            .Where(r => r.IsActive)
            .OrderBy(r => r.Id)
            .ToListAsync();

        var expected = string.Empty;

        foreach (var rule in activeRules)
        {
            if (request.Value % rule.Divisor == 0)
            {
                expected += rule.Text;
            }
        }

        // If no rules apply, expect the number as a string.
        if (string.IsNullOrEmpty(expected))
        {
            expected = request.Value.ToString();
        }
        

        // Compare user's input to the expected output (case-insensitive)
        return string.Equals(request.Text, expected, StringComparison.OrdinalIgnoreCase);
    }
}