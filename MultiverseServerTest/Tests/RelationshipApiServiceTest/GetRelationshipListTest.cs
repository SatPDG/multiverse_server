using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response.User;
using MultiverseServer.ApiServices;
using MultiverseServer.DatabaseContext;
using MultiverseServerTest.Database;
using MultiverseServerTest.TestSetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MultiverseServerTest.Tests.RelationshipApiServiceTest
{
    [Collection("TestSetUp")]
    public class GetRelationshipListTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public GetRelationshipListTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetFollowerList_GetList_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 20,
            };

            ApiResponse response = RelationshipApiService.GetFollowerList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(6, model.totalSize);
            Assert.Equal(6, model.users.Count);

            int[] userIDList = new int[] { 7, 8, 9, 10, 11, 12 };
            for(int i = 0; i < 6; i++)
            {
                Assert.Contains(model.users[i].userID, userIDList);
            }
        }

        [Fact]
        public void GetFollowerList_BadListAccess_CountIsCorrected()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 30,
            };

            ApiResponse response = RelationshipApiService.GetFollowerList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(6, model.totalSize);
            Assert.Equal(6, model.users.Count);

            int[] userIDList = new int[] { 7, 8, 9, 10, 11, 12 };
            for (int i = 0; i < 6; i++)
            {
                Assert.Contains(model.users[i].userID, userIDList);
            }
        }

        [Fact]
        public void GetFollowedList_GetList_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 20,
            };

            ApiResponse response = RelationshipApiService.GetFollowedList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(5, model.totalSize);
            Assert.Equal(5, model.users.Count);

            int[] userIDList = new int[] { 2, 3, 4, 5, 6 };
            for (int i = 0; i < 5; i++)
            {
                Assert.Contains(model.users[i].userID, userIDList);
            }
        }

        [Fact]
        public void GetFollowedList_BadListAccess_CountIsCorrected()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 30,
            };

            ApiResponse response = RelationshipApiService.GetFollowedList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(5, model.totalSize);
            Assert.Equal(5, model.users.Count);

            int[] userIDList = new int[] { 2, 3, 4, 5, 6 };
            for (int i = 0; i < 5; i++)
            {
                Assert.Contains(model.users[i].userID, userIDList);
            }
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
