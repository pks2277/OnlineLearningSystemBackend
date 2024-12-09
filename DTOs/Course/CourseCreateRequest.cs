using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Course
{
    // DTOs/Course/CourseCreateRequest.cs
    public class CourseCreateRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string DifficultyLevel { get; set; }

        public int Duration { get; set; }

        public string Status { get; set; } = "Draft";

        public IFormFile Thumbnail { get; set; }
    }





}
