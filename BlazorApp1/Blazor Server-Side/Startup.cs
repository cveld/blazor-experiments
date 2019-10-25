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
using BlazorServerSide.Data;
using BlazorServerSide;
using BlazorServerSide.Queue;
using BlazorServerSide.ConnectedAppServiceInstances;
using BlazorServerSide.Services;
using BlazorState;
using System.Reflection;
using BlazorServerSide.Features.Counter;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using BlazoredMaterialDesignModal;
using EmbeddedBlazorContent;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace BlazorServerSide
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
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation (validating against a single issuer value, as we do in
                    // line of business apps), we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    // If your application needs to authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            });

            services.AddServerSideBlazor().AddHubOptions(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            }).AddCircuitOptions(options => { options.DetailedErrors = true; });

            // Remove the following line to disable Azure SignalR Service for the browser connectivity:
            services.AddSignalR().AddAzureSignalR();

            //services.AddControllersWithViews(options =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();
            //    options.Filters.Add(new AuthorizeFilter(policy));
            //});

            services.AddRazorPages().AddRazorPagesOptions(options => {
                options.RootDirectory = "/RazorPages";
            }); ;
            services.AddSingleton<WeatherForecastService>();
            //services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<SignalRClient>();
            services.AddSingleton<CounterService>();  // naive counter service
            services.AddSingleton<SignalRServer>();
            services.AddTransient<BlazorServerSide.ConnectedAppServiceInstances.Shared>();
            services.AddSingleton<IQueueManager, SignalRQueueManager>();
            services.AddBlazorState(
                (aOptions) => aOptions.Assemblies =
                    new Assembly[]
                    {
                        typeof(Startup).GetTypeInfo().Assembly,
                    }
            );
            services.AddSingleton<CounterState>();  // BlazorState CounterService
            services.AddDbContext<VacationContext>(options => {
                    options.UseSqlite(Configuration.GetConnectionString("VacationDatabase"));
                    options.UseLazyLoadingProxies();
                    }
                );
            services.AddBlazoredModal();
            services.AddSingleton<CrossCircuitCommunication.CrossCircuitCommunication, CrossCircuitCommunication.CrossCircuitCommunication>();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                // endpoints.MapRazorPages();
            });

            app.UseEmbeddedBlazorContent(typeof(MatBlazor.BaseMatComponent).Assembly);
        }
    }
}
