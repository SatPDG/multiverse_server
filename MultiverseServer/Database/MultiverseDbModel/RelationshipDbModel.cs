using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseModel
{
    public class RelationshipDbModel
    {
        public int relationshipID { get; set; }
        public int followerID { get; set; }
        public int followedID { get; set; }
    }
}
