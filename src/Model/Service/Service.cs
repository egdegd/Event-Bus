using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Service
    {
        public int ID { get; set; }
        public string name { get; set; }

        public Service(int Id, string nm)
        {
            name = nm;
            ID = Id;
        }
        public Service(string nm)
        {
            name = nm;
            ID = -1;
        }
        public void ReceiveMsg(Message msg)
        {
            Console.WriteLine($"My name is {name} and I receive message \"{msg.Text}\" from {msg.From.name}");
        }
        public Message SendMsg(string text, Service recipient)
        {
            Message msg = new Message { From = this, To = recipient, Text = text };
            return msg;
        }
    }
}
