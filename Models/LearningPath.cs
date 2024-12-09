// Models/LearningPath.cs
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class LearningPath
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> CourseIds { get; set; }
    public string CompletionCriteria { get; set; }
    public bool CertificationAwarded { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public int EstimatedDuration { get; set; } // In hours
    public string Difficulty { get; set; }
    public string Category { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedAt { get; set; }
}

public class Certification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public string LearningPathId { get; set; }
    public DateTime IssuedDate { get; set; }
    public string CertificateNumber { get; set; }
    public bool IsValid { get; set; }
    public string CertificateUrl { get; set; }
}