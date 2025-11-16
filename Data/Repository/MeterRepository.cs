using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Data.Repository
{
    public class MeterRepository : IMeterRepository
    {
        private readonly AmidbContext _context;

        public MeterRepository(AmidbContext context)
        {
            _context = context;
        }

        // 🔹 Get all meters
        public async Task<IEnumerable<MeterDTO>> GetAllMetersAsync()
        {
            return await _context.Meters
                .Select(m => new MeterDTO
                {
                    MeterSerialNo = m.MeterSerialNo,
                    IpAddress = m.IpAddress,
                    Iccid = m.Iccid,
                    Imsi = m.Imsi,
                    Manufacturer = m.Manufacturer,
                    Firmware = m.Firmware,
                    Category = m.Category,
                    InstallTsUtc = m.InstallTsUtc,
                    Status = m.Status,
                    ConsumerId = m.ConsumerId,
                    OrgUnitId = m.OrgUnitId,
                    TariffId = m.TariffId
                })
                .ToListAsync();
        }

        // 🔹 Get meter by serial number
        public async Task<MeterDTO?> GetMeterBySerialAsync(string meterSerialNo)
        {
            var meter = await _context.Meters
                .FirstOrDefaultAsync(m => m.MeterSerialNo == meterSerialNo);

            if (meter == null) return null;

            return new MeterDTO
            {
                MeterSerialNo = meter.MeterSerialNo,
                IpAddress = meter.IpAddress,
                Iccid = meter.Iccid,
                Imsi = meter.Imsi,
                Manufacturer = meter.Manufacturer,
                Firmware = meter.Firmware,
                Category = meter.Category,
                InstallTsUtc = meter.InstallTsUtc,
                Status = meter.Status,
                ConsumerId = meter.ConsumerId,
                OrgUnitId = meter.OrgUnitId,
                TariffId = meter.TariffId
            };
        }

        // 🔹 Get meters by Consumer ID
        public async Task<IEnumerable<MeterDTO>> GetMetersByConsumerIdAsync(long consumerId)
        {
            return await _context.Meters
                .Where(m => m.ConsumerId == consumerId)
                .Select(m => new MeterDTO
                {
                    MeterSerialNo = m.MeterSerialNo,
                    IpAddress = m.IpAddress,
                    Iccid = m.Iccid,
                    Imsi = m.Imsi,
                    Manufacturer = m.Manufacturer,
                    Firmware = m.Firmware,
                    Category = m.Category,
                    InstallTsUtc = m.InstallTsUtc,
                    Status = m.Status,
                    ConsumerId = m.ConsumerId,
                    OrgUnitId = m.OrgUnitId,
                    TariffId = m.TariffId
                })
                .ToListAsync();
        }

        // 🔹 Get meters by OrgUnit (Zone/Substation/Feeder/DTR)
        public async Task<IEnumerable<MeterDTO>> GetMetersByOrgUnitIdAsync(int orgUnitId)
        {
            return await _context.Meters
                .Where(m => m.OrgUnitId == orgUnitId)
                .Select(m => new MeterDTO
                {
                    MeterSerialNo = m.MeterSerialNo,
                    IpAddress = m.IpAddress,
                    Iccid = m.Iccid,
                    Imsi = m.Imsi,
                    Manufacturer = m.Manufacturer,
                    Firmware = m.Firmware,
                    Category = m.Category,
                    InstallTsUtc = m.InstallTsUtc,
                    Status = m.Status,
                    ConsumerId = m.ConsumerId,
                    OrgUnitId = m.OrgUnitId,
                    TariffId = m.TariffId
                })
                .ToListAsync();
        }

        // 🔹 Add new meter
        public async Task<MeterDTO> AddMeterAsync(MeterDTO dto)
        {
            var entity = new Meter
            {
                MeterSerialNo = dto.MeterSerialNo,
                IpAddress = dto.IpAddress,
                Iccid = dto.Iccid,
                Imsi = dto.Imsi,
                Manufacturer = dto.Manufacturer,
                Firmware = dto.Firmware,
                Category = dto.Category,
                InstallTsUtc = dto.InstallTsUtc,
                Status = dto.Status,
                ConsumerId = dto.ConsumerId,
                OrgUnitId = dto.OrgUnitId,
                TariffId = dto.TariffId
            };

            _context.Meters.Add(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        // 🔹 Update existing meter
        public async Task<MeterDTO?> UpdateMeterAsync(MeterDTO dto)
        {
            var meter = await _context.Meters
                .Include(m => m.Consumer) // ✅ Load consumer for status check
                .FirstOrDefaultAsync(m => m.MeterSerialNo == dto.MeterSerialNo);

            if (meter == null) return null;

            // 🔹 Update meter fields
            meter.IpAddress = dto.IpAddress;
            meter.Iccid = dto.Iccid;
            meter.Imsi = dto.Imsi;
            meter.Manufacturer = dto.Manufacturer;
            meter.Firmware = dto.Firmware;
            meter.Category = dto.Category;
            meter.InstallTsUtc = dto.InstallTsUtc;
            meter.Status = dto.Status;
            meter.ConsumerId = dto.ConsumerId;
            meter.OrgUnitId = dto.OrgUnitId;
            meter.TariffId = dto.TariffId;

            // ✅ If meter is reactivated but consumer is inactive → reactivate consumer
            if (dto.Status == "Active" && meter.Consumer != null && meter.Consumer.Status == "Inactive")
            {
                meter.Consumer.Status = "Active";
                meter.Consumer.UpdatedAt = DateTime.UtcNow;
                //meter.Consumer.UpdatedBy = dto.UpdatedBy; // optional, if DTO includes this
            }

            await _context.SaveChangesAsync();

            return dto;
        }


        // 🔹 Delete meter
        //public async Task<bool> DeleteMeterAsync(string meterSerialNo)
        //{
        //    var meter = await _context.Meters.FindAsync(meterSerialNo);
        //    if (meter == null) return false;

        //    meter.Status = "Inactive"; // Soft delete
        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        public async Task<bool> DeleteMeterAsync(string meterSerialNo)
        {
            var meter = await _context.Meters
                .Include(m => m.DailyReadings)
                .Include(m => m.MonthlyBills)
                .FirstOrDefaultAsync(m => m.MeterSerialNo == meterSerialNo);

            if (meter == null) return false;

            _context.DailyReadings.RemoveRange(meter.DailyReadings);
            _context.MonthlyBills.RemoveRange(meter.MonthlyBills);

            _context.Meters.Remove(meter);

            await _context.SaveChangesAsync();
            return true;
        }



        // 🔹 Update meter status
        public async Task<bool> UpdateMeterStatusAsync(string meterSerialNo, string newStatus)
        {
            var entity = await _context.Meters.FindAsync(meterSerialNo);
            if (entity == null) return false;

            entity.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
