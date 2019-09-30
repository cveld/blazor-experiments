using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorApp1.Data;
using BlazorServerSide;
using BlazorServerSide.Queue;
using BlazorServerSide.ConnectedAppServiceInstances;
using BlazorServerSide.Services;
using BlazorState;
using System.Reflection;
using BlazorServerSide.Features.Counter;
using Newtonsoft.Json;
using BlazorServerSide.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR().AddAzureSignalR();
            services.AddRazorPages();            
            services.AddServerSideBlazor().AddHubOptions(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            }).AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddSingleton<WeatherForecastService>();
            //services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<SignalRClient>();
            services.AddSingleton<CounterService>();  // naive counter service
            services.AddSingleton<SignalRServer>();
            services.AddTransient<Shared>();
            services.AddSingleton<IQueueManager, SignalRQueueManager>();
            services.AddBlazorState(
                (aOptions) => aOptions.Assemblies =
                    new Assembly[]
                    {
                        typeof(Startup).GetTypeInfo().Assembly,
                    }
            );
            services.AddSingleton<CounterState>();  // BlazorState CounterService
            services.AddDbContext<VacationContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("VacationDatabase")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
