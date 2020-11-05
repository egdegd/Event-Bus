using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceStorage
    {
        public int lastID = -1;
        Dictionary<int, Service> services = new Dictionary<int, Service>();
        public int AddService(Service service)
        {
            lastID += 1;
            service.ID = lastID;
            services.Add(lastID, service);
            return lastID;
        }
        public void PrintServices()
        {
            foreach (KeyValuePair<int, Service> kvp in services)
            {
                Console.WriteLine($"ID: {kvp.Key}, Name: {kvp.Value.name}");
            }
        }
        public Message SendMessage(string text, int fromID, int toID)
        {
            if (!services.ContainsKey(fromID))
            {
                Console.WriteLine($"Service with ID {fromID} does not exist");
                return null;
            }
            if (!services.ContainsKey(toID))
            {
                Console.WriteLine($"Service with ID {toID} does not exist");
                return null;
            }
            Message msg = services[fromID].SendMsg(text, services[toID]);
            return msg;
        }
        public void Delete(int ID)
        {
            if (!services.ContainsKey(ID))
            {
                Console.WriteLine($"Service with ID {ID} does not exist");
                return;
            }
            services.Remove(ID);
        }
    }
}
