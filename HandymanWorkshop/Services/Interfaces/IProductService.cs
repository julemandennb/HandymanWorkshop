using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Product;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IProductService : IBaseModelService<ProductDto, Product>, ISelectOption
    {
        public Task<List<SelectOptionDto>> getSelectoptionByDepartment(Guid uuid);

    }
}
