using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.src.UI
{
    

    class Program
    {
        static void Main(string[] args)
        {
            EventBus eventBus = new EventBus();
            Client client = new Client();
            client.Run();
        }
    }

}