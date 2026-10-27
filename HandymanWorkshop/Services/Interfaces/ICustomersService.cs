using HandymanWorkshop.Model;
using Shared.DTO.Customer;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface ICustomersService : IBaseModelService<CustomerDto, Customer>
    {

        public Task<List<CustomerDto>> ByApplicationUserId(string ApplicationUserId);
    }
}
