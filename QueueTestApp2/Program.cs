using System;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace QueueTestApp2
{
    class Program
    {
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=myqueues;AccountKey=wX9ojEKqoG7kLE+daWH4dQiBsAPugpPrOej0tOyIssucfJ5GzDx/OCCR6lb/vPzUz+zpG8mr+XZ5dEtCUtQEIA==;EndpointSuffix=core.windows.net";
    

        static async Task Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("mystoragequeue");


            string num = await ReceiveMessageAsync(queue);
            Console.WriteLine($"Přijata zpráva: {num}\n");

        }

        static void addToXML(string num)
        {
            if (!File.Exists(@"./received.xml"))
            {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                XmlNode numsNode = doc.CreateElement("Numbers");
                doc.AppendChild(numsNode);

                XmlNode numNode = doc.CreateElement("Number");
                XmlText numText = doc.CreateTextNode(num);
                numNode.AppendChild(numText);
                numsNode.AppendChild(numNode);

                doc.Save(@"./received.xml");
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"./received.xml");

                XmlNode numsNode = doc.SelectSingleNode("/Numbers");
                XmlNode numNode = doc.CreateElement("Number");
                XmlText numText = doc.CreateTextNode(num);
                numNode.AppendChild(numText);
                numsNode.AppendChild(numNode);
                doc.Save(@"./received.xml");
            }
        }

        static async Task<string> ReceiveMessageAsync(CloudQueue theQueue)
        {
            bool exists = await theQueue.ExistsAsync();

            if (exists)
            {
                CloudQueueMessage retrievedMessage = await theQueue.GetMessageAsync();

                if (retrievedMessage != null)
                {
                    string theMessage = retrievedMessage.AsString;
                    await theQueue.DeleteMessageAsync(retrievedMessage);
                    addToXML(theMessage);
                    return theMessage;
                }
                else
                {
                    Console.Write("Fronta je prázdná. Odstranit? (A/N) ");
                    string response = Console.ReadLine();

                    if (response == "A" || response == "a")
                    {
                        await theQueue.DeleteIfExistsAsync();
                        return "Fronta byla smazaná";
                    }
                    else
                    {
                        return "Fronta byla zachovaná.";
                    }
                }
            }
            else
            {
                return "Fronta neexistuje.";
            }
        }
    }
}
