using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Database.MultiverseDbModel
{
    public class AuthenticationDbModel
    {
        public int userID { get; set; }
        public string token { get; set; }
        public DateTime expireTime { get; set; }
    }
}
