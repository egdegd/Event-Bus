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
    public partial class EventBusController : ApiController
    {
        static readonly HashSet<string> filesForEvents = new HashSet<string>();
        static readonly List<EventToFile>[] eventsToWrite = new List<EventToFile>[concurrencyLevel];
        static readonly object[] eventsLocks = new object[concurrencyLevel];

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
                        ThreadPool.QueueUserWorkItem(state => SaveEventsInDataBase(file));

                        Logger.Info($"created new thread for file: {file}");
                    }
                }
            }

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
                filesForEvents.Remove(filename);
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

        [Route("publish")]
        [HttpPost]
        public HttpResponseMessage Publish([FromBody] Event e)
        {
            if (e == null)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given event is invalid", new MediaTypeHeaderValue("text/json"));
            }

            SpinWait spinWait = new SpinWait();
            while (Interlocked.CompareExchange(ref countOfThreads, concurrencyLevel, concurrencyLevel) >= concurrencyLevel)
                spinWait.SpinOnce();

            ThreadPool.QueueUserWorkItem(state => AddEventInList(e));
            Logger.Info($"created new thread for adding event from {e.Organizer} with description: {e.Description}");
            
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
        public HttpResponseMessage SendEvent(string name)   //TODO: add threading
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new events", new MediaTypeHeaderValue("text/json"));
                }
                
                var events = EventDataProvider.GetNewEvents(name);
                if (events.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new events", new MediaTypeHeaderValue("text/json"));
                }
                
                List<Event> eventsToResponse = new List<Event>();
                
                foreach (EventDTO e in events)
                {
                    eventsToResponse.Add(e.ToEvent());
                }

                foreach (Event e in eventsToResponse)
                {
                    Console.WriteLine($"eventBus notified {name} about event from {e.Organizer} with description: \"{e.Description}\"");
                }

                var response = Request.CreateResponse<List<Event>>(HttpStatusCode.Accepted, eventsToResponse);

                foreach (EventDTO e in events)
                {
                    EventDataProvider.UpdateIsSent(e.Id);
                    Logger.Info($"Status of the event {e.Id} has been updated");
                }

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
