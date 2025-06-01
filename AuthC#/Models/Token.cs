namespace AuthC_.Models;

public class Token
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string RefreshToken { get; set; }
    public required string ExpiresIn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? User { get; set; }
}
