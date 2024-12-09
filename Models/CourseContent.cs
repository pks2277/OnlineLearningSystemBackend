using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class CourseContent
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string CourseId { get; set; }
    public string Title { get; set; }
    public string ContentType { get; set; }   //"PDF", "Text", "Interactive"
    public string FileUrl { get; set; }
    public string ContentDescription { get; set; }
    public int ContentOrder { get; set; }
    public bool IsDownloadable { get; set; }public string Duration { get; set; }  // For video content
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
