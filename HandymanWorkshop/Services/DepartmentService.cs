using HandymanWorkshop.Data;
using HandymanWorkshop.Help.Departments;
using HandymanWorkshop.Model;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using Shared.DTO.Departments;

namespace HandymanWorkshop.Services
{
    public class DepartmentService : IDepartmentService 
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .ToListAsync();

            return departments
                .Select(DepartmentToDto.ToDto)
                .ToList();
        }

        public async Task<DepartmentDto?> GetByUuidAsync(Guid uuid)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return department == null
                ? null
                : DepartmentToDto.ToDto(department);
        }

        public async Task<DepartmentDto?> GetDetailedByUuidAsync(Guid uuid)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .Include(d => d.Employees)
                .Include(d => d.WorkOrders)
                .Include(d => d.DepartmentProducts)
                    .ThenInclude(dp => dp.Product)
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return department == null
                ? null
                : DepartmentToDto.ToDetailedDto(department);
        }

        public async Task<DepartmentDto> CreateAsync(DepartmentDto dto)
        {
            var department = new Department
            {
                Name = dto.Name,
                Email = dto.Email,
                Address = dto.Address,
                Postcode = dto.Postcode
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return DepartmentToDto.ToDto(department);
        }

        public async Task<bool> UpdateAsync(Guid uuid, DepartmentDto dto)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (department == null)
                return false;

            department.Name = dto.Name;
            department.Email = dto.Email;
            department.Address = dto.Address;
            department.Postcode = dto.Postcode;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid uuid)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            if (department == null)
                return false;

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Department?> GetModelByUuidAsync(Guid uuid)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Uuid == uuid);

            return department == null
                ? null
                : department;
        }

        public async Task<List<SelectOptionDto>> GetSelectOption()
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .ToListAsync();

            return departments
                .Select(DepartmentToDto.ToSelectOptionDto)
                .ToList();
        }
    }
}
