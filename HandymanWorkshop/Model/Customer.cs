
using HandymanWorkshop.Data;
using System.ComponentModel.DataAnnotations;

namespace HandymanWorkshop.Model
{
    public class Customer : BaseEntity
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            @"^\d{4}$",
            ErrorMessage = "Postcode must contain exactly four digits.")]
        public string Postcode { get; set; } = string.Empty;

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
            = new List<WorkOrder>();
    }
}
