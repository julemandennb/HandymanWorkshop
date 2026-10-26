using Shared.DTO.WorkOrder;

namespace Shared.DTO.Customer
{
    public class CustomerDto : BaseEntityDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Postcode { get; set; } = string.Empty;

        public ICollection<WorkOrderDto>? WorkOrders { get; set; }

    }
}
