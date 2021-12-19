using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Request.New;
using MultiverseServer.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiServices
{
    public static class NewsApiService
    {
        public static ApiResponse SendNew(MultiverseDbContext dbContext, int userID, SendNewRequestModel request)
        {
            return null;
        }

        public static ApiResponse UpdateNew(MultiverseDbContext dbContext, int userID, int newID, UpdateNewRequestModel request)
        {
            return null;
        }

        public static ApiResponse DeleteNew(MultiverseDbContext dbContext, int userID, int newID)
        {
            return null;
        }

        public static ApiResponse GetNewsList(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            return null;
        }

        public static ApiResponse GetRelationshipNews(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            return null;
        }

        public static ApiResponse GetLocationNews(MultiverseDbContext dbContext, int userID, ListRequestModel request)
        {
            return null;
        }

    }
}
