using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Data;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Repository
{
    public class StaffRepository : IUniversity<Staff, int>
    {
        private readonly UnivContext _context;

        public StaffRepository(UnivContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Staff>> GetAll()
        {
            return await _context.Staffs
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<Staff> GetById(int id)
        {
            return await _context.Staffs
                .Include(s => s.User)
                .FirstOrDefaultAsync(a => a.StaffId == id);
        }
        public async Task<Staff> Add(Staff staff)
        {
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task Update(int id, Staff staff)
        {
            if (id != staff.StaffId) throw new KeyNotFoundException();
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(a => a.StaffId == id);
            if (staff == null) throw new KeyNotFoundException();
            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
        }

    }
}
