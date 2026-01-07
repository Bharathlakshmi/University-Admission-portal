using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using University_Admission_Portal.Email_Notification;

namespace University_Admission_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                await _emailService.SendEmailAsync("krishika1119@gmail.com", "Test Email", "This is a test email from University Admission Portal.");
                return Ok("Test email sent successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send email: {ex.Message}");
            }
        }
    }
}
