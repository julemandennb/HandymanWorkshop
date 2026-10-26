using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace HandymanWorkshop.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthenticationStateDeserialization();

            builder.Services.AddScoped(s =>

                new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
            );

            await builder.Build().RunAsync();
        }
    }
}
