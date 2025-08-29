namespace FizzBuzzGame.API.Models.Repository;

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