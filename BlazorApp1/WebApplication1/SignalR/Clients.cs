using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.SignalR.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.SignalR
{
    public class Clients
    {
        const string hubName = "Management";
        const string Target = "Target";
        const string user = "User";

        private readonly Services services;
        private readonly IServiceManager _serviceManager;

        public Clients(Services services)
        {
            this.services = services;            
            _serviceManager = new ServiceManagerBuilder()
                .WithOptions(o => o.ConnectionString = services.connectionstring)
                .Build();
        }

        List<HubConnection> connections = new List<HubConnection>();
        public List<string> MessagesList = new List<string>();

        public async Task SetupClientsAsync()
        {
            var connection1 = CreateConnectionDirect();
            await services.StartConnection("user1", connection1, MessagesList);
            connections.Add(connection1);
            var connection2 = CreateConnectionDirect();
            await services.StartConnection("user2", connection2, MessagesList);
            connections.Add(connection2);
        }

        public HubConnection CreateConnectionViaManager()
        {
            var url = "http://localhost:5000/Management?user=User";
            var connection = new HubConnectionBuilder().WithUrl(url).Build();
            return connection;
        }

        public HubConnection CreateConnectionDirect()
        {
            var url = _serviceManager.GetClientEndpoint(hubName);

            var accessToken = _serviceManager.GenerateClientAccessToken(hubName, user);
    
            var connection = new HubConnectionBuilder()
                    .WithUrl(url, option =>
                    {
                        option.AccessTokenProvider = () =>
                    {
                    
                    return Task.FromResult(accessToken);
                };
                    })
                    // .WithUrl("http://localhost:53353/ChatHub")
                    .Build();

            connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                };

            return connection;
        }
    }
}
