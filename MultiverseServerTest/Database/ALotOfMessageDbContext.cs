using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database
{
    class ALotOfMessageDbContext
    {
        private ALotOfMessageDbContext()
        {

        }

        public static void SetUp(MultiverseDbContext dbContext)
        {
            // Add user
            int i = 1;
            for(i = 0; i < 2; i++)
            {
                UserDbModel dbModel = new UserDbModel
                {
                    firstname = "firstname_user" + i.ToString(),
                    lastname = "lastname_user" + i.ToString(),
                    email = "email_user" + i.ToString(),
                    password = "password_user" + i.ToString(),
                    lastLocation = new NetTopologySuite.Geometries.Point(0, 0),
                };
                dbContext.user.Add(dbModel);
                dbContext.SaveChanges();
            }
            

            // Add conversation
            for(i = 0; i < 2; i++)
            {
                ConversationDbModel model = new ConversationDbModel()
                {
                    name = "conversation_" + i,
                    lastUpdate = DateTime.Now,
                };
                dbContext.conversation.Add(model);
                dbContext.SaveChanges();
            }
            

            // Add user to conv
            ConversationUserDbModel convUdb = new ConversationUserDbModel()
            {
                conversationID = 1,
                userID = 1
            };
            dbContext.conversationUser.Add(convUdb);
            dbContext.SaveChanges();
            convUdb = new ConversationUserDbModel()
            {
                conversationID = 2,
                userID = 1
            };
            dbContext.conversationUser.Add(convUdb);
            dbContext.SaveChanges();

            // Add message
            for (i = 0; i < 10; i++)
            {
                MessageDbModel messageDbModel = new MessageDbModel
                {
                    authorID = 1,
                    conversationID = 1,
                    messageType = 0,
                    message = "message_" + i,
                    publishedTime = DateTime.Now.AddSeconds(-i * 10),
                };
                dbContext.message.Add(messageDbModel);
            }
                dbContext.SaveChanges();
        }
    }
}
