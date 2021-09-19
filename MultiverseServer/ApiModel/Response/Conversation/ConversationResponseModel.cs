using MultiverseServer.ApiModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response
{
    public class ConversationResponseModel
    {
        public int conversationID { get; set; }
        public string name { get; set; }
        public string lastUpdate { get; set; }
        public int nbrOfUser { get; set; }

    }
}
