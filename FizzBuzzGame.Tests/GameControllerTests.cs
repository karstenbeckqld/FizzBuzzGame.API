using Moq;
using FizzBuzzGame.API.Controllers;
using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.Repository;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.Tests;

public class GameControllerTests
{
    private readonly Mock<IGameRepository> _mockRepo;
    private readonly GameController _controller;

    public GameControllerTests()
    {
        _mockRepo = new Mock<IGameRepository>();
        _controller = new GameController(_mockRepo.Object);
    }

    [Fact]
    public Task GetRandomRule_ReturnsNumberBetween1And100()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetRandomNumber()).Returns(42);

        // Act
        var result = _controller.GetRandomRule();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<int>(okResult.Value);
        Assert.InRange(value, 1, 1000);
        _mockRepo.Verify(repo => repo.GetRandomNumber(), Times.Once());
        return Task.CompletedTask;
    }

    [Fact]
    public async Task VerifyAnswer_CorrectAnswerWithRules_ReturnsTrue()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 15, Text = "FizzBuzz" };
        _mockRepo.Setup(repo => repo.VerifyAnswer(request)).ReturnsAsync(true);

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var value = Assert.IsType<bool>(okResult.Value);
        Assert.True(value);
        _mockRepo.Verify(repo => repo.VerifyAnswer(request), Times.Once());
    }

    [Fact]
    public async Task VerifyAnswer_CorrectAnswerNoRulesApply_ReturnsTrue()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 7, Text = "7" };
        _mockRepo.Setup(repo => repo.VerifyAnswer(request)).ReturnsAsync(true);

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var value = Assert.IsType<bool>(okResult.Value);
        Assert.True(value);
        _mockRepo.Verify(repo => repo.VerifyAnswer(request), Times.Once());
    }

    [Fact]
    public async Task VerifyAnswer_IncorrectAnswer_ReturnsFalse()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 15, Text = "Fizz" };
        _mockRepo.Setup(repo => repo.VerifyAnswer(request)).ReturnsAsync(false);

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var value = Assert.IsType<bool>(okResult.Value);
        Assert.False(value);
        _mockRepo.Verify(repo => repo.VerifyAnswer(request), Times.Once());
    }

    [Fact]
    public async Task VerifyAnswer_NullInput_ReturnsBadRequest()
    {
        // Arrange
        UserInputTransferDto request = null;
        _mockRepo.Setup(repo => repo.VerifyAnswer(request))
            .ReturnsAsync(new BadRequestObjectResult(new { Message = "Input cannot be null or empty." }));

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var error = badRequestResult.Value as dynamic;
        Assert.NotNull(error);
        Assert.Equal("Input cannot be null or empty.", badRequestResult.Value);
        _mockRepo.Verify(repo => repo.VerifyAnswer(request), Times.Once());
    }

    [Fact]
    public async Task VerifyAnswer_EmptyText_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = 15, Text = "" };
        _mockRepo.Setup(repo => repo.VerifyAnswer(request))
            .ReturnsAsync(new ArgumentException(new { Message = "Input cannot be null or empty." }));

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var error = badRequestResult.Value as dynamic;
        Assert.NotNull(error);
        Assert.Equal("Input cannot be null or empty.", error.Message.ToString());
        _mockRepo.Verify(repo => repo.VerifyAnswer(request), Times.Once());
    }
    
    [Fact]
    public async Task VerifyAnswer_NegativeValue_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserInputTransferDto { Value = -15, Text = "Fizz" };
        _mockRepo.Setup(repo => repo.VerifyAnswer(request)).ReturnsAsync(new BadRequestObjectResult(new { Message = "Value must be positive." }));

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<bool>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var error = badRequestResult.Value as dynamic;
        Assert.NotNull(error);
        Assert.Equal("Value must be positive.", badRequestResult.Value);
        _mockRepo.Verify(repo => repo.VerifyAnswer(request), Times.Once());
    }
}