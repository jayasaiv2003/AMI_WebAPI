namespace AMI_WebAPI.Models.DTOs
{
    public class MeterReadingResponse
    {
        public string MeterSerialNo { get; set; } = string.Empty;

        public IEnumerable<DailyReading> DailyReadings { get; set; } = new List<DailyReading>();

        public IEnumerable<MonthlyTotalDto> MonthlyTotals { get; set; } = new List<MonthlyTotalDto>();
    }
}
