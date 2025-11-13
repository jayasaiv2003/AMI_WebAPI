using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly IBillingRepository _billingRepository;

        public BillingController(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }
        

        // ✅ Generate bill for one meter
        [HttpPost("generate/{meterSerialNo}")]
        public async Task<IActionResult> GenerateBill(string meterSerialNo, [FromQuery] DateOnly billingMonth)
        {
            try
            {
                var success = await _billingRepository.GenerateMonthlyBillAsync(meterSerialNo, billingMonth);
                if (!success)
                    return BadRequest($"Bill for {meterSerialNo} already exists for {billingMonth:yyyy-MM}.");

                return Ok($"Bill generated for meter {meterSerialNo} for {billingMonth:yyyy-MM}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ Generate bills for all active meters
        [HttpPost("generate-all")]
        public async Task<IActionResult> GenerateForAll([FromQuery] DateOnly? billingMonth = null)
        {
            try
            {
                int count;

                if (billingMonth.HasValue)
                {
                    // Case 1: Specific month given
                    count = await _billingRepository.GenerateBillsForAllActiveMetersAsync(billingMonth.Value);
                    return Ok($"{count} bills generated for {billingMonth:yyyy-MM}.");
                }
                else
                {
                    // Case 2: No month given → generate for all available months
                    count = await _billingRepository.GenerateBillsForAllAvailableMonthsAsync();
                    return Ok($"{count} bills generated for all available months.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        // ✅ Get all bills for a consumer
        [HttpGet("consumer/{consumerId}")]
        public async Task<IActionResult> GetConsumerBills(long consumerId)
        {
            try
            {
                var bills = await _billingRepository.GetBillsByConsumerAsync(consumerId);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ Get all bills for a meter
        [HttpGet("meter/{meterSerialNo}")]
        public async Task<IActionResult> GetMeterBills(string meterSerialNo)
        {
            try
            {
                var bills = await _billingRepository.GetBillsByMeterAsync(meterSerialNo);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ Get single bill details
        [HttpGet("{billId}")]
        public async Task<IActionResult> GetBill(long billId)
        {
            try
            {
                var bill = await _billingRepository.GetBillDetailsAsync(billId);
                if (bill == null)
                    return NotFound($"Bill with ID {billId} not found.");

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        
    }
}
