using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Request
{
    public class ListRequestModel
    {
        public int count { get; set; }
        public int offset { get; set; }
    }
}
