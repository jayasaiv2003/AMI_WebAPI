using AMI_WebAPI.Models;
using AMI_WebAPI.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AMI_WebAPI.Controllers
{
    [Route("api/orgunits")]
    [ApiController]
    public class OrgUnitController : ControllerBase
    {
        private readonly IOrgUnitRepository _orgunitRepository;

        public OrgUnitController(IOrgUnitRepository orgunitRepository)
        {
            _orgunitRepository = orgunitRepository;
        }

        // 🔹 Get all Org Units
        [HttpGet]
        public async Task<IActionResult> GetAllOrgUnits()
        {
            var units = await _orgunitRepository.GetAllOrgunitAsync();
            return Ok(units);
        }

        // 🔹 Get all Zones
        [HttpGet("zones")]
        public async Task<IActionResult> GetZones()
        {
            var zones = await _orgunitRepository.GetZonesAsync();
            return Ok(zones);
        }

        // 🔹 Get Substations under a Zone
        [HttpGet("zones/{zoneId}/substations")]
        public async Task<IActionResult> GetSubstationsByZone(int zoneId)
        {
            var substations = await _orgunitRepository.GetSubstationsByZoneAsync(zoneId);
            return Ok(substations);
        }

        // 🔹 Get Feeders under a Substation
        [HttpGet("substations/{substationId}/feeders")]
        public async Task<IActionResult> GetFeedersBySubstation(int substationId)
        {
            var feeders = await _orgunitRepository.GetFeedersBySubstationAsync(substationId);
            return Ok(feeders);
        }

        // 🔹 Get DTRs under a Feeder
        [HttpGet("feeders/{feederId}/dtrs")]
        public async Task<IActionResult> GetDtrsByFeeder(int feederId)
        {
            var dtrs = await _orgunitRepository.GetDtrsByFeederAsync(feederId);
            return Ok(dtrs);
        }

        // 🔹 Get all DTRs
        [HttpGet("dtrs")]
        public async Task<IActionResult> GetAllDtrs()
        {
            var dtrs = await _orgunitRepository.GetAllDtrsAsync();
            return Ok(dtrs);
        }

        // 🔹 Get hierarchy path (Zone → Substation → Feeder → DTR)
        [HttpGet("{orgUnitId}/path")]
        public async Task<IActionResult> GetHierarchyPath(int orgUnitId)
        {
            var path = await _orgunitRepository.GetFullHierarchyPathAsync(orgUnitId);
            return Ok(new { hierarchy = path });
        }

        // 🔹 Add a new Org Unit
        [HttpPost]
        public async Task<IActionResult> AddOrgUnit([FromBody] OrgUnitDTO model)
        {
            if (model == null)
                return BadRequest("Invalid data.");

            var newOrgUnit = new OrgUnit
            {
                Type = model.Type,
                Name = model.Name,
                ParentId = model.ParentId,
            };

            await _orgunitRepository.AddAsync(newOrgUnit);
            return Ok(new { message = "Org Unit added successfully!" });
        }

        // 🔹 Update an Org Unit
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrgUnit(int id, [FromBody] OrgUnitDTO model)
        {
            if (model == null || model.OrgUnitId != id)
                return BadRequest("Invalid data.");

            var updatedOrgUnit = new OrgUnit
            {
                OrgUnitId = id,
                Type = model.Type,
                Name = model.Name,
                ParentId = model.ParentId
            };

            await _orgunitRepository.UpdateAsync(updatedOrgUnit);
            return Ok(new { message = "Org Unit updated successfully!" });
        }

        // 🔹 Delete an Org Unit
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrgUnit(int id)
        {
            await _orgunitRepository.DeleteAsync(id);
            return Ok(new { message = "Org Unit deleted successfully!" });
        }
    }
}
