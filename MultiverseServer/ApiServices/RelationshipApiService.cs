using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request;
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
    public class RelationshipApiService
    {
        private RelationshipApiService()
        {

        }

        public static ApiResponse GetFollowerList(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetFollowerOfUser(dbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetFollowerOfUserCount(dbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse GetFollowedList(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetFollowedOfUser(dbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetFollowedOfUserCount(dbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse GetFollowerRequestList(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetRequestFollowerOfUser(dbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetRequestFollowerOfUserCount(dbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse GetFollowedRequestList(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            // Get the list from the db
            IList<UserDbModel> userList = RelationshipDbService.GetRequestFollowedOfUser(dbContext, userID, request.offset, request.count);
            int totalSize = RelationshipDbService.GetRequestFollowedOfUserCount(dbContext, userID);

            // Convert to api model
            UserListResponseModel apiModel = UserListResponseModel.ToApiModel(userList, request.count, request.offset, totalSize);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse AcceptFollowerRequest(MultiverseDbContext dbContext, int followerID, int followedID)
        {
            ApiResponse response = new ApiResponse();

            // Accept the relationship
            bool isDone = RelationshipDbService.AcceptFriendshipRequest(dbContext, followerID, followedID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
            }

            return response;
        }

        public static ApiResponse DeleteFollowerRequest(MultiverseDbContext dbContext, int followerID, int followedID)
        {
            ApiResponse response = new ApiResponse();

            // Delete the relationship request
            bool isDone = RelationshipDbService.DeleteFriendshipRequest(dbContext, followerID, followedID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            return response;
        }

        public static ApiResponse DeleteFollowedRequest(MultiverseDbContext dbContext, int followerID, int followedID)
        {
            ApiResponse response = new ApiResponse();

            // Delete the relationship request
            bool isDone = RelationshipDbService.DeleteFriendshipRequest(dbContext, followerID, followedID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            return response;
        }

        public static ApiResponse DeleteFollower(MultiverseDbContext dbContext, int followerID, int followedID)
        {
            ApiResponse response = new ApiResponse();

            // Delete the relationship
            bool isDone = RelationshipDbService.DeleteFriendship(dbContext, followerID, followedID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            return response;
        }

        public static ApiResponse DeleteFollowed(MultiverseDbContext dbContext, int followerID, int followedID)
        {
            ApiResponse response = new ApiResponse();

            // Delete the relationship
            bool isDone = RelationshipDbService.DeleteFriendship(dbContext, followerID, followedID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            return response;
        }

        public static ApiResponse SendRequest(MultiverseDbContext dbContext, int followerID, int followedID)
        {
            ApiResponse response = new ApiResponse();

            // Send the relationship request
            bool isDone = RelationshipDbService.SendFriendshipRequest(dbContext, followerID, followedID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            return response;
        }
    }
}
