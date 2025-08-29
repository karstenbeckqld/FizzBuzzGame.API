using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Dtos;
using FizzBuzzGame.API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FizzBuzzGame.API.Utilities;

public class CheckUserInput(FizzBuzzGameContext context) : ICheckUserInput
{
    public bool Check(UserInputTransferDto userInput)
    {
        var result = true;
        
        var number = userInput.Value;

        // Retrieve all active rules from the database.
        var activeRules = context.Rules
            .Where(r => r.IsActive == true)
            .Select(r => r.Text)
            .ToList();

        // Split the user input into individual words and remove empty entries as well as whitespace.
        var userWords = userInput.Text
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Trim())
            .Where(word => !string.IsNullOrEmpty(word))
            .ToList();

        foreach (var word in userWords)
        {
            
        }
        
        var allWordsAreValid = userWords.All(word =>
            activeRules.Contains(word, StringComparer.OrdinalIgnoreCase));

        if (!allWordsAreValid)
        {
            result = false;
        }

        Console.WriteLine("CheckUserInput result: " + result);
        
        return result;
    }
}