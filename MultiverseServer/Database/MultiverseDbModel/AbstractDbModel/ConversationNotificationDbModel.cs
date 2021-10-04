using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Database.MultiverseDbModel.AbstractDbModel
{
    public class ConversationNotificationDbModel
    {
        public int notificationID { get; set; }
        public DateTime notificationDate { get; set; }
        public byte notificationType { get; set; }
        public int conversationID { get; set; }
        public string conversationName { get; set; }
    }
}
