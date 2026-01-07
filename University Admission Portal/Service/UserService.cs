using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Data;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Service
{
    public class UserService : IUserService
    {
        private readonly IUniversity<User, int> _repo;
        private readonly UnivContext _context;
        public UserService(IUniversity<User, int> repo, UnivContext context)
        {
            _repo = repo;
            _context = context;
        }
        public async Task<IEnumerable<GetUserDTO>> GetAllUsers()
        {
            var users = await _repo.GetAll();
            return users.Select(x => new GetUserDTO
            {
                UserId = x.UserId,
                Name = x.Name,
                Email = x.Email,
                Password = x.Password,
                Role = x.Role

            });
        }

        public async Task<GetUserDTO> GetUserById(int id)
        {
            var u = await _repo.GetById(id);
            if (u == null) return null;
            return new GetUserDTO
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password,
                Role = u.Role
            };
        }

        public async Task<UserDTO> CreateUser(CreateUserDTO dto)
        {
            bool emailExists = await _context.Users
               .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
                throw new InvalidOperationException("This Email is already registered");

            var user = new User
            {
                 Name = dto.Name,
                 Email = dto.Email,
                 Password = dto.Password,
                 Role = dto.Role
            };

            var created = await _repo.Add(user);
            return new UserDTO
            {
                UserId= created.UserId,
                Name = created.Name,
                Email = created.Email,
                Password = created.Password,
                Role = created.Role
            };
        }


        public async Task UpdateUser(int id, CreateUserDTO dto)
        {
            var u = new User
            {
                UserId = id,
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role

            };
            await _repo.Update(id, u);
        }

        public async Task<bool> DeleteUser(int id)
        {
            try { await _repo.Delete(id); return true; }
            catch { return false; }
        }

       

    }
}
