using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.Azure.Storage; // Namespace for CloudStorageAccount
using Microsoft.Azure.Storage.Queue; // Namespace for Queue storage types
using Microsoft.Extensions.Configuration;

namespace BlazorServerSide.Queue
{
    // based off https://docs.microsoft.com/nl-nl/azure/storage/queues/storage-dotnet-how-to-use-queues
    // and https://www.dotnetcurry.com/visualstudio/1328/visual-studio-connected-services-aspnet-core-azure-storage
    public class StorageQueueManager : IQueueManager
    {
        IConfiguration _configurationRoot;
        CloudQueueClient queueClient;
        CloudQueue queue;

        public StorageQueueManager(IConfiguration configurationRoot)
        {
            _configurationRoot = configurationRoot;
            var section = _configurationRoot.GetSection("StorageConnectionString");
            var connStr = section.Value;

            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);

            // Create the queue client.
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            queue = queueClient.GetQueueReference("carlintveld");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();
        }

        public async Task AddMessageAsync(string input)
        {
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(input);
            await queue.AddMessageAsync(message);
        }
    }
}
