using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
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
    public class DeleteRelationshipRequestTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public DeleteRelationshipRequestTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void DeleteFollowerRequest_DeleteRequest_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowerRequest(DbContext, 1, 13);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.relationshipRequest.Where(rr => rr.followerID == 1 && rr.followedID == 13).Count();
            Assert.Equal(0, size);
        }

        [Fact]
        public void DeleteFollowerRequest_RequestNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowerRequest(DbContext, 1, 2);

            Assert.Equal((int) HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void DeleteFollowerRequest_UserNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowerRequest(DbContext, 1, 100);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void DeleteFollowed_DeleteRequest_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowedRequest(DbContext, 16, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.relationshipRequest.Where(rr => rr.followerID == 16 && rr.followedID == 1).Count();
            Assert.Equal(0, size);
        }

        [Fact]
        public void DeleteFollowedRequest_RequestNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowerRequest(DbContext, 2, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void DeleteFollowedRequest_UserNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowerRequest(DbContext, 100, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
