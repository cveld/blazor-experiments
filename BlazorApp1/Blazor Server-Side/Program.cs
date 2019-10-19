using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using BlazorServerSide.ConnectedAppServiceInstances;
using BlazorServerSide.Data;

namespace BlazorServerSide
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
			// https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            IHost webHost = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(webHost);

            // Create a new scope
            using (var scope = webHost.Services.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<SignalRClient>();
                // explicitly don't await this callback so that the webhost can kick off in parallel
                _ = client.SetupClientAsync();
            }

            await webHost.StartAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // copy the file appsettings.local.example.json and fill in your connectionstrings
                    config.AddJsonFile(
                    "appsettings.local.json", optional: true, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<VacationContext>();
                    VacationsDbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}
