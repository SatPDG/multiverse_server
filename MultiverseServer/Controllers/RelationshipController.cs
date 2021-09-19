using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response.User;
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
            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS));
            }

            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetFollowerOfUser(DbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetFollowerOfUserCount(DbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            return new JsonResult(apiModel);
        }

        [Authorize]
        [HttpPost("followed")]
        public IActionResult GetFollowedList([FromBody] ListRequestModel request)
        {
            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS));
            }

            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetFollowedOfUser(DbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetFollowedOfUserCount(DbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            return new JsonResult(apiModel);
        }

        [Authorize]
        [HttpDelete("follower/{id}")]
        public IActionResult DeteleFollowerUser(int id)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Delete the relationship
            bool isDone = RelationshipDbService.DeleteFriendship(DbContext, id, userID);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            return new JsonResult(new EmptyResult());
        }

        [Authorize]
        [HttpDelete("followed/{id}")]
        public IActionResult DeleteFollowedUser(int id)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Delete the relationship
            bool isDone = RelationshipDbService.DeleteFriendship(DbContext, userID, id);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            return new JsonResult(new EmptyResult());
        }

        [Authorize]
        [HttpPost("follower/request")]
        public IActionResult GetFollowerRequestList([FromBody] ListRequestModel request)
        {
            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS));
            }

            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetRequestFollowerOfUser(DbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetRequestFollowerOfUserCount(DbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            return new JsonResult(apiModel);
        }

        [Authorize]
        [HttpPost("followed/request")]
        public IActionResult GetFollowedRequestList([FromBody] ListRequestModel request)
        {
            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS));
            }

            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetRequestFollowedOfUser(DbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetRequestFollowedOfUserCount(DbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            return new JsonResult(apiModel);
        }

        /// <summary>
        /// I accept id has a follower 
        /// </summary>
        [Authorize]
        [HttpPost("follower/request/{id}")]
        public IActionResult AcceptFollowerRequest(int id)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Accept the relationship
            bool isDone = RelationshipDbService.AcceptFriendshipRequest(DbContext, id, userID);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            return new JsonResult(new EmptyResult());
        }

        // I refuse to be follow by id
        [Authorize]
        [HttpDelete("follower/request/{id}")]
        public IActionResult DeleteFollowerRequest(int id)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Delete the relationship request
            bool isDone = RelationshipDbService.DeleteFriendshipRequest(DbContext, id, userID);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            return new JsonResult(new EmptyResult());
        }

        // I want to followed id.
        [Authorize]
        [HttpPost("followed/request/{id}")]
        public IActionResult SendFollowedRequest(int id)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Delete the relationship request
            bool isDone = RelationshipDbService.SendFriendshipRequest(DbContext, userID, id);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            return new JsonResult(new EmptyResult());
        }

        // I delete my request to follow id
        [Authorize]
        [HttpDelete("followed/request/{id}")]
        public IActionResult DeleteFollowedRequest(int id)
        {
            // Get the user id
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Delete the relationship request
            bool isDone = RelationshipDbService.DeleteFriendshipRequest(DbContext, userID, id);
            if (!isDone)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            return new JsonResult(new EmptyResult());
        }
    }
}
