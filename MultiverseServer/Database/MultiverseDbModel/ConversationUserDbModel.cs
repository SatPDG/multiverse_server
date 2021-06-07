using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseModel
{
    public class ConversationUserDbModel
    {
        public int conversationUserID {get; set; }
        public int conversationID { get; set; }
        public int userID { get; set; }
    }
}
