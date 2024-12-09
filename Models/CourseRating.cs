// Models/CourseRating.cs
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class CourseRating
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string CourseId { get; set; }
    public string UserId { get; set; }
    public int RatingValue { get; set; }  // 1-5
    public string ReviewComments { get; set; }
    public DateTime RatingDate { get; set; } = DateTime.UtcNow;
    public List<string> Helpful { get; set; } = new List<string>();  // UserIds who found review helpful
}
