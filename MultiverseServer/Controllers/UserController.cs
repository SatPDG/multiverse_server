using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request.User;
using MultiverseServer.ApiModel.Response;
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
        [HttpGet("{id}")]
        public IActionResult UserProfil(int id)
        {
            // Get the user information.
            UserDbModel model =  UserDbService.GetUser(DbContext, id);
            if(model == null)
            {
                // There is no user with this id.
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER));
            }

            // Create the api model
            UserResponseModel apiModel = new UserResponseModel();
            apiModel.userID = model.userID;
            apiModel.firstname = model.firstname;
            apiModel.lastname = model.lastname;
            apiModel.nbrOfFollower = RelationshipDbService.GetFollowerOfUserCount(DbContext, id);
            apiModel.nbrOfFollowed = RelationshipDbService.GetFollowedOfUserCount(DbContext, id);

            return new JsonResult(apiModel);
        }
        
        /// <summary>
        /// Get a list of the closest user.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("users")]
        public IActionResult Users()
        {
            // Get the user id.
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the user location
            UserDbModel model = UserDbService.GetUser(DbContext, userID);

            // Get the list of users
            IList<UserDbModel> userList = UserDbService.SearchUserByLocation(DbContext, model.lastLocation, 0, 10);

            // Convert it to a response
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, userList.Count, 0, userList.Count);

            return new JsonResult(apiModel);
        }

        [Authorize]
        [HttpPost("search")]
        public IActionResult SearchForUsers([FromBody] UserSearchRequestModel request)
        {
            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS));
            }

            IList<UserDbModel> userList = null;

            // Make the search
            if (string.IsNullOrEmpty(request.nameSearch))
            {
                // Make a location search
                userList = UserDbService.SearchUserByLocation(DbContext, request.locationSearch.ToDbModel(), request.offset, request.count);
            }
            else
            {
                // Make a name search
                userList = UserDbService.SearchUserByName(DbContext, request.nameSearch, request.offset, request.count);
            }

            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, -1);
            return new JsonResult(apiModel);
        }
    }
}
