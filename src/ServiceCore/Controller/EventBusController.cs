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
        static readonly List<Message> messagesToWrite = new List<Message>();
        static readonly List<EventToFile> eventsToWrite = new List<EventToFile>();
        public static int concurrencyLevel = Convert.ToInt32(ConfigurationManager.AppSettings["ConcurrencyLevel"]);
        public static int maxNote = Convert.ToInt32(ConfigurationManager.AppSettings["MaxNote"]);
        static readonly HashSet<string> filesForMessages = new HashSet<string>();
        static readonly HashSet<string> filesForEvents = new HashSet<string>();
        static int countOfThreads = 0;
        static readonly object locker = new object();
        public void WriteMessages()
        {
            Guid filename = Guid.NewGuid();
            string path = Environment.CurrentDirectory + @"\messages\" + filename + @".txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (Message msg in messagesToWrite)
                    {
                        string jsonString;
                        jsonString = JsonSerializer.Serialize<Message>(msg);
                        sw.WriteLine(jsonString);
                    }
                    messagesToWrite.Clear();
                    Logger.Info($"eventBus wrote last {maxNote} messages in a file: {path}");
                    Console.WriteLine($"eventBus wrote last {maxNote} messages in a file");

                }
            }
        }

        public void WriteEvents()
        {
            Guid filename = Guid.NewGuid();
            string path = Environment.CurrentDirectory + @"\events\" + filename + @".txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (EventToFile evnt in eventsToWrite)
                    {
                        string jsonString;
                        jsonString = JsonSerializer.Serialize<EventToFile>(evnt);
                        sw.WriteLine(jsonString);
                    }
                    eventsToWrite.Clear();
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
                        while (true)
                        {
                            if (countOfThreads < concurrencyLevel)
                            {
                                break;
                            } 
                        }
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
                        while (true)
                        {
                            if (countOfThreads < concurrencyLevel)
                            {
                                break;
                            }
                        }
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
            lock (locker)
            {
                countOfThreads += 1;
            }
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
            msgsToDB = msgsToDB.Remove(msgsToDB.Length - 2);
            MessageDataProvider.AddMessages(msgsToDB);
            Logger.Info($"eventBus added last {maxNote} messages in data base");
            Console.WriteLine($"eventBus added last {maxNote} messages in data base");
            File.Delete(filename);
            Logger.Info($"deleted file: {filename}");
            lock (locker)
            {
                countOfThreads -= 1;
            }
        }

        public static void SaveEventsInDataBase(string filename)
        {
            lock (locker)
            {
                countOfThreads += 1;
            }
            string[] lines = File.ReadAllLines(filename);
            string eventsToDB = "";
            foreach (string s in lines)
            {
                Console.WriteLine(s);
                EventToFile e = JsonSerializer.Deserialize<EventToFile>(s);
                eventsToDB += $"('{Guid.NewGuid()}', '{e.Type}', '{e.Description}', '{e.Organizer}', '{e.Subscriber}', 0), ";
            }
            eventsToDB = eventsToDB.Remove(eventsToDB.Length - 2);
            EventDataProvider.AddEvents(eventsToDB);
            Logger.Info($"eventBus added last {maxNote} events in data base");
            Console.WriteLine($"eventBus added last {maxNote} events in data base");
            File.Delete(filename);
            Logger.Info($"deleted file: {filename}");
            lock (locker)
            {
                countOfThreads -= 1;
            }
        }

        [Route("sendmsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string name)
        {
            HttpResponseMessage result = null;
            while (true)
            {
                if (countOfThreads < concurrencyLevel)
                {
                    break;
                }
            }
            Thread t = new Thread(() => { result = SendMsgThread(name); });
            t.Start();
            t.Join();
            //TODO return good response
            return result;
        }

        public HttpResponseMessage SendMsgThread(string name)
        {
            Thread.Sleep(10000);
            lock (locker)
            {
                countOfThreads += 1;
            }
            try
            {
                var messages = MessageDataProvider.GetNewMessages(name);
                if (messages.Count == 0)
                {
                    lock (locker)
                    {
                        countOfThreads -= 1;
                    }
                    //return "no new messages";
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new messages", new MediaTypeHeaderValue("text/json"));
                }

                Message msg = messages[0].ToMessage();
                Console.WriteLine($"eventBus sent message from {msg.From} to {msg.To} with text: \"{msg.Text}\"");
                var response = Request.CreateResponse<Message>(HttpStatusCode.Accepted, msg);
                MessageDataProvider.UpdateIsSent(messages[0].Id);
                Logger.Info($"Status of the message {messages[0].Id} has been updated");
                lock (locker)
                {
                    countOfThreads -= 1;
                }
                //return "OK";
                return response;
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
                lock (locker)
                {
                    countOfThreads -= 1;
                }
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
            try
            {
                messagesToWrite.Add(msg);
                Logger.Info($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in preliminary list");
                Console.WriteLine($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in preliminary list");
                if (messagesToWrite.Count >= maxNote)
                {
                    WriteMessages();
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Message added successfully", new MediaTypeHeaderValue("text/json"));
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

                    eventsToWrite.Add(etf);
                    Logger.Info($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in preliminary list");
                    Console.WriteLine($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in preliminary list");
                    if (eventsToWrite.Count >= maxNote)
                    {
                        WriteEvents();
                    }

                }
                return Request.CreateResponse(HttpStatusCode.OK, "Event added successfully!", new MediaTypeHeaderValue("text/json"));
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
    }
}
