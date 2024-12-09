namespace OnlineLearningPlatform.DTOs.Course
{
    // DTOs/Course/CourseResponse.cs
    public class CourseResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InstructorName { get; set; }
        public string Category { get; set; }
        public string DifficultyLevel { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public double AverageRating { get; set; }
        public int EnrollmentCount { get; set; }
        public string ThumbnailURL { get; set; }
    }
}
