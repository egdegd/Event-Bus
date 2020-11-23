using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DataStorage.Mappers
{
    public class MessageDTOMapper : IMapper<MessageDTO>
    {
        public MessageDTO ReadItem(SqlDataReader rd)
        {
            return new MessageDTO
            {
                Id = (int)rd["Id"],
                From = (string)rd["From"],
                To = (string)rd["To"],
                Text = (string)rd["Text"],
                IsSent = (int)rd["IsSent"],
            };
        }
    }
}
