using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

using Model;
using System.Threading.Tasks;
using DataStorage.DataProviders;
using Log;

namespace View
{
    class Program
    {
        
        static void Main(string[] args)
        {
            ConsoleClient client = new ConsoleClient();
            client.Run();
            //инициализация - требуется один раз в начале
            //Logger.Info("huee");
            //var res = SubscriberDataProvider.GetSubscribers("eventC");
            //Console.WriteLine(res.Count);
            //Console.ReadLine();
        }
    }
}
 