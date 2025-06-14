using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        public UserController(IUserService userService, IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(CreateUser), new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var user = await _userService.LoginAsync(loginRequest.Email, loginRequest.Password);

                var claims = new[]{
                    new Claim(ClaimTypes.Name, loginRequest.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var jwtSecretKey = _configuration["JwtSettings:SecretKey"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
