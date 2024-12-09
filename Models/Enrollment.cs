// Models/Enrollment.cs
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Enrollment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public string CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string CompletionStatus { get; set; } // "NotStarted", "InProgress", "Completed"
    public double ProgressPercentage { get; set; }
    public DateTime LastAccessDate { get; set; }
    public bool IsPaid { get; set; }
    public string PaymentStatus { get; set; } // "Pending", "Completed", "Failed"
    public string PaymentId { get; set; }
    public decimal AmountPaid { get; set; }
}
