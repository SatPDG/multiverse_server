using MultiverseServer.ApiModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Request.User
{
    public class UserSearchRequestModel
    {
        public string nameSearch { get; set; }
        public LocationApiModel locationSearch { get; set; }
        public int count { get; set; }
        public int offset { get; set; }

    }
}
