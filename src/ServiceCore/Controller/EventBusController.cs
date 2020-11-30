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

namespace WebAPI.Core.Controller
{
    [RoutePrefix("api/eventbus")]
    public class EventBusController : ApiController
    {
        static List<Message> messagesToWrite = new List<Message>();
        public int maxMessages = Convert.ToInt32(ConfigurationManager.AppSettings["MaxMessages"]);
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
                }
            }
        }

        [Route("sendmsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string name)
        {
            try
            {
                var messages = MessageDataProvider.GetNewMessages(name);
                if (messages.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                        "no new messages", new MediaTypeHeaderValue("text/json"));
                }

                Message msg = messages[0].ToMessage();
                Console.WriteLine($"eventBus sent message from {msg.From} to {msg.To} with text: \"{msg.Text}\"");
                var response = Request.CreateResponse<Message>(HttpStatusCode.Accepted, msg);
                MessageDataProvider.UpdateIsSent(messages[0].Id);
                Logger.Info($"Status of the message {messages[0].Id} has been updated");
                return response;
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
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
                if (messagesToWrite.Count >= maxMessages)
                {
                    WriteMessages();
                }
                MessageDataProvider.AddMessage(msg.From, msg.To, msg.Text);
                Logger.Info($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in data base");
                Console.WriteLine($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in broker");

                return Request.CreateResponse(HttpStatusCode.OK, "Message added successfully", new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception e)
            {
                Logger.Error("EventBus error", e);
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
                    EventDataProvider.AddEvent(type, e.Description, e.Organizer, subscriber);
                }
                Logger.Info($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in broker");
                Console.WriteLine($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in broker");
                return Request.CreateResponse(HttpStatusCode.OK, "Event added successfully!", new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception exc)
            {
                Logger.Error("EventBus error", exc);
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
