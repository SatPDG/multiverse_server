using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseModel
{
    public class MessageDbModel
    {
        public int messageID { get; set; }
        public int conversationID { get; set; }
        public int authorID { get; set; }
        public DateTime publishedTime { get; set; }
        public byte messageType { get; set; }
        public string message { get; set; }
    }

    public enum MessageType : byte
    {
        TEXT_MESSAGE = 0,
        IMAGE_MESSAGE = 1,
    }
}
