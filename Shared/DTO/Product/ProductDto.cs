using Shared.DTO.Pivot;

namespace Shared.DTO.Product
{
    public class ProductDto : BaseEntityDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public ICollection<DepartmentProductDto>? DepartmentProducts { get; set; }

        public ICollection<WorkOrderProductDto>? WorkOrderProducts { get; set; }
    }
}
