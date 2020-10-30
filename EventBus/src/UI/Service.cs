using System;

namespace EventBus.src.UI
{
    class Service
    {
        public int ID { get; set; }
        public string name { get; set; }

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
