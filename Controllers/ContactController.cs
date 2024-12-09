using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models;
using OnlineLearningPlatform.Services;

namespace OnlineLearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (contact == null)
            {
                return BadRequest("Invalid contact data.");
            }

            await _contactService.CreateAsync(contact);
            return Ok(new { message = "Contact saved successfully" });
        }
    }
}
