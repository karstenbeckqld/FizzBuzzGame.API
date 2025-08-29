using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.DataManager;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RulesController(RulesManager rulesManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rule>>> GetRules()
    {
        return Ok(await rulesManager.GetAllRulesAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Rule>> AddRule([FromBody] Rule rule)
    {
        if (rule.Divisor <= 0) return BadRequest("Divisor must be positive.");
        await rulesManager.AddRuleAsync(rule);
        return CreatedAtAction(nameof(GetRule), new { id = rule.Id }, rule);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Rule>> GetRule(int id)
    {
        var rule = await rulesManager.GetRuleByIdAsync(id);
        if (rule == null) return NotFound();
        return Ok(rule);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRule(int id, [FromBody] Rule updatedRule)
    {
        var rule = await rulesManager.GetRuleByIdAsync(id);
        if (rule == null) return NotFound();

        rule.Divisor = updatedRule.Divisor > 0 ? updatedRule.Divisor : rule.Divisor;
        rule.Text = updatedRule.Text ?? rule.Text;
        rule.IsActive = updatedRule.IsActive;
        await rulesManager.UpdateRuleAsync(rule);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRule(int id)
    {
        var rule = await rulesManager.GetRuleByIdAsync(id);
        if (rule == null) return NotFound();

        if (await rulesManager.GetRuleCountAsync() <= 1)
            return BadRequest("At least one rule required.");

        await rulesManager.DeleteRuleAsync(id);
        return NoContent();
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Rule>>> GetActiveRules()
    {
        return Ok(await rulesManager.GetActiveRulesAsync());
    }
}