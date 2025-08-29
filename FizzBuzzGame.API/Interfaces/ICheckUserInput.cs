using FizzBuzzGame.API.Dtos;

namespace FizzBuzzGame.API.Interfaces;

public interface ICheckUserInput
{
    bool Check(UserInputTransferDto  userInput);
}