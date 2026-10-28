using HandymanWorkshop.Model;
using Shared.DTO.Product;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IProductService : IBaseModelService<ProductDto, Product>, ISelectOption
    {
    }
}
