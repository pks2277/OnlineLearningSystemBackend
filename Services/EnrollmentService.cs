// Services/EnrollmentService.cs
using MongoDB.Driver;
using OnlineLearningPlatform.DTOs.Enrollment;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Services;

public class EnrollmentService
{
    private readonly IMongoCollection<Enrollment> _enrollments;
    private readonly IMongoCollection<Course> _courses;
    private readonly IMongoCollection<CourseContent> _courseContents;
    private readonly EmailService _emailService;
    private readonly IMongoCollection<User> _users;  // Add this

    public EnrollmentService(MongoDbContext context, EmailService emailService)
    {
        _enrollments = context.Enrollments;
        _courses = context.Courses;
        _courseContents = context.CourseContents;
        _emailService = emailService;
        _users = context.Users;
    }

    public async Task NotifyUsersInInstructorCourses(string instructorId, string courseTitle)
    {
        try
        {
            // Get all courses by the instructor
            var instructorCourses = await _courses
                .Find(c => c.InstructorId == instructorId)
                .ToListAsync();

            var courseIds = instructorCourses.Select(c => c.Id).ToList();

            // Get all enrollments for these courses
            var enrollments = await _enrollments
                .Find(e => courseIds.Contains(e.CourseId))
                .ToListAsync();

            // Get unique user IDs
            var userIds = enrollments.Select(e => e.UserId).Distinct().ToList();

            // Get instructor details
            var instructor = await _users.Find(u => u.Id == instructorId).FirstOrDefaultAsync();
            if (instructor == null)
            {
                throw new KeyNotFoundException("Instructor not found");
            }

            // Get user details
            var users = await _users
                .Find(u => userIds.Contains(u.Id))
                .ToListAsync();

            var courseDictionary = instructorCourses.ToDictionary(c => c.Id);

            // Group enrollments by user
            foreach (var user in users)
            {
                    //var course = courseDictionary[enrollment.CourseId];
                    await _emailService.SendCourseCreationNotifEmail(
                        instructor.Name,
                        user.Email,
                        courseTitle
                    );
            }
        }
        catch (Exception ex)
        {
            // Log the error
            throw new ApplicationException("Error notifying enrolled users", ex);
        }
    }

    public async Task<List<EnrollmentResponse>> EnrolledCourses(string userId)
    {
        // Retrieve enrollments for the given userId
        var enrollments = await _enrollments.Find(e => e.UserId == userId).ToListAsync();

        // Get the list of course IDs from the enrollments
        var courseIds = enrollments.Select(e => e.CourseId).ToList();

        // Retrieve the courses that match the course IDs
        var courses = await _courses.Find(c => courseIds.Contains(c.Id)).ToListAsync();

        // Create a dictionary for faster lookup of Course details by Id
        var courseDictionary = courses.ToDictionary(c => c.Id, c => new { c.Title, c.ThumbnailUrl });

        // Create a list of EnrollmentResponse
        var enrollmentResponses = new List<EnrollmentResponse>();

        foreach (var enrollment in enrollments)
        {
            // Look up course details for this enrollment
            if (courseDictionary.TryGetValue(enrollment.CourseId, out var courseDetails))
            {
                var response = new EnrollmentResponse
                {
                    Id = enrollment.Id,
                    CourseId = enrollment.CourseId,
                    CourseTitle = courseDetails.Title, // From course
                    EnrollmentDate = enrollment.EnrollmentDate,
                    CompletionStatus = enrollment.CompletionStatus,
                    ProgressPercentage = enrollment.ProgressPercentage,
                    IsPaid = enrollment.IsPaid,
                    PaymentStatus = enrollment.PaymentStatus,
                    ThumbnailURL = courseDetails.ThumbnailUrl // From course
                };

                enrollmentResponses.Add(response);
            }
        }

        return enrollmentResponses;
    }
    public async Task<bool> IsEnrolled(string userId, string courseId)
    {
        var enrollment = await _enrollments
            .Find(e => e.UserId == userId && e.CourseId == courseId)
            .FirstOrDefaultAsync();

        return enrollment != null &&
               (enrollment.IsPaid == false || enrollment.PaymentStatus == "Completed");
    }

    public async Task<EnrollmentResponse> EnrollInCourse(string userId, string courseId, string paymentMethod = null)
    {
        // Check if already enrolled
        if (await IsEnrolled(userId, courseId))
        {
            throw new InvalidOperationException("Already enrolled in this course");
        }

        // Get course details
        var course = await _courses.Find(c => c.Id == courseId).FirstOrDefaultAsync();
        if (course == null)
        {
            throw new KeyNotFoundException("Course not found");
        }

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow,
            CompletionStatus = "NotStarted",
            ProgressPercentage = 0,
            LastAccessDate = DateTime.UtcNow,
            IsPaid = !course.IsFree
        };

        if (!course.IsFree)
        {
            if (string.IsNullOrEmpty(paymentMethod))
            {
                throw new Exception("Payment method required for paid courses");
            }

            // Handle payment process here
            var paymentResult = await ProcessPayment(userId, course.Price, paymentMethod);

            enrollment.PaymentStatus = paymentResult.Status;
            enrollment.PaymentId = paymentResult.PaymentId;
            enrollment.AmountPaid = course.Price;
        }
        else
        {
            enrollment.PaymentStatus = "NotRequired";
        }

        await _enrollments.InsertOneAsync(enrollment);

        // Update course enrollment count
        await _courses.UpdateOneAsync(
            c => c.Id == courseId,
            Builders<Course>.Update.Inc(c => c.EnrollmentCount, 1)
        );
        // Send enrollment confirmation email
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        await _emailService.SendEnrollmentEmail(user.Email, course.Title);

        return new EnrollmentResponse
        {
            Id = enrollment.Id,
            CourseId = enrollment.CourseId,
            CourseTitle = course.Title,
            EnrollmentDate = enrollment.EnrollmentDate,
            CompletionStatus = enrollment.CompletionStatus,
            ProgressPercentage = enrollment.ProgressPercentage,
            IsPaid = enrollment.IsPaid,
            PaymentStatus = enrollment.PaymentStatus
        };
    }

    public async Task<List<CourseContent>> GetCourseContent(string userId, string courseId)
    {
        if (!await IsEnrolled(userId, courseId))
        {
            throw new UnauthorizedAccessException("Not enrolled in this course");
}

return await _courseContents
    .Find(cc => cc.CourseId == courseId)
    .SortBy(cc => cc.ContentOrder)
    .ToListAsync();
    }

    private async Task<PaymentResult> ProcessPayment(string userId, decimal amount, string paymentMethod)
{
    // Implement payment processing logic here
    // This is just a placeholder
    return new PaymentResult
    {
        Status = "Completed",
        PaymentId = Guid.NewGuid().ToString()
    };
}
}

public class PaymentResult
{
    public string Status { get; set; }
    public string PaymentId { get; set; }
}
