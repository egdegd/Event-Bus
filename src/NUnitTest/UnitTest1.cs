using NUnit.Framework;
using EventBus;
using Controller;
using Service;
using NUnit.Framework.Internal.Execution;
using System.Runtime.ExceptionServices;
using System.Collections.Generic;
using System.IO;
using System;

namespace NUnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCreateService()
        {
            Service.Service First = new Service.Service(0, "First");
            Assert.AreEqual(First.ID, 0);
            Assert.AreEqual(First.name, "First");

        }
        [Test]
        public void TestSendMsgInService()
        {
            Service.Service First = new Service.Service(0, "First");
            Service.Service Second = new Service.Service(0, "Second");
            Message msg = First.SendMsg("Hello", Second);
            Assert.AreEqual(msg.From, First);
            Assert.AreEqual(msg.To, Second);
            Assert.AreEqual(msg.Text, "Hello");
        }
        [Test]
        public void TestAddMsgInEventBus()
        {
            EventBus.EventBus eventBus = new EventBus.EventBus();
            Service.Service First = new Service.Service(0, "First");
            Message msg = new Message { From = First, To = First, Text = "Hi" };
            eventBus.AddMsgInBroker(msg);
            Queue<Message> messages = eventBus.GetMessages();
            Assert.AreEqual(messages.Count, 1);
            Assert.IsTrue(messages.Contains(msg));
        }

        [Test]
        public void TestSendMsgFromEventBus()
        {
            EventBus.EventBus eventBus = new EventBus.EventBus();
            Service.Service First = new Service.Service(0, "First");
            eventBus.SendMsgAsync();
            Message msg = new Message { From = First, To = First, Text = "Hi" };
            eventBus.AddMsgInBroker(msg);

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                using (var sr = new StringReader("Start"))
                {
                    Console.SetIn(sr);
                    string expected = "eventBus send message\r\n";
                    Assert.AreEqual(sw.ToString(), expected);
                }
            }
        }
    }
}