using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Service;

namespace University_Admission_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _service;

        public CourseController(ICourseService service)
        {
            _service = service;
        }

        [HttpGet]
        // [Authorize(Roles ="admin,Student,Staff")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetCourseDTO>>> GetAllCourses()
        {
            var courses = await _service.GetAllCourses();
            return Ok(courses);
        }

        [HttpGet("{id}")]
      //  [Authorize(Roles = "admin,Student,Staff")]
        public async Task<ActionResult<GetCourseDTO>> GetCourseById(int id)
        {
            var course = await _service.GetCourseById(id);
            if (course == null) return NotFound("Course not found");
            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<CourseDTO>> CreateCourse(CreateCourseDTO dto)
        {
            var created = await _service.CreateCourse(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
     //   [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateCourse(int id, CreateCourseDTO dto)
        {
            await _service.UpdateCourse(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
     //   [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var deleted = await _service.DeleteCourse(id);
            if (!deleted) return NotFound("Course not found");
            return NoContent();
        }

       
    }
}
