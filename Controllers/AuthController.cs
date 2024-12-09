using JWTAuthorization.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnlineLearningPlatform.DTOs.Auth;
using OnlineLearningPlatform.Models;

namespace OnlineLearningPlatform.Controllers
{
    // Controllers/AuthController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly JwtService _jwtService;

        public AuthController(MongoDbContext context, JwtService jwtService)
        {
            _users = context.Users;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // Check if username exists
            if (await _users.Find(u => u.Username == request.Username).AnyAsync())
                return BadRequest(new { message = "Username already exists" });

            // Check if email exists
            if (await _users.Find(u => u.Email == request.Email).AnyAsync())
                return BadRequest(new { message = "Email already exists" });

            var user = new User
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                IsActive = true
            };

            await _users.InsertOneAsync(user);

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _users.Find(u => u.Username == request.Username).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password" });

            if (!user.IsActive)
                return Unauthorized(new { message = "Account is deactivated" });

            var token = _jwtService.GenerateToken(user);

            var response = new LoginResponse
            {
                Token = token,
                UserDetails = new UserDto
                {
                    Id = user.Id,
                    Name=user.Name,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                }
            };

            return Ok(response);
        }
    }

}
