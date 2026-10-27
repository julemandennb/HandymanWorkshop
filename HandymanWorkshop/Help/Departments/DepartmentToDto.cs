
using HandymanWorkshop.Help.DepartmentProducts;
using HandymanWorkshop.Help.Employees;
using HandymanWorkshop.Help.WorkOrders;
using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Departments;


namespace HandymanWorkshop.Help.Departments
{
    internal static class DepartmentToDto
    {
        // Basic department information
        internal static DepartmentDto ToDto(Department department)
        {
            return new DepartmentDto
            {
                Uuid = department.Uuid,
                Name = department.Name ?? "",
                Email = department.Email ?? "",
                Address = department.Address ?? "",
                Postcode = department.Postcode ?? "",

                Employees = null,
                WorkOrders = null,
                DepartmentProducts = null
            };
        }

        // Detailed department information
        internal static DepartmentDto ToDetailedDto(Department department)
        {
            return ToAddDto(
                department,
                addEmployees: true,
                addWorkOrders: true,
                addDepartmentProducts: true);
        }

        // Optionally include related data
        internal static DepartmentDto ToAddDto(
            Department department,
            bool addEmployees = false,
            bool addWorkOrders = false,
            bool addDepartmentProducts = false)
        {
            return new DepartmentDto
            {
                Uuid = department.Uuid,
                Name = department.Name ?? "",
                Email = department.Email ?? "",
                Address = department.Address ?? "",
                Postcode = department.Postcode ?? "",

                Employees = addEmployees && department.Employees != null
                    ? department.Employees
                        .Select(e => EmployeeToDto.ToAddDto(e))
                        .ToList()
                    : null,

                WorkOrders = addWorkOrders && department.WorkOrders != null
                ? department.WorkOrders
                    .Select(WorkOrderToDto.ToDto)
                    .ToList()
                : null,

                DepartmentProducts =
                addDepartmentProducts && department.DepartmentProducts != null
                ? department.DepartmentProducts
                    .Select(DepartmentProductToDto.ToDto)
                    .ToList()
                : null
            };
        }

        internal static SelectOptionDto ToSelectOptionDto(Department department)
        {
            return new SelectOptionDto
            {
                Text = department.Name ?? "",
                Value = department.Uuid.ToString()
            };
        }
    }
}
