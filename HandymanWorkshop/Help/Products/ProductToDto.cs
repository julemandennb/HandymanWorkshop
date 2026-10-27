using HandymanWorkshop.Help.DepartmentProducts;
using HandymanWorkshop.Help.WorkOrderProducts;
using HandymanWorkshop.Help.WorkOrders;
using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Product;

namespace HandymanWorkshop.Help.Products
{
    internal static class ProductToDto
    {
        internal static ProductDto ToDto(Product product)
        {
            return new ProductDto
            {
                Uuid = product.Uuid,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,


            };
        }

        // Detailed Customer information
        internal static ProductDto ToDetailedDto(Product product)
        {
            return ToAddDto(
                product,
                addDepartment:true,
                addWorkOrders: true);
        }

        internal static ProductDto ToAddDto(
            Product product,
            bool addDepartment = false,
            bool addWorkOrders = false)
        {
            return new ProductDto
            {
                Uuid = product.Uuid,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,

                DepartmentProducts = addDepartment && product.DepartmentProducts != null
                    ? product.DepartmentProducts
                        .Select(DepartmentProductToDto.ToDto)
                        .ToList()
                    : null,

                WorkOrderProducts = addWorkOrders && product.WorkOrderProducts != null
                    ? product.WorkOrderProducts
                        .Select(WorkOrderProductToDto.ToDto)
                        .ToList()
                    : null


            };
        }

        internal static SelectOptionDto ToSelectOptionDto(Product product)
        {
            return new SelectOptionDto
            {
                Text = product.Name ?? "",
                Value = product.Uuid.ToString()
            };
        }
    }
}
