using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.DataManager;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FizzBuzzGame.Tests;

public class GameManagerTests : IDisposable
{
    private readonly FizzBuzzGameContext _context;
    private readonly GameManager _gameManager;

    public GameManagerTests()
    {
        var options = new DbContextOptionsBuilder<FizzBuzzGameContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FizzBuzzGameContext(options);
        _gameManager = new GameManager(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Rules.AddRange(
            new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
            new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = true },
            new Rule { Id = 3, Divisor = 2, Text = "Even", IsActive = true }
        );
        _context.SaveChanges();
    }

    [Fact]
    public void GetRandomNumber_ShouldReturnNumberBetween1And100()
    {
        // Act
        var result = _gameManager.GetRandomNumber();

        // Assert
        result.Should().BeInRange(1, 100);
    }

    [Fact]
    public async Task VerifyAnswer_WithNullRequest_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _gameManager.VerifyAnswer(null));
    }

    [Fact]
    public async Task VerifyAnswer_WithEmptyText_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 15, Text = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _gameManager.VerifyAnswer(request));
    }

    [Fact]
    public async Task VerifyAnswer_WithCorrectFizzAnswer_ShouldReturnTrue()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 3, Text = "Fizz" };

        // Act
        var result = await _gameManager.VerifyAnswer(request);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyAnswer_WithCorrectBuzzAnswer_ShouldReturnTrue()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 5, Text = "Buzz" };

        // Act
        var result = await _gameManager.VerifyAnswer(request);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyAnswer_WithCorrectFizzBuzzAnswer_ShouldReturnTrue()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 15, Text = "FizzBuzz" };

        // Act
        var result = await _gameManager.VerifyAnswer(request);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyAnswer_WithCorrectNumericAnswer_ShouldReturnTrue()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 7, Text = "7" };

        // Act
        var result = await _gameManager.VerifyAnswer(request);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyAnswer_WithIncorrectAnswer_ShouldReturnFalse()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 3, Text = "Buzz" };

        // Act
        var result = await _gameManager.VerifyAnswer(request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyAnswer_ShouldStoreResultInDatabase()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 3, Text = "Fizz" };

        // Act
        await _gameManager.VerifyAnswer(request);

        // Assert
        var storedResult = await _context.GameResults.FirstOrDefaultAsync();
        storedResult.Should().NotBeNull();
        storedResult.Value.Should().Be(3);
        storedResult.UserInput.Should().Be("Fizz");
        storedResult.ExpectedOutput.Should().Be("Fizz");
        storedResult.IsCorrect.Should().BeTrue();
        storedResult.SessionId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task VerifyAnswer_CaseInsensitive_ShouldReturnTrue()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 3, Text = "fizz" };

        // Act
        var result = await _gameManager.VerifyAnswer(request);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetGameResultsFromDatabase_WithStoredResults_ShouldReturnCorrectStats()
    {
        // Arrange
        await _gameManager.VerifyAnswer(new UserInputTransferDto { Value = 6, Text = "FizzEven" });
        await _gameManager.VerifyAnswer(new UserInputTransferDto { Value = 5, Text = "Buzz" });
        await _gameManager.VerifyAnswer(new UserInputTransferDto { Value = 7, Text = "Wrong" });

        // Act
        var results = await _gameManager.EndGameAndGetResults();

        // Assert
        results.Should().NotBeNull();
        results.TotalAttempts.Should().Be(3);
        results.CorrectAnswers.Should().Be(2);
        results.AccuracyPercentage.Should().BeApproximately(66.67, 0.01);
        results.Results.Should().HaveCount(3);
        results.SessionId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task EndGameAndGetResults_ShouldDeleteResultsFromDatabase()
    {
        // Arrange
        await _gameManager.VerifyAnswer(new UserInputTransferDto { Value = 3, Text = "FizzEven" });

        // Act
        await _gameManager.EndGameAndGetResults();

        // Assert
        var remainingResults = await _context.GameResults.CountAsync();
        remainingResults.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}