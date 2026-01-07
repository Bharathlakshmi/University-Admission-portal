using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Data;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Repository
{
    public class CourseRepository : IUniversity<Course, int>
    {
        private readonly UnivContext _context;



        public CourseRepository(UnivContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAll()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetById(int id)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<Course> Add(Course entity)
        {
            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Update(int id, Course entity)
        {
            if (id != entity.CourseId) throw new KeyNotFoundException();
            _context.Courses.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var crs = await _context.Courses.FindAsync(id);
            if (crs == null) throw new KeyNotFoundException();
            _context.Courses.Remove(crs);
            await _context.SaveChangesAsync();
        }

      
    }
}
