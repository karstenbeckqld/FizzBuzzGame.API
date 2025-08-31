# Fizz Buzz online Game by Karsten Beck 

The code in this repository contains the API for a Fizz Buzz online game. It is written in C# using .NET Core version 8.

The API uses the repository design pattern mediated by the IRuleRepository:

```csharp
public interface IRuleRepository<TEntity, TKey> where TEntity : class
{
Task<List<Rule>> GetAllRulesAsync();
Task<Rule?> GetRuleByIdAsync(int id);
Task AddRuleAsync(TEntity rule);
Task UpdateRuleAsync(TEntity rule);
Task DeleteRuleAsync(TKey id);
Task<TKey> GetRuleCountAsync();
Task<List<TEntity>> GetActiveRulesAsync();
}
```
And the IGameRepository

```csharp
public interface IGameRepository
{
    int GetRandomNumber();
    Task<bool> VerifyAnswer([FromBody] UserInputTransferDto request);
    Task<GameSessionResultDto> EndGameAndGetResults();
}
```

Both repositories are registered in Program.cs with their respective injectables RulesManager and GamesManager. This approach made it easier for testing as well.

As ORM we use Microsoft Entity Framework Core and a AQLite database to store the game rules and user responses. 
The database access is established via a dbContext. The 'Migrations' folder inside Data contains all required data migrations to build the schema of the SQLite database and initial data is seeded via the DbContext, or a dedicated Seed file, depending on the circumstances.

User responses are stored in a database table, however, the entries are deleted when the user stops the game. Simply to have the database not becoming to large.
The application doesn't require authentication or authorisation.

The API allows for game rules to be added, deleted or updated through an Admin portal in the Frontend.

It will check that there is always one rule in the DB and ensures to activate this last remaining rule.
The GameManager can compare user inputs to single entries (e.g. 'Fizz'), combined entries (e.g. 'FizzBuzzEven') and an entered number if no rule applies.

All user gameplay is saved in the DB and returned to the user at the end of the game.

The API can be launched with 'dotnet run'.

