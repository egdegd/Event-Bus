 using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

using Model;
using System.Threading.Tasks;
using System.Threading;

namespace View
{
    public class ConsoleClient
    {
        bool endApp = false;
        static void RequestMsg()
        {
            var client = new HttpClient();
            while (true)
            {
                Thread.Sleep(1000);
                var response = client.GetAsync("http://localhost:9001/api/serviceA/requestMsg").Result;
                response = client.GetAsync("http://localhost:9001/api/serviceA/requestEvent").Result;
                response = client.GetAsync("http://localhost:9002/api/serviceB/requestMsg").Result;
                response = client.GetAsync("http://localhost:9002/api/serviceB/requestEvent").Result;
                response = client.GetAsync("http://localhost:9003/api/serviceC/requestMsg").Result;
                response = client.GetAsync("http://localhost:9003/api/serviceC/requestEvent").Result;
                response = client.GetAsync("http://localhost:9004/api/serviceD/requestMsg").Result;
                response = client.GetAsync("http://localhost:9004/api/serviceD/requestEvent").Result;
            }
        }
        static async void RequestMsgAsync()
        {
            await Task.Run(() => RequestMsg());
        }

        public void Run()
        {
            RequestMsgAsync();
            Help();

            while (!endApp)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "send":
                        SendMessage();
                        break;
                    case "help":
                        Help();
                        break;
                    case "subscribe":
                        Subscribe();
                        break;
                    case "publish":
                        Publish();
                        break;
                    case "exit":
                        endApp = true;
                        break;

                }
            }
        }
        public void Help()
        {
            Console.WriteLine("Choose a command from the following list:");
            Console.WriteLine("\thelp - show a list of commands on the screen");
            Console.WriteLine("\tsend - send message <text> from <senderName> to <receipientName>");
            Console.WriteLine("\tsubscribe - subscribe to event with type <eventType>");
            Console.WriteLine("\tpublish - publish event with type <eventType> and <description>");
            Console.WriteLine("\texit - end program execution");
        }
        public void SendMessage()
        {
            Console.WriteLine("Text:");
            string text = Console.ReadLine();
            Console.WriteLine("Sender Name:");
            string fromName = Console.ReadLine();
            Console.WriteLine("Localhost:");
            string localhost = Console.ReadLine();
            Console.WriteLine("Recipient Name:");
            string toName = Console.ReadLine();
            var client = new HttpClient();
            string url = "http://localhost:" + localhost + "/api/" + fromName + "/sendmsg?recipient=" + toName + "&text=" + text;
            var response = client.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync().Result;
        }
        public void Subscribe()
        {
            Console.WriteLine("Service name:");
            string name = Console.ReadLine();
            Console.WriteLine("Localhost:");
            string localhost = Console.ReadLine();
            Console.WriteLine("Event type:");
            string type = Console.ReadLine();
            var client = new HttpClient();
            string url = "http://localhost:" + localhost + "/api/" + name + "/subscribe?type=" + type;
            var response = client.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync().Result;
        }
        public void Publish()
        {
            Console.WriteLine("Service name:");
            string name = Console.ReadLine();
            Console.WriteLine("Localhost:");
            string localhost = Console.ReadLine();
            Console.WriteLine("Type:");
            string type = Console.ReadLine();
            Console.WriteLine("Description:");
            string description = Console.ReadLine();
            var client = new HttpClient();
            string url = "http://localhost:" + localhost + "/api/" + name + "/publishevent?type=" + type + "&description=" + description;
            var response = client.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync().Result;
        }
    }
}
