using Shared.DTO.Pivot;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.Product
{
    public class ProductDto : BaseEntityDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "1000000")]
        public decimal Price { get; set; }

        public ICollection<DepartmentProductDto>? DepartmentProducts { get; set; } = new List<DepartmentProductDto>();

        public ICollection<WorkOrderProductDto>? WorkOrderProducts { get; set; } = new List<WorkOrderProductDto>();
    }
}
