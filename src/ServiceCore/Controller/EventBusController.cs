using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using System.Collections.Generic;
using Model;
using System;

namespace WebAPI.Core.Controller
{
    [RoutePrefix("api/eventbus")]
    public class EventBusController : ApiController
    {
        static Dictionary<string, Queue<Message>> messages = new Dictionary<string, Queue<Message>>();
        static Dictionary<string, Queue<Event>> events = new Dictionary<string, Queue<Event>>();
        static Dictionary<string, List<string>> subscribers = new Dictionary<string, List<string>>();

        [Route("sendmsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string name)
        {
            if ((!messages.ContainsKey(name)) || (messages[name].Count == 0))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    "no new messages", new MediaTypeHeaderValue("text/json"));
            }
            
            Message msg = messages[name].Dequeue();
            Console.WriteLine($"eventBus sent message from {msg.From} to {msg.To} with text: \"{msg.Text}\"");
            var response = Request.CreateResponse<Message>(HttpStatusCode.Accepted, msg);
            return response;
        }

        [Route("addmsg")]
        [HttpPost]
        public HttpResponseMessage AddMsgInBroker([FromBody] Message msg)
        {
            if (msg == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given message is invalid", new MediaTypeHeaderValue("text/json"));
            if (!messages.ContainsKey(msg.To)) {
                messages[msg.To] = new Queue<Message>();
            }
            messages[msg.To].Enqueue(msg);
            Console.WriteLine($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in broker");

            return Request.CreateResponse(HttpStatusCode.OK, "Message added successfully", new MediaTypeHeaderValue("text/json"));
        }

        [Route("subscribe")]
        [HttpPost]
        public HttpResponseMessage Subscribe([FromBody] Pair p)
        {
            string name = p.First;
            string type = p.Second;
            if (!subscribers.ContainsKey(type))
            {
                subscribers[type] = new List<string>();
            }
            subscribers[type].Add(name);
            return Request.CreateResponse(HttpStatusCode.OK, "Subscription completed successfully!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("publish")]
        [HttpPost]
        public HttpResponseMessage Publish([FromBody] Event e)
        {
            if (e == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given event is invalid", new MediaTypeHeaderValue("text/json"));
            string type = e.Type;
            if (subscribers.ContainsKey(type))
            {
                foreach(string service in subscribers[type])
                {
                    if (!events.ContainsKey(service))
                    {
                        events[service] = new Queue<Event>();
                    }
                    events[service].Enqueue(e);
                }
            }
            Console.WriteLine($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in broker");
            return Request.CreateResponse(HttpStatusCode.OK, "Event added successfully!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("sendevent")]
        [HttpGet]
        public HttpResponseMessage SendEvent(string name)
        {
            if ((!events.ContainsKey(name)) || (events[name].Count == 0))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,
                    "no new events", new MediaTypeHeaderValue("text/json"));
            }

            Event e = events[name].Dequeue();
            Console.WriteLine($"eventBus notified {name} about event from {e.Organizer} with description: \"{e.Description}\"");
            var response = Request.CreateResponse<Event>(HttpStatusCode.Accepted, e);
            return response;
        }
    }
}
