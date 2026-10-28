using HandymanWorkshop.Data;
using Shared.DTO.ApplicationUser;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser> CreateAsync(
                    string email, string role);

        Task<ApplicationUser> CreateAsync(
            string email, string password, string role);

        Task<ApplicationUser?> GetAsync(string email);

        Task<List<ApplicationUserDto>> GetAllAsyncDTO();

        Task<bool> DeleteUserAsync(string id);

        Task<ApplicationUserDto?> NewUser(
            ApplicationNewUserDto dto);

        Task<bool> ResendNewPasswordByIdAsync(string id);
    }
}
