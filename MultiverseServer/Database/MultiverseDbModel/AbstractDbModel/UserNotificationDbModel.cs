using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Database.MultiverseDbModel.AbstractDbModel
{
    public class UserNotificationDbModel
    {
        public int notificationID { get; set; }
        public DateTime notificationDate { get; set; }
        public byte notificationType { get; set; }
        public int userID { get; set; }
        public string userFirstname { get; set; }
        public string userLastname { get; set; }
    }
}
