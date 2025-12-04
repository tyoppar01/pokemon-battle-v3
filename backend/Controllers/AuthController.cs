using Microsoft.AspNetCore.Mvc;
using PokemonBattle.Services;
using PokemonBattle.DTOs;

namespace PokemonBattle.Controllers
{
    /// <summary>
    /// Controller for authentication and token management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="request">Login credentials containing user ID</param>
        /// <returns>JWT token with user information</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validation: Check if user ID is provided
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest(new { error = "User ID is required" });
            }

            // Validation: Check if ID is valid format
            if (!System.Guid.TryParse(request.UserId, out _))
            {
                return BadRequest(new { error = "Invalid User ID format" });
            }

            // Check if user exists
            var user = _userService.GetUser(request.UserId);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(request.UserId, user.Name);
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            var response = new LoginResponse
            {
                Token = token,
                UserId = request.UserId,
                Username = user.Name,
                ExpiresAt = expiresAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Validates the current JWT token
        /// </summary>
        /// <returns>Token validation status</returns>
        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { error = "No token provided" });
            }

            var principal = _jwtService.ValidateToken(token);

            if (principal == null)
            {
                return Unauthorized(new { error = "Invalid or expired token" });
            }

            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            return Ok(new { 
                valid = true, 
                userId = userId, 
                username = username 
            });
        }
    }
}
