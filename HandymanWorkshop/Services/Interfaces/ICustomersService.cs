using HandymanWorkshop.Model;
using Shared.DTO;
using Shared.DTO.Customer;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface ICustomersService : IBaseModelService<CustomerDto, Customer>, ISelectOption
    {

        public Task<List<CustomerDto>> ByApplicationUserId(string ApplicationUserId);

        Task<List<SelectOptionDto>> GetSelectOption(string ApplicationUserId);
    }
}
