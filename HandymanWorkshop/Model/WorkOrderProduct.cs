using HandymanWorkshop.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HandymanWorkshop.Model
{
    public class WorkOrderProduct : BaseEntity
    {
        public int WorkOrderId { get; set; }

        public virtual WorkOrder WorkOrder { get; set; } = null!;

        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Precision(10, 2)]
        [Range(typeof(decimal), "0.01", "1000000")]
        public decimal UnitPrice { get; set; }

        [Required]
        public string AddedByUserId { get; set; } = string.Empty;

        public virtual ApplicationUser? AddedByUser { get; set; }
    }
}
