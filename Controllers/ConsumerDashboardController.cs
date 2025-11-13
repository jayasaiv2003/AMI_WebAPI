using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Consumer")]
    public class ConsumerDashboardController : ControllerBase
    {
        private readonly IMeterRepository _meterRepo;
        private readonly IBillingRepository _billingRepo;
        private readonly IConsumerRepository _consumerRepo;

        public ConsumerDashboardController(
            IMeterRepository meterRepo,
            IBillingRepository billingRepo,
            IConsumerRepository consumerRepo)
        {
            _meterRepo = meterRepo;
            _billingRepo = billingRepo;
            _consumerRepo = consumerRepo;
        }

        // 🧩 Helper: Get consumerId from JWT token
        private long GetConsumerIdFromToken()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(idClaim, out var consumerId) ? consumerId : 0;
        }

        // ✅ GET: api/ConsumerDashboard/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var consumerId = GetConsumerIdFromToken();
            if (consumerId == 0)
                return Unauthorized("Invalid consumer token.");

            var consumer = await _consumerRepo.GetConsumerByIdAsync(consumerId);
            if (consumer == null)
                return NotFound("Consumer not found.");

            return Ok(consumer);
        }

        // ✅ GET: api/ConsumerDashboard/meters
        [HttpGet("meters")]
        public async Task<IActionResult> GetMyMeters()
        {
            var consumerId = GetConsumerIdFromToken();
            if (consumerId == 0)
                return Unauthorized("Invalid consumer token.");

            var meters = await _meterRepo.GetMetersByConsumerIdAsync(consumerId);
            return Ok(meters);
        }

        // ✅ GET: api/ConsumerDashboard/bills
        [HttpGet("bills")]
        public async Task<IActionResult> GetMyBills()
        {
            var consumerId = GetConsumerIdFromToken();
            if (consumerId == 0)
                return Unauthorized("Invalid consumer token.");

            var bills = await _billingRepo.GetBillsByConsumerAsync(consumerId);
            return Ok(bills);
        }

        // ✅ GET: api/ConsumerDashboard/bills/meter/{meterSerialNo}
        [HttpGet("bills/meter/{meterSerialNo}")]
        public async Task<IActionResult> GetBillsByMeter(string meterSerialNo)
        {
            var consumerId = GetConsumerIdFromToken();
            if (consumerId == 0)
                return Unauthorized("Invalid consumer token.");

            // Optional: Security check — ensure meter belongs to consumer
            var meters = await _meterRepo.GetMetersByConsumerIdAsync(consumerId);
            if (!meters.Any(m => m.MeterSerialNo == meterSerialNo))
                return Forbid("You are not authorized to access this meter’s bills.");

            var bills = await _billingRepo.GetBillsByMeterAsync(meterSerialNo);
            return Ok(bills);
        }

        [HttpPut("bills/pay")]
        public async Task<IActionResult> PayBill([FromBody] PayBillRequest request)
        {
            var consumerId = GetConsumerIdFromToken();
            if (consumerId == 0)
                return Unauthorized("Invalid consumer token.");

            // Get the bill by consumer and month (or billId)
            var bills = await _billingRepo.GetBillsByConsumerAsync(consumerId);
            var bill = bills.FirstOrDefault(b =>
                (request.BillId > 0 && b.BillId == request.BillId) ||
                (!string.IsNullOrEmpty(request.Month) &&
                 b.BillingMonth.ToString("yyyy-MM") == request.Month));

            if (bill == null)
                return NotFound("Bill not found for the given month or ID.");

            if (bill.Status == "Paid")
                return BadRequest("Bill is already marked as Paid.");

            // ✅ Update the bill status
            bill.Status = "Paid";
            //bill.PaidDate = DateTime.UtcNow; // optional: store payment timestamp

            await _billingRepo.UpdateBillAsync(bill);

            return Ok(new
            {
                Message = $"Bill for {bill.BillingMonth:yyyy-MM} marked as Paid successfully.",
                BillId = bill.BillId
            });
        }
    }
}
