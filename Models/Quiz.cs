// Models/Quiz.cs
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Quiz
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int TotalQuestions { get; set; }
    public int Duration { get; set; }  // in minutes
    public decimal PassingScore { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class Question
{
    public string Text { get; set; }
    public List<string> Options { get; set; }
    public int CorrectOptionIndex { get; set; }
    public int Points { get; set; }
}