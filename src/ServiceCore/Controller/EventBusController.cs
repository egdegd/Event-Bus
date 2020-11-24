using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using System.Collections.Generic;
using Model;
using System;
using DataStorage.DataProviders;

namespace WebAPI.Core.Controller
{
    [RoutePrefix("api/eventbus")]
    public class EventBusController : ApiController
    {

        [Route("sendmsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string name)
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
            return response;
        }

        [Route("addmsg")]
        [HttpPost]
        public HttpResponseMessage AddMsgInBroker([FromBody] Message msg)
        {
            if (msg == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given message is invalid", new MediaTypeHeaderValue("text/json"));

            MessageDataProvider.AddMessage(msg.From, msg.To, msg.Text);
            Console.WriteLine($"eventBus added message from {msg.From} to {msg.To} with text: \"{msg.Text}\" in broker");

            return Request.CreateResponse(HttpStatusCode.OK, "Message added successfully", new MediaTypeHeaderValue("text/json"));
        }

        [Route("subscribe")]
        [HttpPost]
        public HttpResponseMessage Subscribe([FromBody] Pair p)
        {
            string name = p.First;
            string type = p.Second;
            SubscriberDataProvider.AddSubscribe(name, type);
            return Request.CreateResponse(HttpStatusCode.OK, "Subscription completed successfully!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("unsubscribe")]
        [HttpPost]
        public HttpResponseMessage Unsubscribe([FromBody] Pair p)
        {
            string name = p.First;
            string type = p.Second;
            SubscriberDataProvider.DeleteSubscribe(name, type);
            return Request.CreateResponse(HttpStatusCode.OK, "Subscription deleted successfully!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("publish")]
        [HttpPost]
        public HttpResponseMessage Publish([FromBody] Event e)
        {
            if (e == null)
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    $"Given event is invalid", new MediaTypeHeaderValue("text/json"));
            string type = e.Type;

            var subscribers = SubscriberDataProvider.GetSubscribers(type);
            foreach (string subscriber in subscribers)
            {
                EventDataProvider.AddEvent(type, e.Description, e.Organizer, subscriber);
            }
            Console.WriteLine($"eventBus added event from {e.Organizer} with description: \"{e.Description}\" in broker");
            return Request.CreateResponse(HttpStatusCode.OK, "Event added successfully!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("sendevent")]
        [HttpGet]
        public HttpResponseMessage SendEvent(string name)
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
            return response;
        }
    }
}
