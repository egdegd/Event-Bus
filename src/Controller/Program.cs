using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class Program
    {
        static void Main(string[] args)
        {
            EventBus.EventBus eventBus = new EventBus.EventBus();
            Client client = new Client();
            client.Run();
        }
    }
}
