using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;

namespace SectorModel.Client
{
    public class Program
    {
       /* public Program(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }*/

        public AppStateContainer appState = new AppStateContainer();      

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");          

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            AppStateContainer appState = new AppStateContainer()
            {
               UserName = "geoff",            
               UserId = Guid.Parse("4F3CBB0D-14B5-4C75-AB29-65543CF4CAA5"),
               CoreModelId = Guid.Parse("FAC8A666-74D8-4531-B3AD-DA7B95360462"),
               SPDRModelID = Guid.Parse("4237E32A-5C60-4141-8658-FA357C28EF28")
            };

            builder.Services.AddSingleton(appState);

            await builder.Build().RunAsync();
        }
    }
}
