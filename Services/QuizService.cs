using MongoDB.Driver;

namespace OnlineLearningPlatform.Services
{
    // Services/QuizService.cs
    public class QuizService
    {
        private readonly IMongoCollection<Quiz> _quizzes;

        public QuizService(MongoDbContext context)
        {
            _quizzes = context.Quizzes;
        }

        public async Task<Quiz> CreateQuiz(Quiz quiz)
        {
            await _quizzes.InsertOneAsync(quiz);
            return quiz;
        }

        public async Task<Quiz> GetQuizById(string id)
        {
            return await _quizzes.Find(q => q.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Quiz>> GetQuizzesByCourse(string courseId)
        {
            return await _quizzes.Find(q => q.CourseId == courseId).ToListAsync();
        }
    }
}
