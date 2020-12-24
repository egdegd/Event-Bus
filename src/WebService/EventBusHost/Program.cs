using Microsoft.Owin.Hosting;
using System;
using WebAPI.Core.Controller;
using System.Threading;
using System.Net.Http;

namespace WebAPI.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string localhost = "9000";
            string name = "eventBus";

            EventBusController.init();
            Thread t1 = new Thread(EventBusController.MessagesWatchDog);
            t1.Start();
            Thread t2 = new Thread(EventBusController.EventsWatchDog);
            t2.Start();
           
            string baseAddress = $"http://localhost:{localhost}/";
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine(name + " started at " + baseAddress);
                Console.WriteLine("Press enter to finish");
                Console.ReadLine();
            }
        }
    }
}
