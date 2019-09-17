using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.SignalR
{
    public class Services
    {
        const string hubName = "Management";
        const string Target = "Target";
        private readonly IConfiguration configuration;
        public string connectionstring;

        public Services(IConfiguration configuration)
        {
            this.configuration = configuration;
            var section = configuration.GetSection("Azure:SignalR:ConnectionString");
            connectionstring = section.Value;
        }

        public async Task SendMessageAsync()
        {
            var serviceManager = new ServiceManagerBuilder().WithOptions(option =>
            {
                option.ConnectionString = connectionstring;
                option.ServiceTransportType = ServiceTransportType.Transient;
            }).Build();

            var _hubContext = await serviceManager.CreateHubContextAsync(hubName, new LoggerFactory());
            await _hubContext.Clients.All.SendAsync(Target, "TEST");
        }


        private string GetClientUrl(string endpoint, string hubName)
        {
            return $"{endpoint}/client/?hub={hubName}";
        }

        /// <summary>
        /// Niet de goede manier om een client aan te melden. Zie Management sample
        /// </summary>
        /// <returns></returns>
        //public HubConnection CreateConnection()
        //{
        //    HubConnection connection;
        //    var serviceUtils = new ServiceUtils(connectionstring);
        //    var url = GetClientUrl(serviceUtils.Endpoint, hubName);
        //    // var signalrUrl = "https://carlintveld.service.signalr.net/client/?hub=blazor";

        //    // https://stackoverflow.com/questions/55493434/could-not-connect-to-azure-signalr-hub
        //    connection = new HubConnectionBuilder()
        //        .WithUrl(url, option =>
        //        {
        //            option.AccessTokenProvider = () =>
        //            {
        //                const string userId = "user";
        //                return Task.FromResult(serviceUtils.GenerateAccessToken(url, userId));
        //            };
        //        })
        //        // .WithUrl("http://localhost:53353/ChatHub")
        //        .Build();

        //    connection.Closed += async (error) =>
        //    {
        //        await Task.Delay(new Random().Next(0, 5) * 1000);
        //        await connection.StartAsync();
        //    };
        //    return connection;
        //}

        

        public async Task StartConnection(string user, HubConnection connection, List<string> MessagesList)
        {
            //connection.On<string, string>(Target, (user, message) =>
            //{
            //    var newMessage = $"{user}: {message}";
            //    MessagesList.Add(newMessage);
            //});

            connection.On(Target, (string message) =>
            {
                var newMessage = $"{user}: {message}";
                MessagesList.Add(newMessage);
            });


            try
            {
                await connection.StartAsync();
                MessagesList.Add("Connection started");
            }
            catch (Exception ex)
            {
                MessagesList.Add(ex.Message);
            }
        }
    }
}
