using Moq;
using FizzBuzzGame.API.Controllers;
using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.Repository;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.Tests;

public class RulesControllerTests
{
    private readonly Mock<IRuleRepository<Rule,int>> _mockRepo;
    private readonly RulesController _controller;

    public RulesControllerTests()
    {
        _mockRepo = new Mock<IRuleRepository<Rule, int>>();
        _controller = new RulesController(_mockRepo.Object);
    }

    [Fact]
    public async Task GetRules_ReturnsAllRules()
    {
        // Arrange
        var rules = new List<Rule>
        {
            new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
            new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = false }
        };
        _mockRepo.Setup(repo => repo.GetAllRulesAsync()).ReturnsAsync(rules);

        // Act
        var result = await _controller.GetRules();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Rule>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnRules = Assert.IsType<List<Rule>>(okResult.Value);
        Assert.Equal(2, returnRules.Count);
        Assert.Contains(returnRules, r => r.Id == 1 && r.Text == "Fizz");
        Assert.Contains(returnRules, r => r.Id == 2 && r.Text == "Buzz");
    }

    [Fact]
    public async Task GetActiveRules_ReturnsOnlyActiveRules()
    {
        // Arrange
        var rules = new List<Rule>
        {
            new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
            new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = false }
        };
        _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules.Where(r => r.IsActive).ToList());

        // Act
        var result = await _controller.GetActiveRules();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Rule>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnRules = Assert.IsType<List<Rule>>(okResult.Value);
        Assert.Single(returnRules);
        Assert.Equal("Fizz", returnRules[0].Text);
    }

    [Fact]
    public async Task AddRule_ValidRule_ReturnsCreated()
    {
        // Arrange
        var rule = new Rule { Id = 1, Divisor = 7, Text = "Boo", IsActive = true };
        _mockRepo.Setup(repo => repo.AddRuleAsync(It.IsAny<Rule>())).Callback<Rule>(r => r.Id = 1);
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync(rule);

        // Act
        var result = await _controller.AddRule(rule);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Rule>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var returnRule = Assert.IsType<Rule>(createdResult.Value);
        Assert.Equal(rule, returnRule);
        Assert.Equal(nameof(_controller.GetRule), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues["id"]);
        _mockRepo.Verify(repo => repo.AddRuleAsync(It.IsAny<Rule>()), Times.Once());
    }

    [Fact]
    public async Task AddRule_NegativeDivisor_ReturnsBadRequest()
    {
        // Arrange
        var rule = new Rule { Divisor = -1, Text = "Boo" };

        // Act
        var result = await _controller.AddRule(rule);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Rule>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        // var error = badRequestResult.Value as dynamic;
        // Assert.NotNull(error);
        Assert.Equal("Divisor must be a positive number.", badRequestResult.Value);
        _mockRepo.Verify(repo => repo.AddRuleAsync(It.IsAny<Rule>()), Times.Never());
    }

    [Fact]
    public async Task AddRule_ZeroDivisor_ReturnsBadRequest()
    {
        // Arrange
        var rule = new Rule { Divisor = 0, Text = "Boo" };

        // Act
        var result = await _controller.AddRule(rule);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Rule>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Divisor must be a positive number.", badRequestResult.Value);
        _mockRepo.Verify(repo => repo.AddRuleAsync(It.IsAny<Rule>()), Times.Never());
    }

    [Fact]
    public async Task GetRule_ValidId_ReturnsRule()
    {
        // Arrange
        var rule = new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true };
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync(rule);

        // Act
        var result = await _controller.GetRule(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Rule>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnRule = Assert.IsType<Rule>(okResult.Value);
        Assert.Equal(rule, returnRule);
    }

    [Fact]
    public async Task GetRule_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync((Rule?)null);

        // Act
        var result = await _controller.GetRule(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Rule>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task UpdateRule_ValidIdAndRule_ReturnsNoContent()
    {
        // Arrange
        var existingRule = new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true };
        var updatedRule = new Rule { Divisor = 7, Text = "Boo", IsActive = false };
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync(existingRule);
        _mockRepo.Setup(repo => repo.UpdateRuleAsync(It.IsAny<Rule>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateRule(1, updatedRule);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepo.Verify(repo => repo.UpdateRuleAsync(It.Is<Rule>(r => 
            r.Divisor == 7 && 
            r.Text == "Boo" && 
            !r.IsActive)), Times.Once());
    }

    [Fact]
    public async Task UpdateRule_NegativeDivisor_ReturnsBadRequest()
    {
        // Arrange
        var updatedRule = new Rule { Divisor = -1, Text = "Boo" };

        // Act
        var result = await _controller.UpdateRule(1, updatedRule);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var error = badRequestResult.Value as dynamic;
        Assert.NotNull(error);
        Assert.Equal("Divisor must be a positive number.", badRequestResult.Value);
        _mockRepo.Verify(repo => repo.UpdateRuleAsync(It.IsAny<Rule>()), Times.Never());
    }

    [Fact]
    public async Task UpdateRule_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var updatedRule = new Rule { Divisor = 7, Text = "Boo", IsActive = true };
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync((Rule?)null);

        // Act
        var result = await _controller.UpdateRule(1, updatedRule);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockRepo.Verify(repo => repo.UpdateRuleAsync(It.IsAny<Rule>()), Times.Never());
    }

    [Fact]
    public async Task DeleteRule_ValidId_ReturnsNoContent()
    {
        // Arrange
        var rule = new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true };
        var remainingRules = new List<Rule>
        {
            new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = true },
            new Rule { Id = 3, Divisor = 7, Text = "Boo", IsActive = true } // Added a second rule
        };
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync(rule);
        _mockRepo.Setup(repo => repo.GetRuleCountAsync()).ReturnsAsync(3); // Initial count: 3 rules
        _mockRepo.Setup(repo => repo.GetAllRulesAsync()).ReturnsAsync(remainingRules);
        _mockRepo.Setup(repo => repo.DeleteRuleAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteRule(1);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        _mockRepo.Verify(repo => repo.DeleteRuleAsync(1), Times.Once());
        _mockRepo.Verify(repo => repo.UpdateRuleAsync(It.IsAny<Rule>()), Times.Never());
    }

    [Fact]
    public async Task DeleteRule_LastRule_ReturnsBadRequest()
    {
        // Arrange
        var rule = new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true };
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync(rule);
        _mockRepo.Setup(repo => repo.GetRuleCountAsync()).ReturnsAsync(1);

        // Act
        var result = await _controller.DeleteRule(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The game must have at least one rule.", badRequestResult.Value);
        _mockRepo.Verify(repo => repo.DeleteRuleAsync(It.IsAny<int>()), Times.Never());
    }

    [Fact]
    public async Task DeleteRule_OnlyInactiveRule_SetsLastRuleActive()
    {
        // Arrange
        var ruleToDelete = new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true };
        var lastRule = new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = false };
        var remainingRules = new List<Rule> { lastRule };
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync(ruleToDelete);
        _mockRepo.Setup(repo => repo.GetRuleCountAsync()).ReturnsAsync(2);
        _mockRepo.Setup(repo => repo.GetAllRulesAsync()).ReturnsAsync(remainingRules);
        _mockRepo.Setup(repo => repo.DeleteRuleAsync(1)).Returns(Task.CompletedTask);
        _mockRepo.Setup(repo => repo.UpdateRuleAsync(It.IsAny<Rule>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteRule(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepo.Verify(repo => repo.DeleteRuleAsync(1), Times.Once());
        _mockRepo.Verify(repo => repo.UpdateRuleAsync(It.Is<Rule>(r => r.Id == 2 && r.IsActive == true)), Times.Once());
    }

    [Fact]
    public async Task DeleteRule_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync((Rule?)null);

        // Act
        var result = await _controller.DeleteRule(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockRepo.Verify(repo => repo.DeleteRuleAsync(It.IsAny<int>()), Times.Never());
    }
}