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

        [TestCleanup]
        public void TestCleanup()
        {
            Thread.Sleep(1000);
            var client = new HttpClient();
            client.GetAsync("http://localhost:9000/api/eventbus/deleteTestMessages");

        }
    }
}
