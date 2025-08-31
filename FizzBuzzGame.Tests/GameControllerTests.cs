using FizzBuzzGame.API.Controllers;
using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.DataManager;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class GameControllerTests : IDisposable
{
    private readonly FizzBuzzGameContext _context;
    private readonly GameController _controller;
    private readonly GameManager _gameManager;

    public GameControllerTests()
    {
        var options = new DbContextOptionsBuilder<FizzBuzzGameContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FizzBuzzGameContext(options);
        _gameManager = new GameManager(_context);
        _controller = new GameController(_gameManager);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Rules.AddRange(
            new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
            new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = true }
        );
        _context.SaveChanges();
    }

    [Fact]
    public void GetRandomNumber_ShouldReturnOkResultWithNumber()
    {
        // Act
        var result = _controller.GetRandomNumber();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeOfType<int>();
        ((int)okResult.Value).Should().BeInRange(1, 100);
    }

    [Fact]
    public async Task VerifyAnswer_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 3, Text = "Fizz" };

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().Be(true);
    }

    [Fact]
    public async Task VerifyAnswer_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 3, Text = null };

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task EndGame_ShouldReturnGameSessionResult()
    {
        // Arrange
        await _gameManager.VerifyAnswer(new UserInputTransferDto { Value = 3, Text = "Fizz" });
        await _gameManager.VerifyAnswer(new UserInputTransferDto { Value = 5, Text = "Buzz" });

        // Act
        var result = await _controller.EndGame();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<GameSessionResultDto>();
        result.TotalAttempts.Should().Be(2);
        result.CorrectAnswers.Should().Be(2);
        result.AccuracyPercentage.Should().Be(100);
        result.Results.Should().HaveCount(2);
    }

    [Fact]
    public async Task EndGame_WithNoGameData_ShouldReturnEmptyResult()
    {
        // Act
        var result = await _controller.EndGame();

        // Assert
        result.Should().NotBeNull();
        result.TotalAttempts.Should().Be(0);
        result.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task VerifyAnswer_ShouldPersistDataBetweenCalls()
    {
        // Arrange & Act
        await _controller.VerifyAnswer(new UserInputTransferDto { Value = 3, Text = "Fizz" });
        await _controller.VerifyAnswer(new UserInputTransferDto { Value = 7, Text = "7" });

        // Assert - Check that data persisted in database
        var storedResults = await _context.GameResults.CountAsync();
        storedResults.Should().Be(2);

        var endGameResult = await _controller.EndGame();
        endGameResult.TotalAttempts.Should().Be(2);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}