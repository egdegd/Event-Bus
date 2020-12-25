 using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Model;
using System.Threading.Tasks;
using System.Threading;
using WebServiceClient;

namespace View
{
    public class ConsoleClient
    {
        bool endApp = false;
        static readonly SampleServiceClient client = new SampleServiceClient();

        static void RequestMsg(string localhost, string name)
        {
            while (true)
            {
                Thread.Sleep(1000);
                client.RequestMsg(localhost, name);
                client.RequestEvent(localhost, name);
            }
        }

        static async void RequestMsgAsync(string localhost, string name)
        {
            await Task.Run(() => RequestMsg(localhost, name));
        }

        public void Run()
        {
            Help();

            while (!endApp)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "addService":
                        AddService();
                        break;
                    case "send":
                        SendMessage();
                        break;
                    case "help":
                        Help();
                        break;
                    case "subscribe":
                        Subscribe();
                        break;
                    case "unsubscribe":
                        Unsubscribe();
                        break;
                    case "publish":
                        Publish();
                        break;
                    case "stress-testing":
                        StressTestAsync();
                        break;
                    case "exit":
                        endApp = true;
                        break;

                }
            }
        }

        public void AddService()
        {
            Console.WriteLine("Localhost:");
            string localhost = Console.ReadLine();

            Console.WriteLine("Name:");
            string name = Console.ReadLine();

            RequestMsgAsync(localhost, name);
        }

        public void Help()
        {
            Console.WriteLine("Choose a command from the following list:");
            Console.WriteLine("\thelp - show a list of commands on the screen");
            Console.WriteLine("\taddService - add service");
            Console.WriteLine("\tsend - send message <text> from <senderName> to <receipientName>");
            Console.WriteLine("\tsubscribe - subscribe to event with type <eventType>");
            Console.WriteLine("\tunsubscribe - unsubscribe from event with type <eventType>");
            Console.WriteLine("\tpublish - publish event with type <eventType> and <description>");
            Console.WriteLine("\tstress-testing - launch the service for testing");
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

            try
            {
                client.SendMessage(localhost, fromName, toName, text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        public void Subscribe()
        {
            Console.WriteLine("Service name:");
            string name = Console.ReadLine();

            Console.WriteLine("Localhost:");
            string localhost = Console.ReadLine();

            Console.WriteLine("Event type:");
            string type = Console.ReadLine();

            try
            {
                client.Subscribe(localhost, name, type);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void Unsubscribe()
        {
            Console.WriteLine("Service name:");
            string name = Console.ReadLine();

            Console.WriteLine("Localhost:");
            string localhost = Console.ReadLine();

            Console.WriteLine("Event type:");
            string type = Console.ReadLine();

            try
            {
                client.Unsubscribe(localhost, name, type);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

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

            try
            {
                client.Publish(localhost, name, type, description);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static async void StressTestAsync()
        {
            Console.WriteLine("Localhost:");
            string localhost = Console.ReadLine();

            Console.WriteLine("messages per second:");
            string messagesPerSecond = Console.ReadLine();

            Console.WriteLine("count of seconds");
            string countOfSeconds = Console.ReadLine();

            await Task.Run(() => StressTest(localhost, messagesPerSecond, countOfSeconds));
        }

        static void StressTest(string localhost, string messagesPerSecond, string countOfSeconds)
        {
            var client = new HttpClient();
            string url = $"http://localhost:{localhost}/api/testService/sendmsg?messagesPerSecond={messagesPerSecond}&countOfSeconds={countOfSeconds}";
            var response = client.GetAsync(url).Result;
            var result = response.Content.ReadAsStringAsync().Result;
        }
    }
}
