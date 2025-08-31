using FizzBuzzGame.API.Models;
using FizzBuzzGame.API.Models.DataManager;
using FizzBuzzGame.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FizzBuzzGame.API.Controllers;

/*
 The RulesController handles all relevant requests regarding the game rules. It
 - Mediates all CRUD functionality between the Client and the Rules Manager -> The Database
 */

[ApiController]
[Route("api/[controller]")]
public class RulesController(IRuleRepository<Rule, int> rulesRepository) : ControllerBase
{
    // There are two rule getting methods, GetRules returns all rules from the database. This endpoint is required
    // for the admin functionality.
    // GET api/rules
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rule>>> GetRules()
    {
        return Ok(await rulesRepository.GetAllRulesAsync());
    }

    // The 'active' endpoint only returns rules that are active to the game page. There is no need for the game to
    // retrieve all available rules.
    // GET api/rules/active
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Rule>>> GetActiveRules()
    {
        return Ok(await rulesRepository.GetActiveRulesAsync());
    }
    
    // The POST api/rules endpoint saves a new rule to the database.
    [HttpPost]
    public async Task<ActionResult<Rule>> AddRule([FromBody] Rule rule)
    {
        if (rule.Divisor <= 0) 
            return BadRequest("Divisor must be a positive number.");
        
        await rulesRepository.AddRuleAsync(rule);
        return CreatedAtAction(nameof(GetRule), new { id = rule.Id }, rule);
    }

    // The api/rules/{id} endpoint returns a rule by its ID. It is not used by the current frontend, but is left in here
    // in case the frontend gets re-designed to have edit form that gets filled from values in the databse.
    // GET /api/rules/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Rule>> GetRule(int id)
    {
        var rule = await rulesRepository.GetRuleByIdAsync(id);
        if (rule == null) return NotFound();
        return Ok(rule);
    }

    // The PUT endpoint receives a Rule object and updates the respective rule in the database.
    // PUT api/rules/1
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRule(int id, [FromBody] Rule updatedRule)
    {
        if (updatedRule.Divisor <= 0) 
            return BadRequest("Divisor must be a positive number.");
        
        var rule = await rulesRepository.GetRuleByIdAsync(id);
        
        if (rule == null) return NotFound();

        rule.Divisor = updatedRule.Divisor;
        rule.Text = updatedRule.Text;
        rule.IsActive = updatedRule.IsActive;
        await rulesRepository.UpdateRuleAsync(rule);
        return NoContent();
    }

    // It makes no sense having one rule left in the game which is not active. Therefore, the DeleteRule method
    // Automatically sets the last remaining rule in the database to active.
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRule(int id)
    {
        var rule = await rulesRepository.GetRuleByIdAsync(id);
        if (rule == null) return NotFound();
        
        // Now we check if the deletion would left us with no rules (validates requirements)
        if (await rulesRepository.GetRuleCountAsync() <= 1)
            return BadRequest("The game must have at least one rule.");
        
        // If more than one rule is left, we can go ahead and delete the rule.
        await rulesRepository.DeleteRuleAsync(id);

        // After the deletion we now check if there are any inactive rules left and set them active.
        var remainingRules = await rulesRepository.GetAllRulesAsync();

        if (remainingRules.Count == 1)
        {
            var lastRule =  remainingRules.First();
            lastRule.IsActive = true;
            await rulesRepository.UpdateRuleAsync(lastRule);
        }
        
        return NoContent();
    }
}