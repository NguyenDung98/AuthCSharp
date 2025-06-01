using AuthC_.Data;
using AuthC_.DTOs;
using AuthC_.Helpers;
using AuthC_.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AuthC_.Services
{
    public class AuthService(UserContext userContext, TokenContext tokenContext, JwtHelper jwtHelper) : IAuthService
    {
        private readonly UserContext _userContext = userContext;
        private readonly TokenContext _tokenContext = tokenContext;
        private readonly JwtHelper _jwtHelper = jwtHelper;

        // Implementation of the IAuthService methods would go here.
        public async Task<UserSignupResDTO> SignUpUser(UserSignupDTO userSignupDTO)
        {
            // Logic for signing up a user
            // Check if the email is available for signup
            if (_userContext.Users.Any(u => u.Email == userSignupDTO.Email))
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

            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();

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
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Email == userSigninDTO.Email)
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
            _tokenContext.Tokens.Add(new Token
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddDays(30).ToString() // Set expiration for 30 days
            });
            await _tokenContext.SaveChangesAsync();

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
            // Find all tokens associated with the user
            var tokens = await _tokenContext.Tokens.Where(t => t.UserId == userId).ToListAsync();
            if (tokens.Count > 0)
            {
                Console.WriteLine($"Signing out user with ID: {userId}");
                // Log the tokens being removed
                tokens.ForEach(token => Console.WriteLine($"Removing token: {token.RefreshToken}, ExpiresIn: {token.ExpiresIn}"));

                // Remove the token from the database
                _tokenContext.Tokens.RemoveRange(tokens);
                await _tokenContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task<RefreshTokenResDTO> RefreshToken(string refreshToken)
        {
            // Find the token in the database
            var token = await _tokenContext.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken)
                        ?? throw new InvalidDataException("Refresh token not found.");

            // Check if the token has expired
            if (DateTime.UtcNow > DateTime.Parse(token.ExpiresIn))
            {
                throw new InvalidOperationException("Refresh token has expired.");
            }

            // Generate a new JWT and refresh token
            var user = _userContext.Users.Find(token.UserId)
                       ?? throw new InvalidOperationException("User not found.");

            string newJwt = _jwtHelper.GenerateJWT(user.Id.ToString(), user.Email);
            string newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // Create a new refresh token and update the existing token in the database
            token.RefreshToken = newRefreshToken;
            token.ExpiresIn = DateTime.UtcNow.AddDays(30).ToString();
            _tokenContext.Tokens.Update(token);
            await _tokenContext.SaveChangesAsync();

            return new RefreshTokenResDTO
            {
                Token = newJwt,
                RefreshToken = newRefreshToken
            };
        }
    }
}
