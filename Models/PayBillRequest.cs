namespace AMI_WebAPI.Models
{
    public class PayBillRequest
    {
        public string? Month { get; set; } // e.g. "2025-10"
        public long BillId { get; set; }   // Optional alternative
    }
}
