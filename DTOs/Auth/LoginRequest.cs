using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Auth
{
    // DTOs/Auth/LoginRequest.cs
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
