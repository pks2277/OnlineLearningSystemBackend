using MongoDB.Driver;
using OnlineLearningPlatform.DTOs.Course;
using OnlineLearningPlatform.DTOs.LearningPath;
using OnlineLearningPlatform.Models;

namespace OnlineLearningPlatform.Services
{
    // Services/LearningPathService.cs
    public class LearningPathService
    {
        private readonly IMongoCollection<LearningPath> _learningPaths;
        private readonly IMongoCollection<Certification> _certifications;
        private readonly IMongoCollection<Course> _courses;
        private readonly IMongoCollection<Enrollment> _enrollments;

        public LearningPathService(MongoDbContext context)
        {
            _learningPaths = context.LearningPaths;
            _certifications = context.Certifications;
            _courses = context.Courses;
            _enrollments = context.Enrollments;
        }

        public async Task<LearningPath> CreateLearningPath(LearningPathCreateRequest request, string createdBy)
        {
            var learningPath = new LearningPath
            {
                Name = request.Name,
                Description = request.Description,
                CourseIds = request.CourseIds,
                CompletionCriteria = request.CompletionCriteria,
                CertificationAwarded = request.CertificationAwarded,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                EstimatedDuration = request.EstimatedDuration,
                Difficulty = request.Difficulty,
                Category = request.Category
            };

            await _learningPaths.InsertOneAsync(learningPath);
            return learningPath;
        }

        public async Task<LearningPathResponse> GetLearningPath(string id, string userId)
        {
            var learningPath = await _learningPaths.Find(lp => lp.Id == id).FirstOrDefaultAsync();
            if (learningPath == null) return null;

            var courses = await _courses
                .Find(c => learningPath.CourseIds.Contains(c.Id))
                .ToListAsync();

            var enrollments = await _enrollments
                .Find(e => e.UserId == userId && learningPath.CourseIds.Contains(e.CourseId))
                .ToListAsync();

            var completionPercentage = CalculateCompletionPercentage(learningPath.CourseIds, enrollments);

            return new LearningPathResponse
            {
                Id = learningPath.Id,
                Name = learningPath.Name,
                Description = learningPath.Description,
                Courses = courses.Select(c => new CourseResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description
                    // Add other course properties
                }).ToList(),
                CompletionCriteria = learningPath.CompletionCriteria,
                CertificationAwarded = learningPath.CertificationAwarded,
                CreatedAt = learningPath.CreatedAt,
                CreatedBy = learningPath.CreatedBy,
                EstimatedDuration = learningPath.EstimatedDuration,
                Difficulty = learningPath.Difficulty,
                Category = learningPath.Category,
                CompletionPercentage = completionPercentage
            };
        }

        public async Task<Certification> GenerateCertification(string userId, string learningPathId)
        {
            var learningPath = await _learningPaths.Find(lp => lp.Id == learningPathId).FirstOrDefaultAsync();
            if (learningPath == null) throw new InvalidOperationException("Learning path not found");

            var enrollments = await _enrollments
                .Find(e => e.UserId == userId && learningPath.CourseIds.Contains(e.CourseId))
                .ToListAsync();

            var completionPercentage = CalculateCompletionPercentage(learningPath.CourseIds, enrollments);
            if (completionPercentage < 100)
                throw new InvalidOperationException("Learning path not completed");

            var certification = new Certification
            {
                UserId = userId,
                LearningPathId = learningPathId,
                IssuedDate = DateTime.UtcNow,
                CertificateNumber = GenerateCertificateNumber(),
                IsValid = true,
                CertificateUrl = await GenerateCertificateDocument(userId, learningPath)
            };

            await _certifications.InsertOneAsync(certification);
            return certification;
        }

        private double CalculateCompletionPercentage(List<string> courseIds, List<Enrollment> enrollments)
        {
            if (!courseIds.Any()) return 0;
            var completedCourses = enrollments.Count(e => e.CompletionStatus == "Completed");
            return (double)completedCourses / courseIds.Count * 100;
        }

        private string GenerateCertificateNumber()
        {
            return $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}".ToUpper();
        }

        private async Task<string> GenerateCertificateDocument(string userId, LearningPath learningPath)
        {
            // Implement certificate PDF generation logic here
            // Return URL to stored certificate
            return $"certificates/{userId}/{learningPath.Id}.pdf";
        }
    }
}
