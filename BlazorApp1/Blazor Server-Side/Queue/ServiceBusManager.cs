using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServerSide.Queue
{
    public class ServiceBusManager : IQueueManager
    {
        IConfiguration _configuration;
        static ITopicClient topicClient;

        public ServiceBusManager(IConfiguration configuration)
        {
            _configuration = configuration;
            var ServiceBusConnectionStringSection = _configuration.GetSection("ServiceBusConnectionString");
            var ServiceBusConnectionString = ServiceBusConnectionStringSection.Value;
            const string TopicName = "carlintveld";
            // ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;
            
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            topicClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
            topicClient.ServiceBusConnection.OperationTimeout = new TimeSpan(0, 0, 10);
        }

        static HttpClient httpClient = new HttpClient();

        public async Task AddMessageAsync(string input)
        {
            var message = new Message(Encoding.UTF8.GetBytes(input));

            var result = await httpClient.GetAsync("https://www.nu.nl/");
            var content = await result.Content.ReadAsStringAsync();


            // Send the message to the topic.
            await topicClient.SendAsync(message);
        }
    }
}
