using HandymanWorkshop.Data;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
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

        private static string GenerateTemporaryPassword()
        {
            return "Aa1!" + Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(18));
        }

       
    }
}
