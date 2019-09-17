using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.SignalR;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration configuration;
        private readonly Services services;
        private readonly Clients clients;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, Services services, Clients clients)
        {
            _logger = logger;
            this.configuration = configuration;
            this.services = services;
            this.clients = clients;
        }

        static HttpClient httpClient = new HttpClient();

        public async Task OnGetAsync()
        {
            var webProxy = new WebProxy(
                new Uri("http://uwvproxy:3128"), BypassOnLocal: false);

            var proxyHttpClientHandler = new HttpClientHandler
            {
                Proxy = webProxy,
                UseProxy = true,
            };

            // https://github.com/Azure/azure-service-bus-dotnet/issues/436
            // var httpClient = new HttpClient(proxyHttpClientHandler);
            var proxy = WebRequest.DefaultWebProxy;
            // AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false))
            // WebRequest.DefaultWebProxy = webProxy;
            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync("https://www.nu.nl");
            var content = await result.Content.ReadAsStringAsync();

            
            // await connection1.SendAsync("ReceiveMessage", $"Leuke tekst {DateTime.Now}");
            await services.SendMessageAsync();
        }

        
        
    }
}
