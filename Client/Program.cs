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
        public AppSettings appState = new AppSettings();      

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");                        

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton(new AppSettings());
			builder.Services.AddSingleton(new AppState());

            await builder.Build().RunAsync();
        }
    }
}
