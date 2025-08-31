namespace FizzBuzzGame.API.Dtos;

/*
 * A data transfer object to store the game result for display.
 */

public class GameResultDto
{
    public int Value { get; set; }
    public string UserInput { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; }  = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime Timestamp { get; set; }
}