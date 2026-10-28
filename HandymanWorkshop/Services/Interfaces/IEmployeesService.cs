using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Customer;
using Shared.DTO.Departments;
using Shared.DTO.Employee;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IEmployeesService : IBaseModelService<EmployeeDto, Employee>, ISelectOption
    {
        public Task<List<EmployeeDto>> ByApplicationUserId(string ApplicationUserId);

        public Task<List<SelectOptionDto>> getSelectoptionByDepartment(Guid uuid);
    }
}
