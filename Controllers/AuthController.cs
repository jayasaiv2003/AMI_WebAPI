using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        // ✅ Combined Login (Admin + Staff)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            // 1️⃣ Dummy Admin Login
            if (user.Username == "admin" && user.Password == "admin123")
            {
                var token = GenerateJwtToken(user.Username, "Admin");
                return Ok(new
                {
                    token,
                    role = "Admin",
                    username = user.Username,
                    displayName = "Administrator"
                });
            }

            // 2️⃣ Check in Users table for Staff Login
            var dbUser = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (dbUser == null)
                return Unauthorized("User not found.");

            // 🧠 For now, simple password match (replace with hashed check later)
            if (dbUser.PasswordHash != user.Password)
                return Unauthorized("Invalid password.");

            // 3️⃣ Generate token for Staff role
            var staffToken = GenerateJwtToken(dbUser.Username, "Staff");

            return Ok(new
            {
                token = staffToken,
                role = "Staff",
                username = dbUser.Username,
                displayName = dbUser.DisplayName
            });
        }

        // ✅ Optional endpoint to test token validity
        [HttpGet("test")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var username = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok($"Hello {username}, you are logged in as {role}!");
        }

        // 🔐 JWT generator
        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // ✅ Login model
    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
