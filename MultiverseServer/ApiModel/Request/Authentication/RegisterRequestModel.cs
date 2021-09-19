using MultiverseServer.ApiModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Request
{
    public class RegisterRequestModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public LocationApiModel lastLocation { get; set; }
    }
}
