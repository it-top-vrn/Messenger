using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class Dialog
    {
        public string idDialog { get; set; }
        public string dateOfSendingLastMessage { get; set; }
        public string dataLastMessage { get; set; }
        public List<Message> Messages { get; set; }
    }
}
