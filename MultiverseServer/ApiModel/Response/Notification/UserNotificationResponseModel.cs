using MultiverseServer.Database.MultiverseDbModel.AbstractDbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response.Notification
{
    public class UserNotificationResponseModel
    {
        public int notificationID { get; set; }
        public string notificationDate { get; set; }
        public byte notificationType { get; set; }
        public int userID { get; set; }
        public string userFirstname { get; set; }
        public string userLastname { get; set; }

        public static UserNotificationResponseModel ToResponseModel(UserNotificationDbModel dbModel)
        {
            UserNotificationResponseModel responseModel = new UserNotificationResponseModel()
            {
                notificationID = dbModel.notificationID,
                notificationDate = dbModel.notificationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                notificationType = dbModel.notificationType,
                userID = dbModel.userID,
                userFirstname = dbModel.userFirstname,
                userLastname = dbModel.userLastname,
            };
            return responseModel;
        }
    }
}
