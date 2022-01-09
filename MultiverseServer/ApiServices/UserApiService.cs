using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request.User;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiModel.Response.User;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
using MultiverseServer.Security.ListAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MultiverseServer.ApiServices
{
    public class UserApiService
    {
        private UserApiService()
        {

        }

        public static ApiResponse GetUserInfo(MultiverseDbContext dbContext, int userID, int callerUserID)
        {
            ApiResponse response = new ApiResponse();

            // Get the user information.
            UserDbModel model = UserDbService.GetUser(dbContext, userID);
            if (model == null)
            {
                // There is no user with this id.
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            // Create the api model
            UserResponseModel apiModel = new UserResponseModel();
            apiModel.userID = model.userID;
            apiModel.firstname = model.firstname;
            apiModel.lastname = model.lastname;
            apiModel.nbrOfFollower = RelationshipDbService.GetFollowerOfUserCount(dbContext, userID);
            apiModel.nbrOfFollowed = RelationshipDbService.GetFollowedOfUserCount(dbContext, userID);
            apiModel.isAFollower = RelationshipDbService.IsFollowerOfUser(dbContext, userID, callerUserID);
            if(apiModel.isAFollower)
            {
                apiModel.isFollowerRequestPending = false;
            }
            else
            {
                apiModel.isFollowerRequestPending = RelationshipDbService.IsRequestFollowerOfUser(dbContext, userID, callerUserID);
            }
            apiModel.isFollowed = RelationshipDbService.IsFollowedOfUser(dbContext, userID, callerUserID);
            if (apiModel.isFollowed)
            {
                apiModel.isFollowedRequestPending = false;
            }
            else
            {
                apiModel.isFollowedRequestPending = RelationshipDbService.IsRequestFollowedOfUser(dbContext, userID, callerUserID);
            }

            response.obj = apiModel;
            return response;
        }

        public static ApiResponse GetUserOwnInfo(MultiverseDbContext dbContext, int userID)
        {
            ApiResponse response = new ApiResponse();

            // Get the user information.
            UserDbModel model = UserDbService.GetUser(dbContext, userID);
            if (model == null)
            {
                // There is no user with this id.
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            // Create the api model
            UserOwnResponseModel apiModel = new UserOwnResponseModel();
            apiModel.userID = model.userID;
            apiModel.firstname = model.firstname;
            apiModel.lastname = model.lastname;
            apiModel.nbrOfFollower = RelationshipDbService.GetFollowerOfUserCount(dbContext, userID);
            apiModel.nbrOfFollowed = RelationshipDbService.GetFollowedOfUserCount(dbContext, userID);
            apiModel.nbrOfRequestFollower = RelationshipDbService.GetRequestFollowerOfUserCount(dbContext, userID);
            apiModel.nbrOfRequestFollowed = RelationshipDbService.GetRequestFollowedOfUserCount(dbContext, userID);
            apiModel.nbrOfConversation = ConversationDbService.GetNumberOfConversation(dbContext, userID);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse GetUserList(MultiverseDbContext dbContext, int userID)
        {
            ApiResponse response = new ApiResponse();

            // Get the user location
            UserDbModel model = UserDbService.GetUser(dbContext, userID);

            // Get the list of users
            IList<UserDbModel> userList = UserDbService.SearchUserByLocation(dbContext, model.lastLocation, 0, 20);

            // Convert it to a response
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, userList.Count, 0, userList.Count);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse SearchForUsers(MultiverseDbContext dbContext, UserSearchRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            IList<UserDbModel> userList = null;

            if (!string.IsNullOrEmpty(request.nameSearch))
            {
                // Make a name search
                userList = UserDbService.SearchUserByName(dbContext, request.nameSearch, request.offset, request.count);
            }
            else if(request.locationSearch != null)
            {
                // Make a location search
                userList = UserDbService.SearchUserByLocation(dbContext, request.locationSearch.ToDbModel(), request.offset, request.count);
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, -1);
            response.obj = apiModel;

            return response;
        }
    }
}
