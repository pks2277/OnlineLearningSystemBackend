using OnlineLearningPlatform.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Rating
{
    // DTOs/Rating/CourseRatingRequest.cs
    public class CourseRatingRequest
    {
        [Required]
        public string CourseId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comments { get; set; }
    }

   

}
