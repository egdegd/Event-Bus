using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

using Model;
using System.Threading.Tasks;
using DataStorage.DataProviders;

namespace View
{
    class Program
    {
        
        static void Main(string[] args)
        {
            ConsoleClient client = new ConsoleClient();
            client.Run();

            //var res = SubscriberDataProvider.GetSubscribers("eventC");
            //Console.WriteLine(res.Count);
            //Console.ReadLine();
        }
    }
}
 