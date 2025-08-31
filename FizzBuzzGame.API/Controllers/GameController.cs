using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.API.Controllers;

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