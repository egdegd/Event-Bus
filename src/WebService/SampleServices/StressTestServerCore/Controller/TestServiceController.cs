using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Model;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Threading;

namespace WebAPI.Core.Controller
{

    [RoutePrefix("api/testService")]
    public class TestServiceController : ApiController
    {
        [Route("sendMsg")]
        [HttpGet]

        public void SendMsg(int messagesPerSecond, int countOfSeconds)
        {
            
            Console.WriteLine(messagesPerSecond);
            Console.WriteLine(countOfSeconds);
            for (int j = 0; j < countOfSeconds; j++)
            {
                for (int i = 0; i < messagesPerSecond; i++)
                {
                    Thread.Sleep(1000 / messagesPerSecond);
                    var client = new HttpClient();
                    var msg = new Message
                    {
                        From = "TestServiceA",
                        To = "TestServiceB",
                        Text = "TestMsg"
                    };

                    var serializedObject = JsonConvert.SerializeObject(msg);
                    var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                    var response = client.PostAsync("http://localhost:9000/api/eventbus/addmsg", content).Result;
                    var text = response.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}
