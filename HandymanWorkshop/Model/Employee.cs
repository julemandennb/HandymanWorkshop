
using HandymanWorkshop.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HandymanWorkshop.Model
{
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "1000000.00",
            ErrorMessage = "Monthly salary must be between 0.01 and 1,000,000 DKK.")]
        [Precision(10, 2)]
        public decimal MonthlySalary { get; set; }

        [Range(typeof(decimal), "0.01", "10000.00")]
        [Precision(10, 2)]
        public decimal HourlyRate { get; set; }

        public int DepartmentId { get; set; }

        public virtual Department? Department { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
            = new List<WorkOrder>();
    }
}
