using OnlineLearningPlatform.DTOs.Auth;

namespace OnlineLearningPlatform.DTOs.Rating
{
    // DTOs/Rating/CourseRatingResponse.cs
    public class CourseRatingResponse
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
        public UserDto User { get; set; }
        public DateTime RatingDate { get; set; }
    }
}
