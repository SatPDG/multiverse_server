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
