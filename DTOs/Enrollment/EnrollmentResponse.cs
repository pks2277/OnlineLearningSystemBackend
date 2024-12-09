namespace OnlineLearningPlatform.DTOs.Enrollment
{
    // DTOs/Enrollment/EnrollmentResponse.cs
    public class EnrollmentResponse
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string CompletionStatus { get; set; }
        public double ProgressPercentage { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentStatus { get; set; }

        public string ThumbnailURL { get; set; }
    }
}
