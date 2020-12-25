using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Configuration;
using System.Collections.Generic;
using Model;
using System;
using DataStorage.DataProviders;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Utils;

namespace WebAPI.Core.Controller
{
    [RoutePrefix("api/eventbus")]
    public partial class EventBusController : ApiController
    {
        public static int concurrencyLevel = Convert.ToInt32(ConfigurationManager.AppSettings["ConcurrencyLevel"]);
        public static int maxNote = Convert.ToInt32(ConfigurationManager.AppSettings["MaxNote"]);
        static readonly HashSet<string> filesForMessages = new HashSet<string>();
        static int countOfThreads = 0;
        static readonly List<Message>[] messagesToWrite = new List<Message>[concurrencyLevel];
        static readonly object[] msgLocks = new object[concurrencyLevel];
        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        public static void Init()
        {
            for (int i = 0; i < concurrencyLevel; i++)
            {
                msgLocks[i] = new object();
                messagesToWrite[i] = new List<Message>();
                eventsLocks[i] = new object();
                eventsToWrite[i] = new List<EventToFile>();
            }
        }

        public void WriteMessages(int index)
        {
            Guid filename = Guid.NewGuid();
            string path = Environment.CurrentDirectory + @"\messages\" + filename + @".txt";

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (Message msg in messagesToWrite[index])
                    {
                        string jsonString;
                        jsonString = JsonSerializer.Serialize<Message>(msg);
                        sw.WriteLine(jsonString);
                    }

                    messagesToWrite[index].Clear();

                    Logger.Info($"eventBus wrote last {maxNote} messages in a file: {path}");
                    Console.WriteLine($"eventBus wrote last {maxNote} messages in a file");

                }
            }
        }

        public static void MessagesWatchDog()
        {
            string path = Environment.CurrentDirectory + @"\messages\";
            while (true)
            {
                Thread.Sleep(1000);
                string[] allfiles = Directory.GetFiles(path);
                foreach (string file in allfiles)
                {
                    if (!filesForMessages.Contains(file))
                    {
                        SpinWait spinWait = new SpinWait();
                        while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
                            spinWait.SpinOnce();

                        filesForMessages.Add(file);
                        ThreadPool.QueueUserWorkItem(state => SaveMessagesInDataBase(file));

                        Logger.Info($"created new thread for file: {file}");
                    }
                }
            }  
        }
        
        public static void SaveMessagesInDataBase(string filename) 
        {
            Interlocked.Increment(ref countOfThreads);
            //Random r = new Random();
            //int rInt = r.Next(1000, 10000);
            //Thread.Sleep(5000);
            string[] lines = File.ReadAllLines(filename);
            string msgsToDB = "";

            if (lines.Length == 0)
            {
                File.Delete(filename);
                Logger.Info($"deleted file: {filename}");
                Interlocked.Decrement(ref countOfThreads);
                return;
            }

            foreach (string s in lines)
            {
                Message msg = JsonSerializer.Deserialize<Message>(s);
                msgsToDB += $"('{Guid.NewGuid()}', '{msg.From}', '{msg.To}', '{msg.Text}', 0), ";
            }

            try
            {
                msgsToDB = msgsToDB.Remove(msgsToDB.Length - 2);
                MessageDataProvider.AddMessages(msgsToDB);

                Logger.Info($"eventBus added last {maxNote} messages in data base");
                Console.WriteLine($"eventBus added last {maxNote} messages in data base");

                File.Delete(filename);
                Logger.Info($"deleted file: {filename}");
                filesForMessages.Remove(filename);
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
            finally
            {
                Interlocked.Decrement(ref countOfThreads);
            }
        }


        [Route("sendmsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new messages", new MediaTypeHeaderValue("text/json"));
                }

                IList<MessageDTO> messages = MessageDataProvider.GetNewMessages(name);

                if (messages.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new messages", new MediaTypeHeaderValue("text/json"));
                }

                List<Message> msgsToResponse = new List<Message>();
                
                foreach (MessageDTO msg in messages)
                {
                    msgsToResponse.Add(msg.ToMessage());
                }

                foreach (Message msg in msgsToResponse)
                {
                    Console.WriteLine($"eventBus sent message from {msg.From} to {msg.To} with text: \"{msg.Text}\"");
                }

                var response = Request.CreateResponse<List<Message>>(HttpStatusCode.Accepted, msgsToResponse);
                
                foreach (MessageDTO msg in messages)
                {
                    MessageDataProvider.UpdateIsSent(msg.Id);
                    Logger.Info($"Status of the message {msg.Id} has been updated");
                }

                return response;
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
        }
        //[Route("sendmsg")]
        //[HttpGet]
        //public HttpResponseMessage SendMsg(string name)
        //{
        //    HttpResponseMessage result = null;
        //    SpinWait spinWait = new SpinWait();
        //    while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
        //        spinWait.SpinOnce();
        //    //Thread t = new Thread(() => { result = SendMsgThread(name); });
        //    ThreadPool.QueueUserWorkItem(state => { result = SendMsgThread(name); });
        //    autoEvent.WaitOne();
        //    //t.Start();
        //    //t.Join();
        //    //TODO return good response
        //    return result;
        //}

        //public HttpResponseMessage SendMsgThread(string name)
        //{
        //    Interlocked.Increment(ref countOfThreads);
        //    try
        //    {
        //        var messages = MessageDataProvider.GetNewMessages(name);
        //        if (messages.Count == 0)
        //        {
        //            Interlocked.Decrement(ref countOfThreads);
        //            //return "no new messages";
        //            return Request.CreateResponse(HttpStatusCode.NotFound,
        //                "no new messages", new MediaTypeHeaderValue("text/json"));
        //        }
        //        Message msg = messages[0].ToMessage();
        //        Console.WriteLine($"eventBus sent message from {msg.From} to {msg.To} with text: \"{msg.Text}\"");
        //        var response = Request.CreateResponse<Message>(HttpStatusCode.Accepted, msg);
        //        MessageDataProvider.UpdateIsSent(messages[0].Id);
        //        Logger.Info($"Status of the message {messages[0].Id} has been updated");
        //        Interlocked.Decrement(ref countOfThreads);
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error("EventBus error", e);
        //        Interlocked.Decrement(ref countOfThreads);
        //        throw;
        //    }
        //}

        [Route("addmsg")]
        [HttpPost]
        public HttpResponseMessage AddMsgInBroker([FromBody] Message msg)
        {
            if (msg == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given message is invalid", new MediaTypeHeaderValue("text/json"));
            }

            SpinWait spinWait = new SpinWait();
            while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
                spinWait.SpinOnce();

            ThreadPool.QueueUserWorkItem(state => AddMsgInList(msg));

            Logger.Info($"created new thread for adding message from {msg.From} to {msg.To} with text: {msg.Text}");
            return Request.CreateResponse(HttpStatusCode.OK, "Message added successfully", new MediaTypeHeaderValue("text/json"));
        }

        [Route("addmsgindb")]
        [HttpPost]
        public HttpResponseMessage AddMsgInDB([FromBody] Message msg)
        {
            if (msg == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given message is invalid", new MediaTypeHeaderValue("text/json"));
            }

            Console.WriteLine($"add msg from {msg.From} to {msg.To} with text {msg.Text} in data base");
            MessageDataProvider.AddMessage(msg.From, msg.To, msg.Text);
            return Request.CreateResponse(HttpStatusCode.OK, "Message added successfully", new MediaTypeHeaderValue("text/json"));
        }

        public void AddMsgInList(Message msg)
        {
            try
            {
                Interlocked.Increment(ref countOfThreads);
                Random r = new Random();
                int index = r.Next(concurrencyLevel);

                lock (msgLocks[index])
                {
                    messagesToWrite[index].Add(msg);
                    
                    Logger.Info($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in preliminary list {index}");
                    Console.WriteLine($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in preliminary list {index}");
                    
                    if (messagesToWrite[index].Count >= maxNote)
                    {
                        WriteMessages(index);
                    }
                }
                Interlocked.Decrement(ref countOfThreads);
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
            
        }
    }
}
