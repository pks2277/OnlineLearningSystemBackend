
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Assignment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string GradingCriteria { get; set; }
    public int MaximumScore { get; set; }
    public string Status { get; set; }  // "Active", "Archived"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}





//// Models/Assignment.cs
//namespace OnlineLearningPlatform.Models
//{
//    public class Assignment
//    {
//        [BsonId]
//        [BsonRepresentation(BsonType.ObjectId)]
//        public string Id { get; set; }
//        public string CourseId { get; set; }
//        public string Title { get; set; }
//        public string Description { get; set; }
//        public DateTime DueDate { get; set; }
//        public string GradingCriteria { get; set; }
//    }

//    public class AssignmentSubmission
//    {
//        [BsonId]
//        [BsonRepresentation(BsonType.ObjectId)]
//        public string Id { get; set; }
//        public string AssignmentId { get; set; }
//        public string UserId { get; set; }
//        public string SubmissionContent { get; set; }
//        public DateTime SubmissionDate { get; set; }
//        public string Status { get; set; }
//        public decimal? Grade { get; set; }
//        public string Feedback { get; set; }
//    }
//}
