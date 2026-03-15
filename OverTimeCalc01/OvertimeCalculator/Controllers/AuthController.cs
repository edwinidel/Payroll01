using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OvertimeCalculator.Data;
using OvertimeCalculator.Dtos;
using OvertimeCalculator.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace OvertimeCalculator.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0", Deprecated = true)]
    public class AuthController : ControllerBase
    {
        private readonly OvertimeDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(OvertimeDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user with the provided credentials.
        /// </summary>
        /// <param name="dto">Registration data containing username, password, and password confirmation.</param>
        /// <returns>A success message upon successful registration, or an error if username already exists.</returns>
        /// <remarks>
        /// Requirements:
        /// - Username must be unique (3-100 characters)
        /// - Password must be confirmed (match ConfirmPassword field)
        /// - Passwords are hashed using BCrypt before storage
        /// </remarks>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                _logger.LogWarning("Registration attempt with existing username: {Username}", dto.Username);
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New user registered: {Username}", dto.Username);
            return Ok("User registered successfully");
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token for API access.
        /// </summary>
        /// <param name="dto">Login credentials containing username and password.</param>
        /// <returns>An AuthResponse object containing JWT token, username, user ID, and token expiration time.</returns>
        /// <remarks>
        /// - Uses BCrypt password verification for security
        /// - Returns 401 Unauthorized if credentials are invalid
        /// - JWT token expires in 1 year from issuance
        /// - Token should be included in Authorization header as "Bearer {token}" for subsequent requests
        /// </remarks>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Payload vacío");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", dto.Username);
                return Unauthorized("Invalid credentials");
            }

            var result = GenerateJwtToken(user);
            _logger.LogInformation("User logged in successfully: {Username}", dto.Username);
            return Ok(result);
        }

        private AuthResponse GenerateJwtToken(User user)
        {
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing");
            var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddYears(1);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                UserId = user.Id,
                ExpiresAt = expiresAt
            };
        }
    }
}