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
    private readonly Mock<IGameController> _mockRepo;
    private readonly GameController _controller;

    public GameControllerTests(FizzBuzzGameContext context)
    {
        _mockRepo = new Mock<IGameController>();
        _controller = new GameController(context);
    }

    // [Fact]
    // public async Task VerifyAnswer_CorrectFizzBuzz_ReturnsTrue()
    // {
    //     // Arrange
    //     var rules = new List<Rule>
    //     {
    //         new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
    //         new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = true }
    //     };
    //     _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules);
    //     var request = new UserInputTransferDto { Value = 15, Text = "FizzBuzz" };
    //
    //     // Act
    //     var result = await _controller.VerifyAnswer(request);
    //
    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.True((bool)okResult.Value);
    // }
    //
    // [Fact]
    // public async Task VerifyAnswer_IncorrectAnswer_ReturnsFalse()
    // {
    //     // Arrange
    //     var rules = new List<Rule>
    //     {
    //         new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true }
    //     };
    //     _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules);
    //     var request = new UserInputTransferDto { Value = 3, Text = "Buzz" };
    //
    //     // Act
    //     var result = await _controller.VerifyAnswer(request);
    //
    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.False((bool)okResult.Value);
    // }
    //
    // [Fact]
    // public async Task VerifyAnswer_NumberIfNoRule_ReturnsTrue()
    // {
    //     // Arrange
    //     _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(new List<Rule>());
    //     var request = new UserInputTransferDto { Value = 7, Text = "7" };
    //
    //     // Act
    //     var result = await _controller.VerifyAnswer(request);
    //
    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.True((bool)okResult.Value);
    // }
    //
    // [Fact]
    // public async Task VerifyAnswer_UnsortedCommaSeparatedAnswer_ReturnsTrue()
    // {
    //     // Arrange
    //     var rules = new List<Rule>
    //     {
    //         new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
    //         new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = true }
    //     };
    //     _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules);
    //     var request = new UserInputTransferDto { Value = 15, Text = "Buzz,Fizz" }; // Unsorted, still valid
    //
    //     // Act
    //     var result = await _controller.VerifyAnswer(request);
    //
    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.True((bool)okResult.Value);
    // }
    //
    // [Fact]
    // public async Task VerifyAnswer_ExtraWhitespaceInAnswer_ReturnsTrue()
    // {
    //     // Arrange
    //     var rules = new List<Rule>
    //     {
    //         new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true }
    //     };
    //     _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules);
    //     var request = new UserInputTransferDto { Value = 3, Text = " Fizz " }; // Extra whitespace
    //
    //     // Act
    //     var result = await _controller.VerifyAnswer(request);
    //
    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.True((bool)okResult.Value);
    // }
    //
    // [Fact]
    // public async Task VerifyAnswer_EmptyAnswerWithRules_ReturnsFalse()
    // {
    //     // Arrange
    //     var rules = new List<Rule>
    //     {
    //         new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true }
    //     };
    //     _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules);
    //     var request = new UserInputTransferDto { Value = 3, Text = "" }; // Empty answer
    //
    //     // Act
    //     var result = await _controller.VerifyAnswer(request);
    //
    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.False((bool)okResult.Value);
    // }
}