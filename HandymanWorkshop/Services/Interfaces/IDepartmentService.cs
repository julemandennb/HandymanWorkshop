using Shared.DTO.Departments;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync();

        Task<DepartmentDto?> GetByUuidAsync(Guid uuid);

        Task<DepartmentDto?> GetDetailedByUuidAsync(Guid uuid);

        Task<DepartmentDto> CreateAsync(DepartmentDto dto);

        Task<bool> UpdateAsync(Guid uuid, DepartmentDto dto);

        Task<bool> DeleteAsync(Guid uuid);
    }
}
