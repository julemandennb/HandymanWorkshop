using System.ComponentModel.DataAnnotations;

namespace HandymanWorkshop.Model
{
    public class DepartmentProduct : BaseEntity
    {
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; } = null!;

        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Total { get; set; }
    }
}
