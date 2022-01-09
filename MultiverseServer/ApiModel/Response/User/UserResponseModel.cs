using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response
{
    public class UserResponseModel
    {
        public int userID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int nbrOfFollower { get; set; }
        public int nbrOfFollowed { get; set; }
        public bool isAFollower { get; set; }
        public bool isFollowerRequestPending { get; set; }
        public bool isFollowed { get; set; }
        public bool isFollowedRequestPending { get; set; }

    }
}
