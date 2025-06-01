namespace AuthC_.Models;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Hash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Token>? Tokens { get; set; }
}