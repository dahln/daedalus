using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorSpinner;
using climatepi.Client.Services;

namespace climatepi.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("climatepi.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("climatepi.ServerAPI"));

            builder.Services.AddScoped<API>();
            builder.Services.AddScoped<SpinnerService>();
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            await builder.Build().RunAsync();
        }
    }
}
