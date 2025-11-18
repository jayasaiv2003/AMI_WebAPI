namespace AMI_WebAPI.Models.DTOs
{
    public class DailyReadingDto
    {
        public DateOnly Date { get; set; }
        public decimal Units { get; set; }
    }

    public class MonthlyTotalDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalUnits { get; set; }
    }

    //public class ConsumerReadingResponse
    //{
    //    public List<DailyReadingDto> DailyReadings { get; set; }
    //    public List<MonthlyTotalDto> MonthlyTotals { get; set; }
    //}
}
