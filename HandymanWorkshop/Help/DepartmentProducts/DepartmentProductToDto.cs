using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Pivot;

namespace HandymanWorkshop.Help.DepartmentProducts
{
    internal static class DepartmentProductToDto
    {
        // Basic department product information
        internal static DepartmentProductDto ToDto(
            DepartmentProduct departmentProduct)
        {
            return new DepartmentProductDto
            {
                Uuid = departmentProduct.Uuid,
                DepartmentUuid = departmentProduct.Department.Uuid,
                DepartmentName = departmentProduct.Department.Name ?? "",
                ProductUuid = departmentProduct.Product.Uuid,
                ProductName = departmentProduct.Product.Name,
                Total = departmentProduct.Total
            };
        }

        internal static SelectOptionDto ToSelectOptionDto(DepartmentProduct departmentProduct)
        {
            return new SelectOptionDto
            {
                Text = departmentProduct.Product.Name ?? "",
                Value = departmentProduct.Uuid.ToString()
            };
        }
    }
}
