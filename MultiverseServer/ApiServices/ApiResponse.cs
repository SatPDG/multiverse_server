using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MultiverseServer.ApiServices
{
    public class ApiResponse
    {
        public int code { get; set; }
        public object obj { get; set; }

        public ApiResponse()
        {
            code = (int)HttpStatusCode.OK;
            obj = new EmptyResult();
        }
    }
}
