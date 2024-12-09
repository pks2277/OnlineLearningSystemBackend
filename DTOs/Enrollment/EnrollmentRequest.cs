using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Enrollment
{
    // DTOs/Enrollment/EnrollmentRequest.cs
    public class EnrollmentRequest
    {
        [Required]
        public string CourseId { get; set; }
        public string PaymentMethod { get; set; } // Only required for paid courses
    }



}
