using System;
using System.Collections.Generic;
using DataStorage.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace DataStorage.Test
{
    [TestClass]
    public class SubscriberDataProviderTest
    {
        [TestMethod]
        public void AddSubscribe1()
        {
            SubscriberDataProvider.AddSubscribe("TestServiceA", "TestType1");
            SubscriberDataProvider.AddSubscribe("TestServiceB", "TestType1");
            IList<String> subscribers1 = SubscriberDataProvider.GetSubscribers("TestType1");
            Assert.AreEqual(subscribers1.Count, 2);
            foreach (string subscriber in subscribers1)
            {
                Assert.IsTrue((subscriber == "TestServiceA") || (subscriber == "TestServiceB"));
            }
        }

        [TestMethod]
        public void AddSubscribe2()
        {
            SubscriberDataProvider.AddSubscribe("TestServiceA", "TestType1");
            SubscriberDataProvider.AddSubscribe("TestServiceB", "TestType1");
            SubscriberDataProvider.AddSubscribe("TestServiceC", "TestType1");
            SubscriberDataProvider.AddSubscribe("TestServiceA", "TestType2");
            SubscriberDataProvider.AddSubscribe("TestServiceB", "TestType2");
            IList<String> subscribers1 = SubscriberDataProvider.GetSubscribers("TestType1");
            Assert.AreEqual(subscribers1.Count, 3);
            foreach (string subscriber in subscribers1)
            {
                Assert.IsTrue((subscriber == "TestServiceA") || (subscriber == "TestServiceB") || (subscriber == "TestServiceC"));
            }
            IList<String> subscribers2 = SubscriberDataProvider.GetSubscribers("TestType2");
            Assert.AreEqual(subscribers2.Count, 2);
            foreach (string subscriber in subscribers2)
            {
                Assert.IsTrue((subscriber == "TestServiceA") || (subscriber == "TestServiceB"));
            }
        }

        [TestMethod]
        public void DeleteSubscribe()
        {
            SubscriberDataProvider.AddSubscribe("TestServiceA", "TestType");
            SubscriberDataProvider.AddSubscribe("TestServiceA", "TestType");
            IList<String> subscribers = SubscriberDataProvider.GetSubscribers("TestType");
            Assert.AreEqual(subscribers.Count, 1);
            Assert.AreEqual(subscribers[0], "TestServiceA");
            SubscriberDataProvider.DeleteSubscribe("TestServiceA", "TestType");
            IList<String> subscribersNew = SubscriberDataProvider.GetSubscribers("TestType");
            Assert.AreEqual(subscribersNew.Count, 0);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            SubscriberDataProvider.DeleteTestSubscribers();
        }

    }
}
