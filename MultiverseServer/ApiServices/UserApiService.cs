using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiModel.Response.User;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
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

        public static ApiResponse GetUserInfo(MultiverseDbContext dbContext, int userID)
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
    }
}
