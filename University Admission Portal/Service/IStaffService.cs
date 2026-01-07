using University_Admission_Portal.DTOs;

namespace University_Admission_Portal.Service
{
    public interface IStaffService
    {
        Task<IEnumerable<GetStaffDTO>> GetAllStaff();
        Task<GetStaffDTO> GetStaffById(int id);
        Task<StaffDTO> CreateStaff(CreateStaffDTO dto);
        Task UpdateStaff(int id, CreateStaffDTO dto);
        Task<bool> DeleteStaff(int id);
    }
}
