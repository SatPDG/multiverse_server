using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseService
{
    public class RelationshipService
    {
        private RelationshipService()
        {

        }

        public static bool SendFriendshipRequest(MultiverseDbContext dbContext, int fromUserID, int toUserID)
        {
            // Make sure the request and the relation does not exist.
            int size = dbContext.relationshipRequest.Where(r => r.followerID == fromUserID && r.followedID == toUserID).Count();
            size += dbContext.relationship.Where(r => r.followerID == fromUserID && r.followedID == toUserID).Count();

            if (size == 0)
            {
                RelationshipRequestDbModel dbModel = new RelationshipRequestDbModel
                {
                    followerID = fromUserID,
                    followedID = toUserID,
                };

                dbContext.relationshipRequest.Add(dbModel);
                dbContext.SaveChanges();
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool AcceptFriendshipRequest(MultiverseDbContext dbContext, int fromUserID, int toUserID)
        {
            // Make sur the friendship request exist
            int size = dbContext.relationshipRequest.Where(r => r.followerID == fromUserID && r.followedID == toUserID).Count();
            if (size >= 1)
            {
                // Remove relation from request
                RelationshipRequestDbModel relationReq = new RelationshipRequestDbModel
                {
                    followerID = fromUserID,
                    followedID = toUserID,
                };
                dbContext.relationshipRequest.Remove(relationReq);

                // Add the relationship
                RelationshipDbModel relation = new RelationshipDbModel
                {
                    followerID = fromUserID,
                    followedID = toUserID,
                };
                dbContext.relationship.Add(relation);

                dbContext.SaveChanges();
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFriendshipRequest(MultiverseDbContext dbContext, int fromUserID, int toUserID)
        {
            // Make sur the friendship request exist
            int size = dbContext.relationshipRequest.Where(r => r.followerID == fromUserID && r.followedID == toUserID).Count();
            if (size >= 1)
            {
                // Remove relation from request
                RelationshipRequestDbModel relationReq = new RelationshipRequestDbModel
                {
                    followerID = fromUserID,
                    followedID = toUserID,
                };
                dbContext.relationshipRequest.Remove(relationReq);
                dbContext.SaveChanges();
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFriendship(MultiverseDbContext dbContext, int fromUserID, int toUserID)
        {
            // Make sur the friendship exist
            int size = dbContext.relationship.Where(r => r.followerID == fromUserID && r.followedID == toUserID).Count();
            if (size >= 1)
            {
                // Remove relation from request
                RelationshipDbModel relation = new RelationshipDbModel
                {
                    followerID = fromUserID,
                    followedID = toUserID,
                };
                dbContext.relationship.Remove(relation);
                dbContext.SaveChanges();
            }
            else
            {
                return false;
            }
            return true;
        }

        public static IList<UserDbModel> GetFollowerOfUser(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            IList<UserDbModel> userList = dbContext.relationship.Join(dbContext.user,
                                                                      r => r.followerID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Skip(offset).Take(count).ToList();

            return userList;
        }

        public static IList<UserDbModel> GetFollowedOfUser(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            IList<UserDbModel> userList = dbContext.relationship.Join(dbContext.user,
                                                                      r => r.followedID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Skip(offset).Take(count).ToList();

            return userList;
        }

        public static IList<UserDbModel> GetRequestFollowerOfUser(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            IList<UserDbModel> userList = dbContext.relationshipRequest.Join(dbContext.user,
                                                                      r => r.followerID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Skip(offset).Take(count).ToList();

            return userList;
        }

        public static IList<UserDbModel> GetRequestFollowedOfUser(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            IList<UserDbModel> userList = dbContext.relationshipRequest.Join(dbContext.user,
                                                                      r => r.followedID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Skip(offset).Take(count).ToList();

            return userList;
        }

        public static bool GetCommonFollowerOfUsers()
        {
            return false;
        }

        public static bool GetCommonFollowedOfUser()
        {
            return false;
        }

        public static int GetFollowerOfUserCount(MultiverseDbContext dbContext, int userID)
        {
            int size = dbContext.relationship.Join(dbContext.user,
                                                                      r => r.followerID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Count();

            return size;
        }

        public static int GetFollowedOfUserCount(MultiverseDbContext dbContext, int userID)
        {
            int size = dbContext.relationship.Join(dbContext.user,
                                                                      r => r.followedID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Count();

            return size;
        }

        public static int GetRequestFollowerOfUserCount(MultiverseDbContext dbContext, int userID)
        {
            int size = dbContext.relationshipRequest.Join(dbContext.user,
                                                                      r => r.followerID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Count();

            return size;
        }

        public static int GetRequestFollowedOfUserCount(MultiverseDbContext dbContext, int userID)
        {
            int size = dbContext.relationshipRequest.Join(dbContext.user,
                                                                      r => r.followedID,
                                                                      u => u.userID,
                                                                      (f, u) =>
                                                                      new
                                                                      {
                                                                          u.userID,
                                                                          u.firstname,
                                                                          u.lastname
                                                                      }).Select(u => new UserDbModel
                                                                      {
                                                                          userID = u.userID,
                                                                          firstname = u.firstname,
                                                                          lastname = u.lastname
                                                                      }).Count();

            return size;
        }

        public static bool IsFollowerOfUser(MultiverseDbContext dbContext, int followerID, int userID)
        {
            int size = dbContext.relationship.Where(r => r.followerID == followerID && r.followedID == userID).Count();

            return size >= 1;
        }

        public static bool IsFollowedOfUser(MultiverseDbContext dbContext, int followedID, int userID)
        {
            int size = dbContext.relationship.Where(r => r.followedID == followedID && r.followerID == userID).Count();

            return size >= 1;
        }

        public static bool IsRequestFollowerOfUser(MultiverseDbContext dbContext, int followerID, int userID)
        {
            int size = dbContext.relationshipRequest.Where(r => r.followerID == followerID && r.followedID == userID).Count();

            return size >= 1;
        }

        public static bool IsRequestFollowedOfUser(MultiverseDbContext dbContext, int followedID, int userID)
        {
            int size = dbContext.relationshipRequest.Where(r => r.followedID == followedID && r.followerID == userID).Count();

            return size >= 1;
        }
    }
}
