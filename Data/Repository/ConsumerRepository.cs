using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Data.Repository
{
    public class ConsumerRepository : IConsumerRepository
    {
        private readonly AmidbContext _context;

        public ConsumerRepository(AmidbContext context)
        {
            _context = context;
        }

        // 🔹 Get all consumers
        public async Task<IEnumerable<ConsumerDTO>> GetAllConsumersAsync()
        {
            return await _context.Consumers
                .Select(c => new ConsumerDTO
                {
                    ConsumerId = c.ConsumerId,
                    Name = c.Name,
                    Address = c.Address,
                    Phone = c.Phone,
                    Email = c.Email,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedAt = c.UpdatedAt,
                    UpdatedBy = c.UpdatedBy
                })
                .ToListAsync();
        }

        // 🔹 Get consumer by ID
        public async Task<ConsumerDTO?> GetConsumerByIdAsync(long consumerId)
        {
            var consumer = await _context.Consumers.FindAsync(consumerId);
            if (consumer == null) return null;

            return new ConsumerDTO
            {
                ConsumerId = consumer.ConsumerId,
                Name = consumer.Name,
                Address = consumer.Address,
                Phone = consumer.Phone,
                Email = consumer.Email,
                Status = consumer.Status,
                CreatedAt = consumer.CreatedAt,
                CreatedBy = consumer.CreatedBy,
                UpdatedAt = consumer.UpdatedAt,
                UpdatedBy = consumer.UpdatedBy
            };
        }

        // 🔹 Get consumers by status
        public async Task<IEnumerable<ConsumerDTO>> GetConsumersByStatusAsync(string status)
        {
            return await _context.Consumers
                .Where(c => c.Status == status)
                .Select(c => new ConsumerDTO
                {
                    ConsumerId = c.ConsumerId,
                    Name = c.Name,
                    Address = c.Address,
                    Phone = c.Phone,
                    Email = c.Email,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedAt = c.UpdatedAt,
                    UpdatedBy = c.UpdatedBy
                })
                .ToListAsync();
        }

        // 🔹 Add new consumer
        public async Task<ConsumerDTO> AddConsumerAsync(ConsumerDTO dto)
        {
            var entity = new Consumer
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.CreatedBy
            };

            _context.Consumers.Add(entity);
            await _context.SaveChangesAsync();

            dto.ConsumerId = entity.ConsumerId;
            dto.CreatedAt = entity.CreatedAt;

            return dto;
        }

        // 🔹 Update consumer
        // 🔹 Update consumer (and cascade status to meters)
        // 🔹 Update consumer (and cascade status to meters only when deactivated)
        public async Task<ConsumerDTO?> UpdateConsumerAsync(ConsumerDTO dto)
        {
            var consumer = await _context.Consumers
                .Include(c => c.Meters) // ✅ Load related meters
                .FirstOrDefaultAsync(c => c.ConsumerId == dto.ConsumerId);

            if (consumer == null) return null;

            // Store old status before updating
            var oldStatus = consumer.Status;

            // 🔹 Update consumer info
            consumer.Name = dto.Name;
            consumer.Address = dto.Address;
            consumer.Phone = dto.Phone;
            consumer.Email = dto.Email;
            consumer.Status = dto.Status;
            consumer.UpdatedAt = DateTime.UtcNow;
            consumer.UpdatedBy = dto.UpdatedBy;

            // ✅ Only deactivate meters when consumer becomes inactive
            if (oldStatus != "Inactive" && dto.Status == "Inactive")
            {
                if (consumer.Meters != null && consumer.Meters.Any())
                {
                    foreach (var meter in consumer.Meters)
                    {
                        meter.Status = "Inactive";         // Force inactive
                        //meter.UpdatedAt = DateTime.UtcNow; // Track change time
                    }
                }
            }

            await _context.SaveChangesAsync();

            return dto;
        }



        // 🔹 Delete consumer
        public async Task<bool> DeleteConsumerAsync(long consumerId)
        {
            var consumer = await _context.Consumers
                .Include(c => c.Meters)
                    .ThenInclude(m => m.DailyReadings)
                .Include(c => c.Meters)
                    .ThenInclude(m => m.MonthlyBills)
                .FirstOrDefaultAsync(c => c.ConsumerId == consumerId);

            if (consumer == null)
                return false;

            // 1️⃣ Delete all daily readings and monthly bills for each meter
            foreach (var meter in consumer.Meters)
            {
                _context.DailyReadings.RemoveRange(meter.DailyReadings);
                _context.MonthlyBills.RemoveRange(meter.MonthlyBills);
            }

            // 2️⃣ Delete all meters
            _context.Meters.RemoveRange(consumer.Meters);

            // 3️⃣ Delete consumer
            _context.Consumers.Remove(consumer);

            await _context.SaveChangesAsync();

            return true;
        }




        // 🔹 Update consumer status
        public async Task<bool> UpdateConsumerStatusAsync(long consumerId, string newStatus)
        {
            var entity = await _context.Consumers.FindAsync(consumerId);
            if (entity == null) return false;

            entity.Status = newStatus;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        //public async Task<Consumer?> GetConsumerByEmailAndNameAsync(string email, string name)
        //{
        //    return await _context.Consumers
        //        .FirstOrDefaultAsync(c =>
        //            c.Email.ToLower().Trim() == email.ToLower().Trim() &&
        //            c.Name.ToLower().Trim() == name.ToLower().Trim() &&
        //            c.Status == "Active");
        //}


        public async Task<Consumer?> GetConsumerByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.Consumers
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
        }

    }
}
