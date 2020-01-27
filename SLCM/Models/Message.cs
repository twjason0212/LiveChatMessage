using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLCM.Models
{
    public class Message
    {
        public string Target { get; set; }
        public string GameID { get; set; }
        public MessageData Data { get; set; }

    }

    public class MessageData
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public int Level { get; set; }
        public string NickName { get; set; }
        public string UserAvatar { get; set; }
    }
}