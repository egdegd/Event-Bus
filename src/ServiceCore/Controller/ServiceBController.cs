using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Model;
using System;
using System.Text;
using Newtonsoft.Json;


namespace WebAPI.Core.Controller
{
    [RoutePrefix("api/serviceB")]
    public class ServiceBController : ApiController
    {
        [Route("sendMsg")]
        [HttpGet]
        public HttpResponseMessage SendMsg(string recipient, string text)
        {
            var client = new HttpClient();
            var msg = new Message
            {
                From = "serviceB",
                To = recipient,
                Text = text
            };

            var serializedObject = JsonConvert.SerializeObject(msg);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/addmsg", content).Result;
            Console.WriteLine($"serviceB sent message to {recipient} with text: \"{text}\"");
            return Request.CreateResponse(HttpStatusCode.OK, "Message is sent", new MediaTypeHeaderValue("text/json"));
        }

        [Route("requestMsg")]
        [HttpGet]
        public HttpResponseMessage RequestMsg()
        {
            var client = new HttpClient();
            var response = client.GetAsync("http://localhost:9000/api/eventbus/sendmsg?name=serviceB").Result;
            var result = response.Content.ReadAsStringAsync().Result;
            if (result != "\"no new messages\"")
            {
                Message actualResultFromGet = JsonConvert.DeserializeObject<Message>(result);
                Console.WriteLine($"serviceB received message \"{actualResultFromGet.Text}\" from {actualResultFromGet.From}");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "OK!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("requestEvent")]
        [HttpGet]
        public HttpResponseMessage RequestEvent()
        {
            var client = new HttpClient();
            var response = client.GetAsync("http://localhost:9000/api/eventbus/sendevent?name=serviceB").Result;
            var result = response.Content.ReadAsStringAsync().Result;
            if (result != "\"no new events\"")
            {
                Event actualResultFromGet = JsonConvert.DeserializeObject<Event>(result);
                Console.WriteLine($"serviceB received an event notification from {actualResultFromGet.Organizer} with description \"{actualResultFromGet.Description}\"");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "OK!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("publishevent")]
        [HttpGet]
        public HttpResponseMessage Publish(string type, string description)
        {
            var client = new HttpClient();
            Event e = new Event
            {
                Type = type,
                Description = description,
                Organizer = "serviceB"
            };

            var serializedObject = JsonConvert.SerializeObject(e);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/publish", content).Result;
            Console.WriteLine($"serviceB published event with description: \"{description}\"");
            return Request.CreateResponse(HttpStatusCode.OK, "Event is published!", new MediaTypeHeaderValue("text/json"));
        }

        [Route("subscribe")]
        [HttpGet]
        public HttpResponseMessage Subscribe(string type)
        {
            var client = new HttpClient();

            Pair p = new Pair
            {
                First = "serviceB",
                Second = type
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/subscribe", content).Result;
            Console.WriteLine($"serviceB subscribed to the event {type}");
            return Request.CreateResponse(HttpStatusCode.OK, "Subscribed", new MediaTypeHeaderValue("text/json"));
        }

        [Route("unsubscribe")]
        [HttpGet]
        public HttpResponseMessage Unubscribe(string type)
        {
            var client = new HttpClient();

            Pair p = new Pair
            {
                First = "serviceB",
                Second = type
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/unsubscribe", content).Result;
            Console.WriteLine($"serviceB unsubscribed from the event {type}");
            return Request.CreateResponse(HttpStatusCode.OK, "Unsubscribed", new MediaTypeHeaderValue("text/json"));
        }
    }
}
