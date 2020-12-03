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
            string id = MessageDataProvider.AddMessage("TestServiceA", "TestServiceB", "Hello");
            IList<MessageDTO> msgs = MessageDataProvider.GetMessageById(id);
            Assert.IsTrue(msgs.Count == 0);
            Assert.AreEqual(msgs[0].Text, "Hello");
            Assert.AreEqual(msgs[0].From, "TestServiceA");
            Assert.AreEqual(msgs[0].To, "TestServiceB");
        }
        [TestMethod]
        public void Test()
        {
            Assert.IsTrue(true);

        }

    }
}
