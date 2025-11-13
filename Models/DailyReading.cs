using System;
using System.Collections.Generic;

namespace AMI_WebAPI.Models;

public partial class DailyReading
{
    public long ReadingId { get; set; }

    public string MeterSerialNo { get; set; } = null!;

    public decimal ReadingKwh { get; set; }

    public DateOnly ReadingDate { get; set; }

    public virtual Meter MeterSerialNoNavigation { get; set; } = null!;
}
