using HandymanWorkshop.Model;
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
                ProductUuid = departmentProduct.Product.Uuid,
                ProductName = departmentProduct.Product.Name,
                Total = departmentProduct.Total
            };
        }
    }
}
