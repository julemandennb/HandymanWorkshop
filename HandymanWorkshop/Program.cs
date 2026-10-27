using HandymanWorkshop.Client.Pages;
using HandymanWorkshop.Components;
using HandymanWorkshop.Components.Account;
using HandymanWorkshop.Data;
using HandymanWorkshop.Services;
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HandymanWorkshop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents()
                .AddAuthenticationStateSerialization();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            builder.Services.AddAuthorization();

            var connectionString = builder.Configuration.GetConnectionString("HandymanWorkshop")
            ?? throw new InvalidOperationException("Connection string 'HandymanWorkshop' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
                     .AddRoles<IdentityRole>()
                     .AddEntityFrameworkStores<ApplicationDbContext>()
                     .AddSignInManager()
                     .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped(c =>
            {
                var NavMan = c.GetRequiredService<NavigationManager>();

                return new HttpClient { BaseAddress = new Uri(NavMan.BaseUri) };
            });

            AddServices(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapControllers();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            await SeedRolesAsync(app);

            var adminEmail = builder.Configuration["AdminProfil:Email"]
            ?? throw new InvalidOperationException("Admin email is missing.");

            var adminPassword = builder.Configuration["AdminProfil:Password"]
             ?? throw new InvalidOperationException("Admin password is missing.");
            await SeedAdminAsync(app, adminEmail, adminPassword);

            await app.RunAsync();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
            builder.Services.AddScoped<IDepartmentService,DepartmentService>();
            builder.Services.AddScoped<IEmployeesService, EmployeesService>();
        }

        private static async Task SeedRolesAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "SuperUser", "User", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to create role '{role}': " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private static async Task SeedAdminAsync(
            WebApplication app,
            string adminEmail,
            string adminPassword)
        {
            using var scope = app.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Failed to create administrator: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    adminUser, "Admin");

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Failed to assign Admin role: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }

    }
}
