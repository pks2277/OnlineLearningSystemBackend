// Models/Course.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineLearningPlatform.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Title { get; set; }

        [BsonRequired]
        public string Description { get; set; }

        [BsonRequired]
        public string InstructorId { get; set; }

        public string InstructorName { get; set; }

        [BsonRequired]
        public string Category { get; set; }

        [BsonRequired]
        public string DifficultyLevel { get; set; }  // Beginner, Intermediate, Advanced

        public int Duration { get; set; }  // In minutes

        public decimal Price { get; set; } // New field
        public bool IsFree { get; set; } // New field

        public string Status { get; set; } = "Draft";  // Draft, Published, Archived

        public double AverageRating { get; set; } = 0.0;

        public int EnrollmentCount { get; set; } = 0;

        public string ThumbnailUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
