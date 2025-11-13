using AMI_WebAPI.Models;
using AMI_WebAPI.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerAuthController : ControllerBase
    {
        private readonly IConsumerRepository _consumerRepository;
        private readonly IConfiguration _configuration;

        public ConsumerAuthController(IConsumerRepository consumerRepository, IConfiguration configuration)
        {
            _consumerRepository = consumerRepository;
            _configuration = configuration;
        }

        // ✅ POST: api/ConsumerAuth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ConsumerLoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and Password are required.");

            // ✅ Use your new repository method (email + password)
            var consumer = await _consumerRepository.GetConsumerByEmailAndPasswordAsync(model.Email, model.Password);
            if (consumer == null)
                return Unauthorized("Invalid email or password.");

            // 🔐 Generate JWT for Consumer
            var token = GenerateJwtToken(consumer.ConsumerId, consumer.Email, "Consumer");

            return Ok(new
            {
                token,
                role = "Consumer",
                consumerId = consumer.ConsumerId,
                name = consumer.Name,
                email = consumer.Email
            });
        }


        private string GenerateJwtToken(long consumerId, string email, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, consumerId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // ✅ Login model for consumers
    public class ConsumerLoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;  // ✅ use Password instead of Name
    }

}
