using University_Admission_Portal.DTOs;
using University_Admission_Portal.Models;

namespace University_Admission_Portal.Service
{
    public interface IApplicationService
    {
        Task<IEnumerable<GetApplicationDTO>> GetAllApplications();
        Task<GetApplicationDTO> GetApplicationById(int id);
        Task<ApplicationDTO> CreateApplication(CreateApplicationDTO dto);
        Task UpdateApplication(int id, CreateApplicationDTO dto);
        Task<bool> DeleteApplication(int id);

        Task UpdateApplicationStatus(int id, ApplicationStatus newStatus, int staffId);

        Task<IEnumerable<CourseApplicationsDTO>> GetAllApplicationsGroupedByCourse();

        Task<IEnumerable<object>> GetApplicationsByStudentId(int studentId);

    }
}

