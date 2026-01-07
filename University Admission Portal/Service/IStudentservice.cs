using University_Admission_Portal.DTOs;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Service
{
    public interface IStudentservice
    {
        Task<IEnumerable<GetStudentDTO>> GetAllStudents();
        Task<GetStudentDTO> GetStudentById(int id);
        Task<StudentDTO> CreateStudent(CreateStudentDTO dto);
        Task UpdateStudent(int id, CreateStudentDTO dto);
        Task<bool> DeleteStudent(int id);

        //Task<Student?> GetStudentEntityById(int id);
    }
}
