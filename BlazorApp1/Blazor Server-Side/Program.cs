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

namespace BlazorApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
			// https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            IHost webHost = CreateHostBuilder(args).Build();

            // Create a new scope
            using (var scope = webHost.Services.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<SignalRClient>();
                await client.SetupClientAsync();
            }

            await webHost.StartAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
