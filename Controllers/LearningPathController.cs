using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.DTOs.LearningPath;
using OnlineLearningPlatform.Services;

namespace OnlineLearningPlatform.Controllers
{
    // Controllers/LearningPathController.cs
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LearningPathController : ControllerBase
    {
        private readonly LearningPathService _learningPathService;

        public LearningPathController(LearningPathService learningPathService)
        {
            _learningPathService = learningPathService;
        }

        [HttpPost]
        [Authorize(Policy = "InstructorOnly")]
        public async Task<ActionResult<LearningPath>> CreateLearningPath([FromBody] LearningPathCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var learningPath = await _learningPathService.CreateLearningPath(request, userId);
            return CreatedAtAction(nameof(GetLearningPath), new { id = learningPath.Id }, learningPath);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LearningPathResponse>> GetLearningPath(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var learningPath = await _learningPathService.GetLearningPath(id, userId);
            if (learningPath == null) return NotFound();
            return Ok(learningPath);
        }

        [HttpPost("{id}/certificate")]
        public async Task<ActionResult<Certification>> GenerateCertificate(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var certification = await _learningPathService.GenerateCertification(userId, id);
                return Ok(certification);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
