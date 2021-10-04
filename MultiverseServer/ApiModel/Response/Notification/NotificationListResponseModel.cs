using MultiverseServer.Database.MultiverseDbModel.AbstractDbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response.Notification
{
    public class NotificationListResponseModel
    {
        public IList<UserNotificationResponseModel> userNotificationList { get; set; }
        public IList<ConversationNotificationResponseModel> conversationNotificationList { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
        public int totalSize { get; set; }

        public static NotificationListResponseModel ToResponseModel(List<UserNotificationDbModel> userNotifications, List<ConversationNotificationDbModel> conversationNotifications, int offset, int count, int totalSize)
        {
            NotificationListResponseModel responseModel = new NotificationListResponseModel();
            responseModel.count = count;
            responseModel.offset = offset;
            responseModel.totalSize = totalSize;
            responseModel.userNotificationList = new List<UserNotificationResponseModel>();
            responseModel.conversationNotificationList = new List<ConversationNotificationResponseModel>();
            
            foreach (UserNotificationDbModel userNotification in userNotifications)
            {
                responseModel.userNotificationList.Add(UserNotificationResponseModel.ToResponseModel(userNotification));
            }
            foreach (ConversationNotificationDbModel conversationNotification in conversationNotifications)
            {
                responseModel.conversationNotificationList.Add(ConversationNotificationResponseModel.ToResponseModel(conversationNotification));
            }

            return responseModel;
        }
    }
}
