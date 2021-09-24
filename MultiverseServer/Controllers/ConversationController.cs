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
            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS));
            }

            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the conversation list
            IList<ConversationDbModel> convList = ConversationDbService.GetConversationList(DbContext, userID, request.offset, request.count);
            int totalNumber = ConversationDbService.GetNumberOfConversation(DbContext, userID);
            
            // Create the api obj
            ConversationListResponseModel apiModel = ConversationListResponseModel.ToApiModel(convList, request.count, request.offset, totalNumber);

            return new JsonResult(apiModel);
        }

        [Authorize]
        [HttpPost("{id}/send")]
        public IActionResult SendMessage(int id, [FromBody] SendMessageRequestModel request)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            MessageApiModel apiModel = null;
            if (!String.IsNullOrWhiteSpace(request.message))
            {
                // Send the string message
                if (request.message.Count() > 250)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return new JsonResult(new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE));
                } 

                // Make sure the user is in the conversation
                if(!ConversationDbService.IsUserInConversation(DbContext, userID, id))
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
                }

                // Send the message
                MessageDbModel dbModel = new MessageDbModel();
                dbModel.message = request.message;
                dbModel.messageType = (byte) MessageType.TEXT_MESSAGE;
                dbModel.publishedTime = DateTime.Now.Date;
                dbModel.authorID = userID;
                dbModel.conversationID = id;
                bool isDone = ConversationDbService.SendMessage(DbContext, dbModel);
                if (!isDone)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
                }

                apiModel = MessageApiModel.ToApiModel(dbModel);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE));
            }

            return new JsonResult(apiModel);
        }

        [Authorize]
        [HttpDelete("{conversationid}/{messageid}")]
        public IActionResult DeleteMessage(int conversationid, int messageid)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Make sure the user is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(DbContext, userID, conversationid);
            if (!isInConv)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            // Make sure the user is the author of the message
            bool isAuthor = ConversationDbService.IsAuthorOfMessage(DbContext, userID, messageid);
            if (!isAuthor)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
            }

            // Delete the conversation
            bool isDone = ConversationDbService.DeleteMessage(DbContext, userID, messageid);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
            }

            return new JsonResult(new EmptyResult());
        }

        [Authorize]
        [HttpPost("{conversationid}/{messageid}")]
        public IActionResult UpdateMessage(int conversationid, int messageid, [FromBody] UpdateMessageRequestModel request)
        {
            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE));
            }

            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the message
            MessageDbModel dbModel = ConversationDbService.GetMessage(DbContext, messageid);
            if(dbModel == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            // Make sure the author is the user and that the right conversation is used
            if(dbModel.authorID != userID || dbModel.conversationID != conversationid)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
            }

            // Update the message
            dbModel.message = request.message;
            ConversationDbService.UpdateMessage(DbContext, userID, messageid, dbModel);

            return new JsonResult(new EmptyResult());
        }

        [Authorize]
        [HttpPost("{conversationid}/user/add")]
        public IActionResult AddUserToConversation(int conversationid, [FromBody] IDListRequestModel request)
        {
            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE));
            }

            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Make sure the user doing the action is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(DbContext, userID, conversationid);
            if (!isInConv)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
            }

            // Add users to conversation
            bool isDone = ConversationDbService.AddUsersToConversation(DbContext, request.idList, conversationid);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
            }

            return new JsonResult(new EmptyResult());
        }

        [Authorize]
        [HttpDelete("{conversationid}/user/{id}")]
        public IActionResult RemoveUserFromConversation(int conversationid, [FromBody] IDListRequestModel request)
        {
            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE));
            }

            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Make sure the user doing the action is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(DbContext, userID, conversationid);
            if (!isInConv)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
            }

            // Remove the users
            bool isDone = ConversationDbService.RemoveUsersFromConversation(DbContext, request.idList, conversationid);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION));
            }

            return new JsonResult(new EmptyResult());
        }
    }
}
