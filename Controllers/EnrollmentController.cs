// Controllers/EnrollmentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.DTOs.Course;
using OnlineLearningPlatform.DTOs.Enrollment;
using OnlineLearningPlatform.Models;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentService _enrollmentService;

    public EnrollmentController(EnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpPost("enroll")]
    [Authorize(Policy = "RequireLearnerRole")]
    public async Task<ActionResult<EnrollmentResponse>> EnrollInCourse([FromBody] EnrollmentRequest request)
    {
        //try
        //{
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var enrollment = await _enrollmentService.EnrollInCourse(
                userId,
                request.CourseId,
                request.PaymentMethod
            );
            return Ok(enrollment);
        //}
        //catch (Exception ex)
        //{
        //    return HandleException(ex);
        //}
    }

    [HttpGet("enrolledCourses")]
    [Authorize(Policy = "RequireLearnerRole")]
    public async Task<IActionResult> GetEnrolledCourses()
    {
        try
        {
            // Get instructor ID from the authenticated user's claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "Invalid userID" });
            }

            var courses = await _enrollmentService.EnrolledCourses(userId);

            //// Return courses with additional statistics
            //var coursesWithStats = courses.Select(course => new
            //{
            //    course.Id,
            //    course.Title,
            //    course.Description,
            //    course.Category,
            //    course.DifficultyLevel,
            //    course.Duration,
            //    course.Status,
            //    course.CreatedAt,
            //    course.UpdatedAt,
            //    course.ThumbnailUrl,
            //    EnrollmentCount = course.EnrollmentCount,
            //    PublishedStatus = course.Status == "Published",
            //    Rating = course.AverageRating
            //}).ToList();

            return Ok(courses);
        }
        catch (Exception ex)
        {
            // Log the error
            return StatusCode(500, new { message = "An error occurred while retrieving instructor courses", error = ex.Message });
        }
    }
    // Controllers/CoursesController.cs

    //[HttpGet("enrolledCourses")]
    //[Authorize(Policy = "RequireLearnerRole")]
    //public async Task<ActionResult<IEumerable<CourseResponse>>> GetEnrolledCourses()

    [HttpGet("course/{courseId}/content")]
    [Authorize(Policy = "RequireLearnerRole")]
    public async Task<ActionResult<List<CourseContent>>> GetCourseContent(string courseId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var content = await _enrollmentService.GetCourseContent(userId, courseId);
            return Ok(content);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    private ActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            UnauthorizedAccessException => Forbid(),
            KeyNotFoundException => NotFound(new { message = ex.Message }),
            InvalidOperationException => BadRequest(new { message = ex.Message }),
            Exception => BadRequest(new { message = ex.Message }),
            _ => StatusCode(500, new { message = "An error occurred while processing your request" })
        };
    }
}
