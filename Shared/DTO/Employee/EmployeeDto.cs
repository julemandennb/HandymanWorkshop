using Shared.DTO.Departments;
using Shared.DTO.WorkOrder;

namespace Shared.DTO.Employee
{
    public class EmployeeDto : BaseEntityDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public decimal MonthlySalary { get; set; } = 0;

        public decimal HourlyRate { get; set; } = 0;

        public DepartmentDto? Department { get; set; }

        public ICollection<WorkOrderDto>? WorkOrders { get; set; }

    }
}
