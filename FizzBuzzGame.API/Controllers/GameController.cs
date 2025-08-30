using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.API.Controllers;

// NOTES:
// Have server send a random number to frontend.
// GameController receives response from player. 
// GameController has method to compare answer with correct result from DB through %-operator.
// So the GameController must know what the current rules are, and compare the answer to these rules. 
// BONUS: Answers and results (correct/incorrect) should be written to a DB and pulled at the end of the game. See if we can implement some SOLID principles here and have a DbWriter Interface or so.

[ApiController]
[Route("api/[controller]")]
public class GameController(IGameRepository repository) : ControllerBase
{
    [HttpGet("random")]
    public ActionResult<int> GetRandomRule()
    {
        // Returns a random integer value between positive 1 - 100
        return Ok(repository.GetRandomNumber());
    }

    [HttpPost("verify")]
    public async Task<ActionResult<bool>> VerifyAnswer([FromBody] UserInputTransferDto request)
    {
        try
        {
            return Ok(await repository.VerifyAnswer(request));
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }
}