namespace AuthC_.Models;

public class UserSigninResDTO
{
    public required UserSignupResDTO User { get; set; }
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}   