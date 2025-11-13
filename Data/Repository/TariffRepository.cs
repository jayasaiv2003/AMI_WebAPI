using AMI_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Data.Repository
{
    public class TariffRepository : ITariffRepository
    {
        private readonly AmidbContext _context;

        public TariffRepository(AmidbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tariff>> GetAllTariffsAsync()
        {
            return await _context.Tariffs.ToListAsync();
        }

        public async Task<Tariff?> GetTariffByIdAsync(int tariffId)
        {
            return await _context.Tariffs.FindAsync(tariffId);
        }

        public async Task AddTariffAsync(Tariff tariff)
        {
            _context.Tariffs.Add(tariff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTariffAsync(Tariff tariff)
        {
            _context.Tariffs.Update(tariff);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTariffAsync(int tariffId)
        {
            var tariff = await _context.Tariffs.FindAsync(tariffId);
            if (tariff != null)
            {
                _context.Tariffs.Remove(tariff);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TariffExistsByNameAsync(string name)
        {
            return await _context.Tariffs.AnyAsync(t => t.Name == name);
        }
    }
}
