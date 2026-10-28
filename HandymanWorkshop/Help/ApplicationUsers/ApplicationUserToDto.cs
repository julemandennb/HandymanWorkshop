
using HandymanWorkshop.Data;
using Shared.DTO.ApplicationUser;
using Shared.DTO.Customer;

namespace HandymanWorkshop.Help.ApplicationUsers
{
    internal static class ApplicationUserToDto
    {
        internal static ApplicationUserDto ToDto(ApplicationUser applicationUser)
        {
            return new ApplicationUserDto
            {
                Email = applicationUser.Email,
                Name = applicationUser.UserName,
                Role = applicationUser.UserName,
                Id = applicationUser.Id

            };
        }
    }
}
