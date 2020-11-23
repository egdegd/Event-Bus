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
    }
}
