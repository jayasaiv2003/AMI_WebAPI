using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Data.Repository
{
    public class TariffSlabRepository : ITariffSlabRepository
    {
        private readonly AmidbContext _context;

        public TariffSlabRepository(AmidbContext context)
        {
            _context = context;
        }

        // 🔹 Get all slabs for a specific tariff
        public async Task<IEnumerable<TariffSlabDTO>> GetSlabsByTariffIdAsync(int tariffId)
        {
            return await _context.TariffSlabs
                .Where(s => s.TariffId == tariffId)
                .Select(s => new TariffSlabDTO
                {
                    TariffSlabId = s.TariffSlabId,
                    TariffId = s.TariffId,
                    FromKwh = s.FromKwh,
                    ToKwh = s.ToKwh,
                    RatePerKwh = s.RatePerKwh
                })
                .ToListAsync();
        }

        // 🔹 Get a specific slab by ID
        public async Task<TariffSlabDTO?> GetSlabByIdAsync(int slabId)
        {
            var slab = await _context.TariffSlabs.FindAsync(slabId);
            if (slab == null)
                return null;

            return new TariffSlabDTO
            {
                TariffSlabId = slab.TariffSlabId,
                TariffId = slab.TariffId,
                FromKwh = slab.FromKwh,
                ToKwh = slab.ToKwh,
                RatePerKwh = slab.RatePerKwh
            };
        }

        // 🔹 Add a new tariff slab
        public async Task<TariffSlabDTO> AddSlabAsync(TariffSlabDTO slabDto)
        {
            var slab = new TariffSlab
            {
                TariffId = slabDto.TariffId,
                FromKwh = slabDto.FromKwh,
                ToKwh = slabDto.ToKwh,
                RatePerKwh = slabDto.RatePerKwh
            };

            _context.TariffSlabs.Add(slab);
            await _context.SaveChangesAsync();

            slabDto.TariffSlabId = slab.TariffSlabId; // return generated ID
            return slabDto;
        }

        // 🔹 Update an existing tariff slab
        public async Task<TariffSlabDTO?> UpdateSlabAsync(TariffSlabDTO slabDto)
        {
            var existingSlab = await _context.TariffSlabs.FindAsync(slabDto.TariffSlabId);
            if (existingSlab == null)
                return null;

            existingSlab.FromKwh = slabDto.FromKwh;
            existingSlab.ToKwh = slabDto.ToKwh;
            existingSlab.RatePerKwh = slabDto.RatePerKwh;

            _context.Entry(existingSlab).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return slabDto;
        }

        // 🔹 Delete a tariff slab by ID
        public async Task<bool> DeleteSlabAsync(int slabId)
        {
            var slab = await _context.TariffSlabs.FindAsync(slabId);
            if (slab == null)
                return false;

            _context.TariffSlabs.Remove(slab);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
