using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DataStorage.Mappers;
using Model;
using SqlHelper;

namespace DataStorage.DataProviders
{
    public class MessageDataProvider
    {
        public static IList<MessageDTO> GetAllMessages()
        {
            string sqlQuery = XmlStrings.GetString(Tables.Messages, "GetAllMessages");

            var result = DBHelper.GetData(
                new MessageDTOMapper(),
                sqlQuery);
            return result;
        }
        public static IList<MessageDTO> GetMessageById(string id)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Messages, "GetMessageById");
            SqlParameter param = new SqlParameter("@messageId", id);
            var result = DBHelper.GetData(
                new MessageDTOMapper(),
                sqlQuery, param);
            return result;
        }
        public static IList<MessageDTO> GetNewMessages(string recipient)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Messages, "GetNewMessages");
            SqlParameter param = new SqlParameter("@recipient", recipient);
            var result = DBHelper.GetData(
                new MessageDTOMapper(),
                sqlQuery, param);
            return result;
        }

        public static string AddMessage(string from, string to, string text)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Messages, "AddMessage");
            string id = Guid.NewGuid().ToString();
            SqlParameter param1 = new SqlParameter("@id", id);
            SqlParameter param2 = new SqlParameter("@from", from);
            SqlParameter param3 = new SqlParameter("@to", to);
            SqlParameter param4 = new SqlParameter("@text", text);
            var result = DBHelper.GetData(
                new MessageDTOMapper(),
                sqlQuery, param1, param2, param3, param4);
            return id;
        }
        public static IList<MessageDTO> AddMessages(string messages)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Messages, "AddMessages");
            sqlQuery += messages;
            var result = DBHelper.GetData(
                new MessageDTOMapper(),
                sqlQuery);
            return result;
        }
        public static IList<MessageDTO> UpdateIsSent(string id)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Messages, "UpdateIsSent");
            SqlParameter param = new SqlParameter("@id", id);
            var result = DBHelper.GetData(
                new MessageDTOMapper(),
                sqlQuery, param);
            return result;
        }
        public static void DeleteMessagesFor(string recipient, int start, int finish)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Messages, "DeleteMessagesFor");
            SqlParameter param1 = new SqlParameter("@recipient", recipient);
            SqlParameter param2 = new SqlParameter("@start", start);
            SqlParameter param3 = new SqlParameter("@finish", finish);
            DBHelper.GetData(
            new MessageDTOMapper(),
            sqlQuery, param1, param2, param3);
        }
    }
}
