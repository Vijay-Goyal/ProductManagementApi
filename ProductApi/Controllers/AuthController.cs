using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest user)
        {
            var newUser = new User
            {
                Username = user.Username,
                Password = user.Password
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(newUser);
        }
        [HttpPost("login")]
        public IActionResult Login(LoginRequest user)
        {
            var existingUser = _context.Users.FirstOrDefault(
                u => u.Username == user.Username && u.Password == user.Password);

            if (existingUser == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, existingUser.Username)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = existingUser.Id
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            _context.SaveChanges();

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refreshToken
            });
        }
        [HttpPost("refresh")]
        public IActionResult Refresh(string refreshToken)
        {
            var existingToken = _context.RefreshTokens
                .FirstOrDefault(r => r.Token == refreshToken);

            if (existingToken == null)
            {
                return Unauthorized();
            }

            if (existingToken.IsRevoked || existingToken.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Id == existingToken.UserId);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.Username)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var newToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(newToken)
            });
        }
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(
                Guid.NewGuid().ToByteArray());
        }
    }
}