// Models/AssignmentSubmission.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineLearningPlatform.Models
{
    public class AssignmentSubmission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AssignmentId { get; set; }
        public string UserId { get; set; }
        public string SubmissionContent { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; }
        public decimal? Grade { get; set; }  // Added Grade property
        public string Feedback { get; set; }
    }
}
