using AMI_WebAPI.Models;

namespace AMI_WebAPI.Data.Repository
{
    public interface ITariffRepository
    {
        // 🔹 Get all tariffs
        Task<IEnumerable<Tariff>> GetAllTariffsAsync();

        // 🔹 Get a single tariff by ID
        Task<Tariff?> GetTariffByIdAsync(int tariffId);

        // 🔹 Add a new tariff
        Task AddTariffAsync(Tariff tariff);

        // 🔹 Update an existing tariff
        Task UpdateTariffAsync(Tariff tariff);

        // 🔹 Delete a tariff by ID
        Task DeleteTariffAsync(int tariffId);

        // 🔹 Check if a tariff with the same name exists (optional validation)
        Task<bool> TariffExistsByNameAsync(string name);

        Task<bool> TariffDateOverlapAsync(string name, DateOnly from, DateOnly to);
    }
}
