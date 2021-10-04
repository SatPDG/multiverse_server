using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.Database.MultiverseDbModel.AbstractDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DbServices
{
    public class NotificationDbService
    {
        private NotificationDbService()
        {

        }

        public static bool AddNotification(MultiverseDbContext dbContext, int userID, byte notificationType, int objectID)
        {
            // Create the notification
            NotificationDbModel dbModel = new NotificationDbModel()
            {
                targetUserID = userID,
                date = DateTime.Now,
                notificationType = notificationType,
                objectID = objectID,
            };
            dbContext.notification.Add(dbModel);
            dbContext.SaveChanges();
            return true;
        }

        public static bool AddNotificationToUsers(MultiverseDbContext dbContext, IList<int> usersID, byte notificationType, int objectID)
        {
            List<NotificationDbModel> dbModelList = new List<NotificationDbModel>();
            foreach(int userID in usersID)
            {
                dbModelList.Add(new NotificationDbModel()
                {
                    date = DateTime.Now,
                    notificationType = notificationType,
                    targetUserID = userID,
                    objectID = objectID,
                });
            }
            dbContext.notification.AddRange(dbModelList);
            dbContext.SaveChanges();

            return true;
        }

        public static bool DeleteNotification(MultiverseDbContext dbContext, int userID, byte notificationType, int objectID)
        {
            NotificationDbModel dbModel = null;
            // Create the notification
            try
            {
                dbModel = dbContext.notification.Where(n => n.targetUserID == userID && n.notificationType == notificationType && n.objectID == objectID).First();
                dbContext.notification.Remove(dbModel);
                dbContext.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteNotificationForUsers(MultiverseDbContext dbContext, IList<byte> notificationsTypes, int objectID)
        {
            List<NotificationDbModel> notificationToRemove = dbContext.notification.Where(n => notificationsTypes.Contains(n.notificationType) && n.objectID == objectID).ToList();
            dbContext.notification.RemoveRange(notificationToRemove);
            dbContext.SaveChanges();
            return true;
        }

        public static bool DeleteNotificationForUsers(MultiverseDbContext dbContext, IList<int> usersID, IList<byte> notificationsTypes, int objectID)
        {
            List<NotificationDbModel> notificationToRemove = dbContext.notification.Where(n => usersID.Contains(n.targetUserID) && notificationsTypes.Contains(n.notificationType) && n.objectID == objectID).ToList();
            dbContext.notification.RemoveRange(notificationToRemove);
            dbContext.SaveChanges();
            return true;
        }

        public static bool UpdateNotification(MultiverseDbContext dbContext, int userID, byte notificationType, int objectID)
        {
            // Check if the notification exists.
            NotificationDbModel dbModel = null;
            try
            {
                dbModel = dbContext.notification.Where(n => n.notificationType == notificationType && n.targetUserID == userID && n.objectID == objectID).First();

                dbModel.date = DateTime.Now;
                dbContext.notification.Update(dbModel);
                dbContext.SaveChanges();
            }
            catch
            {
                // Create the notification because it does not exists
                dbModel = new NotificationDbModel()
                {
                    targetUserID = userID,
                    date = DateTime.Now,
                    notificationType = notificationType,
                    objectID = objectID,
                };
                dbContext.notification.Add(dbModel);
                dbContext.SaveChanges();
            }
            return true;
        }

        public static bool UpdateNotifications(MultiverseDbContext dbContext, IList<int> usersID, byte notificationType, int objectID)
        {
            // Get existing notifications
            List<NotificationDbModel> notificationList = dbContext.notification.Where(n => n.notificationType == notificationType && usersID.Contains(n.targetUserID) && n.objectID == objectID).ToList();

            // Get the users that do not have the notification
            List<int> noNotifUserList = usersID.Where(u => !(notificationList.Select(n => n.targetUserID).Contains(u))).ToList();

            // Update the users that already have a notification
            notificationList.ForEach(n => n.date = DateTime.Now);
            dbContext.notification.UpdateRange(notificationList);

            if (noNotifUserList.Count > 0)
            {
                // Add the new notifications
                List<NotificationDbModel> newNotif = new List<NotificationDbModel>();
                foreach (int userID in noNotifUserList)
                {
                    newNotif.Add(new NotificationDbModel()
                    {
                        date = DateTime.Now,
                        notificationType = notificationType,
                        targetUserID = userID,
                        objectID = objectID,
                    });
                }
                dbContext.notification.AddRange(newNotif);
            }
            dbContext.SaveChanges();

            return true;
        }

        public static bool GetNotificationList(MultiverseDbContext dbContext, int userID, int offset, int count, out List<UserNotificationDbModel> userNotificationDbList, out List<ConversationNotificationDbModel> conversationNotificationDbList)
        {
            // Get the top notification
            List<NotificationDbModel> notificationList = dbContext.notification.Where(n => n.targetUserID == userID).OrderBy(n => n.date).Skip(offset).Take(count).ToList();

            // Sort the different notification
            List<int> userNotificationList = notificationList.Where(n => n.IsUserTargetOfNotification()).Select( n => n.notificationID).ToList();
            List<int> conversationNotificationList = notificationList.Where(n => n.IsConversationTargetOfNotification()).Select(n => n.notificationID).ToList();

            // Fetch user notification info
            userNotificationDbList = dbContext.notification.Where(n => userNotificationList.Contains(n.notificationID)).OrderBy(n => n.date)
                                                                            .Join(dbContext.user,
                                                                                  n => n.objectID,
                                                                                  u => u.userID,
                                                                                  (n, u) => new
                                                                                  {
                                                                                      n.notificationID,
                                                                                      n.date,
                                                                                      n.notificationType,
                                                                                      u.userID,
                                                                                      u.firstname,
                                                                                      u.lastname,
                                                                                  }).Select(n => new UserNotificationDbModel()
                                                                                  {
                                                                                      notificationID = n.notificationID,
                                                                                      notificationDate = n.date,
                                                                                      notificationType = n.notificationType,
                                                                                      userID = n.userID,
                                                                                      userFirstname = n.firstname,
                                                                                      userLastname = n.lastname,
                                                                                  }).ToList();
            conversationNotificationDbList = dbContext.notification.Where(n => conversationNotificationList.Contains(n.notificationID)).OrderBy(n => n.date)
                                                                                            .Join(dbContext.conversation,
                                                                                                  n => n.objectID,
                                                                                                  c => c.conversationID,
                                                                                                  (n, c) => new
                                                                                                  {
                                                                                                      n.notificationID,
                                                                                                      n.date,
                                                                                                      n.notificationType,
                                                                                                      c.conversationID,
                                                                                                      c.name,
                                                                                                  }).Select(n => new ConversationNotificationDbModel()
                                                                                                  {
                                                                                                      notificationID = n.notificationID,
                                                                                                      notificationDate = n.date,
                                                                                                      notificationType = n.notificationType,
                                                                                                      conversationID = n.conversationID,
                                                                                                      conversationName = n.name,
                                                                                                  }).ToList();
            return true;
        }

        public static int GetNumberOfNotificationForUser(MultiverseDbContext dbContext, int userID)
        {
            return dbContext.notification.Where(n => n.targetUserID == userID).Count();
        }
    }
}
