namespace FizzBuzzGame.API.Models;

public class GameResult
{
    public int Id { get; set; }
    public int Value { get; set; }
    public string UserInput { get; set; }
    public string ExpectedOutput { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime Timestamp { get; set; }
    public string? SessionId { get; set; } // To group results by game session
}