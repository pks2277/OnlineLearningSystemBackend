using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Quiz
{
    // DTOs/Assignment/AssignmentCreateRequest.cs
    public class AssignmentCreateRequest
    {
        [Required]
        public string Title { get; set; }
        public string CourseId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string GradingCriteria { get; set; }
    }
}
