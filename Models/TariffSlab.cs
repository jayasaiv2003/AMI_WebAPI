using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AMI_WebAPI.Models;

public partial class TariffSlab
{
    public int TariffSlabId { get; set; }

    public int TariffId { get; set; }

    public decimal FromKwh { get; set; }

    public decimal ToKwh { get; set; }

    public decimal RatePerKwh { get; set; }

    [JsonIgnore]

    public virtual Tariff Tariff { get; set; } = null!;
}
