using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Model;
using Utils;
using Newtonsoft.Json;
using System.Configuration;

namespace WebServiceClient
{
    public class EventBusClient
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string eventBusHost = ConfigurationManager.AppSettings["EventBusHost"];
        public HttpResponseMessage AddMessage(string from, string recipient, string text)
        {
            try
            {
                var msg = new Message
                {
                    From = from,
                    To = recipient,
                    Text = text
                };

                var serializedObject = JsonConvert.SerializeObject(msg);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{eventBusHost}/addmsg", content).Result;
                return response;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public string SendMessage(string name)
        {
            try
            {
                var response = client.GetAsync($"{eventBusHost}/sendmsg?name={name}").Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }

        }

        public string SendEvent(string name)
        {
            try
            {
                var response = client.GetAsync($"{eventBusHost}/sendevent?name={name}").Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public string Publish(string organizer, string type, string description)
        {
            try
            {
                Event e = new Event
                {
                    Type = type,
                    Description = description,
                    Organizer = organizer
                };

                var serializedObject = JsonConvert.SerializeObject(e);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{eventBusHost}/publish", content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public string Subscribe(string name, string type)
        {
            try
            {
                Pair p = new Pair
                {
                    First = name,
                    Second = type
                };
                var serializedObject = JsonConvert.SerializeObject(p);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{eventBusHost}/subscribe", content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public string Unsubscribe(string name, string type)
        {
            try
            {
                Pair p = new Pair
                {
                    First = name,
                    Second = type
                };
                var serializedObject = JsonConvert.SerializeObject(p);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{eventBusHost}/unsubscribe", content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public HttpResponseMessage AddMessageInDB(string from, string recipient, string text)
        {
            try
            {
                var msg = new Message
                {
                    From = from,
                    To = recipient,
                    Text = text
                };

                var serializedObject = JsonConvert.SerializeObject(msg);
                var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{eventBusHost}/addmsgindb", content).Result;
                return response;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }
    }
}
