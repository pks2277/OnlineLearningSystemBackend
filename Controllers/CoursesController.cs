// Controllers/CoursesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.DTOs.Course;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using OnlineLearningPlatform.Services;

namespace OnlineLearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMongoCollection<Course> _courses;
        private readonly EmailService _emailService;
        private readonly EnrollmentService _enrollmentService;
        private readonly IMongoCollection<CourseContent> _courseContents;
        private readonly string _uploadDirectory;  // Add this field
        public CoursesController(MongoDbContext context, EmailService emailService, EnrollmentService enrollmentService)
        {
            _courses = context.Courses;
            _emailService = emailService;
            _enrollmentService = enrollmentService;
            _courseContents = context.CourseContents;
            _uploadDirectory = Path.Combine("uploads/thumbnails");

            // Ensure that the upload directory exists
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        // Public endpoint - Everyone can view courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAllCourses(
            [FromQuery] string? category,
            [FromQuery] string? difficultyLevel,
            [FromQuery] string? searchTerm)
        {
            var filterBuilder = Builders<Course>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(category))
                filter &= filterBuilder.Eq(c => c.Category, category);

            if (!string.IsNullOrEmpty(difficultyLevel))
                filter &= filterBuilder.Eq(c => c.DifficultyLevel, difficultyLevel);

            if (!string.IsNullOrEmpty(searchTerm))
                filter &= filterBuilder.Regex(c => c.Title, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));

            var courses = await _courses.Find(filter)
                .ToListAsync();

            var courseResponses = courses.Select(c => new CourseResponse
            {
                Id = c.Id.ToString(),
                Title = c.Title,
                Description = c.Description,
                Category = c.Category,
                DifficultyLevel = c.DifficultyLevel,
                Duration = c.Duration,
                Status = c.Status,
                InstructorName = c.InstructorName,
                CreatedAt = c.CreatedAt,
                AverageRating = c.AverageRating,
                EnrollmentCount = c.EnrollmentCount,
                ThumbnailURL = c.ThumbnailUrl
            });

            return Ok(courseResponses);
        }

        // Public endpoint - Everyone can view course details
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponse>> GetCourseById(string id)
        {
            var course = await _courses.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (course == null)
                return NotFound(new { message = "Course not found" });

            var response = new CourseResponse
            {
                Id = course.Id.ToString(),
                Title = course.Title,
                Description = course.Description,
                Category = course.Category,
                DifficultyLevel = course.DifficultyLevel,
                Duration = course.Duration,
                Status = course.Status,
                InstructorName = course.InstructorName,
                CreatedAt = course.CreatedAt,
                AverageRating = course.AverageRating,
                EnrollmentCount = course.EnrollmentCount,
                ThumbnailURL = course.ThumbnailUrl
            };

            return Ok(response);
        }

        // Protected endpoint - Only instructors can create courses
        [HttpPost]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<ActionResult<CourseResponse>> CreateCourse([FromForm] CourseCreateRequest request)
        {
            Console.WriteLine("Create Course");
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var instructorEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            string thumbnailUrl = null;
            if (request.Thumbnail != null && request.Thumbnail.Length > 0)
            {
                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Thumbnail.FileName)}";
                var filePath = Path.Combine(_uploadDirectory, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Thumbnail.CopyToAsync(stream);
                }

                // Generate URL for the thumbnail
                thumbnailUrl = $"/uploads/thumbnails/{fileName}";
            }


            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                DifficultyLevel = request.DifficultyLevel,
                Duration = request.Duration,
                Status = request.Status,
                InstructorId = instructorId,
                InstructorName = instructorName,
                ThumbnailUrl = thumbnailUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _enrollmentService.NotifyUsersInInstructorCourses(instructorId, course.Title);
            await _courses.InsertOneAsync(course);
            _emailService.SendCourseCreationEmail(instructorEmail, course.Title);
            
            return CreatedAtAction(nameof(GetCourseById),
                new { id = course.Id.ToString() },
                new CourseResponse { Id = course.Id.ToString(), Title = course.Title });
        }

        // Protected endpoint - Only instructors can update their courses
        [HttpPut("{id}")]
        [Authorize(Policy = "Instructor")]
        public async Task<IActionResult> UpdateCourse(string id, [FromBody] CourseUpdateRequest request)
        {
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var course = await _courses.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (course == null)
                return NotFound(new { message = "Course not found" });

            if (course.InstructorId != instructorId)
                return Forbid();

            var update = Builders<Course>.Update
                .Set(c => c.Title, request.Title)
                .Set(c => c.Description, request.Description)
                .Set(c => c.Category, request.Category)
                .Set(c => c.DifficultyLevel, request.DifficultyLevel)
                .Set(c => c.Duration, request.Duration)
                .Set(c => c.Status, request.Status)
                .Set(c => c.UpdatedAt, DateTime.UtcNow);

            await _courses.UpdateOneAsync(c => c.Id == id, update);

            return NoContent();
        }

        // Protected endpoint - Only instructors can delete their courses
        [HttpDelete("{id}")]
        [Authorize(Policy = "Instructor")]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var course = await _courses.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (course == null)
                return NotFound(new
                {
                    message = "Course not Found"
                });

            if (course.InstructorId != instructorId)
                return Forbid();

            await _courses.DeleteOneAsync(c => c.Id == id);
            await _courseContents.DeleteManyAsync(cc => cc.CourseId == id);

            return NoContent();
        }

        // Protected endpoint - Only enrolled users can access course content
        [HttpGet("{id}/content")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CourseContent>>> GetCourseContent(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Check if user is instructor or enrolled in the course
            var course = await _courses.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (course == null)
                return NotFound(new { message = "Course not found" });

            // Allow access if user is the instructor or admin
            if (userRole != "Instructor" && course.InstructorId != userId)
            {
                // Check enrollment for learners
                // You would need to implement this check with your enrollment service
                var isEnrolled = await CheckEnrollment(userId, id);
                if (!isEnrolled)
                    return Forbid();
            }

            var contents = await _courseContents
                .Find(cc => cc.CourseId == id)
                .SortBy(cc => cc.ContentOrder)
                .ToListAsync();

            return Ok(contents);
        }

        // Controllers/CoursesController.cs
        [HttpPost("{courseId}/content")]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<ActionResult<CourseContent>> AddCourseContent(string courseId, [FromForm] CourseContentCreateRequest request)
        {
            Console.WriteLine((request.File != null).ToString());
            var course = await _courses.Find(c => c.Id == courseId).FirstOrDefaultAsync();
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (course.InstructorId != instructorId)
                return Forbid();
            // Handle file upload
            var fileName = "";
            if (request.File != null)
            {
                Debug.Print(request.File.FileName);
                //fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);
                //var filePath = Path.Combine("uploads", fileName);
                // Generate unique filename
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
                var filePath = Path.Combine("uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }
            }

            var content = new CourseContent
            {
                CourseId = courseId,
                Title = request.Title,
                ContentType = request.ContentType,
                FileUrl = fileName != "" ? $"/uploads/{fileName}" : null,
                ContentDescription = request.ContentDescription,
                ContentOrder = request.ContentOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _courseContents.InsertOneAsync(content);
            return Ok(content);
        }

        [HttpGet("instructor/courses")]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<IActionResult> GetInstructorCourses()
        {
            try
            {
                // Get instructor ID from the authenticated user's claims
                var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(instructorId))
                {
                    return BadRequest(new { message = "Invalid instructor ID" });
                }

                // Query courses collection for instructor's courses
                var courses = await _courses
                    .Find(c => c.InstructorId == instructorId)
                    .ToListAsync();

                // Return courses with additional statistics
                var coursesWithStats = courses.Select(course => new
                {
                    course.Id,
                    course.Title,
                    course.Description,
                    course.Category,
                    course.DifficultyLevel,
                    course.Duration,
                    course.Status,
                    course.CreatedAt,
                    course.UpdatedAt,
                    course.ThumbnailUrl,
                    EnrollmentCount = course.EnrollmentCount,
                    PublishedStatus = course.Status == "Published",
                    Rating = course.AverageRating
                }).ToList();

                return Ok(coursesWithStats);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, new { message = "An error occurred while retrieving instructor courses", error = ex.Message });
            }
        }
        // Controllers/CoursesController.cs

        [HttpGet("admin/all")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAllCoursesAdmin()
        {
            try
            {
                var courses = await _courses
                    .Find(_ => true)
                    .ToListAsync();

                var courseResponses = courses.Select(course => new CourseResponse
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    InstructorName = course.InstructorName,
                    Category = course.Category,
                    DifficultyLevel = course.DifficultyLevel,
                    Duration = course.Duration,
                    Status = course.Status,
                    CreatedAt = course.CreatedAt,
                    ThumbnailURL = course.ThumbnailUrl,
                    AverageRating = course.AverageRating,
                    EnrollmentCount = course.EnrollmentCount
                }).ToList();

                return Ok(courseResponses);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, new { message = "An error occurred while retrieving courses", error = ex.Message });
            }
        }


        private async Task<bool> CheckEnrollment(string userId, string courseId)
        {
            // Implement enrollment check logic here
            // This should check your Enrollments collection
            return true; // Temporary placeholder
        }
    }
}
