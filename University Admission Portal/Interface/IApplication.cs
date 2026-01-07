using University_Admission_Portal.Models;

namespace University_Admission_Portal.Interface
{
    public interface IApplication
    {
        Task Update(Application entity);
        Task<IEnumerable<Course>> GetAllCoursesWithApplications();
        Task<IEnumerable<Application>> GetApplicationsByStudentId(int studentId);
    }
}
