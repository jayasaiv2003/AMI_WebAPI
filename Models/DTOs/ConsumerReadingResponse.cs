namespace AMI_WebAPI.Models.DTOs
{
    public class ConsumerReadingResponse
    {
        public long ConsumerId { get; set; }
        public string ConsumerName { get; set; } = string.Empty;

        public List<MeterReadingResponse> Meters { get; set; } = new();
    }
}
