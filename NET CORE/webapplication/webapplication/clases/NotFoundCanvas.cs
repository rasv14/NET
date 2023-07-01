using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class NotFoundCanvas
    {
        public List<Message> errors { get; set; }
    }

    public class Message
    {
        public string message { get; set; }
    }
}
