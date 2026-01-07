using Microsoft.EntityFrameworkCore;
using University_Admission_Portal.Data;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Repository
{
    public class StudentRepository : IUniversity<Student, int>
    {
        private readonly UnivContext _context;

        public StudentRepository(UnivContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> GetById(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(a => a.StudentId == id);
        }
        public async Task<Student> Add(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task Update(int id, Student student)
        {
            if (id != student.StudentId) throw new KeyNotFoundException();
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(a => a.StudentId == id);
            if (student == null) throw new KeyNotFoundException();
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }



    }
}
