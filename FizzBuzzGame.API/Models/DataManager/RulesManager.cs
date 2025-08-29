using FizzBuzzGame.API.Data;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FizzBuzzGame.API.Models.DataManager;

public class RulesManager(FizzBuzzGameContext context) : IRuleRepository<Rule, int>
{
    public FizzBuzzGameContext GetContext()
    {
        return context;
    }

    public async Task<List<Rule>> GetAllRulesAsync()
    {
        return await context.Rules.ToListAsync();
    }

    public async Task<Rule?> GetRuleByIdAsync(int id)
    {
        return await context.Rules.FindAsync(id);
    }

    public async Task AddRuleAsync(Rule rule)
    {
        context.Rules.Add(rule);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRuleAsync(Rule rule)
    {
        context.Rules.Update(rule);
        await context.SaveChangesAsync();
    }

    public async Task DeleteRuleAsync(int id)
    {
        var rule = await context.Rules.FindAsync(id);
        if (rule != null)
        {
            context.Rules.Remove(rule);
            await context.SaveChangesAsync();
        }
    }

    public async Task<int> GetRuleCountAsync()
    {
        return await context.Rules.CountAsync();
    }

    public async Task<List<Rule>> GetActiveRulesAsync()
    {
        return await context.Rules
            .Where(r => r.IsActive)
            .OrderBy(r => r.Id)
            .ToListAsync();
    }
}