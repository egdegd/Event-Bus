using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
    }
    public class MessageDTO
    {
        public string Id { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Text { get; set; }

        public int IsSent { get; set; }

        public Message ToMessage()
        {
            return new Message
            {
                From = From,
                To = To,
                Text = Text
            };
        }
    }
}
