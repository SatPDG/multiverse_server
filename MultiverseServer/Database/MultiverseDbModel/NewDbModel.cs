using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Database.MultiverseDbModel
{
    public class NewDbModel
    {
        public int newID { get; set; }
        public int authorID { get; set; }
        public DateTime date { get; set; }
        public byte broadcastType { get; set; }
        public byte newType { get; set; }
        public string message { get; set; }
    }

    public enum NewType : byte
    {
        TEXT_NEW = 0,
    }

    public enum NewBroadcastType : byte
    {
        RELATIONSHIP_BROADCAST = 0,
        LOCATION_BROADCAST = 1,
    }
}
