using HandymanWorkshop.Model;
using Shared.DTO.Departments;
using Shared.DTO.Employee;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IEmployeesService : IBaseModelService<EmployeeDto,Employee>
    {
    }
}
