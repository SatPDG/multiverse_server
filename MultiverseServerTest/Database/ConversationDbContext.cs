using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database
{
    public class ConversationDbContext
    {
        public static DateTime date;

        private ConversationDbContext()
        {

        }
        public static void SetUp(MultiverseDbContext dbContext)
        {
            date = DateTime.Now;
            // Add users
            for (int i = 1; i < 7; i++)
            {
                UserDbModel model = new UserDbModel()
                {
                    firstname = "firstname_" + i,
                    lastname = "lastname_" + i,
                    email = "email_" + i,
                    password = "password_" + i,
                    lastLocation = new NetTopologySuite.Geometries.Point(0, 0),
                };
                dbContext.user.Add(model);
                dbContext.SaveChanges();
            }

            // Add conversations
            for(int i = 1; i < 4; i++)
            {
                ConversationDbModel model = new ConversationDbModel()
                {
                    name = "conversation_" + i,
                    lastUpdate = date,
                };
                dbContext.conversation.Add(model);
                dbContext.SaveChanges();
            }


            // Add conversation users to conv
            for(int i = 1; i < 4; i++)
            {
                ConversationUserDbModel convU1 = new ConversationUserDbModel()
                {
                    conversationID = i,
                    userID = i,
                };
                ConversationUserDbModel convU2 = new ConversationUserDbModel()
                {
                    conversationID = i,
                    userID = i + 1,
                };
                ConversationUserDbModel convU3 = new ConversationUserDbModel()
                {
                    conversationID = i,
                    userID = i + 2,
                };
                dbContext.conversationUser.Add(convU1);
                dbContext.conversationUser.Add(convU2);
                dbContext.conversationUser.Add(convU3);
                dbContext.SaveChanges();
            }

            // Add message to conv
            for(int i = 1; i < 5; i++)
            {
                MessageDbModel mess1 = new MessageDbModel()
                {
                    authorID = 1,
                    messageType = 0,
                    conversationID = 1,
                    publishedTime = DateTime.Now,
                    message = "message_" + i,
                };
                MessageDbModel mess2 = new MessageDbModel()
                {
                    authorID = 2,
                    messageType = 0,
                    conversationID = 2,
                    publishedTime = DateTime.Now,
                    message = "message_" + i,
                };
                MessageDbModel mess3 = new MessageDbModel()
                {
                    authorID = 3,
                    messageType = 0,
                    conversationID = 3,
                    publishedTime = DateTime.Now,
                    message = "message_" + i,
                };
                dbContext.message.Add(mess1);
                dbContext.message.Add(mess2);
                dbContext.message.Add(mess3);
                dbContext.SaveChanges();
            }
            {
                MessageDbModel mess1 = new MessageDbModel()
                {
                    authorID = 2,
                    messageType = 0,
                    conversationID = 1,
                    publishedTime = DateTime.Now,
                    message = "message_extra1",
                };
                MessageDbModel mess2 = new MessageDbModel()
                {
                    authorID = 3,
                    messageType = 0,
                    conversationID = 1,
                    publishedTime = DateTime.Now,
                    message = "message_extra2",
                };
                MessageDbModel mess3 = new MessageDbModel()
                {
                    authorID = 3,
                    messageType = 0,
                    conversationID = 3,
                    publishedTime = DateTime.Now,
                    message = "message_extra3",
                };
                dbContext.message.Add(mess1);
                dbContext.message.Add(mess2);
                dbContext.message.Add(mess3);
                dbContext.SaveChanges();
            }


            dbContext.ChangeTracker.Clear();
        }
    }
}
