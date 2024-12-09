using MongoDB.Driver;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.DTOs;

namespace OnlineLearningPlatform.Services
{
    // Services/AssignmentService.cs
    // Services/AssignmentService.cs
    public class AssignmentService
    {
        private readonly IMongoCollection<Assignment> _assignments;
        private readonly IMongoCollection<AssignmentSubmission> _submissions;

        public AssignmentService(MongoDbContext context)
        {
            _assignments = context.Assignments;
            _submissions = context.AssignmentSubmissions;
        }

        public async Task<Assignment> CreateAssignment(Assignment assignment)
        {
            await _assignments.InsertOneAsync(assignment);
            return assignment;
        }

        public async Task<AssignmentSubmission> SubmitAssignment(AssignmentSubmission submission)
        {
            submission.SubmissionDate = DateTime.UtcNow;
            submission.Status = "Submitted";
            await _submissions.InsertOneAsync(submission);
            return submission;
        }

        public async Task<Assignment> GetAssignment(string id)
        {
            return await _assignments.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<AssignmentSubmission> GradeSubmission(string submissionId, GradeSubmissionDTO gradeDto)
        {
            var filter = Builders<AssignmentSubmission>.Filter.Eq(s => s.Id, submissionId);
            var update = Builders<AssignmentSubmission>.Update
                .Set(s => s.Grade, gradeDto.Grade)
                .Set(s => s.Feedback, gradeDto.Feedback)
                .Set(s => s.Status, "Graded");

            await _submissions.UpdateOneAsync(filter, update);
            return await _submissions.Find(filter).FirstOrDefaultAsync();
        }
    }

}
