// Models/Discussion.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Discussion
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string CourseId { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Status { get; set; } // Active, Closed, Archived
    public List<Comment> Comments { get; set; } = new();
    public int ViewCount { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string DiscussionId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsInstructorResponse { get; set; }
    public List<string> Likes { get; set; } = new();
}