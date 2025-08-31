namespace FizzBuzzGame.API.Dtos;

/*
 * A Data transfer object for the user input during gameplay
 */

public class UserInputTransferDto
{
    public int Value { get; set; }
    public string Text { get; set; } = string.Empty;
}