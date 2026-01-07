using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Service;

namespace University_Admission_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }
        [HttpGet]
       
        public async Task<ActionResult<IEnumerable<GetUserDTO>>> GetAllUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDTO>> GetUserById(int id)
        {
            var user = await _service.GetUserById(id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<CreateUserDTO>> CreateUser(CreateUserDTO userDto)
        {
            try
            {
                var created = await _service.CreateUser(userDto);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, CreateUserDTO userDto)
        {
            await _service.UpdateUser(id, userDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deleted = await _service.DeleteUser(id);
            if (!deleted) return NotFound("User not found");
            return NoContent();
        }
    }
}
