using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Data;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Service
{
    public class CourseService : ICourseService
    {
        private readonly IUniversity<Course, int> _repo;
        private readonly IUniversity<Application, int> _apprepo;
        private readonly UnivContext _context;


        public CourseService(IUniversity<Course, int> repo, IUniversity<Application, int> apprepo, UnivContext context)
        {
            _repo = repo;
            _apprepo = apprepo;
            _context = context;
        }

        public async Task<IEnumerable<GetCourseDTO>> GetAllCourses()
        {
            var courses = await _repo.GetAll();

            //
            var applications = await _apprepo.GetAll();



            return courses.Select(x => new GetCourseDTO
            {
                CourseId = x.CourseId,
                CourseCode = x.CourseCode,
                CourseName = x.CourseName,
                Duration = x.Duration,
                TotalSeats = x.TotalSeats,

                 RemainingSeats = x.TotalSeats - applications.Count(a =>
                     a.CourseId == x.CourseId &&
                     a.Status == ApplicationStatus.Approved)
            });
        }

        public async Task<GetCourseDTO> GetCourseById(int id)
        {
            var c = await _repo.GetById(id);
            if (c == null) return null;

            //
            var applications = await _apprepo.GetAll();

            return new GetCourseDTO
            {
                CourseId = c.CourseId,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Duration = c.Duration,
                TotalSeats = c.TotalSeats,
                RemainingSeats = c.TotalSeats - applications.Count(a =>
                    a.CourseId == c.CourseId &&
                    a.Status == ApplicationStatus.Approved)
            };
        }

        public async Task<CourseDTO> CreateCourse(CreateCourseDTO dto)
        {
            bool coursecodeExists = await _context.Courses
               .AnyAsync(u => u.CourseCode == dto.CourseCode);

            if (coursecodeExists)
                throw new InvalidOperationException("This Course Code is already registered");

            bool courseNameExists = await _context.Courses
               .AnyAsync(u => u.CourseName == dto.CourseName);

            if (courseNameExists)
                throw new InvalidOperationException("This Course Name is already registered");



            var c = new Course
            {
                CourseCode = dto.CourseCode,
                CourseName = dto.CourseName,
                Duration = dto.Duration,
                TotalSeats = dto.TotalSeats
            };
            var created = await _repo.Add(c);
            return new CourseDTO
            {
                CourseId = created.CourseId,
                CourseCode = created.CourseCode,
                CourseName = created.CourseName,
                Duration = created.Duration,
                TotalSeats = created.TotalSeats
            };
        }

        public async Task UpdateCourse(int id, CreateCourseDTO dto)
        {
            var c = new Course
            {
                CourseId = id,
                CourseCode = dto.CourseCode,
                CourseName = dto.CourseName,
                Duration = dto.Duration,
                TotalSeats = dto.TotalSeats
            };
            await _repo.Update(id, c);
        }

        public async Task<bool> DeleteCourse(int id)
        {
            try { await _repo.Delete(id); return true; }
            catch { return false; }
        }

        /*
        public async Task<IEnumerable<CourseWithStudentsDTO>> GetAllCoursesWithStudents()
        {
            var courses = await _repo.GetAll();
            var applications = await _apprepo.GetAll();

            var result = courses.Select(c => new CourseWithStudentsDTO
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Students = applications
                    .Where(a => a.CourseId == c.CourseId && a.Student != null)
                    .Select(a => new StudentInCourseDTO
                    {
                        StudentId = a.Student.StudentId,
                        Name = a.Student.User?.Name ?? "Unknown",
                        City = a.Student.City,
                        PhotoBase64 = a.Student.Photo != null
                            ? Convert.ToBase64String(a.Student.Photo)
                            : null,
                        MarksheetBase64 = a.Student.Marksheet != null
                            ? Convert.ToBase64String(a.Student.Marksheet)
                            : null
                    })
                    .ToList()
            });

            return result;
        }*/
    }
}

