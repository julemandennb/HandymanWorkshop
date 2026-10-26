using Shared.DTO.Pivot;
using Shared.DTO.Employee;
using Shared.DTO.WorkOrder;

namespace Shared.DTO.Departments
{
    public class DepartmentDto: BaseEntityDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;

        public ICollection<EmployeeDto>? Employees { get; set; }

        public ICollection<WorkOrderDto>? WorkOrders { get; set; }

        public ICollection<DepartmentProductDto>? DepartmentProducts { get; set; }
    }
}
