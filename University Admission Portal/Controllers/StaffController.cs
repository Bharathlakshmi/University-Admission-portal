using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using University_Admission_Portal.Data;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Service;

namespace University_Admission_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;
        private readonly UnivContext _context;

        public StaffController(IStaffService service, UnivContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetStaffDTO>>> GetAllStaff()
        {
            var staff = await _service.GetAllStaff();
            return Ok(staff);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetStaffDTO>> GetStaffById(int id)
        {
            var staffMember = await _service.GetStaffById(id);
            if (staffMember == null) return NotFound("Staff not found");
            return Ok(staffMember);
        }

        [HttpPost]
        public async Task<ActionResult<StaffDTO>> CreateStaff(CreateStaffDTO dto)
        {
            var created = await _service.CreateStaff(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStaff(int id, CreateStaffDTO dto)
        {
            await _service.UpdateStaff(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        
        public async Task<ActionResult> DeleteStaff(int id)
        {
            var deleted = await _service.DeleteStaff(id);
            if (!deleted) return NotFound("Staff not found");
            return NoContent();
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentStaff()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user token");
                }

                // Find staff by user ID
                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (staff == null)
                {
                    return NotFound("Staff not found");
                }

                // ✅ Return only the StaffId
                return Ok(new { staffId = staff.StaffId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
