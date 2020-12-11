using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Configuration;
using System.Collections.Generic;
using Model;
using System;
using DataStorage.DataProviders;
using Log;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace WebAPI.Core.Controller
{
    [RoutePrefix("api/eventbus")]
    public class EventBusController : ApiController
    {
        public static int concurrencyLevel = Convert.ToInt32(ConfigurationManager.AppSettings["ConcurrencyLevel"]);
        public static int maxNote = Convert.ToInt32(ConfigurationManager.AppSettings["MaxNote"]);
        static readonly HashSet<string> filesForMessages = new HashSet<string>();
        static readonly HashSet<string> filesForEvents = new HashSet<string>();
        static int countOfThreads = 0;
        static readonly List<Message>[] messagesToWrite = new List<Message>[concurrencyLevel];
        static object[] msgLocks = new object[concurrencyLevel];
        static readonly List<EventToFile>[] eventsToWrite = new List<EventToFile>[concurrencyLevel];
        static object[] eventsLocks = new object[concurrencyLevel];


        public static void init()
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
                    //lock (locks[index])
                    //{
                        foreach (Message msg in messagesToWrite[index])
                        {
                            string jsonString;
                            jsonString = JsonSerializer.Serialize<Message>(msg);
                            sw.WriteLine(jsonString);
                        }
                        messagesToWrite[index].Clear();
                    //}
                    Logger.Info($"eventBus wrote last {maxNote} messages in a file: {path}");
                    Console.WriteLine($"eventBus wrote last {maxNote} messages in a file");

                }
            }
        }

        public void WriteEvents(int index)
        {
            Guid filename = Guid.NewGuid();
            string path = Environment.CurrentDirectory + @"\events\" + filename + @".txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (EventToFile evnt in eventsToWrite[index])
                    {
                        string jsonString;
                        jsonString = JsonSerializer.Serialize<EventToFile>(evnt);
                        sw.WriteLine(jsonString);
                    }
                    eventsToWrite[index].Clear();
                    Logger.Info($"eventBus wrote last {maxNote} events in a file: {path}");
                    Console.WriteLine($"eventBus wrote last {maxNote} events in a file");
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
                        Thread t = new Thread(() => SaveMessagesInDataBase(file));
                        Logger.Info($"created new thread for file: {file}");
                        t.Start();
                    }
                }
            }
            
        }

        public static void EventsWatchDog()
        {
            string path = Environment.CurrentDirectory + @"\events\";
            while (true)
            {
                Thread.Sleep(1000);
                string[] allfiles = Directory.GetFiles(path);
                foreach (string file in allfiles)
                {
                    if (!filesForEvents.Contains(file))
                    {
                        SpinWait spinWait = new SpinWait();
                        while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
                            spinWait.SpinOnce();
                        filesForEvents.Add(file);
                        Thread t = new Thread(() => SaveEventsInDataBase(file));
                        Logger.Info($"created new thread for file: {file}");
                        t.Start();
                    }
                }
            }

        }
        public static void SaveMessagesInDataBase(string filename) 
        {
            Interlocked.Increment(ref countOfThreads);
            //Random r = new Random();
            //int rInt = r.Next(1000, 10000);
            //Thread.Sleep(rInt);
            string[] lines = File.ReadAllLines(filename);
            string msgsToDB = "";
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
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
            
            Interlocked.Decrement(ref countOfThreads);
        }

        public static void SaveEventsInDataBase(string filename)
        {
            Interlocked.Increment(ref countOfThreads);
            string[] lines = File.ReadAllLines(filename);
            string eventsToDB = "";
            foreach (string s in lines)
            {
                EventToFile e = JsonSerializer.Deserialize<EventToFile>(s);
                eventsToDB += $"('{Guid.NewGuid()}', '{e.Type}', '{e.Description}', '{e.Organizer}', '{e.Subscriber}', 0), ";
            }
            try
            {
                eventsToDB = eventsToDB.Remove(eventsToDB.Length - 2);
                EventDataProvider.AddEvents(eventsToDB);
                Logger.Info($"eventBus added last {maxNote} events in data base");
                Console.WriteLine($"eventBus added last {maxNote} events in data base");
                File.Delete(filename);
                Logger.Info($"deleted file: {filename}");
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
           
            Interlocked.Decrement(ref countOfThreads);

        }

        [Route("sendmsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string name)
        {
            HttpResponseMessage result = null;
            SpinWait spinWait = new SpinWait();
            while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
                spinWait.SpinOnce();
            Thread t = new Thread(() => { result = SendMsgThread(name); });
            t.Start();
            t.Join();
            //TODO return good response
            return result;
        }

        public HttpResponseMessage SendMsgThread(string name)
        {
            Interlocked.Increment(ref countOfThreads);
            try
            {
                var messages = MessageDataProvider.GetNewMessages(name);
                if (messages.Count == 0)
                {
                    Interlocked.Decrement(ref countOfThreads);
                    //return "no new messages";
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new messages", new MediaTypeHeaderValue("text/json"));
                }

                Message msg = messages[0].ToMessage();
                Console.WriteLine($"eventBus sent message from {msg.From} to {msg.To} with text: \"{msg.Text}\"");
                var response = Request.CreateResponse<Message>(HttpStatusCode.Accepted, msg);
                MessageDataProvider.UpdateIsSent(messages[0].Id);
                Logger.Info($"Status of the message {messages[0].Id} has been updated");
                Interlocked.Decrement(ref countOfThreads);
                return response;
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                Interlocked.Decrement(ref countOfThreads);
                throw;
            }
        }

        [Route("addmsg")]
        [HttpPost]
        public HttpResponseMessage AddMsgInBroker([FromBody] Message msg)
        {
            if (msg == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given message is invalid", new MediaTypeHeaderValue("text/json"));
            SpinWait spinWait = new SpinWait();
            while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
                spinWait.SpinOnce();
            Thread t = new Thread(() => AddMsgInList(msg));
            Logger.Info($"created new thread for adding message from {msg.From} to {msg.To} with text: {msg.Text}");
            t.Start();
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

        [Route("publish")]
        [HttpPost]
        public HttpResponseMessage Publish([FromBody] Event e)
        {
            if (e == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given event is invalid", new MediaTypeHeaderValue("text/json"));
            SpinWait spinWait = new SpinWait();
            while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
                spinWait.SpinOnce();
            Thread t = new Thread(() => AddEventInList(e));
            Logger.Info($"created new thread for adding event from {e.Organizer} with description: {e.Description}");
            t.Start();
            return Request.CreateResponse(HttpStatusCode.OK, "Event added successfully!", new MediaTypeHeaderValue("text/json"));
        }

        public void AddEventInList(Event e)
        {
            try
            {
                string type = e.Type;
                var subscribers = SubscriberDataProvider.GetSubscribers(type);
                foreach (string subscriber in subscribers)
                {
                    EventToFile etf = new EventToFile
                    {
                        Type = e.Type,
                        Description = e.Description,
                        Organizer = e.Organizer,
                        Subscriber = subscriber
                    };
                    Random r = new Random();
                    int index = r.Next(concurrencyLevel);
                    eventsToWrite[index].Add(etf);
                    Logger.Info($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in preliminary list {index}");
                    Console.WriteLine($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in preliminary list {index}");
                    if (eventsToWrite[index].Count >= maxNote)
                    {
                        WriteEvents(index);
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.Error("EventBus error", exc);
                throw;
            }

        }

        [Route("subscribe")]
        [HttpPost]
        public HttpResponseMessage Subscribe([FromBody] Pair p)
        {
            try
            {
                string name = p.First;
                string type = p.Second;
                SubscriberDataProvider.AddSubscribe(name, type);
                Logger.Info($"eventBus added information about the subscription of {name} to the {type}");
                Console.WriteLine($"eventBus added information about the subscription of {name} to the {type}");
                return Request.CreateResponse(HttpStatusCode.OK, "Subscription completed successfully!", new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
           
        }

        [Route("unsubscribe")]
        [HttpPost]
        public HttpResponseMessage Unsubscribe([FromBody] Pair p)
        {
            try
            {
                string name = p.First;
                string type = p.Second;
                SubscriberDataProvider.DeleteSubscribe(name, type);
                Logger.Info($"eventBus added information about the unsubscription of {name} to the {type}");
                Console.WriteLine($"eventBus added information about the unsubscription of {name} to the {type}");
                return Request.CreateResponse(HttpStatusCode.OK, "Subscription deleted successfully!", new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
        }


        [Route("sendevent")]
        [HttpGet]
        public HttpResponseMessage SendEvent(string name)
        {
            try
            {
                var events = EventDataProvider.GetNewEvents(name);
                if (events.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new events", new MediaTypeHeaderValue("text/json"));
                }
                Event e = events[0].ToEvent();
                Console.WriteLine($"eventBus notified {name} about event from {e.Organizer} with description: \"{e.Description}\"");
                var response = Request.CreateResponse<Event>(HttpStatusCode.Accepted, e);
                EventDataProvider.UpdateIsSent(events[0].Id);
                Logger.Info($"Status of the event {events[0].Id} has been updated");
                return response;
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                throw;
            }
            
        }

        [Route("deleteTestMessages")]
        [HttpGet]
        public void DeleteTestMessages()
        {
            MessageDataProvider.DeleteTestMessages();
        }
    }
}
