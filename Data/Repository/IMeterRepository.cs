using AMI_WebAPI.Models.DTOs;

namespace AMI_WebAPI.Data.Repository
{
    public interface IMeterRepository
    {
        Task<IEnumerable<MeterDTO>> GetAllMetersAsync();

        // 🔹 Get meter by serial number
        Task<MeterDTO?> GetMeterBySerialAsync(string meterSerialNo);

        // 🔹 Get meters by consumer ID
        Task<IEnumerable<MeterDTO>> GetMetersByConsumerIdAsync(long consumerId);

        // 🔹 Get meters by OrgUnit (Zone/Substation/Feeder/DTR)
        Task<IEnumerable<MeterDTO>> GetMetersByOrgUnitIdAsync(int orgUnitId);

        // 🔹 Add new meter
        Task<MeterDTO> AddMeterAsync(MeterDTO meter);

        // 🔹 Update existing meter
        Task<MeterDTO?> UpdateMeterAsync(MeterDTO meter);

        // 🔹 Delete meter
        Task<bool> DeleteMeterAsync(string meterSerialNo);

        // 🔹 Change meter status (e.g. Active → Inactive)
        Task<bool> UpdateMeterStatusAsync(string meterSerialNo, string newStatus);
    }
}
