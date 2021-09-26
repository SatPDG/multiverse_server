using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response.User;
using MultiverseServer.ApiServices;
using MultiverseServer.DatabaseContext;
using MultiverseServerTest.Database;
using MultiverseServerTest.TestSetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MultiverseServerTest.Tests.RelationshipApiServiceTest
{
    [Collection("TestSetUp")]
    public class GetRelationshipRequestTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public GetRelationshipRequestTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetFollowerRequestList_GetList_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 20,
            };

            ApiResponse response = RelationshipApiService.GetFollowerRequestList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(5, model.totalSize);
            Assert.Equal(5, model.users.Count);

            int[] userIDList = new int[] { 16, 17, 18, 19, 20 };
            for (int i = 0; i < 5; i++)
            {
                Assert.Contains(model.users[i].userID, userIDList);
            }
        }

        [Fact]
        public void GetFollowerRequestList_BadListAccess_CountIsCorrected()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 30,
            };

            ApiResponse response = RelationshipApiService.GetFollowerRequestList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(5, model.totalSize);
            Assert.Equal(5, model.users.Count);

            int[] userIDList = new int[] { 16, 17, 18, 19, 20 };
            for (int i = 0; i < 5; i++)
            {
                Assert.Contains(model.users[i].userID, userIDList);
            }
        }

        [Fact]
        public void GetFollowedRequestList_GetList_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 20,
            };

            ApiResponse response = RelationshipApiService.GetFollowedRequestList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(3, model.totalSize);
            Assert.Equal(3, model.users.Count);

            int[] userIDList = new int[] { 13, 14, 15 };
            for (int i = 0; i < 3; i++)
            {
                Assert.Contains(model.users[i].userID, userIDList);
            }
        }

        [Fact]
        public void GetFollowedRequestList_BadListAccess_CountIsCorrected()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel()
            {
                offset = 0,
                count = 30,
            };

            ApiResponse response = RelationshipApiService.GetFollowedRequestList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.count);
            Assert.Equal(3, model.totalSize);
            Assert.Equal(3, model.users.Count);

            int[] userIDList = new int[] { 13, 14, 15 };
            for (int i = 0; i < 3; i++)
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
