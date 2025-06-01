using AuthC_.DTOs;

namespace AuthC_.Services
{
    public interface IAuthService
    {
        Task<UserSignupResDTO> SignUpUser(UserSignupDTO userDto);
        Task<UserSigninResDTO> SignInUser(UserSigninDTO userSigninDTO);
        Task<bool> SignOut(int userId);
        Task<RefreshTokenResDTO> RefreshToken(string refreshToken);
    }
}
