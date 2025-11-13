using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApp : Controller
    {
        private readonly IUserRepository _userRepo;

        public UserApp(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepo.GetAllUsersAsync();
            return Ok(users);
        }

        // 🔹 GET: api/User_App/get-user-by-id/{id}
        [HttpGet("get-user-by-id/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(user);
        }



        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userRepo.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user); // returns the actual User model, not DTO
        }

        // 🔹 POST: api/User_App/add-user
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUser = await _userRepo.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(GetUserById),
                new { id = createdUser.UserId },
                createdUser);
        }

        // 🔹 PUT: api/User_App/update-user/{id}
        [HttpPut("update-user/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            var success = await _userRepo.UpdateUserAsync(id, userDto);
            if (!success)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(new { message = "User updated successfully." });
        }

        // 🔹 DELETE: api/User_App/delete-user/{id}
        [HttpDelete("delete-user/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userRepo.DeleteUserAsync(id);
            if (!success)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
