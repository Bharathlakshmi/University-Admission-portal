using University_Admission_Portal.DTOs;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Service
{
    public interface ICourseService
    {
        Task<IEnumerable<GetCourseDTO>> GetAllCourses();
        Task<GetCourseDTO> GetCourseById(int id);
        Task<CourseDTO> CreateCourse(CreateCourseDTO dto);
        Task UpdateCourse(int id, CreateCourseDTO dto);
        Task<bool> DeleteCourse(int id);
       // Task<IEnumerable<CourseWithStudentsDTO>> GetAllCoursesWithStudents();
    }
}
