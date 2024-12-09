using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Services;

namespace OnlineLearningPlatform.Controllers
{
    // Controllers/QuizController.cs
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly QuizService _quizService;

        public QuizController(QuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateQuiz([FromBody] Quiz quiz)
        {
            var createdQuiz = await _quizService.CreateQuiz(quiz);
            return CreatedAtAction(nameof(GetQuiz), new { id = createdQuiz.Id }, createdQuiz);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuiz(string id)
        {
            var quiz = await _quizService.GetQuizById(id);
            return quiz != null ? Ok(quiz) : NotFound();
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetQuizzesByCourse(string courseId)
        {
            var quizzes = await _quizService.GetQuizzesByCourse(courseId);
            return Ok(quizzes);
        }
    }
}
