using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Model;
using System;
using System.Text;
using Newtonsoft.Json;
using WebServiceClient;
using System.Collections.Generic;

namespace WebAPI.Core.Controller
{
    [RoutePrefix("api/serviceD")]
    public class ServicDAController : ApiController
    {
        static readonly EventBusClient client = new EventBusClient();

        [Route("sendMsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string recipient, string text)
        {
            client.AddMessage("serviceD", recipient, text);
            Console.WriteLine($"serviceD sent message to {recipient} with text: \"{text}\"");
            return Request.CreateResponse(HttpStatusCode.OK, "Message is sent", new MediaTypeHeaderValue("text/json"));
        }

        [Route("requestMsg")]
        [HttpGet]
        public HttpResponseMessage RequestMsg()
        {
            var result = client.SendMessage("serviceD");
            if (result != "\"no new messages\"")
            {
                List<Message> actualResultFromGet = JsonConvert.DeserializeObject<List<Message>>(result);
                foreach (Message msg in actualResultFromGet)
                {
                    Console.WriteLine($"serviceD received message \"{msg.Text}\" from {msg.From}");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "OK!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("requestEvent")]
        [HttpGet]
        public HttpResponseMessage RequestEvent()
        {
            var result = client.SendEvent("serviceD");
            if (result != "\"no new events\"")
            {
                List<Event> actualResultFromGet = JsonConvert.DeserializeObject<List<Event>>(result);
                foreach (Event e in actualResultFromGet)
                {
                    Console.WriteLine($"serviceD received an event notification from {e.Organizer} with description \"{e.Description}\"");

                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "OK!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("publishevent")]
        [HttpGet]
        public HttpResponseMessage Publish(string type, string description)
        {
            client.Publish("serviceD", type, description);
            Console.WriteLine($"serviceD published event with description: \"{description}\"");
            return Request.CreateResponse(HttpStatusCode.OK, "Event is published!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("subscribe")]
        [HttpGet]
        public HttpResponseMessage Subscribe(string type)
        {
            client.Subscribe("serviceD", type);
            Console.WriteLine($"serviceD subscribed to the event {type}");
            return Request.CreateResponse(HttpStatusCode.OK, "Subscribed", new MediaTypeHeaderValue("text/json"));
        }

        [Route("unsubscribe")]
        [HttpGet]
        public HttpResponseMessage Unubscribe(string type)
        {
            client.Unsubscribe("serviceD", type);
            Console.WriteLine($"serviceD unsubscribed from the event {type}");
            return Request.CreateResponse(HttpStatusCode.OK, "Unsubscribed", new MediaTypeHeaderValue("text/json"));
        }
    }
}
