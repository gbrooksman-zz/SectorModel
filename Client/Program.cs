using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SectorModel.Shared.Entities;
using SectorModel.Client.Entities;

namespace SectorModel.Client
{
	public class Program
    {
		public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");                        

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            AppSettings appSettings = new AppSettings();

            appSettings.CoreModelId = Guid.Parse("FAC8A666-74D8-4531-B3AD-DA7B95360462");
            appSettings.SPDRModelId = Guid.Parse("4237E32A-5C60-4141-8658-FA357C28EF28");

            builder.Services.AddSingleton(appSettings);

			builder.Services.AddSingleton(new AppState());

            await builder.Build().RunAsync();
        }
    }
}
