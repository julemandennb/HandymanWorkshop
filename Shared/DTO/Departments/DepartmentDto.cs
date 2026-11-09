using Shared.DTO.Employee;
using Shared.DTO.Pivot;
using Shared.DTO.WorkOrder;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.Departments
{
    public class DepartmentDto: BaseEntityDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must contain exactly four digits.")]
        public string Postcode { get; set; } = string.Empty;

        public ICollection<EmployeeDto>? Employees { get; set; }

        public ICollection<WorkOrderDto>? WorkOrders { get; set; }

        public ICollection<DepartmentProductDto>? DepartmentProducts { get; set; }
    }
}
