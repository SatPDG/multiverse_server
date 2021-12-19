using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Request.New
{
    public class UpdateNewRequestModel
    {
        public byte broadcastType { get; set; }
        public byte newType { get; set; }
        public string message { get; set; }
    }
}
