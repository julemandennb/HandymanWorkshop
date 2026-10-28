using Shared.DTO;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface ISelectOption
    {
        Task<List<SelectOptionDto>> GetSelectOption();
    }
}
