using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AmidbContext _context;

        public UserRepository(AmidbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    DisplayName = u.DisplayName,
                    Email = u.Email,
                    Phone = u.Phone,
                    LastLogin = u.LastLogin,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Phone = user.Phone,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                DisplayName = userDto.DisplayName,
                Email = userDto.Email,
                Phone = userDto.Phone,
                PasswordHash = "Default@123", // You can hash later if needed
                LastLogin = userDto.LastLogin,
                IsActive = userDto.IsActive
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDto.UserId = user.UserId;
            return userDto;
        }

        public async Task<bool> UpdateUserAsync(int id, UserDTO userDto)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            existingUser.Username = userDto.Username;
            existingUser.DisplayName = userDto.DisplayName;
            existingUser.Email = userDto.Email;
            existingUser.Phone = userDto.Phone;
            existingUser.LastLogin = userDto.LastLogin;
            existingUser.IsActive = userDto.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower().Trim() == username.ToLower().Trim()
                    && u.IsActive);
        }

    }
}
