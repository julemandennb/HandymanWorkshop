using HandymanWorkshop.Data;

namespace HandymanWorkshop.Services.Interfaces
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser> CreateAsync(
           string email,
           string role
           );

        Task<ApplicationUser> CreateAsync(
       string email,
       string password,
       string role);

        Task<ApplicationUser?> GetAsync(
          string email);
    }
}
