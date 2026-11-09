using Shared.DTO.Departments;
using Shared.DTO.WorkOrder;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.Employee
{
    public class EmployeeDto : BaseEntityDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "1000000.00",
           ErrorMessage = "Monthly salary must be between 0.01 and 1,000,000 DKK.")]
        public decimal MonthlySalary { get; set; } = 0;

        [Range(typeof(decimal), "0.01", "10000.00")]
        public decimal HourlyRate { get; set; } = 0;

        public DepartmentDto? Department { get; set; } = new DepartmentDto();

        public ICollection<WorkOrderDto>? WorkOrders { get; set; }

    }
}
