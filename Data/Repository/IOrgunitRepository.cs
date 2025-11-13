using AMI_WebAPI.Models;

namespace AMI_WebAPI.Data.Repository
{
    public interface IOrgUnitRepository
    {
        // 🔹 Get All Org Units
        Task<IEnumerable<OrgUnit>> GetAllOrgunitAsync();

        // 🔹 Get Org Unit by ID
        Task<OrgUnit?> GetunitByIdAsync(int id);

        // 🔹 Get all Zones
        Task<IEnumerable<OrgUnit>> GetZonesAsync();

        // 🔹 Get Substations under a Zone
        Task<IEnumerable<OrgUnit>> GetSubstationsByZoneAsync(int zoneId);

        // 🔹 Get Feeders under a Substation
        Task<IEnumerable<OrgUnit>> GetFeedersBySubstationAsync(int substationId);

        // 🔹 Get DTRs under a Feeder
        Task<IEnumerable<OrgUnit>> GetDtrsByFeederAsync(int feederId);

        // 🔹 Get DTR directly by ID (used in “Select DTR Directly” dropdown)
        Task<IEnumerable<OrgUnit>> GetAllDtrsAsync();

        // 🔹 Create new Org Unit
        Task AddAsync(OrgUnit orgUnit);

        // 🔹 Update an existing Org Unit
        Task UpdateAsync(OrgUnit orgUnit);

        // 🔹 Delete Org Unit
        Task DeleteAsync(int id);

        // 🔹 Get full hierarchy path (for "Selected Hierarchy" display)
        Task<string> GetFullHierarchyPathAsync(int orgUnitId);
    }
}
