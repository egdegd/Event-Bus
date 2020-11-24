using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DataStorage.Mappers;
using Model;
using SqlHelper;

namespace DataStorage.DataProviders
{
    public class EventDataProvider
    {
        public static IList<EventDTO> GetAllEvents()
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "GetAllEvents");

            var result = DBHelper.GetData(
                new EventDTOMapper(),
                sqlQuery);
            return result;
        }
        public static IList<EventDTO> GetNewEvents(string subscriber)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "GetNewEvents");
            SqlParameter param = new SqlParameter("@subscriber", subscriber);
            var result = DBHelper.GetData(
                new EventDTOMapper(),
                sqlQuery, param);
            return result;
        }

        public static IList<EventDTO> AddEvent(string type, string description, string organizer, string subscriber)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "AddEvent");
            SqlParameter param1 = new SqlParameter("@id", Guid.NewGuid());
            SqlParameter param2 = new SqlParameter("@type", type);
            SqlParameter param3 = new SqlParameter("@description", description);
            SqlParameter param4 = new SqlParameter("@organizer", organizer);
            SqlParameter param5 = new SqlParameter("subscriber", subscriber);
            var result = DBHelper.GetData(
                new EventDTOMapper(),
                sqlQuery, param1, param2, param3, param4, param5);
            return result;
        }
        public static IList<EventDTO> UpdateIsSent(string id)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "UpdateIsSent");
            SqlParameter param = new SqlParameter("@id", id);
            var result = DBHelper.GetData(
                new EventDTOMapper(),
                sqlQuery, param);
            return result;
        }
    }
}
