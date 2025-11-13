using System.ComponentModel.DataAnnotations.Schema;

namespace AMI_WebAPI.Models.DTOs
{
    public class TariffSlabDTO
    {
        public int TariffSlabId { get; set; }

        [ForeignKey("TariffId")]
        public int TariffId { get; set; }

        public decimal FromKwh { get; set; }

        public decimal ToKwh { get; set; }

        public decimal RatePerKwh { get; set; }

        //public virtual Tariff Tariff { get; set; } = null!;
    }
}
