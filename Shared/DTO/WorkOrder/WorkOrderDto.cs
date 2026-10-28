using Shared.DTO.Customer;
using Shared.DTO.Departments;
using Shared.DTO.Employee;
using Shared.DTO.Pivot;
using Shared.Enum;

namespace Shared.DTO.WorkOrder
{
    public class WorkOrderDto : BaseEntityDto
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Pending;

        public DateTime? ScheduledDate { get; set; }

        public decimal EstimatedHours { get; set; } = 0;

        public decimal HourlyRate { get; set; } = 0;

        public EmployeeDto? Employee { get; set; } = new EmployeeDto();

        public DepartmentDto? Department { get; set; } = new DepartmentDto();

        public ICollection<WorkOrderProductDto>? WorkOrderProducts { get; set; } = new List<WorkOrderProductDto>();

        public CustomerDto? Customer { get; set; } = new CustomerDto();
    }
}
