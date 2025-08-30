using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Interfaces;
using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.DataManager;
using FizzBuzzGame.API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FizzBuzzGame.API.Controllers;

// Have server send a random number to frontend.
// GameController receives response from player. 
// GameController has method to compare answer with correct result from DB through %-operator.
// So the GameController must know what the current rules are, and compare the answer to these rules. 
// Answers and results (correct/incorrect) should be written to a DB and pulled at the end of the game. See if we can implement some SOLID principles here and have a DbWriter Interface or so.

[ApiController]
[Route("api/[controller]")]
public class GameController(FizzBuzzGameContext context) : ControllerBase, IGameController
{
    [HttpGet("random")]
    public ActionResult<int> GetRandomRule()
    {
        // Returns a random integer value between positive 1 - 1000
        var random = new Random();
        return Ok(random.Next(1, 1001));
    }

    [HttpPost("verify")]
    public async Task<ActionResult<bool>> VerifyAnswer([FromBody] UserInputTransferDto request)
    {
        Console.WriteLine(request.Value);
        Console.WriteLine(request.Text);

        var result = false;
        
        var activeRules = await context.Rules
            .Where(r => r.IsActive)
            .OrderBy(r => r.Id)
            .ToListAsync();

        var expected = "";

        foreach (var rule in activeRules)
        {
            if (request.Value % rule.Divisor == 0)
            {
                expected += rule.Text;
            }
        }
        
        result = !string.IsNullOrEmpty(expected);
        //expected = string.IsNullOrEmpty(expected) ? request.Value.ToString() : expected;
        Console.WriteLine(result);
        return Ok(result);
    }
}