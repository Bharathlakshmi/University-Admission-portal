using University_Admission_Portal.DTOs;

namespace University_Admission_Portal.Service
{
    public interface IUserService
    {
        Task<IEnumerable<GetUserDTO>> GetAllUsers();
        Task<GetUserDTO> GetUserById(int id);
       
        Task<UserDTO> CreateUser(CreateUserDTO dto);
        Task UpdateUser(int id, CreateUserDTO dto);

        Task<bool> DeleteUser(int id);
    }
}
