namespace FizzBuzzGame.Tests;

public class RulesControllerTests
{
    private readonly Mock<IRuleRepository> _mockRepo;
    private readonly RulesController _controller;

    public RulesControllerTests()
    {
        _mockRepo = new Mock<IRuleRepository>();
        _controller = new RulesController(_mockRepo.Object);
    }

    [Fact]
    public async Task GetRules_ReturnsAllRules()
    {
        // Arrange
        var rules = new List<Rule>
        {
            new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
            new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = true }
        };
        _mockRepo.Setup(repo => repo.GetAllRulesAsync()).ReturnsAsync(rules);

        // Act
        var result = await _controller.GetRules();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnRules = Assert.IsType<List<Rule>>(okResult.Value);
        Assert.Equal(2, returnRules.Count);
    }

    [Fact]
    public async Task AddRule_ValidRule_ReturnsCreated()
    {
        // Arrange
        var rule = new Rule { Id = 1, Divisor = 7, Text = "Boo", IsActive = true };
        _mockRepo.Setup(repo => repo.AddRuleAsync(It.IsAny<Rule>())).Callback<Rule>(r => r.Id = 1);

        // Act
        var result = await _controller.AddRule(rule);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(rule, createdResult.Value);
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
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteRule_LastRule_ReturnsBadRequest()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetRuleByIdAsync(1)).ReturnsAsync(new Rule { Id = 1 });
        _mockRepo.Setup(repo => repo.GetRuleCountAsync()).ReturnsAsync(1);

        // Act
        var result = await _controller.DeleteRule(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}