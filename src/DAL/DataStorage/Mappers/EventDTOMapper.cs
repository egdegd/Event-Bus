using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DataStorage.Mappers
{
    public class EventDTOMapper : IMapper<EventDTO>
    {
        public EventDTO ReadItem(SqlDataReader rd)
        {
            return new EventDTO
            {
                Id = (string)rd["Id"],
                Type = (string)rd["Type"],
                Description = (string)rd["Description"],
                Organizer = (string)rd["Organizer"],
                Subscriber = (string)rd["Subscriber"],
                IsSent = (int)rd["IsSent"],
            };
        }
    }
}
