using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Pivot;

namespace HandymanWorkshop.Help.WorkOrderProducts
{
    internal static class WorkOrderProductToDto
    {
        internal static WorkOrderProductDto ToDto(
        WorkOrderProduct WorkOrderProduct)
        {
            return new WorkOrderProductDto
            {
                Uuid = WorkOrderProduct.Uuid,
                WorkOrderName = WorkOrderProduct.WorkOrder.Title ?? "",
                ProductName = WorkOrderProduct.Product.Name,
                Quantity = WorkOrderProduct.Quantity,
                UnitPrice = WorkOrderProduct.UnitPrice,
                AddedByUser = WorkOrderProduct.AddedByUser?.UserName ?? ""

            };
        }

        internal static SelectOptionDto ToSelectOptionDto(WorkOrderProduct WorkOrderProduct)
        {
            return new SelectOptionDto
            {
                Text = WorkOrderProduct.Product.Name +" " + WorkOrderProduct.UnitPrice  ?? "",
                Value = WorkOrderProduct.Uuid.ToString()
            };
        }
    }
}
