using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request.Conversation;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
using MultiverseServer.Security.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MultiverseServer.ApiServices
{
    public class ConversationApiService
    {
        private ConversationApiService()
        {

        }

        public static ApiResponse GetConversationInfo(MultiverseDbContext dbContext, int userID, int conversationID)
        {
            ApiResponse response = new ApiResponse();

            // Make sur the user is in the conversation
            if (!ConversationDbService.IsUserInConversation(dbContext, userID, conversationID))
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IdentificationNumberDoNotGrantAccess, ErrorMessage.IDENTIFICATION_NUMBER_DO_NOT_GRANT_ACCESS);
                return response;
            }

            // Get the conversation info
            ConversationDbModel conversationDbModel = ConversationDbService.GetConversation(dbContext, conversationID);
            int nbrOfUser = ConversationDbService.GetNumberOfUser(dbContext, conversationID);

            // Create the api obj
            ConversationResponseModel apiModel = new ConversationResponseModel();
            apiModel.conversationID = conversationDbModel.conversationID;
            apiModel.name = conversationDbModel.name;
            apiModel.lastUpdate = conversationDbModel.lastUpdate.ToString();
            apiModel.nbrOfUser = nbrOfUser;
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse SetConversationInfo(MultiverseDbContext dbContext, int userID, int conversationID, UpdateConversationRequest request)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Make sur the user is in the conversation
            if (!ConversationDbService.IsUserInConversation(dbContext, userID, conversationID))
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IdentificationNumberDoNotGrantAccess, ErrorMessage.IDENTIFICATION_NUMBER_DO_NOT_GRANT_ACCESS);
                return response;
            }

            // Update the conversation
            ConversationDbModel dbModel = new ConversationDbModel();
            dbModel.name = request.name;
            dbModel.lastUpdate = DateTime.Now.Date;
            bool isDone = ConversationDbService.UpdateConversation(dbContext, conversationID, dbModel);

            if (!isDone)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.obj = new ErrorApiModel((int)ErrorType.UnknowDbError, "");
                return response;
            }

            return response;
        }

        public static ApiResponse CreateConversation(MultiverseDbContext dbContext, int userID, CreateConversationRequest request)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Make sure the user is in the conversation user list
            if (!request.users.Contains(userID))
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Create the conversation
            ConversationDbModel dbModel = new ConversationDbModel();
            dbModel.name = request.name;
            dbModel.lastUpdate = DateTime.Now;
            dbModel = ConversationDbService.CreateConversation(dbContext, dbModel, request.users);

            if (dbModel == null)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            ConversationApiModel apiModel = ConversationApiModel.ToApiModel(dbModel);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse DeleteConversation(MultiverseDbContext dbContext, int userID, int conversationID)
        {
            ApiResponse response = new ApiResponse();

            // Make sur the user is in the conversation
            if (!ConversationDbService.IsUserInConversation(dbContext, userID, conversationID))
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IdentificationNumberDoNotGrantAccess, ErrorMessage.IDENTIFICATION_NUMBER_DO_NOT_GRANT_ACCESS);
                return response;
            }

            // Delete the conversation
            bool isDone = ConversationDbService.DeleteConversation(dbContext, conversationID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            return response;
        }
    }
}
