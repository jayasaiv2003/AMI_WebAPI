using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AMI_WebAPI.Models;

public partial class MonthlyBill
{
    public long BillId { get; set; }

    public string MeterSerialNo { get; set; } = null!;

    public DateOnly BillingMonth { get; set; }

    public decimal TotalKwh { get; set; }

    public decimal BaseRateApplied { get; set; }

    public decimal SlabCharge { get; set; }

    public decimal TotalBillAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }

    public DateOnly DueDate { get; set; }

    [JsonIgnore]
    public virtual Meter MeterSerialNoNavigation { get; set; } = null!;
}
