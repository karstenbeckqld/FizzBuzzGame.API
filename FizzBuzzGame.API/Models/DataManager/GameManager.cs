using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace FizzBuzzGame.API.Models.DataManager;

public class GameManager(FizzBuzzGameContext context) : IGameRepository
{
    // Generate a static sessionId per instance of GameManager 
    private static readonly string SessionId = Guid.NewGuid().ToString();

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
        var result = string.Equals(request.Text, expected, StringComparison.OrdinalIgnoreCase);

        // Store the result
        var gameResult = new GameResult
        {
            Value = request.Value,
            UserInput = request.Text,
            ExpectedOutput = expected,
            IsCorrect = result,
            Timestamp = DateTime.UtcNow,
            SessionId = GetSessionId(),
        };

        context.GameResults.Add(gameResult);
        await context.SaveChangesAsync();

        Console.WriteLine($"Storing: " +
                          $"Value={gameResult.Value}, " +
                          $"UserInput='{gameResult.UserInput}', " +
                          $"Expected='{gameResult.ExpectedOutput}', " +
                          $"Correct={gameResult.IsCorrect}");

        return result;
    }

    public async Task<GameSessionResultDto> EndGameAndGetResults()
    {
        var results = await GetGameResultsFromDatabase();

        // Not bloat the database, we delete the results after they've been retrieved.
        var sessionId = GetSessionId();
        var sessionResults = await context.GameResults
            .Where(gr => gr.SessionId == sessionId)
            .ToListAsync();

        context.GameResults.RemoveRange(sessionResults);
        await context.SaveChangesAsync();

        // Now we return the results here.
        return results;
    }

    private async Task<GameSessionResultDto> GetGameResultsFromDatabase()
    {
        var sessionId = GetSessionId();

        var sessionResults = await context.GameResults
            .Where(gr => gr.SessionId == sessionId)
            .OrderBy(gr => gr.Timestamp)
            .ToListAsync();

        var totalAttempts = sessionResults.Count;
        var correctAnswers = sessionResults.Count(r => r.IsCorrect);

        return new GameSessionResultDto
        {
            SessionId = GetSessionId(),
            Results = sessionResults.Select(sr => new GameResultDto
            {
                Value = sr.Value,
                UserInput = sr.UserInput,
                ExpectedOutput = sr.ExpectedOutput,
                IsCorrect = sr.IsCorrect,
                Timestamp = sr.Timestamp
            }).ToList(),
            TotalAttempts = totalAttempts,
            CorrectAnswers = correctAnswers,
            AccuracyPercentage = totalAttempts > 0 ? (double)correctAnswers / totalAttempts * 100 : 0
        };
    }

    private static string GetSessionId()
    {
        return SessionId;
    }
}