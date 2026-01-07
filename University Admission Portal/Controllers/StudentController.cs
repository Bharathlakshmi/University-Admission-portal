using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using University_Admission_Portal.Controllers;
using University_Admission_Portal.Data;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Models;
using University_Admission_Portal.Service;

namespace University_Admission_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentservice _service;
        private readonly UnivContext _context;

        public StudentController(IStudentservice service,UnivContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetStudentDTO>>> GetAllStudents()
        {
            var students = await _service.GetAllStudents();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetStudentDTO>> GetStudentById(int id)
        {
            var student = await _service.GetStudentById(id);
            if (student == null) return NotFound("Student not found");
            return Ok(student);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<StudentDTO>> CreateStudent(CreateStudentDTO dto)
        {
            var created = await _service.CreateStudent(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateStudent(int id, CreateStudentDTO dto)
        {
            await _service.UpdateStudent(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
     
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var deleted = await _service.DeleteStudent(id);
            if (!deleted) return NotFound("Student not found");
            return NoContent();
        }

        // [Authorize(Roles = "Student")]
        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentStudent()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user token");
                }

                // Find student by user ID
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                return Ok(new { studentId = student.StudentId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }


}

