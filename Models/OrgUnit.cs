using System;
using System.Collections.Generic;

namespace AMI_WebAPI.Models;

public partial class OrgUnit
{
    public int OrgUnitId { get; set; }

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    public virtual ICollection<OrgUnit> InverseParent { get; set; } = new List<OrgUnit>();

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();

    public virtual OrgUnit? Parent { get; set; }
}
