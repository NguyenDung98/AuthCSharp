using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthC_.Models;
using BCrypt.Net;

namespace AuthC_.Controllers
{
    [Route("api/")]
    [ApiController]
    public partial class AuthController : ControllerBase
    {
        private readonly UserContext _context;

        public AuthController(UserContext context)
        {
            _context = context;
        }

        // POST: api/signup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("signup")]
        public async Task<ActionResult<UserDTO>> SignUp(UserSignupDTO userSignupDTO)
        {
            try
            {
                // Validate the user signup data
                if (!ValidateUserSignup(userSignupDTO, out string? errorMessage))
                {
                    return BadRequest(errorMessage);
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

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ItemToDTO(user));
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error during signup: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // GET: api/Auth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int? id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        public static UserDTO ItemToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        // Email validation regex
        // This regex checks for a basic email format: non-empty local part, '@', non-empty domain part, and a '.' followed by a non-empty top-level domain.
        [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial System.Text.RegularExpressions.Regex MyRegex();


        private bool ValidateUserSignup(UserSignupDTO userSignupDTO, out string? errorMessage)
        {
            errorMessage = null;

            // Check if the email is not null or empty
            if (string.IsNullOrEmpty(userSignupDTO.Email))
            {
                errorMessage = "Email cannot be null or empty.";
                return false;
            }

            // Check if the password is not null or empty
            if (string.IsNullOrEmpty(userSignupDTO.Password))
            {
                errorMessage = "Password cannot be null or empty.";
                return false;
            }

            // Check if the email is available
            if (_context.Users.Any(u => u.Email == userSignupDTO.Email))
            {
                errorMessage = "Email is already in use.";
                return false;
            }

            // Check if the email format is valid
            if (!MyRegex().IsMatch(userSignupDTO.Email))
            {
                errorMessage = "Invalid email format.";
                return false;
            }

            // Check if the password meets the minimum length requirement
            if (userSignupDTO.Password.Length < 8 || userSignupDTO.Password.Length > 20)
            {
                errorMessage = "Password must be between 8 and 20 characters long.";
                return false;
            }

            return true;
        }
    }
}