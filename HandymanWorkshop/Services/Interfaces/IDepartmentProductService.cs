using Shared.DTO.Pivot;


namespace HandymanWorkshop.Services.Interfaces
{
    public interface IDepartmentProductService: ISelectOption
    {
        Task<IEnumerable<DepartmentProductDto>> GetAllAsync();

        Task<DepartmentProductDto?> GetByUuidAsync(Guid uuid);

        Task SyncAsync(
            Guid departmentUuid,
            ICollection<DepartmentProductDto> dtoList);

        Task SyncProductDepartmentsAsync(
            Guid productUuid,
            ICollection<DepartmentProductDto> dtoList);
    }
}
