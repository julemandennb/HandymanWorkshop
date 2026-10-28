using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace HandymanWorkshop.Model
{
    public class WorkOrder : BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(150)]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public WorkOrderStatus Status { get; set; }
            = WorkOrderStatus.Pending;

        public DateTime? ScheduledDate { get; set; }

        [Range(typeof(decimal), "0.1", "1000")]
        [Precision(8, 2)]
        public decimal EstimatedHours { get; set; }

        [Range(typeof(decimal), "0.01", "10000")]
        [Precision(10, 2)]
        public decimal HourlyRate { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee? Employee { get; set; }

        public int DepartmentId { get; set; }

        public virtual Department? Department { get; set; }

        public virtual ICollection<WorkOrderProduct> WorkOrderProducts { get; set; }
          = new List<WorkOrderProduct>();


        [Required]
        public int CustomerId { get; set; }

        public virtual Customer? Customer { get; set; }

    }
}
