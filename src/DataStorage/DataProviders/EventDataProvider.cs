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

        public static IList<EventDTO> GetEventById(string id)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "GetEventById");
            SqlParameter param = new SqlParameter("@eventId", id);
            var result = DBHelper.GetData(
                new EventDTOMapper(),
                sqlQuery, param);
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

        public static string AddEvent(string type, string description, string organizer, string subscriber)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "AddEvent");
            string id = Guid.NewGuid().ToString();
            SqlParameter param1 = new SqlParameter("@id", id);
            SqlParameter param2 = new SqlParameter("@type", type);
            SqlParameter param3 = new SqlParameter("@description", description);
            SqlParameter param4 = new SqlParameter("@organizer", organizer);
            SqlParameter param5 = new SqlParameter("subscriber", subscriber);
            DBHelper.GetData(
                new EventDTOMapper(),
                sqlQuery, param1, param2, param3, param4, param5);
            return id;
        }

        public static IList<EventDTO> AddEvents(string events)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "AddEvents");
            sqlQuery += events;
            var result = DBHelper.GetData(
                new EventDTOMapper(),
                sqlQuery);
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
        public static void DeleteTestEvents()
        {
            string sqlQuery = XmlStrings.GetString(Tables.Events, "DeleteTestEvents");
            DBHelper.GetData(
            new EventDTOMapper(),
            sqlQuery);
        }
    }
}
