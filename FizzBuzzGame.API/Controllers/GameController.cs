using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.API.Controllers;

/*
 The GameController handles all relevant requests regarding the game play. It
 - Passes the randomly generated Number (between 1 and 100) to the frontend.
 - It receives the user input.
 - It arranges for ending the game. 
 */

[ApiController]
[Route("api/[controller]")]
public class GameController(IGameRepository repository) : ControllerBase
{
    [HttpGet("random")]
    public ActionResult<int> GetRandomNumber()
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

    [HttpGet("end-game")]
    public async Task<GameSessionResultDto> EndGame()
    {
        return await repository.EndGameAndGetResults();
    }
}