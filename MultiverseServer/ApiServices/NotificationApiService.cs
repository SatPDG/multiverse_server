using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response.Notification;
using MultiverseServer.Database.MultiverseDbModel.AbstractDbModel;
using MultiverseServer.DatabaseContext;
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
    public static class NotificationApiService
    {

        public static ApiResponse GetNotificationList(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            ApiResponse response = new ApiResponse();
            // Validate the list access
            if (!ListAccessValidator.NormalizeListAccess(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.BadListAccess, ErrorMessage.BAD_LIST_ACCESS);
                return response;
            }

            List<UserNotificationDbModel> userNotification = null;
            List<ConversationNotificationDbModel> conversationNotification = null;
            bool isDone = NotificationDbService.GetNotificationList(dbContext, userID, request.offset, request.count, out userNotification, out conversationNotification);
            if (!isDone)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.IllegalAction, ErrorMessage.ILLEGAL_ACTION);
                return response;
            }
            int totalSize = NotificationDbService.GetNumberOfNotificationForUser(dbContext, userID);

            // Build the api response
            NotificationListResponseModel apiModel = NotificationListResponseModel.ToResponseModel(userNotification, conversationNotification, request.offset, request.count, totalSize);
            response.obj = apiModel;

            return response;
        }

    }
}
