namespace FizzBuzzGame.API.Dtos;

public class GameResultDto
{
    public int Value { get; set; }
    public string UserInput { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; }  = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime Timestamp { get; set; }
}