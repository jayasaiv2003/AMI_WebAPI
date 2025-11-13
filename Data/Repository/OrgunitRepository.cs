
using AMI_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Data.Repository
{
    public class OrgunitRepository : IOrgUnitRepository
    {
        private readonly AmidbContext _context;

        public OrgunitRepository(AmidbContext context)
        {
            _context = context;
        }

        // 🔹 Add new OrgUnit
        public async Task AddAsync(OrgUnit orgUnit)
        {
            await _context.OrgUnits.AddAsync(orgUnit);
            await _context.SaveChangesAsync();
        }

        // 🔹 Delete OrgUnit
        public async Task DeleteAsync(int id)
        {
            var unit = await _context.OrgUnits.FindAsync(id);
            if (unit != null)
            {
                _context.OrgUnits.Remove(unit);
                await _context.SaveChangesAsync();
            }
        }

        // 🔹 Get all OrgUnits
        public async Task<IEnumerable<OrgUnit>> GetAllOrgunitAsync()
        {
            return await _context.OrgUnits.AsNoTracking().ToListAsync();
        }

        // 🔹 Get by ID
        public async Task<OrgUnit?> GetunitByIdAsync(int id)
        {
            return await _context.OrgUnits.AsNoTracking().FirstOrDefaultAsync(o => o.OrgUnitId == id);
        }

        // 🔹 Get Zones
        public async Task<IEnumerable<OrgUnit>> GetZonesAsync()
        {
            return await _context.OrgUnits
                .Where(o => o.Type == "Zone")
                .AsNoTracking()
                .ToListAsync();
        }

        // 🔹 Get Substations by Zone
        public async Task<IEnumerable<OrgUnit>> GetSubstationsByZoneAsync(int zoneId)
        {
            return await _context.OrgUnits
                .Where(o => o.Type == "Substation" && o.ParentId == zoneId)
                .AsNoTracking()
                .ToListAsync();
        }

        // 🔹 Get Feeders by Substation
        public async Task<IEnumerable<OrgUnit>> GetFeedersBySubstationAsync(int substationId)
        {
            return await _context.OrgUnits
                .Where(o => o.Type == "Feeder" && o.ParentId == substationId)
                .AsNoTracking()
                .ToListAsync();
        }

        // 🔹 Get DTRs by Feeder
        public async Task<IEnumerable<OrgUnit>> GetDtrsByFeederAsync(int feederId)
        {
            return await _context.OrgUnits
                .Where(o => o.Type == "DTR" && o.ParentId == feederId)
                .AsNoTracking()
                .ToListAsync();
        }

        // 🔹 Get all DTRs (for direct selection)
        public async Task<IEnumerable<OrgUnit>> GetAllDtrsAsync()
        {
            return await _context.OrgUnits
                .Where(o => o.Type == "DTR")
                .AsNoTracking()
                .ToListAsync();
        }

        // 🔹 Update OrgUnit
        public async Task UpdateAsync(OrgUnit orgUnit)
        {
            _context.OrgUnits.Update(orgUnit);
            await _context.SaveChangesAsync();
        }

        // 🔹 Get Full Hierarchy Path (Zone → Substation → Feeder → DTR)
        public async Task<string> GetFullHierarchyPathAsync(int orgUnitId)
        {
            var path = new List<string>();
            var current = await _context.OrgUnits.FindAsync(orgUnitId);

            while (current != null)
            {
                path.Insert(0, current.Name); // prepend
                if (current.ParentId == null) break;
                current = await _context.OrgUnits.FindAsync(current.ParentId);
            }

            return string.Join(" → ", path);
        }
    }
}