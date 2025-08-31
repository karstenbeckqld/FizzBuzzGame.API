namespace FizzBuzzGame.API.Dtos;

/*
 * A data transfer object to send the game stats to the client for display.
 */

public class GameSessionResultDto
{
    public List<GameResultDto> Results { get; set; } = new List<GameResultDto>();
    public int TotalAttempts { get; set; }
    public int CorrectAnswers { get; set; }
    public double AccuracyPercentage { get; set; }
    public string SessionId {get; set;} =  string.Empty;
}