using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request.User;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiModel.Response.User;
using MultiverseServer.ApiServices;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
using MultiverseServer.Security.ListAccess;
using MultiverseServer.Security.Token;
using MultiverseServer.Util.HttpRequestUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MultiverseServer.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public UserController(IConfiguration config, MultiverseDbContext DbContext)
        {
            this.Config = config;
            this.DbContext = DbContext;
        }

        [Authorize]
        [HttpGet("{userID}")]
        public IActionResult GetUserInfo(int userID)
        {
            ApiResponse response = UserApiService.GetUserInfo(DbContext, userID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpGet("user")]
        public IActionResult GetUserOwnInfo()
        {
            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = UserApiService.GetUserOwnInfo(DbContext, userID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }
        
        /// <summary>
        /// Get a list of the closest user.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("users")]
        public IActionResult GetUserList()
        {
            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = UserApiService.GetUserList(DbContext, userID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("search")]
        public IActionResult SearchForUsers([FromBody] UserSearchRequestModel request)
        {
            ApiResponse response = UserApiService.SearchForUsers(DbContext, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }
    }
}
