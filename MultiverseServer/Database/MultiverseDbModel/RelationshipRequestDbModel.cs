using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseModel
{
    public class RelationshipRequestDbModel
    {
        public int relationshipRequestID { get; set; }
        public int followerID { get; set; }
        public int followedID { get; set; }
    }
}
