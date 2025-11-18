using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;

namespace AMI_WebAPI.Data.Repository
{
    public interface IReadingRepository
    {
        Task<IEnumerable<DailyReading>> GetDailyReadingsAsync(string meterSerialNo);
        Task<IEnumerable<MonthlyTotalDto>> GetMonthlyTotalsAsync(string meterSerialNo);
    }
}
