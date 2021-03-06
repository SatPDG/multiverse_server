using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request;
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
    public class RelationshipController : ControllerBase
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public RelationshipController(IConfiguration config, MultiverseDbContext DbContext)
        {
            this.Config = config;
            this.DbContext = DbContext;
        }

        [Authorize]
        [HttpPost("follower")]
        public IActionResult GetFollowerList([FromBody] ListRequestModel request)
        {
            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.GetFollowerList(DbContext, userID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("followed")]
        public IActionResult GetFollowedList([FromBody] ListRequestModel request)
        {
            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.GetFollowedList(DbContext, userID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpDelete("follower/{followerID}")]
        public IActionResult DeteleFollowerUser(int followerID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.DeleteFollower(DbContext, followerID, userID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpDelete("followed/{followedID}")]
        public IActionResult DeleteFollowedUser(int followedID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.DeleteFollowed(DbContext, userID, followedID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("follower/request")]
        public IActionResult GetFollowerRequestList([FromBody] ListRequestModel request)
        {
            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.GetFollowerRequestList(DbContext, userID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("followed/request")]
        public IActionResult GetFollowedRequestList([FromBody] ListRequestModel request)
        {
            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.GetFollowedRequestList(DbContext, userID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        /// <summary>
        /// I accept id has a follower 
        /// </summary>
        [Authorize]
        [HttpPost("followed/request/{followerID}")]
        public IActionResult AcceptFollowedRequest(int followerID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.AcceptFollowedRequest(DbContext, followerID, userID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        // I refuse to be follow by id
        [Authorize]
        [HttpDelete("follower/request/{followerID}")]
        public IActionResult DeleteFollowerRequest(int followerID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.DeleteFollowerRequest(DbContext, followerID, userID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        // I want to followed id.
        [Authorize]
        [HttpPost("follower/request/{followedID}")]
        public IActionResult SendFollowerRequest(int followedID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.SendRequest(DbContext, userID, followedID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        // I delete my request to follow id
        [Authorize]
        [HttpDelete("followed/request/{followedID}")]
        public IActionResult DeleteFollowedRequest(int followedID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = RelationshipApiService.DeleteFollowedRequest(DbContext, userID, followedID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }
    }
}
