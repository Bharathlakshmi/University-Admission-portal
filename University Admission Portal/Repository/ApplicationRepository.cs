using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Data;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Repository
{
    public class ApplicationRepository : IUniversity<Application,int>,IApplication
    {
        private readonly UnivContext _context;

        public ApplicationRepository(UnivContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Application>> GetAll()
        {
            return await _context.Applications
        .Include(a => a.Student)
            .ThenInclude(s => s.User)
        .Include(a => a.Course)
        .Include(a => a.ApprovedByStaff)
            .ThenInclude(st => st.User)
        .ToListAsync();
        }

        public async Task<Application> GetById(int id)
        {
            return await _context.Applications
                   .Include(a => a.Student)
                       .ThenInclude(s => s.User)
                   .Include(a => a.Course)
                   .Include(a => a.ApprovedByStaff)
                       .ThenInclude(st => st.User)
                   .FirstOrDefaultAsync(a => a.ApplicationId == id);
        }

        public async Task<Application> Add(Application entity)
        {
            _context.Applications.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Update(int id, Application entity)
        {
            if (id != entity.CourseId) throw new KeyNotFoundException();
            _context.Applications.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Application app)
        {
            _context.Applications.Update(app);
            await _context.SaveChangesAsync();
        }


        public async Task Delete(int id)
        {
            var appcn = await _context.Applications.FindAsync(id);
            if (appcn == null) throw new KeyNotFoundException();
            _context.Applications.Remove(appcn);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Course>> GetAllCoursesWithApplications()
        {
            return await _context.Courses
                .Include(c => c.Applications)
                    .ThenInclude(a => a.Student)
                        .ThenInclude(s => s.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetApplicationsByStudentId(int studentId)
        {
            return await _context.Applications
              .Include(a => a.Course)
              .Where(a => a.StudentId == studentId)
              .ToListAsync();
        }


    }
}
