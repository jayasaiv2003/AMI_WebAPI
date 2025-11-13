using AMI_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMI_WebAPI.Data.Repository
{
    public interface IBillingRepository
    {
        Task<bool> GenerateMonthlyBillAsync(string meterSerialNo, DateOnly billingMonth);
        Task<IEnumerable<MonthlyBill>> GetBillsByConsumerAsync(long consumerId);
        Task<IEnumerable<MonthlyBill>> GetBillsByMeterAsync(string meterSerialNo);
        Task<MonthlyBill?> GetBillDetailsAsync(long billId);
        Task<int> GenerateBillsForAllActiveMetersAsync(DateOnly billingMonth);

        Task<int> GenerateBillsForAllAvailableMonthsAsync();

        Task UpdateBillAsync(MonthlyBill bill);

    }
}
