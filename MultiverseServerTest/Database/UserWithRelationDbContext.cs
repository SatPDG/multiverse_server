using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.Security.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database
{
    class UserWithRelationDbContext
    {
        private UserWithRelationDbContext()
        {

        }

        public static void TestSetUp(MultiverseDbContext dbContext)
        {
            SHA256Hash hash = new SHA256Hash();
            // Create 4 users
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "Mike";
                dbModel.lastname = "Ward";
                dbModel.email = "mikeward@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("fuckUJeremy");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "John";
                dbModel.lastname = "Doe";
                dbModel.email = "doe@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("nothing");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "Bird";
                dbModel.lastname = "Man";
                dbModel.email = "pitpit@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("feather");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "Morty";
                dbModel.lastname = "Smith";
                dbModel.email = "smith@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("i8grandpa");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            dbContext.SaveChanges();

            // Create relation
            {
                // user1 follow user2
                RelationshipDbModel dbModel = new RelationshipDbModel();
                dbModel.followerID = 1;
                dbModel.followedID = 2;
                dbContext.relationship.Add(dbModel);
            }
            {
                // user1 send follow request to user3
                RelationshipRequestDbModel dbModel = new RelationshipRequestDbModel();
                dbModel.followerID = 1;
                dbModel.followedID = 3;
                dbContext.relationshipRequest.Add(dbModel);
            }
            {
                // user4 follow user1
                RelationshipDbModel dbModel = new RelationshipDbModel();
                dbModel.followerID = 4;
                dbModel.followedID = 1;
                dbContext.relationship.Add(dbModel);
            }
            dbContext.SaveChanges();

            // Create a conversation
            {
                ConversationDbModel convModel = new ConversationDbModel();
                convModel.name = "conv";
                convModel.lastUpdate = DateTime.Now;
                dbContext.conversation.Add(convModel);
                dbContext.SaveChanges();
                ConversationUserDbModel convUser = new ConversationUserDbModel();
                convUser.conversationID = 1;
                convUser.userID = 1;
                dbContext.conversationUser.Add(convUser);
                dbContext.SaveChanges();
            }
        }
    }
}
