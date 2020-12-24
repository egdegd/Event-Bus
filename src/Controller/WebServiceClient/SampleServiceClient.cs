using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace WebServiceClient
{
    public class SampleServiceClient
    {
        static readonly HttpClient client = new HttpClient();

        public string SendMessage(string localhost, string fromName, string toName, string text)
        {
            string url = $"http://localhost:{localhost}/api/{fromName}/sendmsg?recipient={toName}&text={text}";
            
            try
            {
                var response = client.GetAsync(url).Result;
                Logger.Info("Client sent request: " + url);

                var result = response.Content.ReadAsStringAsync().Result;
                Logger.Info("Client received response: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public string Subscribe(string localhost, string name, string type)
        {
            string url = $"http://localhost:{localhost}/api/{name}/subscribe?type={type}";

            try
            {
                var response = client.GetAsync(url).Result;
                Logger.Info("Client sent request: " + url);

                var result = response.Content.ReadAsStringAsync().Result;
                Logger.Info("Client received response: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public string Unsubscribe(string localhost, string name, string type)
        {
            string url = $"http://localhost:{localhost}/api/{name}/unsubscribe?type={type}";

            try
            {
                var response = client.GetAsync(url).Result;
                Logger.Info("Client sent request: " + url);

                var result = response.Content.ReadAsStringAsync().Result;
                Logger.Info("Client received response: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

        public string Publish(string localhost, string name, string type, string description)
        {
            string url = $"http://localhost:{localhost}/api/{name}/publishevent?type={type}&description={description}";

            try
            {
                var response = client.GetAsync(url).Result;
                Logger.Info("Client sent request: " + url);

                var result = response.Content.ReadAsStringAsync().Result;
                Logger.Info("Client received response: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Request error", e);
                throw;
            }
        }

    }
}
