using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseModel
{
    public class ConversationDbModel
    {
        public int conversationID { get; set; }
        public string name { get; set; }
        public DateTime lastUpdate { get; set; }

    }
}
