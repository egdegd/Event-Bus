using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Model
{
    public class Event
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Organizer { get; set; }
    }
    public class EventDTO : Event
    {
        public string Id { get; set; }
        public string Subscriber { get; set; }
        public int IsSent { get; set; }

        public Event ToEvent()
        {
            return new Event
            {
                Type = Type,
                Description = Description,
                Organizer = Organizer
            };
        }

    }
    public class EventToFile : Event
    {
        public string Subscriber { get; set; }
    }
}
