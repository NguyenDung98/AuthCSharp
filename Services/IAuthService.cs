using AuthC_.DTOs;
using AuthC_.Models;

namespace AuthC_.Services
{
    public interface IAuthService
    {
        Task<UserSignupResDTO> SignUpUser(UserSignupDTO userDto);
        Task<UserSigninResDTO> SignInUser(UserSigninDTO userSigninDTO);
        Task<bool> SignOut(int userId);
        Task<string> RefreshToken(string refreshToken);
    }
}
