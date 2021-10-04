using MultiverseServer.Database.MultiverseDbModel.AbstractDbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response.Notification
{
    public class ConversationNotificationResponseModel
    {
        public int notificationID { get; set; }
        public string notificationDate { get; set; }
        public byte notificationType { get; set; }
        public int conversationID { get; set; }
        public string conversationName { get; set; }

        public static ConversationNotificationResponseModel ToResponseModel(ConversationNotificationDbModel dbModel)
        {
            ConversationNotificationResponseModel responseModel = new ConversationNotificationResponseModel()
            {
                notificationID = dbModel.notificationID,
                notificationDate = dbModel.notificationDate.ToString(),
                notificationType = dbModel.notificationType,
                conversationID = dbModel.conversationID,
                conversationName = dbModel.conversationName,
            };
            return responseModel;
        }
    }
}
