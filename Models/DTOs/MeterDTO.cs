using System.ComponentModel.DataAnnotations.Schema;

namespace AMI_WebAPI.Models.DTOs
{
    public class MeterDTO
    {
        public string MeterSerialNo { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public string Iccid { get; set; } = null!;

        public string Imsi { get; set; } = null!;

        public string Manufacturer { get; set; } = null!;

        public string? Firmware { get; set; }

        public string Category { get; set; } = null!;

        public DateTime InstallTsUtc { get; set; }

        public string Status { get; set; } = null!;

        [ForeignKey("ConsumerId")]
        public long? ConsumerId { get; set; }

        [ForeignKey("OrgUnitId")]
        public int OrgUnitId { get; set; }

        [ForeignKey("TariffId")]
        public int TariffId { get; set; }

        //public virtual Consumer? Consumer { get; set; }

        //public virtual ICollection<DailyReading> DailyReadings { get; set; } = new List<DailyReading>();

        //public virtual ICollection<MonthlyBill> MonthlyBills { get; set; } = new List<MonthlyBill>();

        //public virtual OrgUnit OrgUnit { get; set; } = null!;

        //public virtual Tariff Tariff { get; set; } = null!;
    }
}
