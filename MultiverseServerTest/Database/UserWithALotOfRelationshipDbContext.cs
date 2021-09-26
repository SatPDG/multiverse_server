using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database
{
    class UserWithALotOfRelationshipDbContext
    {

        private UserWithALotOfRelationshipDbContext()
        {

        }

        public static void SetUp(MultiverseDbContext dbContext)
        {
            // Add users
            {
                for(int i = 1; i < 22; i++)
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
            }

            // Add relationships
            {
                // Add follower to user1
                for(int i = 2; i < 7; i++)
                {
                    RelationshipDbModel dbModel = new RelationshipDbModel()
                    {
                        followedID = i,
                        followerID = 1,
                    };
                    dbContext.relationship.Add(dbModel);
                }
                dbContext.SaveChanges();

                // Add followed to user1
                for(int i = 7; i < 13; i++)
                {
                    RelationshipDbModel dbModel = new RelationshipDbModel()
                    {
                        followedID = 1,
                        followerID = i,
                    };
                    dbContext.relationship.Add(dbModel);
                }
                dbContext.SaveChanges();

                // Add follower request to user1
                for(int i = 13; i < 16; i++)
                {
                    RelationshipRequestDbModel dbModel = new RelationshipRequestDbModel()
                    {
                        followedID = i,
                        followerID = 1,
                    };
                    dbContext.relationshipRequest.Add(dbModel);
                }
                dbContext.SaveChanges();

                // Add followed request to user1
                for(int i = 16; i < 21; i++)
                {
                    RelationshipRequestDbModel dbModel = new RelationshipRequestDbModel()
                    {
                        followedID = 1,
                        followerID = i,
                    };
                    dbContext.relationshipRequest.Add(dbModel);
                }
                dbContext.SaveChanges();
            }
        }
    }
}
