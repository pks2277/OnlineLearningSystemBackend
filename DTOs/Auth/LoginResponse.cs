using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Auth
{

    // DTOs/Auth/LoginResponse.cs
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserDto UserDetails { get; set; }
    }

}
