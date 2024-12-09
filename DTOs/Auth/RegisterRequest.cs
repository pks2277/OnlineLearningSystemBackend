using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Auth
{
    // DTOs/Auth/RegisterRequest.cs
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength (50)]
        public string Name {  get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]

        public string Role { get; set; } = "Learner"; // Default role
    }

    

   
    

}
