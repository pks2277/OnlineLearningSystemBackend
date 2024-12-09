using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.DTOs.Quiz
{
    // DTOs/Quiz/QuizCreateRequest.cs
    public class QuizCreateRequest
    {
        [Required]
        public string Title { get; set; }
        public string CourseId { get; set; }
        public int TotalQuestions { get; set; }
        public int Duration { get; set; }
        public int PassingScore { get; set; }
        public List<QuizQuestionDto> Questions { get; set; }
    }

    


}
