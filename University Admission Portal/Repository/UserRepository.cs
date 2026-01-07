using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Data;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;


namespace University_Admission_Portal.Repository
{
    public class UserRepository : IUniversity<User, int>
    {
        private readonly UnivContext _context;

        public UserRepository(UnivContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(a=>a.UserId==id);
        }
        public async Task<User> Add(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task Update(int id, User user)
        {
            if(id!=user.UserId) throw new KeyNotFoundException();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var user=await _context.Users.FirstOrDefaultAsync(a=>a.UserId==id);
            if(user==null) throw new KeyNotFoundException();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

     

       
    }
}
