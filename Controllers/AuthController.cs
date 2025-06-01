using Microsoft.AspNetCore.Mvc;
using AuthC_.DTOs;
using AuthC_.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthC_.Controllers
{
    [Route("api/")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ApiController]
    public partial class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        // POST: api/signup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("signup")]
        public async Task<ActionResult<UserSignupResDTO>> SignUp(UserSignupDTO userSignupDTO)
        {
            try
            {
                UserSignupResDTO userRes = await _authService.SignUpUser(userSignupDTO);

                return StatusCode(StatusCodes.Status201Created, userRes);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error during signup: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
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
                UserSigninResDTO userSigninRes = await _authService.SignInUser(userSigninDTO);

                return Ok(userSigninRes);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error during signup: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error during signin: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // POST: api/signout
        [Authorize]
        [HttpPost("signout")]
        public async Task<ActionResult> SignOutUser() 
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID not found in the token.");
                }
                
                await _authService.SignOut(int.Parse(userId));
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during signout: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}