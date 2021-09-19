using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Request.Conversation
{
    public class CreateConversationRequest
    {
        public string name { get; set; }
        public IList<int> users { get; set; }
    }
}
