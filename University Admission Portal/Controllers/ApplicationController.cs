using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Models;
using University_Admission_Portal.Service;

namespace University_Admission_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _service;

        public ApplicationController(IApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        //   [Authorize(Roles = "admin,Student,Staff")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetApplicationDTO>>> GetAllApplications()
        {
            var applications = await _service.GetAllApplications();
            return Ok(applications);
        }

        [HttpGet("{id}")]
    //    [Authorize(Roles = "admin,Student,Staff")]
        public async Task<ActionResult<GetApplicationDTO>> GetApplicationById(int id)
        {
            var application = await _service.GetApplicationById(id);
            if (application == null) return NotFound("Application not found");
            return Ok(application);
        }

        [HttpPost]
        //    [Authorize(Roles = "admin,Student,Staff")]
        [Authorize]
        public async Task<ActionResult<ApplicationDTO>> CreateApplication(CreateApplicationDTO dto)
        {
            var created = await _service.CreateApplication(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
       //     [Authorize(Roles = "Staff,admin")]
        public async Task<ActionResult> UpdateApplication(int id, CreateApplicationDTO dto)
        {
            await _service.UpdateApplication(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
      //      [Authorize(Roles = "admin,Student,Staff")]
        public async Task<ActionResult> DeleteApplication(int id)
        {
            var deleted = await _service.DeleteApplication(id);
            if (!deleted) return NotFound("Application not found");
            return NoContent();
        }

        [HttpPut("{id}/status")]
        //      [Authorize(Roles = "Staff,admin")]
        [Authorize]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromQuery] ApplicationStatus status, [FromQuery] int staffId)
        {
            try
            {
                await _service.UpdateApplicationStatus(id, status, staffId);
                return Ok("Application status updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpGet("course-wise")]
        [Authorize]
        public async Task<IActionResult> GetAllApplicationsGroupedByCourse()
        {
            var result = await _service.GetAllApplicationsGroupedByCourse();
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetApplicationsByStudentId( int studentId)
        {
            var result = await _service.GetApplicationsByStudentId(studentId);
            return Ok(result);
        }




    }
}
