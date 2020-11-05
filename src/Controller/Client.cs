using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service;
namespace Controller
{
    public class Client
    {
        bool endApp = false;
        ServiceStorage serviceStorage = new ServiceStorage();
        EventBus.EventBus eventBus = new EventBus.EventBus();
        public void Run()
        {
            eventBus.SendMsgAsync();
            Help();
            while (!endApp)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "create":
                        Create();
                        break;
                    case "send":
                        SendMessage();
                        break;
                    case "services":
                        serviceStorage.PrintServices();
                        break;
                    case "delete":
                        DeleteService();
                        break;
                    case "help":
                        Help();
                        break;
                    case "exit":
                        endApp = true;
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
        }
        public void Help()
        {
            Console.WriteLine("Choose a command from the following list:");
            Console.WriteLine("\tcreate - create service with name <serviceName>");
            Console.WriteLine("\tdelete -delete service with ID <ID>");
            Console.WriteLine("\tsend - send message <text> from <senderId> to <receipientId>");
            Console.WriteLine("\tservices - show a list of current services");
            Console.WriteLine("\thelp - show a list of commands on the screen");
            Console.WriteLine("\texit - end program execution");
        }
        public void Create()
        {
            Console.WriteLine("service name:");
            string serviceName = Console.ReadLine();
            Service.Service service = new Service.Service(serviceName);
            int ID = serviceStorage.AddService(service);
            Console.WriteLine($"You create service {serviceName} with ID {ID} ");
        }
        public void SendMessage()
        {
            Console.WriteLine("Text:");
            string text = Console.ReadLine();
            Console.WriteLine("Sender ID:");
            int fromID = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Recipient ID:");
            int toID = Convert.ToInt32(Console.ReadLine());
            Message msg = serviceStorage.SendMessage(text, fromID, toID);
            if (msg is null)
            {
                return;
            }
            eventBus.AddMsgInBroker(msg);
        }
        public void DeleteService()
        {
            Console.WriteLine("ID:");
            int IDToDelete = Convert.ToInt32(Console.ReadLine());
            serviceStorage.Delete(IDToDelete);
        }
    }
}
