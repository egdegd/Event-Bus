using System.Net.Http;
using System.Text;
using System.Threading;
using DataStorage.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Newtonsoft.Json;

namespace EventBusServer.Test
{
    [TestClass]
    public class EventBusServerTest
    {
        [TestMethod]
        public void AddMessage()
        {
            var client = new HttpClient();
            Message msg = new Message
            {
                From = "TestServiceA",
                To = "TestServiceB",
                Text = "TestText"
            };

            var serializedObject = JsonConvert.SerializeObject(msg);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/addmsg", content).Result;
            var text = response.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(text, "\"Message added successfully\"");
        }

        [TestMethod]
        public void AddNullableMessage()
        {
            var client = new HttpClient();
            Message msg = null;
            var serializedObject = JsonConvert.SerializeObject(msg);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/addmsg", content).Result;
            var text = response.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(text, "\"Given message is invalid\"");
        }

        [TestMethod]
        public void Add1000Message()
        {
            var client = new HttpClient();
            Message msg = new Message
            {
                From = "TestServiceA",
                To = "TestServiceB",
                Text = "TestText"
            };

            var serializedObject = JsonConvert.SerializeObject(msg);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            for (int i = 0; i < 1000; i++)
            {
                var response = client.PostAsync("http://localhost:9000/api/eventbus/addmsg", content).Result;
                var text = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(text, "\"Message added successfully\"");
            }
        }

        [TestMethod]
        public void Add50000Message()
        {
            var client = new HttpClient();
            Message msg = new Message
            {
                From = "TestServiceA",
                To = "TestServiceB",
                Text = "TestText"
            };

            var serializedObject = JsonConvert.SerializeObject(msg);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            for (int i = 0; i < 5000; i++)
            {
                var response = client.PostAsync("http://localhost:9000/api/eventbus/addmsg", content).Result;
                var text = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(text, "\"Message added successfully\"");
            }
        }

        [TestMethod]
        public void SendMessage()
        {
            var client = new HttpClient();
            Message msg = new Message
            {
                From = "TestServiceA",
                To = "TestServiceB",
                Text = "TestText"
            };

            var serializedObject = JsonConvert.SerializeObject(msg);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/addmsgindb", content).Result;
            response = client.GetAsync("http://localhost:9000/api/eventbus/sendmsg?name=TestServiceB").Result;
            var result = response.Content.ReadAsStringAsync().Result;
            Message actualResultFromGet = JsonConvert.DeserializeObject<Message>(result);
            Assert.AreEqual(actualResultFromGet.Text, "TestText");
            Assert.AreEqual(actualResultFromGet.From, "TestServiceA");

        }

        [TestMethod]
        public void SendNoNewMessage()
        {
            var client = new HttpClient();
            var response = client.GetAsync("http://localhost:9000/api/eventbus/sendmsg?name=TestServiceB").Result;
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(result, "\"no new messages\"");

        }

        [TestMethod]
        public void Subscribe()
        {
            var client = new HttpClient();

            Pair p = new Pair
            {
                First = "TestServiceA",
                Second = "TestType"
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/subscribe", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(result, "\"Subscription completed successfully!\"");

        }

        [TestMethod]
        public void Unsubscribe()
        {
            var client = new HttpClient();

            Pair p = new Pair
            {
                First = "TestServiceA",
                Second = "TestType"
            };
            var serializedObject = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:9000/api/eventbus/unsubscribe", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(result, "\"Subscription deleted successfully!\"");

        }

        [TestCleanup]
        public void TestCleanup()
        {
            Thread.Sleep(1000);
            var client = new HttpClient();
            client.GetAsync("http://localhost:9000/api/eventbus/deleteTestMessages");

        }
    }
}
