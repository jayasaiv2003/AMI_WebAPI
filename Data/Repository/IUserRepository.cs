using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;

namespace AMI_WebAPI.Data.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task<bool> UpdateUserAsync(int id, UserDTO userDto);
        Task<bool> DeleteUserAsync(int id);

        Task<User?> GetUserByUsernameAsync(string username);
    }
}
