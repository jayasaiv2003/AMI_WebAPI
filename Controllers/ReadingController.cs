using AMI_WebAPI.Data.Repository;
using AMI_WebAPI.Models;
using AMI_WebAPI.Models.DTOs;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingController : ControllerBase
    {
        private readonly IReadingRepository _readingRepo;
        private readonly IConsumerRepository _userRepo;

        public ReadingController(IReadingRepository readingRepo, IConsumerRepository userRepo)
        {
            _readingRepo = readingRepo;
            _userRepo = userRepo;
        }

        // GET: api/Reading/Consumer/{consumerId}
        [HttpGet("Consumer/{consumerId}")]
        public async Task<IActionResult> GetReadingsForConsumer(int consumerId)
        {
            var consumer = await _userRepo.GetConsumerByIdforreadingAsync(consumerId);

            if (consumer == null)
                return NotFound("Consumer not found");

            if (consumer.Meters == null || consumer.Meters.Count == 0)
                return BadRequest("Consumer has no meters assigned");

            var response = new ConsumerReadingResponse
            {
                ConsumerId = consumer.ConsumerId,
                ConsumerName = consumer.Name,
                Meters = new List<MeterReadingResponse>()
            };

            foreach (var meter in consumer.Meters)
            {
                var daily = await _readingRepo.GetDailyReadingsAsync(meter.MeterSerialNo);
                var monthly = await _readingRepo.GetMonthlyTotalsAsync(meter.MeterSerialNo);

                response.Meters.Add(new MeterReadingResponse
                {
                    MeterSerialNo = meter.MeterSerialNo,
                    DailyReadings = daily,
                    MonthlyTotals = monthly
                });
            }

            return Ok(response);
        }
    }

}