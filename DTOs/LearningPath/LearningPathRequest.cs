using OnlineLearningPlatform.DTOs.Course;

namespace OnlineLearningPlatform.DTOs.LearningPath
{
    public class LearningPathCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> CourseIds { get; set; }
        public string CompletionCriteria { get; set; }
        public bool CertificationAwarded { get; set; }
        public int EstimatedDuration { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
    }

    public class LearningPathResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CourseResponse> Courses { get; set; }
        public string CompletionCriteria { get; set; }
        public bool CertificationAwarded { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public int EstimatedDuration { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
        public double CompletionPercentage { get; set; }
    }
}
