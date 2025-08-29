using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FizzBuzzGame.API.Models;

public class Rule
{
    
    public int Id { get; set; }
    
    [Required(ErrorMessage = "A number is required")]
    public int Divisor { get; set; }
    
    [StringLength(10, MinimumLength = 2, ErrorMessage = "The length of the string must be between 2 and 10 characters")]
    public required string Text { get; set; }

    public bool IsActive { get; set; }
}