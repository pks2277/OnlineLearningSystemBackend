// Data/MongoDbContext.cs
using MongoDB.Driver;
using OnlineLearningPlatform.Models;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, IConfiguration configuration)
    {
        var databaseName = configuration["MongoDbDatabaseName"];
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Course> Courses => _database.GetCollection<Course>("Courses");
    public IMongoCollection<LearningPath> LearningPaths => _database.GetCollection<LearningPath>("LearningPaths");
    public IMongoCollection<Certification> Certifications => _database.GetCollection<Certification>("Certifications");

    public IMongoCollection<Enrollment> Enrollments => _database.GetCollection<Enrollment>("Enrollments");
    public IMongoCollection<Quiz> Quizzes => _database.GetCollection<Quiz>("Quizzes");
    public IMongoCollection<Assignment> Assignments =>_database.GetCollection<Assignment>("Assignments");
    public IMongoCollection<AssignmentSubmission> AssignmentSubmissions =>_database.GetCollection<AssignmentSubmission>("AssignmentSubmissions");
    public IMongoCollection<Discussion> Discussions => _database.GetCollection<Discussion>("Discussions");
    public IMongoCollection<CourseContent> CourseContents => _database.GetCollection<CourseContent>("CourseContents");
    public IMongoCollection<CourseRating> CourseRatings => _database.GetCollection<CourseRating>("CourseRatings");
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
    public IMongoCollection<Contact> Contacts => _database.GetCollection<Contact>("Contacts");
}
