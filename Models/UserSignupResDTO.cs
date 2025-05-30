namespace AuthC_.Models;

public class UserSignupResDTO
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string DisplayName => $"{FirstName} {LastName}";
}