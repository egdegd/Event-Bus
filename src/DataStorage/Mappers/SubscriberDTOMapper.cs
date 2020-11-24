using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DataStorage.Mappers
{
    public class SubscriberDTOMapper : IMapper<String>
    {
        public string ReadItem(SqlDataReader rd)
        {
            return (string)rd["Subscriber"];
        }
    }
}
