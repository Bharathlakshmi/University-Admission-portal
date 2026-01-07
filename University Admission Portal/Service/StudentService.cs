using University_Admission_Portal.Data;
using University_Admission_Portal.DTOs;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Service
{
    public class StudentService : IStudentservice
    {
        private readonly IUniversity<Student, int> _repo;
        private readonly IUniversity<Application, int> _applicationRepo;
        private readonly IUniversity<Course, int> _courseRepo;
        private readonly UnivContext _context;

        public StudentService(
            IUniversity<Student, int> repo,
            IUniversity<Application, int> applicationRepo,
            IUniversity<Course, int> courseRepo, UnivContext context)
        {
            _repo = repo;
            _applicationRepo = applicationRepo;
            _courseRepo = courseRepo;
            _context = context;
        }

        public async Task<IEnumerable<GetStudentDTO>> GetAllStudents()
        {
            var students = await _repo.GetAll();
            var applications = await _applicationRepo.GetAll();
            var courses = await _courseRepo.GetAll();

            return students.Select(s =>
            {
                var studentApplications = applications
                    .Where(a => a.StudentId == s.StudentId)
                    .Select(a => courses.FirstOrDefault(c => c.CourseId == a.CourseId)?.CourseName ?? "Unknown")
                    .ToList();

                return new GetStudentDTO
                {
                    StudentId = s.StudentId,
                    UserId = s.UserId,
                    Gender = s.Gender,
                    DateOfBirth = s.DateOfBirth,
                    City = s.City,
                    ContactNumber = s.ContactNumber,
                    PhotoBase64 = s.Photo != null ? Convert.ToBase64String(s.Photo) : null,
                    MarksheetBase64 = s.Marksheet != null ? Convert.ToBase64String(s.Marksheet) : null,
                    ApplicationCourse = studentApplications
                };
            });
        }

        public async Task<GetStudentDTO> GetStudentById(int id)
        {
            var s = await _repo.GetById(id);
            if (s == null) return null;

            var applications = await _applicationRepo.GetAll();
            var courses = await _courseRepo.GetAll();

            var appliedCourses = applications
                .Where(a => a.StudentId == s.StudentId)
                .Select(a => courses.FirstOrDefault(c => c.CourseId == a.CourseId)?.CourseName ?? "Unknown")
                .ToList();

            return new GetStudentDTO
            {
                StudentId = s.StudentId,
                UserId = s.UserId,
                Gender = s.Gender,
                DateOfBirth = s.DateOfBirth,
                City = s.City,
                ContactNumber = s.ContactNumber,
                PhotoBase64 = s.Photo != null ? Convert.ToBase64String(s.Photo) : null,
                MarksheetBase64 = s.Marksheet != null ? Convert.ToBase64String(s.Marksheet) : null,
                ApplicationCourse = appliedCourses
            };
        }

        public async Task<StudentDTO> CreateStudent(CreateStudentDTO dto)
        {
            byte[]? photoBytes = null;
            byte[]? marksheetBytes = null;

            if (dto.Photo != null)
            {
                using var ms = new MemoryStream();
                await dto.Photo.CopyToAsync(ms);
                photoBytes = ms.ToArray();
            }

            if (dto.Marksheet != null)
            {
                using var ms = new MemoryStream();
                await dto.Marksheet.CopyToAsync(ms);
                marksheetBytes = ms.ToArray();
            }

            //age >= 16
            var age = DateTime.Now.Year - dto.DateOfBirth.Year;
            if (dto.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;

            if (age < 16)
                throw new InvalidOperationException("Students must be at least 16 years old to register.");

            var s = new Student
            {
                UserId = dto.UserId,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                City = dto.City,
                ContactNumber = dto.ContactNumber,
                Photo = photoBytes,
                Marksheet = marksheetBytes
            };
            var created = await _repo.Add(s);
            return new StudentDTO
            {
                StudentId = created.StudentId,
                UserId = created.UserId,
                Gender = created.Gender,
                DateOfBirth = created.DateOfBirth,
                City = created.City,
                ContactNumber = created.ContactNumber,
                Photo = created.Photo,
                Marksheet = created.Marksheet
            };
        }

        public async Task UpdateStudent(int id, CreateStudentDTO dto)
        {
            byte[]? photoBytes = null;
            byte[]? marksheetBytes = null;

            if (dto.Photo != null)
            {
                using var ms = new MemoryStream();
                await dto.Photo.CopyToAsync(ms);
                photoBytes = ms.ToArray();
            }

            if (dto.Marksheet != null)
            {
                using var ms = new MemoryStream();
                await dto.Marksheet.CopyToAsync(ms);
                marksheetBytes = ms.ToArray();
            }

            //age >= 16
            var age = DateTime.Now.Year - dto.DateOfBirth.Year;
            if (dto.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;
            if (age < 16)
                throw new InvalidOperationException("Students must be at least 16 years old to register.");


            var s = new Student
            {
                StudentId = id,
                UserId = dto.UserId,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                City = dto.City,
                ContactNumber = dto.ContactNumber,
                Photo = photoBytes,
                Marksheet = marksheetBytes
            };
            await _repo.Update(id, s);
        }

        public async Task<bool> DeleteStudent(int id)
        {
            try { await _repo.Delete(id); return true; }
            catch { return false; }
        }

        
       
    }
}
