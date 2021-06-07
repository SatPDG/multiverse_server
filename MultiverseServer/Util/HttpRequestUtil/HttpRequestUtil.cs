using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Util.HttpRequestUtil
{
    public class HttpRequestUtil
    {

        private HttpRequestUtil()
        {

        }

        public static string GetTokenFromRequest(HttpRequest request)
        {
            string token =  request.Headers["authorization"];
            // Remove the Bearer word
            token = token.Substring(7);
            return token;
        }
    }
}
