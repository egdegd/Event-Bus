using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DataStorage.Mappers;
using Model;
using SqlHelper;

namespace DataStorage.DataProviders
{
    public class SubscriberDataProvider
    {
        public static IList<String> GetSubscribers(string type)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Subscribers, "GetSubscribers");
            SqlParameter param = new SqlParameter("@type", type);
            var result = DBHelper.GetData(
                new SubscriberDTOMapper(),
                sqlQuery, param);
            return result;
        }

        public static IList<String> AddSubscribe(string subscriber, string type)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Subscribers, "AddSubscribe");
            SqlParameter param1 = new SqlParameter("@subscriber", subscriber);
            SqlParameter param2 = new SqlParameter("@type", type);
            var result = DBHelper.GetData(
                new SubscriberDTOMapper(),
                sqlQuery, param1, param2);
            return result;
        }

        public static IList<String> DeleteSubscribe(string subscriber, string type)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Subscribers, "DeleteSubscribe");
            SqlParameter param1 = new SqlParameter("@subscriber", subscriber);
            SqlParameter param2 = new SqlParameter("@type", type);
            var result = DBHelper.GetData(
                new SubscriberDTOMapper(),
                sqlQuery, param1, param2);
            return result;
        }

        public static void DeleteSubscribers(string subscriber, int start, int finish)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Subscribers, "DeleteSubscribers");
            SqlParameter param1 = new SqlParameter("@subscriber", subscriber);
            SqlParameter param2 = new SqlParameter("@start", start);
            SqlParameter param3 = new SqlParameter("@finish", finish);
            DBHelper.GetData(
            new SubscriberDTOMapper(),
            sqlQuery, param1, param2, param3);
        }
    }
}
