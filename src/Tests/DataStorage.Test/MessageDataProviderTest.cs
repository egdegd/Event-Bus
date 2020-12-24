using System;
using System.Collections.Generic;
using DataStorage.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace DataStorage.Test
{
    [TestClass]
    public class MessageDataProviderTest
    {
        [TestMethod]
        public void TestAddMessage()
        {
            string id = MessageDataProvider.AddMessage("TestServiceA", "TestServiceB", "TestText");
            IList<MessageDTO> msgs = MessageDataProvider.GetMessageById(id);
            Assert.AreEqual(msgs.Count, 1);
            Assert.AreEqual(msgs[0].Text, "TestText");
            Assert.AreEqual(msgs[0].From, "TestServiceA");
            Assert.AreEqual(msgs[0].To, "TestServiceB");
            Assert.AreEqual(msgs[0].IsSent, 0);
        }
        [TestMethod]
        public void GetNewMessages()
        {
            MessageDataProvider.AddMessage("TestServiceA", "TestServiceB", "TestText1");
            MessageDataProvider.AddMessage("TestServiceA", "TestServiceB", "TestText2");
            MessageDataProvider.AddMessage("TestServiceB", "TestServiceA", "TestText3");
            MessageDataProvider.AddMessage("TestServiceA", "TestServiceB", "TestText4");
            IList<MessageDTO> msgs = MessageDataProvider.GetNewMessages("TestServiceB");
            Assert.AreEqual(msgs.Count, 3);
            foreach(MessageDTO msg in msgs)
            {
                Assert.AreEqual(msg.From, "TestServiceA");
                Assert.IsTrue((msg.Text == "TestText1") || (msg.Text == "TestText2") || (msg.Text == "TestText4"));
            }

        }

        [TestMethod]
        public void AddMessages()
        {
            string msgsToAdd = "";

            for (int i = 0; i < 10; i++)
            {
                msgsToAdd += $"('{Guid.NewGuid()}', 'TestServiceA', 'TestServiceB', 'TestText', 0), ";
            }
            msgsToAdd = msgsToAdd.Remove(msgsToAdd.Length - 2);
            MessageDataProvider.AddMessages(msgsToAdd);
            IList<MessageDTO> msgs = MessageDataProvider.GetNewMessages("TestServiceB");
            Assert.AreEqual(msgs.Count, 10);
            foreach (MessageDTO msg in msgs)
            {
                Assert.AreEqual(msg.From, "TestServiceA");
                Assert.AreEqual(msg.Text, "TestText");
                Assert.AreEqual(msg.To, "TestServiceB");
                Assert.AreEqual(msg.IsSent, 0);
            }

        }

        [TestMethod]
        public void UpdateIsSent()
        {
            string id = MessageDataProvider.AddMessage("TestServiceA", "TestServiceB", "TestText");
            IList<MessageDTO> msgs = MessageDataProvider.GetMessageById(id);
            Assert.AreEqual(msgs.Count, 1);
            Assert.AreEqual(msgs[0].IsSent, 0);
            MessageDataProvider.UpdateIsSent(id);
            IList<MessageDTO> msgsNew = MessageDataProvider.GetMessageById(id);
            Assert.AreEqual(msgsNew.Count, 1);
            Assert.AreEqual(msgsNew[0].IsSent, 1);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MessageDataProvider.DeleteMessagesFor("Test", 1, 4);
        }

    }
}
