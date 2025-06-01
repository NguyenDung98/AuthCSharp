using AuthC_.Data;
using AuthC_.DTOs;
using AuthC_.Helpers;
using AuthC_.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthC_.Services
{
    public class AuthService(AppDbContext appDbContext, JwtHelper jwtHelper) : IAuthService
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly JwtHelper _jwtHelper = jwtHelper;
        private readonly int RefreshTokenExpireDays = 30;

        // Implementation of the IAuthService methods would go here.
        public async Task<UserSignupResDTO> SignUpUser(UserSignupDTO userSignupDTO)
        {
            // Logic for signing up a user
            // Check if the email is available for signup
            if (_appDbContext.Users.Any(u => u.Email == userSignupDTO.Email))
            {
                throw new InvalidOperationException("Email is already in use.");
            }

            // Create a new user object
            // Hash the password using BCrypt before saving it to the database
            var user = new User
            {
                Email = userSignupDTO.Email,
                Hash = BCrypt.Net.BCrypt.HashPassword(userSignupDTO.Password),
                FirstName = userSignupDTO.FirstName,
                LastName = userSignupDTO.LastName
            };

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            return new UserSignupResDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<UserSigninResDTO> SignInUser(UserSigninDTO userSigninDTO)
        {
            // Find the user by email
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userSigninDTO.Email)
                        ?? throw new InvalidOperationException("User not found.");

            // Verify the password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(userSigninDTO.Password, user.Hash))
            {
                throw new InvalidOperationException("Invalid password.");
            }

            // If the password is correct, generate JWT and refresh token
            string jwt = _jwtHelper.GenerateJWT(user.Id.ToString(), user.Email);
            string refreshToken = _jwtHelper.GenerateRefreshToken();

            // Save the refresh token in the database
            _appDbContext.Tokens.Add(new Token
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddDays(RefreshTokenExpireDays).ToString()
            });
            await _appDbContext.SaveChangesAsync();

            // Return the user details excluding the password hash
            return new UserSigninResDTO
            {
                User = new UserSignupResDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                },
                Token = jwt,
                RefreshToken = refreshToken
            };
        }

        public async Task<bool> SignOut(int userId)
        {
            // Delete all tokens for the user in a single database operation
            await _appDbContext.Tokens
                .Where(t => t.UserId == userId)
                .ExecuteDeleteAsync();
            return true;
        }

        public async Task<RefreshTokenResDTO> RefreshToken(string refreshToken)
        {
            // Find the token in the database
            var token = await _appDbContext.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken)
                        ?? throw new InvalidDataException("Refresh token not found.");

            // Check if the token has expired
            if (DateTime.UtcNow > DateTime.Parse(token.ExpiresIn))
            {
                throw new InvalidOperationException("Refresh token has expired.");
            }

            // Generate a new JWT and refresh token
            var user = _appDbContext.Users.Find(token.UserId)
                       ?? throw new InvalidOperationException("User not found.");

            string newJwt = _jwtHelper.GenerateJWT(user.Id.ToString(), user.Email);
            string newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // Remove the old token & create a new refresh token
            _appDbContext.Tokens.Remove(token);
            _appDbContext.Tokens.Add(new Token 
            {
                UserId = token.UserId,
                RefreshToken = newRefreshToken,
                ExpiresIn = DateTime.UtcNow.AddDays(RefreshTokenExpireDays).ToString(),
            });
            await _appDbContext.SaveChangesAsync();

            return new RefreshTokenResDTO
            {
                Token = newJwt,
                RefreshToken = newRefreshToken
            };
        }
    }
}
