using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml; // Install-Package EPPlus
using Microsoft.Data.SqlClient; // ✅ use Microsoft.Data.SqlClient (modern)
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AMI_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterUploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MeterUploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("upload")]
        
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            ExcelPackage.License.SetNonCommercialPersonal("Jay");


            var connectionString = _configuration.GetConnectionString("ConnectionDB"); // ✅ match your appsettings key
            var readings = new List<(string MeterSerialNo, DateTime ReadingDate, decimal ReadingKwh)>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Skip header row
                    {
                        var meter = worksheet.Cells[row, 1].Text;
                        var date = DateTime.Parse(worksheet.Cells[row, 2].Text);
                        var kwh = decimal.Parse(worksheet.Cells[row, 3].Text);

                        readings.Add((meter, date, kwh));
                    }
                }
            }

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                foreach (var r in readings)
                {
                    var cmd = new SqlCommand(
                        "INSERT INTO ami.DailyReading (MeterSerialNo, ReadingDate, ReadingKwh) VALUES (@MeterSerialNo, @ReadingDate, @ReadingKwh)",
                        conn);

                    cmd.Parameters.AddWithValue("@MeterSerialNo", r.MeterSerialNo);
                    cmd.Parameters.AddWithValue("@ReadingDate", r.ReadingDate);
                    cmd.Parameters.AddWithValue("@ReadingKwh", r.ReadingKwh);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { message = $"{readings.Count} records uploaded successfully." });
        }
    }
}
