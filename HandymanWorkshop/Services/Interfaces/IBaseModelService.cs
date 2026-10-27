using Shared.DTO;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IBaseModelService<TDto,T>
    {
        Task<List<TDto>> GetAllAsync();

        Task<TDto?> GetByUuidAsync(Guid uuid);

        Task<TDto?> GetDetailedByUuidAsync(Guid uuid);

        Task<TDto> CreateAsync(TDto dto);

        Task<bool> UpdateAsync(Guid uuid, TDto dto);

        Task<bool> DeleteAsync(Guid uuid);

        Task<T?> GetModelByUuidAsync(Guid uuid);

        Task<List<SelectOptionDto>> GetSelectOption();
    }
}
