using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response.User
{
    public class UserOwnResponseModel
    {
        public int userID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public int nbrOfFollower { get; set; }
        public int nbrOfFollowed { get; set; }
        public int nbrOfRequestFollower { get; set; }
        public int nbrOfRequestFollowed { get; set; }
        public int nbrOfConversation { get; set; }
    }
}
