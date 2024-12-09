namespace OnlineLearningPlatform.DTOs.Quiz
{
    // DTOs/Quiz/QuizQuestionDto.cs
    public class QuizQuestionDto
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
        public int Score { get; set; }
    }

}
