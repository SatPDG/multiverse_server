using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database
{
    static class NotificationDbContext
    {
        public static void SetUp(MultiverseDbContext dbContext)
        {
            // Create 3 users
            for (int i = 1; i < 4; i++)
            {
                UserDbModel dbModel = new UserDbModel()
                {
                    firstname = "firstname_" + i,
                    lastname = "lastname_" + i,
                    email = "email_" + i,
                    password = "password_" + i,
                    lastLocation = new NetTopologySuite.Geometries.Point(0, 0),
                };
                dbContext.user.Add(dbModel);
            }
            dbContext.SaveChanges();

            // Create 2 conversations
            for (int i = 1; i < 3; i++)
            {
                ConversationDbModel model = new ConversationDbModel()
                {
                    name = "conversation_" + i,
                    lastUpdate = DateTime.Now,
                };
                dbContext.conversation.Add(model);
                dbContext.SaveChanges();
            }

            // Create 2 users notifications
            for(int i = 1; i < 3; i++)
            {
                NotificationDbModel model = new NotificationDbModel()
                {
                    targetUserID = 1,
                    objectID = i+1,
                };

                if(i == 1)
                {
                    model.notificationType = (byte)NotificationType.NEW_FOLLOWER_REQ;
                    model.date = DateTime.Now.AddMinutes(15);
                }
                else
                {
                    model.notificationType = (byte)NotificationType.NEW_FOLLOWED;
                    model.date = DateTime.Now.AddMinutes(30);
                }

                dbContext.notification.Add(model);
            }
            dbContext.SaveChanges();

            // Create 2 conversation notifications
            for (int i = 1; i < 3; i++)
            {
                NotificationDbModel model = new NotificationDbModel()
                {
                    date = DateTime.Now,
                    targetUserID = 1,
                    objectID = i,
                };
                if (i == 1)
                {
                    model.notificationType = (byte)NotificationType.NEW_CONVERSATION;
                    model.date = DateTime.Now.AddMinutes(45);
                }
                else
                {
                    model.notificationType = (byte)NotificationType.NEW_MESSAGE;
                    model.date = DateTime.Now.AddMinutes(50);
                }
                dbContext.notification.Add(model);
            }
            dbContext.SaveChanges();
        }
    }
}
