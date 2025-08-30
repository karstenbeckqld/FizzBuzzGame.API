using FizzBuzzGame.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.API.Controllers;

public interface IGameController
{
    ActionResult<int> GetRandomRule();
    Task<ActionResult<bool>> VerifyAnswer([FromBody] UserInputTransferDto request);
}