using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Consumer_App : ControllerBase
    {
        private readonly IConsumerRepository _consumerRepository;

        public Consumer_App(IConsumerRepository consumerRepository)
        {
            _consumerRepository = consumerRepository;
        }
        private string GetUsernameFromToken()
        {
            // Safely extract the username from the JWT claim
            return User?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? "System";
        }


        // 🔹 Get all consumers
        [HttpGet("all")]
        public async Task<IActionResult> GetAllConsumers()
        {
            var consumers = await _consumerRepository.GetAllConsumersAsync();
            return Ok(consumers);
        }

        // 🔹 Get consumer by ID
        [HttpGet("{consumerId}")]
        public async Task<IActionResult> GetConsumerById(long consumerId)
        {
            var consumer = await _consumerRepository.GetConsumerByIdAsync(consumerId);
            if (consumer == null)
                return NotFound($"Consumer with ID {consumerId} not found.");

            return Ok(consumer);
        }

        // 🔹 Get consumers by status
        // 🔹 Get consumers by status
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetConsumersByStatus(string status)
        {
            var consumers = await _consumerRepository.GetConsumersByStatusAsync(status);
            if (!consumers.Any())
                return NotFound($"No consumers found with status '{status}'.");
            return Ok(consumers);
        }

        // 🔹 Add a new consumer
        [HttpPost("add")]
        public async Task<IActionResult> AddConsumer([FromBody] ConsumerDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid consumer data.");

            //var username = GetUsernameFromToken();

            //dto.CreatedAt = DateTime.UtcNow;
            //dto.CreatedBy = username;
            //dto.UpdatedAt = null;
            //dto.UpdatedBy = null;

            var createdConsumer = await _consumerRepository.AddConsumerAsync(dto);
            return Ok(new
            {
                message = "Consumer added successfully!",
                data = createdConsumer
            });
        }

        // 🔹 Update existing consumer
        [HttpPut("update")]
        public async Task<IActionResult> UpdateConsumer([FromBody] ConsumerDTO dto)
        {
            if (dto == null || dto.ConsumerId == 0)
                return BadRequest("Invalid consumer data.");
           
            //var username = GetUsernameFromToken();

            //dto.CreatedAt = DateTime.UtcNow;
            //dto.CreatedBy = username;
            //dto.UpdatedAt = null;
            //dto.UpdatedBy = null;

            var updatedConsumer = await _consumerRepository.UpdateConsumerAsync(dto);
            if (updatedConsumer == null)
                return NotFound($"Consumer with ID {dto.ConsumerId} not found.");

            return Ok(new
            {
                message = "Consumer updated successfully!",
                data = updatedConsumer
            });
        }

        // 🔹 Delete consumer by ID
        [HttpDelete("delete/{consumerId}")]
        public async Task<IActionResult> DeleteConsumer(long consumerId)
        {
            var result = await _consumerRepository.DeleteConsumerAsync(consumerId);
            if (!result)
                return NotFound($"Consumer with ID {consumerId} not found.");

            return Ok(new { message = "Consumer deleted successfully!" });
        }

        // 🔹 Update consumer status (Active/Inactive)
        [HttpPatch("status/{consumerId}")]
        public async Task<IActionResult> UpdateConsumerStatus(long consumerId, [FromQuery] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                return BadRequest("New status must be provided.");

            var success = await _consumerRepository.UpdateConsumerStatusAsync(consumerId, newStatus);
            if (!success)
                return NotFound($"Consumer with ID {consumerId} not found.");

            return Ok(new { message = $"Consumer status updated to '{newStatus}' successfully!" });
        }
    }
}

