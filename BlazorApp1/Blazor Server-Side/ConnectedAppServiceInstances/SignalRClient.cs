using Microsoft.Azure.SignalR.Management;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorServerSide.Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Serialization;
using MediatR;
using System.IO;
using Newtonsoft.Json;
using BlazorState;

namespace BlazorServerSide.ConnectedAppServiceInstances
{
    /// <summary>
    /// Singleton class that's responsible for receiving messages from the connected app service instances
    /// </summary>
    public class SignalRClient
    {
        private readonly IServiceManager _serviceManager;
        private readonly Shared shared;
        private readonly ILogger<SignalRClient> logger;
        private readonly IServiceProvider serviceProvider;        
        private readonly string user;
        private HubConnection hubConnection;

        public SignalRClient(Shared shared, ILogger<SignalRClient> logger, IServiceProvider serviceProvider)
        {
            _serviceManager = new ServiceManagerBuilder()
                .WithOptions(o => o.ConnectionString = shared.connectionstring)
                .Build();
            this.shared = shared;
            this.logger = logger;
            this.serviceProvider = serviceProvider;            
            user = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            if (string.IsNullOrEmpty(user))
            {
                // We assume you are running this code within the context of an app service
                // if we are running outside App Service the environment variable is not set and we need to assign a random value:
                user = Guid.NewGuid().ToString();
            }
            string siteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
        }
        public HubConnection CreateConnectionDirect()
        {
            var url = _serviceManager.GetClientEndpoint(Shared.hubname);

            var accessToken = _serviceManager.GenerateClientAccessToken(Shared.hubname, user);

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

        public async Task SetupClientAsync()
        {
            hubConnection = CreateConnectionDirect();
            await StartConnection();
        }

        public event EventHandler<MessageReceivedArgs> MessageReceived;

        private async Task StartConnection()
        {
            //connection.On<string, string>(Target, (user, message) =>
            //{
            //    var newMessage = $"{user}: {message}";
            //    MessagesList.Add(newMessage);
            //});

            hubConnection.On(Shared.stringmethodname, (string message) =>
            {
                
                MessageReceived?.Invoke(this, new MessageReceivedArgs { Message = message });                
            });

            hubConnection.On(Shared.requestmethodname, async (string message) =>
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedProvider = scope.ServiceProvider;
                    var mediator = scopedProvider.GetRequiredService<IMediator>();

                    using (var reader = new JsonTextReader(new StringReader(message)))
                    {

                        object obj = shared.jsonSerializer.Value.Deserialize(reader);
                        // object obj = JsonConvert.DeserializeObject(message);
                        // Type t = obj.GetType().GetInterfaces()[0].GenericTypeArguments[0];
                        // obj.GetType().GetInterfaces()[0].GetGenericArguments();
                        // IRequest request = (IRequest)obj;
                        var genericmethod = typeof(IMediator).GetMethod("Send").MakeGenericMethod(obj.GetType().GetInterfaces()[0].GetGenericArguments());
                        var result = genericmethod.Invoke(mediator, new object[] { obj, null });
                        Task t = (Task)result;
                        // await mediator.Send(obj);
                        await t;
                    }
                }
            });


            try
            {
                await hubConnection.StartAsync();                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected exception when starting SignalR connection");
            }
        }
    }
}
