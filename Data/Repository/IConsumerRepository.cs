using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMI_WebAPI.Data.Repository
{
    public interface IConsumerRepository
    {
        // 🔹 Get all consumers
        Task<IEnumerable<ConsumerDTO>> GetAllConsumersAsync();

        // 🔹 Get consumer by ID
        Task<ConsumerDTO?> GetConsumerByIdAsync(long consumerId);

        // 🔹 Get consumers by status (e.g., Active, Inactive)
        Task<IEnumerable<ConsumerDTO>> GetConsumersByStatusAsync(string status);

        // 🔹 Add a new consumer
        Task<ConsumerDTO> AddConsumerAsync(ConsumerDTO consumer);

        // 🔹 Update an existing consumer
        Task<ConsumerDTO?> UpdateConsumerAsync(ConsumerDTO consumer);

        // 🔹 Delete a consumer by ID
        Task<bool> DeleteConsumerAsync(long consumerId);

        // 🔹 Change consumer status (e.g. Active → Inactive)
        Task<bool> UpdateConsumerStatusAsync(long consumerId, string newStatus);


        //Task<Consumer?> GetConsumerByEmailAndNameAsync(string email, string name);

        Task<Consumer?> GetConsumerByEmailAndPasswordAsync(string email, string password);


    }
}
