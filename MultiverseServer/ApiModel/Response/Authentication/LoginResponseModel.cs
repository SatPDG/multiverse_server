using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response
{
    public class LoginResponseModel
    {
        public int userID { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
