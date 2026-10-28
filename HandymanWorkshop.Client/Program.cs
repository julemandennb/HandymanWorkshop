using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

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

            builder.Services.AddScoped(sp =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(
                        builder.HostEnvironment.BaseAddress)
                };

                return client;
            });

            await builder.Build().RunAsync();
        }
    }
}
