using FizzBuzzGame.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.API.Models.Repository;

public interface IGameRepository
{
    int GetRandomNumber();
    Task<bool> VerifyAnswer([FromBody] UserInputTransferDto request);
}