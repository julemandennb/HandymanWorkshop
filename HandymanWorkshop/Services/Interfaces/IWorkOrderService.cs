using HandymanWorkshop.Model;
using Shared.DTO.Customer;
using Shared.DTO.WorkOrder;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IWorkOrderService : IBaseModelService<WorkOrderDto, WorkOrder>
    {
        public Task<List<WorkOrderDto>> ByApplicationUserId(string ApplicationUserId);

    }
}
