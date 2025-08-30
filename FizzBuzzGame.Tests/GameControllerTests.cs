namespace FizzBuzzGame.Tests;

public class GameControllerTests
{
    private readonly Mock<IRuleRepository> _mockRepo;
    private readonly GameController _controller;

    public GameControllerTests()
    {
        _mockRepo = new Mock<IRuleRepository>();
        _controller = new GameController(_mockRepo.Object);
    }

    [Fact]
    public async Task VerifyAnswer_CorrectFizzBuzz_ReturnsTrue()
    {
        // Arrange
        var rules = new List<Rule>
        {
            new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true },
            new Rule { Id = 2, Divisor = 5, Text = "Buzz", IsActive = true }
        };
        _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules);
        var request = new VerifyRequest { Number = 15, Answer = "FizzBuzz" };

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task VerifyAnswer_IncorrectAnswer_ReturnsFalse()
    {
        // Arrange
        var rules = new List<Rule>
        {
            new Rule { Id = 1, Divisor = 3, Text = "Fizz", IsActive = true }
        };
        _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(rules);
        var request = new VerifyRequest { Number = 3, Answer = "Buzz" };

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.False((bool)okResult.Value);
    }

    [Fact]
    public async Task VerifyAnswer_NumberIfNoRule_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetActiveRulesAsync()).ReturnsAsync(new List<Rule>());
        var request = new VerifyRequest { Number = 7, Answer = "7" };

        // Act
        var result = await _controller.VerifyAnswer(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);
    }
}