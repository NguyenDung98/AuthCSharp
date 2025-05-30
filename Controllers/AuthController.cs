using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthC_.Models;

namespace AuthC_.Controllers
{
    [Route("api/")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ApiController]
    public partial class AuthController(UserContext context) : ControllerBase
    {
        private readonly UserContext _context = context;

        // POST: api/signup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("signup")]
        public async Task<ActionResult<UserSignupResDTO>> SignUp(UserSignupDTO userSignupDTO)
        {
            try
            {
                // Check if the email is available for signup
                if (_context.Users.Any(u => u.Email == userSignupDTO.Email))
                {
                    return BadRequest(new ValidationProblemDetails
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Email", new[] { "Email is already in use." } }
                        },
                    });
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

                return StatusCode(StatusCodes.Status201Created, new UserSignupResDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error during signup: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // POST: api/signin
        [HttpPost("signin")]
        public async Task<ActionResult<UserSigninResDTO>> SignIn(UserSigninDTO userSigninDTO)
        {
            try
            {
                // Find the user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userSigninDTO.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Verify the password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(userSigninDTO.Password, user.Hash))
                {
                    return Unauthorized("Invalid password.");
                }

                // Return the user details excluding the password hash
                return Ok(new UserSigninResDTO
                {
                    User = new UserSignupResDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    },
                    Token = "dummy-jwt-token", // Replace with actual JWT token generation logic
                    RefreshToken = "dummy-refresh-token" // Replace with actual refresh token generation logic
                });
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error during signin: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}