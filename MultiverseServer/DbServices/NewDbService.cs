using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.Database.MultiverseDbModel.AbstractDbModel;
using MultiverseServer.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DbServices
{
    public static class NewDbService
    {
        public static NewDbModel CreateNew(MultiverseDbContext dbContext, int userID, NewBroadcastType broadcastType, NewType newType, string message)
        {
            return null;
        }

        public static bool DeleteNew(MultiverseDbContext dbContext, int newID)
        {
            return false;
        }

        public static NewDbModel UpdateNew(MultiverseDbContext dbContext, int userID, NewBroadcastType broadcastType, NewType newType, string message)
        {
            return null;
        }

        public static IList<UserNewDbModel> GetNewsList(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            return null;
        }

        public static IList<UserNewDbModel> GetRelationshipNewsList(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            return null;
        }

        public static IList<UserNewDbModel> GetLocationNewsList(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            return null;
        }
    }
}
