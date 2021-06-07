using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Request
{
    public class RefreshResponseModel
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
