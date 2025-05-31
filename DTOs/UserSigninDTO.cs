using System.ComponentModel.DataAnnotations;

namespace AuthC_.DTOs;

public class UserSigninDTO
{
    [Required]
    [EmailAddress]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }
    [Required]
    [MinLength(8)]
    [MaxLength(20)]
    public required string Password { get; set; }
}