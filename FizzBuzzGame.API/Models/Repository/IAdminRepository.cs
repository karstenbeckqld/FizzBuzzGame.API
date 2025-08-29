namespace FizzBuzzGame.API.Models.Repository;

public interface IAdminRepository<TEntity, TKey> where TEntity : class
{
    List<TEntity> GetCurrentRuleSet();
    TEntity AddNewRule(TEntity rule);
    TKey DeleteRule(TEntity rule);
    TKey UpdateRule(TEntity rule);
    
}