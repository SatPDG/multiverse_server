using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Database.MultiverseDbModel.AbstractDbModel
{
    public class UserNewDbModel
    {
        public int newID { get; set; }
        public DateTime date { get; set; }
        public int authorID { get; set; }
        public string authorFirstname { get; set; }
        public string authorLastname { get; set; }
        public byte broadcastType { get; set; }
        public byte newType { get; set; }
        public string message { get; set; }
    }
}
