using System;
//using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service;

namespace EventBus
{
    public class EventBus
    {
        Queue<Message> messages = new Queue<Message>();
        public void AddMsgInBroker(Message msg)

        {
            messages.Enqueue(msg);
            //string filePath = @"A:\Repos\EventBus\EventBus\src\WriteLines.txt";
            string dirPath = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.Parent.Parent.FullName;
            string filePath = System.IO.Path.Combine(dirPath, @"src\WriteLines.txt");

            //using StreamWriter file = new StreamWriter(filePath, true);
            //string msgJson = JsonSerializer.Serialize<Message>(msg);
            //file.WriteLine(msgJson);
        }
        public void SendMsg()
        {
            while (true)
            {
                if (messages.Count != 0)
                {
                    Message msg = messages.Dequeue();
                    Console.WriteLine($"eventBus send message");
                    msg.To.ReceiveMsg(msg);
                }
            }
        }
        public async void SendMsgAsync()
        {
            await Task.Run(() => SendMsg());
        }
        public Queue<Message> GetMessages()
        {
            return messages;
        }
    }
}
