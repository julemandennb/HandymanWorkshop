using Shared.DTO.WorkOrder;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.Customer
{
    public class CustomerDto : BaseEntityDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            @"^\d{4}$",
            ErrorMessage = "Postcode must contain exactly four digits.")]
        public string Postcode { get; set; } = string.Empty;

        public ICollection<WorkOrderDto>? WorkOrders { get; set; }

    }
}
