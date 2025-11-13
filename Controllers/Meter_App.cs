using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMI_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Meter_App : ControllerBase
    {
        private readonly IMeterRepository _meterRepository;

        public Meter_App(IMeterRepository meterRepository)
        {
            _meterRepository = meterRepository;
        }

        // 🔹 GET: api/Meter_App
        [HttpGet]
        public async Task<IActionResult> GetAllMeters()
        {
            var meters = await _meterRepository.GetAllMetersAsync();
            return Ok(meters);
        }

        // 🔹 GET: api/Meter_App/serial/{meterSerialNo}
        [HttpGet("serial/{meterSerialNo}")]
        public async Task<IActionResult> GetMeterBySerial(string meterSerialNo)
        {
            var meter = await _meterRepository.GetMeterBySerialAsync(meterSerialNo);
            if (meter == null)
                return NotFound(new { Message = "Meter not found." });

            return Ok(meter);
        }

        // 🔹 GET: api/Meter_App/byConsumer/{consumerId}
        [HttpGet("byConsumer/{consumerId:long}")]
        public async Task<IActionResult> GetMetersByConsumer(long consumerId)
        {
            var meters = await _meterRepository.GetMetersByConsumerIdAsync(consumerId);
            return Ok(meters);
        }

        // 🔹 GET: api/Meter_App/byOrgUnit/{orgUnitId}
        [HttpGet("byOrgUnit/{orgUnitId:int}")]
        public async Task<IActionResult> GetMetersByOrgUnit(int orgUnitId)
        {
            var meters = await _meterRepository.GetMetersByOrgUnitIdAsync(orgUnitId);
            return Ok(meters);
        }

        // 🔹 POST: api/Meter_App
        [HttpPost]
        public async Task<IActionResult> AddMeter([FromBody] MeterDTO meter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedMeter = await _meterRepository.AddMeterAsync(meter);
            return CreatedAtAction(nameof(GetMeterBySerial),
                new { meterSerialNo = addedMeter.MeterSerialNo },
                addedMeter);
        }

        // 🔹 PUT: api/Meter_App/{meterSerialNo}
        [HttpPut("{meterSerialNo}")]
        public async Task<IActionResult> UpdateMeter(string meterSerialNo, [FromBody] MeterDTO meter)
        {
            if (meterSerialNo != meter.MeterSerialNo)
                return BadRequest("Meter serial number mismatch.");

            var updatedMeter = await _meterRepository.UpdateMeterAsync(meter);
            if (updatedMeter == null)
                return NotFound(new { Message = "Meter not found." });

            return Ok(updatedMeter);
        }

        // 🔹 PATCH: api/Meter_App/status/{meterSerialNo}
        [HttpPatch("status/{meterSerialNo}")]
        public async Task<IActionResult> UpdateMeterStatus(string meterSerialNo, [FromQuery] string newStatus)
        {
            var result = await _meterRepository.UpdateMeterStatusAsync(meterSerialNo, newStatus);
            if (!result)
                return NotFound(new { Message = "Meter not found or status unchanged." });

            return Ok(new { Message = "Meter status updated successfully." });
        }

        // 🔹 DELETE: api/Meter_App/{meterSerialNo}
        [HttpDelete("{meterSerialNo}")]
        public async Task<IActionResult> DeleteMeter(string meterSerialNo)
        {
            var deleted = await _meterRepository.DeleteMeterAsync(meterSerialNo);
            if (!deleted)
                return NotFound(new { Message = "Meter not found." });

            return Ok(new { Message = "Meter deleted successfully." });
        }
    }
}
