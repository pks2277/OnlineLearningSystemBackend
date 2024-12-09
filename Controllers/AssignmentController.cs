// Controllers/AssignmentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.DTOs;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentController : ControllerBase
{
    private readonly AssignmentService _assignmentService;

    public AssignmentController(AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpPost]
    [Authorize(Policy = "RequireInstructorRole")]
    public async Task<IActionResult> CreateAssignment(Assignment assignment)
    {
        var result = await _assignmentService.CreateAssignment(assignment);
        return CreatedAtAction(nameof(GetAssignment), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignment(string id)
    {
        var assignment = await _assignmentService.GetAssignment(id);
        if (assignment == null)
            return NotFound();
        return Ok(assignment);
    }

    [HttpPost("submit")]
    [Authorize(Policy = "RequireLearnerRole")]
    public async Task<IActionResult> SubmitAssignment(AssignmentSubmission submission)
    {
        var result = await _assignmentService.SubmitAssignment(submission);
        return Ok(result);
    }

    [HttpPost("{submissionId}/grade")]
    [Authorize(Policy = "RequireInstructorRole")]
    public async Task<IActionResult> GradeSubmission(string submissionId, GradeSubmissionDTO gradeDto)
    {
        var result = await _assignmentService.GradeSubmission(submissionId, gradeDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
}
