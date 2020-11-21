using Microsoft.Owin.Hosting;
using System;

namespace WebAPI.SelfHost
{
    class Program
    {
        // Test URL:
        // http://localhost:9000/api/accesstest/test
        // http://localhost:9000/api/simplecheck/getitem?id=0
        // http://localhost:9000/api/eventbus/sendmsg?id=0
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
