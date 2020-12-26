using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading;
using DataStorage.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Newtonsoft.Json;
using WebServiceClient;

namespace EventBusServer.Test
{
    [TestClass]
    public class EventBusServerTest
    {
        static readonly EventBusClient client = new EventBusClient();

        [TestMethod]
        public void AddMessage()
        {
            HttpResponseMessage response = client.AddMessage("TestServiceA", "TestServiceB", "TestText");
            string text = response.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(text, "\"Message added successfully\"");
        }

        [TestMethod]
        public void AddNullableMessage()
        {
            var client = new HttpClient();
            Message msg = null;
            var serializedObject = JsonConvert.SerializeObject(msg);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            string localhost = ConfigurationManager.AppSettings["EventBusHost"];
            var response = client.PostAsync($"{localhost}/addmsg", content).Result;
            var text = response.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(text, "\"Given message is invalid\"");
        }

        [TestMethod]
        public void Add1000Message()
        {
            for (int i = 0; i < 1000; i++)
            {
                HttpResponseMessage response = client.AddMessage("TestServiceA", "TestServiceB", "TestText");
                var text = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(text, "\"Message added successfully\"");
            }
        }

        [TestMethod]
        public void Add50000Message()
        {
            for (int i = 0; i < 5000; i++)
            {
                HttpResponseMessage response = client.AddMessage("TestServiceA", "TestServiceB", "TestText");
                var text = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(text, "\"Message added successfully\"");
            }
        }

        [TestMethod]
        public void SendMessage()
        {
            client.AddMessageInDB("TestServiceA", "TestServiceB", "TestText");
            string result = client.SendMessage("TestServiceB");
            List<Message> actualResultFromGet = JsonConvert.DeserializeObject<List<Message>>(result);
            Assert.AreEqual(actualResultFromGet[0].Text, "TestText");
            Assert.AreEqual(actualResultFromGet[0].From, "TestServiceA");

        }

        [TestMethod]
        public void SendNoNewMessage()
        {
            string result = client.SendMessage("TestServiceB");
            Assert.AreEqual(result, "\"no new messages\"");

        }

        [TestMethod]
        public void Subscribe()
        {
            string result = client.Subscribe("TestServiceA", "TestType");
            Assert.AreEqual(result, "\"Subscription completed successfully!\"");
        }

        [TestMethod]
        public void Unsubscribe()
        {
            string result = client.Unsubscribe("TestServiceA", "TestType");
            Assert.AreEqual(result, "\"Subscription deleted successfully!\"");

        }

        [TestCleanup]
        public void TestCleanup()
        {
            Thread.Sleep(1000);
            MessageDataProvider.DeleteMessagesFor("Test", 1, 4);
            SubscriberDataProvider.DeleteSubscribers("Test", 1, 4);
            EventDataProvider.DeleteEventsFor("Test", 1, 4);
        }
    }
}
