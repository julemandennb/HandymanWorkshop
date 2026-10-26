using System.ComponentModel.DataAnnotations;

namespace HandymanWorkshop.Model
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(typeof(decimal), "0.01", "1000000")]
        public decimal Price { get; set; }

        public virtual ICollection<DepartmentProduct> DepartmentProducts { get; set; }
     = new List<DepartmentProduct>();

        public virtual ICollection<WorkOrderProduct> WorkOrderProducts { get; set; }
            = new List<WorkOrderProduct>();
    }
}
