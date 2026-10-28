using HandymanWorkshop.Model;
using Shared.DTO.Pivot;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IWorkOrderProductService
    {
        Task<IEnumerable<WorkOrderProductDto>> GetAllAsync();

        Task<WorkOrderProductDto?> GetByUuidAsync(Guid uuid);

        Task<IEnumerable<Product>>
            GetAllProductsFromWorkOrderAsync(Guid workOrderUuid);

        Task<IEnumerable<WorkOrder>>
            GetAllWorkOrdersFromProductAsync(Guid productUuid);

        Task SyncAsync(
            Guid workOrderUuid,
            ICollection<WorkOrderProductDto> dtoList,
            string addedByUserId);

        Task SyncProductWorkOrdersAsync(
            Guid productUuid,
            ICollection<WorkOrderProductDto> dtoList,
            string addedByUserId);
    }
}