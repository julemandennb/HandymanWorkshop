using HandymanWorkshop.Data;
using HandymanWorkshop.Help.ApplicationUsers;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Shared.DTO.ApplicationUser;
using System.Security.Cryptography;

namespace HandymanWorkshop.Services
{
    public class ApplicationUserService : IApplicationUserService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<ApplicationUser> CreateAsync(string email, string role)
        {
            var password = GenerateTemporaryPassword();

            return await CreateAsync(email, password, role);
        }

        public async Task<ApplicationUser> CreateAsync(
            string email,
            string password,
            string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new ArgumentException(
                    $"Role '{role}' does not exist.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(
                user, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user, role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                throw new InvalidOperationException(
                    string.Join(", ",
                        roleResult.Errors.Select(e => e.Description)));
            }

            return user;
        }

        public async Task<ApplicationUser?> GetAsync(string email)
        {
            var User = await _userManager.FindByEmailAsync(email);
            return User;
        }

        public async Task<ApplicationUserDto?> GetAllAsyncDTO()
        {
            //List<ApplicationUser> applicationUsers = await _userManager.

            return null;
        }

        public async Task<bool> DeleteUser(Guid guid)
        {
            //await _userManager.DeleteAsync();

            return true;
        }

        public async Task<ApplicationUserDto?> NewUser(ApplicationNewUserDto dto)
        {

            if (dto.Password != dto.PasswordCheck)
                return null;

            ApplicationUser applicationUser = await CreateAsync(dto.Email, dto.Password,dto.Role);

            ApplicationUserDto userDto = ApplicationUserToDto.ToDto(applicationUser);

            return userDto;
        }

        public async Task<bool> ReSendtNewPassword(string email)
        {
            var User = await _userManager.FindByEmailAsync(email);
            var password = GenerateTemporaryPassword();

            await _userManager.RemovePasswordAsync(User);
            await _userManager.AddPasswordAsync(User, password);

            //set mail 


            return true;
        }

        private static string GenerateTemporaryPassword()
        {
            return "Aa1!" + Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(18));
        }

       
    }
}
