using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Tariff_App : ControllerBase
    {
        private readonly ITariffRepository _tariffRepository;

        public Tariff_App(ITariffRepository tariffRepository)
        {
            _tariffRepository = tariffRepository;
        }

        // 🔹 1. Get all tariffs
        [HttpGet("get-tariffs")]
        public async Task<IActionResult> GetAllTariffs()
        {
            var tariffs = await _tariffRepository.GetAllTariffsAsync();
            return Ok(tariffs);
        }

        // 🔹 2. Get tariff by ID
        [HttpGet("get-tariff-by-id/{id}")]
        public async Task<IActionResult> GetTariffById(int id)
        {
            var tariff = await _tariffRepository.GetTariffByIdAsync(id);
            if (tariff == null)
                return NotFound(new { message = "Tariff not found." });

            return Ok(tariff);
        }

        // 🔹 3. Add a new tariff
        //[HttpPost("add-tariff")]
        [HttpPost("add-tariff")]
        public async Task<IActionResult> AddTariff([FromBody] TariffDTO model)
        {
            if (model == null)
                return BadRequest("Invalid data.");

            // Check date overlap
            var overlap = await _tariffRepository.TariffDateOverlapAsync(
    model.Name,
    model.EffectiveFrom ,  // in case you allow nullable start date
    model.EffectiveTo ?? DateOnly.MaxValue      // null means "open ended"
);


            if (overlap)
                return BadRequest("Tariff dates overlap with an existing tariff.");

            var tariff = new Tariff
            {
                Name = model.Name,
                EffectiveFrom = model.EffectiveFrom,
                EffectiveTo = model.EffectiveTo,
                BaseRate = model.BaseRate,
                TaxRate = model.TaxRate
            };

            await _tariffRepository.AddTariffAsync(tariff);

            return Ok(new { message = "Tariff added successfully!" });
        }



        // 🔹 4. Update existing tariff
        [HttpPut("update-tariff/{id}")]
        public async Task<IActionResult> UpdateTariff(int id, [FromBody] TariffDTO model)
        {
            if (model == null || model.TariffId != id)
                return BadRequest("Invalid data.");

            var existingTariff = await _tariffRepository.GetTariffByIdAsync(id);
            if (existingTariff == null)
                return NotFound("Tariff not found.");

            existingTariff.Name = model.Name;
            existingTariff.EffectiveFrom = model.EffectiveFrom;
            existingTariff.EffectiveTo = model.EffectiveTo;
            existingTariff.BaseRate = model.BaseRate;
            existingTariff.TaxRate = model.TaxRate;

            await _tariffRepository.UpdateTariffAsync(existingTariff);
            return Ok(new { message = "Tariff updated successfully!" });
        }

        // 🔹 5. Delete a tariff
        [HttpDelete("delete-tariff/{id}")]
        public async Task<IActionResult> DeleteTariff(int id)
        {
            await _tariffRepository.DeleteTariffAsync(id);
            return Ok(new { message = "Tariff deleted successfully!" });
        }
    }
}
