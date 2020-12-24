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
            Console.WriteLine("Choose localhost:");
            string localhost = Console.ReadLine();
            Console.WriteLine("Service name:");
            string name = Console.ReadLine();
           
            string baseAddress = "http://localhost:" + localhost + "/";
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine(name + " started at " + baseAddress);
                Console.WriteLine("Press enter to finish");
                Console.ReadLine();
            }
        }
    }
}
