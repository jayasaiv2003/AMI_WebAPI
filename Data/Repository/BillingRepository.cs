using AMI_WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMI_WebAPI.Data.Repository
{
    public class BillingRepository : IBillingRepository
    {
        private readonly AmidbContext _context;

        public BillingRepository(AmidbContext context)
        {
            _context = context;
        }

        // ✅ Generate bill for one meter safely
        public async Task<bool> GenerateMonthlyBillAsync(string meterSerialNo, DateOnly billingMonth)
        {
            var start = new DateOnly(billingMonth.Year, billingMonth.Month, 1);
            var end = start.AddMonths(1);

            // ✅ Check if a bill already exists for this meter and month
            bool exists = await _context.MonthlyBills
                .AnyAsync(b => b.MeterSerialNo == meterSerialNo && b.BillingMonth == start);
            if (exists)
            {
                Console.WriteLine($"ℹ️ Bill already exists for {meterSerialNo} ({start:yyyy-MM}). Skipping.");
                return false;
            }

            // ✅ Fetch meter with tariff and slabs
            var meter = await _context.Meters
                .Include(m => m.Tariff)
                    .ThenInclude(t => t.TariffSlabs)
                .FirstOrDefaultAsync(m => m.MeterSerialNo == meterSerialNo);

            if (meter == null)
            {
                Console.WriteLine($"⚠️ Meter not found: {meterSerialNo}");
                return false;
            }

            if (meter.Tariff == null)
            {
                Console.WriteLine($"⚠️ No tariff assigned to meter {meterSerialNo}");
                return false;
            }

            // ✅ Sum monthly readings
            var totalKwh = await _context.DailyReadings
                .Where(r => r.MeterSerialNo == meterSerialNo &&
                            r.ReadingDate >= start &&
                            r.ReadingDate < end)
                .SumAsync(r => (decimal?)r.ReadingKwh) ?? 0m;

            if (totalKwh <= 0)
            {
                Console.WriteLine($"⚠️ No readings found for {meterSerialNo} ({start:yyyy-MM}). Skipping.");
                return false;
            }

            var tariff = meter.Tariff;
            decimal baseRate = tariff.BaseRate;
            decimal slabCharge = 0m;

            var slabs = tariff.TariffSlabs.OrderBy(s => s.FromKwh).ToList();

            // ✅ Apply slab rates properly
            foreach (var slab in slabs)
            {
                if (totalKwh > slab.FromKwh)
                {
                    decimal upperLimit = Math.Min(totalKwh, slab.ToKwh);
                    decimal applicableUnits = upperLimit - slab.FromKwh;

                    if (applicableUnits > 0)
                        slabCharge += applicableUnits * slab.RatePerKwh;
                }
            }

            // ✅ Handle extra units beyond last slab
            var lastSlab = slabs.LastOrDefault();
            if (lastSlab != null && totalKwh > lastSlab.ToKwh)
            {
                decimal extraUnits = totalKwh - lastSlab.ToKwh;
                slabCharge += extraUnits * lastSlab.RatePerKwh;
            }

            // ✅ Final total calculation
            decimal totalBeforeTax = baseRate + slabCharge;
            decimal taxAmount = totalBeforeTax * tariff.TaxRate;
            decimal totalAmount = totalBeforeTax + taxAmount;

            // ✅ Create and insert monthly bill
            var newBill = new MonthlyBill
            {
                MeterSerialNo = meterSerialNo,
                BillingMonth = start,
                TotalKwh = totalKwh,
                BaseRateApplied = totalBeforeTax,
                SlabCharge = taxAmount,
                TotalBillAmount = totalAmount,
                Status = "Unpaid",
                GeneratedAt = DateTime.UtcNow,
                DueDate = new DateOnly(billingMonth.Year, billingMonth.Month, 15)
            };

            _context.MonthlyBills.Add(newBill);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Bill generated for {meterSerialNo} ({start:yyyy-MM}) — {totalKwh} kWh, ₹{totalAmount:F2}");
            return true;
        }

        // ✅ Get bills by consumer
        public async Task<IEnumerable<MonthlyBill>> GetBillsByConsumerAsync(long consumerId)
        {
            return await _context.MonthlyBills
                .Include(b => b.MeterSerialNoNavigation)
                .Where(b => b.MeterSerialNoNavigation.ConsumerId == consumerId)
                .OrderByDescending(b => b.BillingMonth)
                .ToListAsync();
        }

        // ✅ Get bills by meter
        public async Task<IEnumerable<MonthlyBill>> GetBillsByMeterAsync(string meterSerialNo)
        {
            return await _context.MonthlyBills
                .Where(b => b.MeterSerialNo == meterSerialNo)
                .OrderByDescending(b => b.BillingMonth)
                .ToListAsync();
        }

        // ✅ Generate bills for all months that have readings but not yet billed
        public async Task<int> GenerateBillsForAllAvailableMonthsAsync()
        {
            int totalBillsGenerated = 0;

            // 1️⃣ Find all (meter, month) pairs in DailyReadings
            var meterMonths = await _context.DailyReadings
                .Select(r => new
                {
                    r.MeterSerialNo,
                    Month = new DateOnly(r.ReadingDate.Year, r.ReadingDate.Month, 1)
                })
                .Distinct()
                .ToListAsync();

            // 2️⃣ For each unique (meter, month), check if bill exists
            foreach (var mm in meterMonths)
            {
                bool alreadyBilled = await _context.MonthlyBills
                    .AnyAsync(b => b.MeterSerialNo == mm.MeterSerialNo && b.BillingMonth == mm.Month);

                if (!alreadyBilled)
                {
                    try
                    {
                        bool created = await GenerateMonthlyBillAsync(mm.MeterSerialNo, mm.Month);
                        if (created)
                            totalBillsGenerated++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Skipped {mm.MeterSerialNo} ({mm.Month:yyyy-MM}): {ex.Message}");
                    }
                }
            }

            return totalBillsGenerated;
        }


        // ✅ Get details for a single bill
        public async Task<MonthlyBill?> GetBillDetailsAsync(long billId)
        {
            return await _context.MonthlyBills
                .Include(b => b.MeterSerialNoNavigation)
                    .ThenInclude(m => m.Consumer)
                .FirstOrDefaultAsync(b => b.BillId == billId);
        }

        // ✅ Generate for all active meters safely
        public async Task<int> GenerateBillsForAllActiveMetersAsync(DateOnly billingMonth)
        {
            var activeMeters = await _context.Meters
                .Where(m => m.Status == "Active")
                .Select(m => m.MeterSerialNo)
                .ToListAsync();

            int generatedCount = 0;

            foreach (var serial in activeMeters)
            {
                try
                {
                    bool created = await GenerateMonthlyBillAsync(serial, billingMonth);
                    if (created) generatedCount++;
                }
                catch (Exception ex)
                {
                    // ✅ Don’t crash entire billing cycle — log & skip
                    Console.WriteLine($"⚠️ Skipped {serial}: {ex.Message}");
                }
            }

            Console.WriteLine($"✅ Billing complete — {generatedCount}/{activeMeters.Count} bills created.");
            return generatedCount;
        }

        public async Task UpdateBillAsync(MonthlyBill bill)
        {
            _context.MonthlyBills.Update(bill);
            await _context.SaveChangesAsync();
        }

    }
}
