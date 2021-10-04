using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Database.MultiverseDbModel
{
    public class NotificationDbModel
    {
        public int notificationID { get; set; }
        public int targetUserID { get; set; }
        public DateTime date { get; set; }
        public byte notificationType { get; set; }
        public int objectID { get; set; }

        public bool IsUserTargetOfNotification()
        {
            return notificationType == (byte)NotificationType.NEW_FOLLOWER_REQ || notificationType == (byte)NotificationType.NEW_FOLLOWED;
        }

        public bool IsConversationTargetOfNotification()
        {
            return notificationType == (byte)NotificationType.NEW_CONVERSATION || notificationType == (byte)NotificationType.NEW_MESSAGE;
        }

        public static IList<byte> GetUserNotificationType()
        {
            return new List<byte>() { (byte)NotificationType.NEW_FOLLOWER_REQ, (byte)NotificationType.NEW_FOLLOWED };
        }

        public static IList<byte> GetConversationNotificationType()
        {
            return new List<byte>() { (byte)NotificationType.NEW_CONVERSATION, (byte)NotificationType.NEW_MESSAGE, (byte) NotificationType.ADDED_IN_CONVERSATION };
        }
    }

    public enum NotificationType : byte
    {
        SYSTEM_NOTIF = 0,
        NEW_FOLLOWER_REQ = 1,
        NEW_FOLLOWED = 2,
        NEW_CONVERSATION = 3,
        NEW_MESSAGE = 4,
        ADDED_IN_CONVERSATION = 5,
    }
}
