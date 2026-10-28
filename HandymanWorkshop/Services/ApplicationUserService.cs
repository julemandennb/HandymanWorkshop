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

        public async Task<List<ApplicationUserDto>> GetAllAsyncDTO()
        {
            var users = _userManager.Users.ToList();
            var result = new List<ApplicationUserDto>();

            foreach (var user in users)
            {
                var dto = ApplicationUserToDto.ToDto(user);
                var roles = await _userManager.GetRolesAsync(user);

                dto.Role = roles.FirstOrDefault() ?? "";
                result.Add(dto);
            }

            return result;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<ApplicationUserDto?> NewUser(ApplicationNewUserDto dto)
        {

            if (dto.Password != dto.PasswordCheck)
                return null;

            ApplicationUser applicationUser = await CreateAsync(dto.Email, dto.Password,dto.Role);

            ApplicationUserDto userDto = ApplicationUserToDto.ToDto(applicationUser);

            return userDto;
        }

        public async Task<bool> ResendNewPasswordByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;

            // Generate a temporary password.
            var password = GenerateTemporaryPassword();

            // Replace the existing password.
            var removeResult = await _userManager.RemovePasswordAsync(user);

            if (!removeResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join(", ",
                        removeResult.Errors.Select(e => e.Description)));

            var addResult = await _userManager.AddPasswordAsync(
                user, password);

            if (!addResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join(", ",
                        addResult.Errors.Select(e => e.Description)));

            // TODO: Send the temporary password to user.Email
            // using your configured email service.

            return true;
        }

        private static string GenerateTemporaryPassword()
        {
            return "Aa1!" + Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(18));
        }

       
    }
}
