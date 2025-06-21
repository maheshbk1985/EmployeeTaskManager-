using EmployeeTaskManager.API.DTOs;
using EmployeeTaskManager.API.Services;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace EmployeeTaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto, [FromServices] JwtTokenService jwtTokenService)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            var user = await _userService.ValidateUserAsync(loginDto.Email, loginDto.Password);

            if (user == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var token = jwtTokenService.GenerateToken(user);
            _logger.LogInformation("JWT token generated for user ID: {UserId}, email: {Email}", user.UserId, loginDto.Email);

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult SecureEndpoint()
        {
            _logger.LogInformation("Secure endpoint accessed by user: {User}", User.Identity?.Name);
            return Ok("✅ You have accessed a protected API successfully!");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            _logger.LogInformation("Register attempt for email: {Email}", userDto.Email);

            try
            {
                if (userDto == null || string.IsNullOrWhiteSpace(userDto.Password))
                {
                    _logger.LogWarning("Invalid registration attempt: missing password.");
                    return BadRequest("Password is required");
                }

                userDto.PasswordHash = HashPassword(userDto.Password);

                var createdUser = await _userService.CreateUserAsync(userDto);
                _logger.LogInformation("New user created with ID: {UserId}", createdUser.UserId);

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering new user.");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Returning user details for ID: {UserId}", id);
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users.");

            try
            {
                var users = await _userService.GetAllUsersAsync();
                _logger.LogInformation("Fetched {Count} users successfully.", users.Count());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users.");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            _logger.LogInformation("Update attempt for user ID: {UserId}", id);

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, userDto);
                if (updatedUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update.", id);
                    return NotFound();
                }

                _logger.LogInformation("User with ID {UserId} updated successfully.", id);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user ID: {UserId}", id);
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Delete attempt for user ID: {UserId}", id);

            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion.", id);
                    return NotFound();
                }

                _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user ID: {UserId}", id);
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }
    }
}
