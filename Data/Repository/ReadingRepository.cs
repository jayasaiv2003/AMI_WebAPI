using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Data.Repository
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly AmidbContext _context;

        public ReadingRepository(AmidbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DailyReading>> GetDailyReadingsAsync(string meterSerialNo)
        {
            return await _context.DailyReadings
                .Where(r => r.MeterSerialNo == meterSerialNo)
                .OrderBy(r => r.ReadingDate)
                .ToListAsync();
        }


        public async Task<IEnumerable<MonthlyTotalDto>> GetMonthlyTotalsAsync(string meterSerialNo)
        {
            return await _context.DailyReadings
                .Where(r => r.MeterSerialNo == meterSerialNo)
                .GroupBy(r => new { r.ReadingDate.Year, r.ReadingDate.Month })
                .Select(g => new MonthlyTotalDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalUnits = g.Sum(x => x.ReadingKwh)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
        }

    }
}
