using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Message
    {
        public Service From { get; set; }
        public Service To { get; set; }
        public string Text { get; set; }
    }
}
