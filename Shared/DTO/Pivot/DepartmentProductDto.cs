namespace Shared.DTO.Pivot
{
    public class DepartmentProductDto : BaseEntityDto
    {
        public Guid? DepartmentUuid { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public Guid? ProductUuid { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Total { get; set; }
    }
}
