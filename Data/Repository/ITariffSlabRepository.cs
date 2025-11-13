using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;

namespace AMI_WebAPI.Data.Repository
{
    public interface ITariffSlabRepository
    {
        // 🔹 Get all slabs for a specific tariff
        Task<IEnumerable<TariffSlabDTO>> GetSlabsByTariffIdAsync(int tariffId);

        // 🔹 Get a specific slab by ID
        Task<TariffSlabDTO?> GetSlabByIdAsync(int slabId);

        // 🔹 Add a new tariff slab
        Task<TariffSlabDTO> AddSlabAsync(TariffSlabDTO slab);

        // 🔹 Update an existing tariff slab
        Task<TariffSlabDTO?> UpdateSlabAsync(TariffSlabDTO slab);

        // 🔹 Delete a tariff slab by ID
        Task<bool> DeleteSlabAsync(int slabId);
    }
}
