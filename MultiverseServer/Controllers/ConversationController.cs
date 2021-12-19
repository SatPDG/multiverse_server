using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Request.Conversation;
using MultiverseServer.ApiModel.Request.Util;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiModel.Response.Conversation;
using MultiverseServer.ApiServices;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
using MultiverseServer.Security.Json;
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
    public class ConversationController : ControllerBase
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public ConversationController(IConfiguration config, MultiverseDbContext DbContext)
        {
            this.Config = config;
            this.DbContext = DbContext;
        }

        [Authorize]
        [HttpGet("{conversationID}")]
        public IActionResult GetConversationInfo(int conversationID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.GetConversationInfo(DbContext, userID, conversationID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("{conversationID}")]
        public IActionResult SetConversationInfo(int conversationID, [FromBody] UpdateConversationRequest request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.SetConversationInfo(DbContext, userID, conversationID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpDelete("{conversationID}")]
        public IActionResult DeleteConversation(int conversationID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.DeleteConversation(DbContext, userID, conversationID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("new")]
        public IActionResult CreateConversation([FromBody] CreateConversationRequest request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.CreateConversation(DbContext, userID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("conversations")]
        public IActionResult GetConversationList([FromBody]ListRequestModel request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.GetConversationList(DbContext, userID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("{conversationID}/send")]
        public IActionResult SendMessage(int conversationID, [FromBody] SendMessageRequestModel request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.SendMessage(DbContext, userID, conversationID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpDelete("{conversationID}/{messageID}")]
        public IActionResult DeleteMessage(int conversationID, int messageID)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.DeleteMessage(DbContext, userID, conversationID, messageID);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("{conversationID}/{messageID}")]
        public IActionResult UpdateMessage(int conversationID, int messageID, [FromBody] UpdateMessageRequestModel request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, userID, conversationID, messageID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("{conversationID}/messages")]
        public IActionResult GetMessages(int conversationID, [FromBody] ListRequestModel request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.GetMessageList(DbContext, userID, conversationID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpPost("{conversationID}/users")]
        public IActionResult AddUserToConversation(int conversationID, [FromBody] IDListRequestModel request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, userID, conversationID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }

        [Authorize]
        [HttpDelete("{conversationID}/users")]
        public IActionResult RemoveUserFromConversation(int conversationID, [FromBody] IDListRequestModel request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, userID, conversationID, request);
            Response.StatusCode = response.code;

            return new JsonResult(response.obj);
        }
    }
}
