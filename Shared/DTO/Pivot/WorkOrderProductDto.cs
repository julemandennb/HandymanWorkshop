namespace Shared.DTO.Pivot
{
    public class WorkOrderProductDto : BaseEntityDto
    {
        public Guid WorkOrderUuid { get; set; }

        public Guid ProductUuid { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string AddedByUser { get; set; } = string.Empty;
    }
}
