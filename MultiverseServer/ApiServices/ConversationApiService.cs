using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Request.Conversation;
using MultiverseServer.ApiModel.Request.Util;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiModel.Response.Conversation;
using MultiverseServer.ApiModel.Response.User;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
using MultiverseServer.DbServices;
using MultiverseServer.Security.Json;
using MultiverseServer.Security.ListAccess;
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

            // Create the notification for each user in the conversation
            NotificationDbService.AddNotificationToUsers(dbContext, request.users, (byte)NotificationType.NEW_CONVERSATION, dbModel.conversationID);

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

            // Delete the notification if it still exists
            NotificationDbService.DeleteNotificationForUsers(dbContext, NotificationDbModel.GetConversationNotificationType(), conversationID);

            return response;
        }

        public static ApiResponse GetConversationList(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            // Get the conversation list
            IList<ConversationDbModel> convList = ConversationDbService.GetConversationList(dbContext, userID, request.offset, request.count);
            int totalNumber = ConversationDbService.GetNumberOfConversation(dbContext, userID);

            // Create the api obj
            ConversationListResponseModel apiModel = ConversationListResponseModel.ToApiModel(convList, request.count, request.offset, totalNumber);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse SendMessage(MultiverseDbContext dbContext, int userID, int conversationID, SendMessageRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            MessageApiModel apiModel = null;
            if (!String.IsNullOrWhiteSpace(request.message))
            {
                // Send the string message
                if (request.message.Count() > 250)
                {
                    response.code = (int)HttpStatusCode.BadRequest;
                    response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                    return response;
                }

                // Make sure the user is in the conversation
                if (!ConversationDbService.IsUserInConversation(dbContext, userID, conversationID))
                {
                    response.code = (int)HttpStatusCode.Forbidden;
                    response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                    return response;
                }

                // Send the message
                MessageDbModel dbModel = new MessageDbModel();
                dbModel.message = request.message;
                dbModel.messageType = (byte)MessageType.TEXT_MESSAGE;
                dbModel.publishedTime = DateTime.Now.Date;
                dbModel.authorID = userID;
                dbModel.conversationID = conversationID;
                bool isDone = ConversationDbService.SendMessage(dbContext, dbModel);
                if (!isDone)
                {
                    response.code = (int)HttpStatusCode.InternalServerError;
                    response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                    return response;
                }

                apiModel = MessageApiModel.ToApiModel(dbModel);
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Update notification or create it if it does not exists for all conversation users
            IList<int> userList = ConversationDbService.GetConversationUser(dbContext, conversationID);
            userList.Remove(userID);
            NotificationDbService.UpdateNotifications(dbContext, userList, (byte)NotificationType.NEW_MESSAGE, conversationID);

            response.obj = apiModel;
            return response;
        }

        public static ApiResponse DeleteMessage(MultiverseDbContext dbContext, int userID, int conversationID, int messageID)
        {
            ApiResponse response = new ApiResponse();

            // Make sure the user is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(dbContext, userID, conversationID);
            if (!isInConv)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            // Make sure the user is the author of the message
            bool isAuthor = ConversationDbService.IsAuthorOfMessage(dbContext, userID, messageID);
            if (!isAuthor)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Delete the conversation
            bool isDone = ConversationDbService.DeleteMessage(dbContext, messageID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            return response;
        }

        public static ApiResponse UpdateMessage(MultiverseDbContext dbContext, int userID, int conversationID, int messageID, UpdateMessageRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Get the message
            MessageDbModel dbModel = ConversationDbService.GetMessage(dbContext, messageID);
            if (dbModel == null)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            // Make sure the author is the user and that the right conversation is used
            if (dbModel.authorID != userID || dbModel.conversationID != conversationID)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Make sure the message is not empty
            if (String.IsNullOrWhiteSpace(request.message))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Update the message
            dbModel.message = request.message;
            bool isDone = ConversationDbService.UpdateMessage(dbContext, userID, messageID, dbModel);

            if (!isDone)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Get the message
            MessageApiModel apiModel = MessageApiModel.ToApiModel(dbModel);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse GetMessageList(MultiverseDbContext dbContext, int userID, int conversationID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            // Make sure the user is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(dbContext, userID, conversationID);
            if (!isInConv)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            // Get the message list
            IList<MessageDbModel> messageList = ConversationDbService.GetMessageList(dbContext, conversationID, request.offset, request.count);

            int totalNumber = ConversationDbService.GetNumberOfMessage(dbContext, conversationID);

            // Create the api obj
            MessageListResponseModel apiModel = MessageListResponseModel.ToApiModel(messageList, request.count, request.offset, totalNumber);
            response.obj = apiModel;

            return response;
        }

        public static ApiResponse GetUserFromConversation(MultiverseDbContext dbContext, int userID, int conversationID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Verify the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            // Make sure the user is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(dbContext, userID, conversationID);
            if (!isInConv)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.BadIdentificationNumber, ErrorMessage.BAD_IDENTIFICATION_NUMBER);
                return response;
            }

            // Get the list of user in the conversation
            IList<UserDbModel> userDbList = ConversationDbService.GetConversationUser(dbContext, conversationID, request.offset, request.count);
            int totalSize = ConversationDbService.GetNumberOfUser(dbContext, conversationID);

            UserListResponseModel responseModel = UserListResponseModel.ToApiModel(userDbList, request.count, request.offset, totalSize);
            response.obj = responseModel;

            return response;
        }

        public static ApiResponse AddUsersToConversation(MultiverseDbContext dbContext, int userID, int conversationID, IDListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Make sure the user doing the action is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(dbContext, userID, conversationID);
            if (!isInConv)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Add users to conversation
            bool isDone = ConversationDbService.AddUsersToConversation(dbContext, request.idList, conversationID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Create the notification for each new user in the conversation
            NotificationDbService.AddNotificationToUsers(dbContext, request.idList, (byte)NotificationType.ADDED_IN_CONVERSATION, conversationID);

            return response;
        }

        public static ApiResponse RemoveUsersFromConversation(MultiverseDbContext dbContext, int userID, int conversationID, IDListRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Make sure the user doing the action is in the conversation
            bool isInConv = ConversationDbService.IsUserInConversation(dbContext, userID, conversationID);
            if (!isInConv)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Remove the users
            bool isDone = ConversationDbService.RemoveUsersFromConversation(dbContext, request.idList, conversationID);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }

            // Delete the notification for each remove user in the conversation
            NotificationDbService.DeleteNotificationForUsers(dbContext, request.idList, NotificationDbModel.GetConversationNotificationType(), conversationID);

            return response;
        }
    }
}
