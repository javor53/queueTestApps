using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace QueueTestApp
{
    class Program
    {
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=myqueues;AccountKey=wX9ojEKqoG7kLE+daWH4dQiBsAPugpPrOej0tOyIssucfJ5GzDx/OCCR6lb/vPzUz+zpG8mr+XZ5dEtCUtQEIA==;EndpointSuffix=core.windows.net";

        static async Task Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("mystoragequeue");

            Random rand = new Random();

            string num = rand.Next().ToString();
            await SendMessageAsync(queue, num);
            Console.WriteLine($"Odesláno: {num}");

        }



        static async Task SendMessageAsync(CloudQueue theQueue, string newMessage)
        {
            bool createdQueue = await theQueue.CreateIfNotExistsAsync();

            if (createdQueue)
            {
                Console.WriteLine("Fronta vytvořena.");
            }

            CloudQueueMessage message = new CloudQueueMessage(newMessage);
            await theQueue.AddMessageAsync(message);
        }  
    }
}
