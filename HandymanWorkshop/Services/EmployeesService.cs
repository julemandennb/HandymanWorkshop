using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Customers;
using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Help.Employees;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.DTO.Customer;
using Shared.DTO.Employee;

namespace HandymanWorkshop.Services
{
    public class EmployeesService : IEmployeesService 
    {
        private readonly ApplicationDbContext _context;

        private readonly IApplicationUserService _applicationUserService;
        private readonly IDepartmentService _departmentService;

        public EmployeesService(ApplicationDbContext context, IApplicationUserService applicationUserService, IDepartmentService departmentService)
        {
            _context = context;
            _applicationUserService = applicationUserService;
            _departmentService = departmentService;
        }

        public async Task<EmployeeDto> CreateAsync(EmployeeDto dto)
        {

            Department? department = await _departmentService
                   .GetModelByUuidAsync(dto.Department.Uuid);

            if (department == null)
            {
                throw new KeyNotFoundException("Department not found.");
            }

            ApplicationUser? user = await _applicationUserService.GetAsync(dto.Email);

            if(user == null)
                user = await _applicationUserService.CreateAsync(
                dto.Email,
                "User");

            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                MonthlySalary = dto.MonthlySalary,
                HourlyRate = dto.HourlyRate,
                DepartmentId = department.Id,
                ApplicationUserId = user.Id
            };

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return EmployeeToDto.ToDto(employee);
        }

        public async Task<bool> DeleteAsync(Guid uuid)
        {
            var employees = await _context.Employees
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (employees == null)
                return false;

            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            var employees = await _context.Employees
                 .AsNoTracking()
                 .ToListAsync();

            return employees
                .Select(EmployeeToDto.ToDto)
                .ToList();
        }

        public async Task<EmployeeDto?> GetByUuidAsync(Guid uuid)
        {
            var employees = await _context.Employees
               .AsNoTracking()
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return employees == null
                ? null
                : EmployeeToDto.ToDto(employees);
        }

        public async Task<EmployeeDto?> GetDetailedByUuidAsync(Guid uuid)
        {
            var employees = await _context.Employees
               .AsNoTracking()
               .Include(d => d.Department)
               .Include(d => d.WorkOrders)
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return employees == null
                ? null
                : EmployeeToDto.ToDetailedDto(employees);
        }

        public async Task<Employee?> GetModelByUuidAsync(Guid uuid)
        {
            var employees = await _context.Employees
               .AsNoTracking()
               .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return employees == null
                ? null
                : employees;
        }

        public async Task<bool> UpdateAsync(Guid uuid, EmployeeDto dto)
        {
            var employees = await _context.Employees.Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (employees == null)
                return false;

            employees.Name = dto.Name;
            employees.Email = dto.Email;
            employees.MonthlySalary = dto.MonthlySalary;
            employees.HourlyRate = dto.HourlyRate;

            if (dto.Department != null &&
                dto.Department.Uuid != employees.Department?.Uuid)
            {
                var department = await _departmentService
                    .GetModelByUuidAsync(dto.Department.Uuid);

                if (department == null)
                {
                    throw new KeyNotFoundException("Department not found.");
                }

                employees.Department = department;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<SelectOptionDto>> GetSelectOption()
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .ToListAsync();

            return employees
                .Select(EmployeeToDto.ToSelectOptionDto)
                .ToList();
        }

        public async Task<List<EmployeeDto>> ByApplicationUserId(string ApplicationUserId)
        {
            var employees = await _context.Employees
                .Where(c => c.ApplicationUserId == ApplicationUserId)
                  .AsNoTracking()
                  .ToListAsync();

            return employees
                .Select(EmployeeToDto.ToDto)
                .ToList();
        }
    }
}
