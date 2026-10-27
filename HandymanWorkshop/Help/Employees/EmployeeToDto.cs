using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Help.WorkOrders;
using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Employee;

namespace HandymanWorkshop.Help.Employees
{
    internal static class EmployeeToDto
    {
        // Basic employee information
        internal static EmployeeDto ToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Uuid = employee.Uuid,
                Name = employee.Name,
                Email = employee.Email,
                HourlyRate = employee.HourlyRate,
                Department = null,
                WorkOrders = null
            };
        }

        // Detailed employee information
        internal static EmployeeDto ToDetailedDto(Employee employee)
        {
            return ToAddDto(
                employee,
                addDepartment: true,
                addWorkOrders: true);
        }

        // Optionally include department and work orders
        internal static EmployeeDto ToAddDto(
            Employee employee,
            bool addDepartment = false,
            bool addWorkOrders = false)
        {
            return new EmployeeDto
            {
                Uuid = employee.Uuid,
                Name = employee.Name,
                Email = employee.Email,
                HourlyRate = employee.HourlyRate,
                MonthlySalary = employee.MonthlySalary,

                Department = addDepartment && employee.Department != null
                    ? DepartmentToDto.ToDto(employee.Department)
                    : null,

                WorkOrders = addWorkOrders && employee.WorkOrders != null
                    ? employee.WorkOrders
                        .Select(WorkOrderToDto.ToDto)
                        .ToList()
                    : null
            };
        }

        internal static SelectOptionDto ToSelectOptionDto(Employee employee)
        {
            return new SelectOptionDto
            {
                Text = employee.Name ?? "",
                Value = employee.Uuid.ToString()
            };
        }
    }
}