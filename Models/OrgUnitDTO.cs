using System.ComponentModel.DataAnnotations.Schema;

namespace AMI_WebAPI.Models
{
    public class OrgUnitDTO
    {
        public int OrgUnitId { get; set; }

        public string Type { get; set; } = null!;

        public string Name { get; set; } = null!;

        [ForeignKey("OrgUnitId")]
        public int? ParentId { get; set; }

       // public virtual ICollection<OrgUnit> InverseParent { get; set; } = new List<OrgUnit>();

        //public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();

        //public virtual OrgUnit? Parent { get; set; }
    }
}
