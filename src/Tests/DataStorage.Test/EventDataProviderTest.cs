using System;
using System.Collections.Generic;
using DataStorage.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace DataStorage.Test
{
    [TestClass]
    public class EventDataProviderTest
    {
        [TestMethod]
        public void AddEvent()
        {
            string id = EventDataProvider.AddEvent("TestType", "TestDescription", "TestServiceA", "TestServiceB");
            IList<EventDTO> events = EventDataProvider.GetEventById(id);
            Assert.AreEqual(events.Count, 1);
            Assert.AreEqual(events[0].Type, "TestType");
            Assert.AreEqual(events[0].Description, "TestDescription");
            Assert.AreEqual(events[0].Organizer, "TestServiceA");
            Assert.AreEqual(events[0].Subscriber, "TestServiceB");
            Assert.AreEqual(events[0].IsSent, 0);
        }

        [TestMethod]
        public void GetNewEvents()
        {
            EventDataProvider.AddEvent("TestType", "TestDescription", "TestServiceA", "TestServiceB");
            EventDataProvider.AddEvent("TestType", "TestDescription", "TestServiceC", "TestServiceB");
            EventDataProvider.AddEvent("TestType", "TestDescription", "TestServiceC", "TestServiceB");
            EventDataProvider.AddEvent("TestType", "TestDescription", "TestServiceC", "TestServiceA");
            IList<EventDTO> events = EventDataProvider.GetNewEvents("TestServiceB");
            Assert.AreEqual(events.Count, 3);
            foreach (EventDTO e in events) 
            {
                Assert.AreEqual(e.Type, "TestType");
                Assert.AreEqual(e.Description, "TestDescription");
                Assert.IsTrue((e.Organizer == "TestServiceA") || (e.Organizer == "TestServiceC"));
                Assert.AreEqual(e.Subscriber, "TestServiceB");
                Assert.AreEqual(e.IsSent, 0);
            }
            
        }

        [TestMethod]
        public void AddEvents()
        {
            string eventsToAdd = "";

            for (int i = 0; i < 10; i++)
            {
                eventsToAdd += $"('{Guid.NewGuid()}', 'TestType', 'TestDescription', 'TestServiceA', 'TestServiceB', 0), ";
            }
            eventsToAdd = eventsToAdd.Remove(eventsToAdd.Length - 2);
            EventDataProvider.AddEvents(eventsToAdd);
            IList<EventDTO> events = EventDataProvider.GetNewEvents("TestServiceB");
            Assert.AreEqual(events.Count, 10);
            foreach (EventDTO e in events)
            {
                Assert.AreEqual(e.Type, "TestType");
                Assert.AreEqual(e.Description, "TestDescription");
                Assert.AreEqual(e.Organizer, "TestServiceA");
                Assert.AreEqual(e.Subscriber, "TestServiceB");
                Assert.AreEqual(e.IsSent, 0);
            }
        }

        [TestMethod]
        public void UpdateIsSent()
        {
            string id = EventDataProvider.AddEvent("TestType", "TestDescription", "TestServiceA", "TestServiceB");
            IList<EventDTO> events = EventDataProvider.GetEventById(id);
            Assert.AreEqual(events.Count, 1);
            Assert.AreEqual(events[0].IsSent, 0);
            EventDataProvider.UpdateIsSent(id);
            IList<EventDTO> eventsNew = EventDataProvider.GetEventById(id);
            Assert.AreEqual(eventsNew.Count, 1);
            Assert.AreEqual(eventsNew[0].IsSent, 1);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            EventDataProvider.DeleteTestEvents();
        }

    }
}
