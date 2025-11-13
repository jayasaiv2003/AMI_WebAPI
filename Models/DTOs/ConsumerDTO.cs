using System.Text.Json.Serialization;

namespace AMI_WebAPI.Models.DTOs
{
    public class ConsumerDTO
    {
        public long ConsumerId { get; set; }

        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string Status { get; set; } = null!;

        
        public DateTime CreatedAt { get; set; }

        
        public string CreatedBy { get; set; } 

        
        public DateTime? UpdatedAt { get; set; }

        
        public string? UpdatedBy { get; set; }

        //public virtual User CreatedByNavigation { get; set; } = null!;

        //public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();

        //public virtual User? UpdatedByNavigation { get; set; }
    }
}
