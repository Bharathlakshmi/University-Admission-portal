using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using University_Admission_Portal.Data;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UnivContext _context;
        private readonly SymmetricSecurityKey _key;

        public TokenController(UnivContext context, IConfiguration config)
        {
            _context = context;
            _key = new SymmetricSecurityKey(UTF8Encoding.UTF8.GetBytes(config["Key"]!));

        }
        [HttpPost]
        public IActionResult GenerateToken([FromBody] loginDTO logindto)
        {
            var user = ValidateUser(logindto.Email, logindto.Password);
            
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var claims = new List<Claim>
               {
                   new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                   new Claim(JwtRegisteredClaimNames.NameId, user.Name!),
                   new Claim(JwtRegisteredClaimNames.Email, user.Email),
               };

            if (user.Role != null)
                claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));

            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var tokenDescription = new SecurityTokenDescriptor
            {
                SigningCredentials = cred,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(30)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var createToken = tokenHandler.CreateToken(tokenDescription);
            var token = tokenHandler.WriteToken(createToken);
            
            return Ok(new { token, role = user.Role });
        }
        private User ValidateUser(string email, string password)
        {
            var users = _context.Users.ToList();
            var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
            return user;
        }
    }
}
