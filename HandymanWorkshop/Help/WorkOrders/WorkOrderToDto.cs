using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Help.Employees;
using HandymanWorkshop.Model;
using Shared.DTO.WorkOrder;

namespace HandymanWorkshop.Help.WorkOrders
{
    internal static class WorkOrderToDto
    {
        // Basic work order information
        internal static WorkOrderDto ToDto(WorkOrder workOrder)
        {
            return new WorkOrderDto
            {
                Uuid = workOrder.Uuid,
                CreatedAt = workOrder.CreatedAt,
                UpdatedAt = workOrder.UpdatedAt,
                Title = workOrder.Title ?? "",
                Description = workOrder.Description ?? "",
                Status = workOrder.Status.ToString(),
                ScheduledDate = workOrder.ScheduledDate,
                EstimatedHours = workOrder.EstimatedHours,
                HourlyRate = workOrder.HourlyRate,

                Employee = null,
                Department = null,
                WorkOrderProducts = null,
                Customer = null
            };
        }

        // Detailed work order information
        internal static WorkOrderDto ToDetailedDto(WorkOrder workOrder)
        {
            return ToAddDto(
                workOrder,
                addEmployee: true,
                addDepartment: true);
        }

        // Optionally include related data
        internal static WorkOrderDto ToAddDto(
            WorkOrder workOrder,
            bool addEmployee = false,
            bool addDepartment = false)
        {
            return new WorkOrderDto
            {
                Uuid = workOrder.Uuid,
                CreatedAt = workOrder.CreatedAt,
                UpdatedAt = workOrder.UpdatedAt,
                Title = workOrder.Title ?? "",
                Description = workOrder.Description ?? "",
                Status = workOrder.Status.ToString(),
                ScheduledDate = workOrder.ScheduledDate,
                EstimatedHours = workOrder.EstimatedHours,
                HourlyRate = workOrder.HourlyRate,

                Employee = addEmployee && workOrder.Employee != null
                    ? EmployeeToDto.ToDto(workOrder.Employee)
                    : null,

                Department = addDepartment && workOrder.Department != null
                    ? DepartmentToDto.ToDto(workOrder.Department)
                    : null,

                WorkOrderProducts = null,
                Customer = null
            };
        }
    }
}