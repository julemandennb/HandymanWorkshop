using System.ComponentModel.DataAnnotations;

namespace HandymanWorkshop.Model
{
    public class Department : BaseEntity
    {

        [Required]
        public string? Name { get; set; }

        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}$",ErrorMessage = "Postcode must contain exactly four digits.")]
        public string? Postcode { get; set; }


        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();

        public virtual ICollection<DepartmentProduct> DepartmentProducts { get; set; }
        = new List<DepartmentProduct>();

    }
}
