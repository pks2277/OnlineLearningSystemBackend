
// Models/Notification.cs
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Type { get; set; }  // "CourseUpdate", "Assignment", "Quiz", "System"
    public string Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public string RelatedId { get; set; }  // CourseId, AssignmentId, etc.
}
