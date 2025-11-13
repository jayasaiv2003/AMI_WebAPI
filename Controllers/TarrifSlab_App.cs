using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TariffSlab_App : ControllerBase
    {
        private readonly ITariffSlabRepository _tariffSlabRepository;

        public TariffSlab_App(ITariffSlabRepository tariffSlabRepository)
        {
            _tariffSlabRepository = tariffSlabRepository;
        }

        // 🔹 Get all slabs for a specific tariff
        [HttpGet("tariff/{tariffId}")]
        public async Task<IActionResult> GetSlabsByTariffId(int tariffId)
        {
            var slabs = await _tariffSlabRepository.GetSlabsByTariffIdAsync(tariffId);
            return Ok(slabs);
        }

        // 🔹 Get specific slab by ID
        [HttpGet("{slabId}")]
        public async Task<IActionResult> GetSlabById(int slabId)
        {
            var slab = await _tariffSlabRepository.GetSlabByIdAsync(slabId);
            if (slab == null)
                return NotFound(new { message = "Tariff slab not found." });

            return Ok(slab);
        }

        // 🔹 Add new tariff slab
        [HttpPost("add")]
        public async Task<IActionResult> AddSlab([FromBody] TariffSlabDTO slabDto)
        {
            if (slabDto == null)
                return BadRequest(new { message = "Invalid slab data." });

            var addedSlab = await _tariffSlabRepository.AddSlabAsync(slabDto);
            return Ok(new { message = "Tariff slab added successfully!", data = addedSlab });
        }

        // 🔹 Update existing tariff slab
        [HttpPut("update/{slabId}")]
        public async Task<IActionResult> UpdateSlab(int slabId, [FromBody] TariffSlabDTO slabDto)
        {
            if (slabDto == null || slabId != slabDto.TariffSlabId)
                return BadRequest(new { message = "Invalid slab update request." });

            var updated = await _tariffSlabRepository.UpdateSlabAsync(slabDto);
            if (updated == null)
                return NotFound(new { message = "Tariff slab not found for update." });

            return Ok(new { message = "Tariff slab updated successfully!", data = updated });
        }

        // 🔹 Delete slab by ID
        [HttpDelete("delete/{slabId}")]
        public async Task<IActionResult> DeleteSlab(int slabId)
        {
            var result = await _tariffSlabRepository.DeleteSlabAsync(slabId);
            if (!result)
                return NotFound(new { message = "Tariff slab not found for deletion." });

            return Ok(new { message = "Tariff slab deleted successfully!" });
        }
    }
}
