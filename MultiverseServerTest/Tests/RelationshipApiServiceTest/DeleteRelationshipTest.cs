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
    public class DeleteRelationshipTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public DeleteRelationshipTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void DeleteFollower_DeleteFollower_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollower(DbContext, 7, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.relationship.Where(r => r.followerID == 7 && r.followedID == 1).Count();
            Assert.Equal(0, size);
        }

        [Fact]
        public void DeleteFollower_RelationshipDoesNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollower(DbContext, 2, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void DeleteFollower_UserDoesNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollower(DbContext, 100, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void DeleteFollowed_DeleteFollowed_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowed(DbContext, 1, 2);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.relationship.Where(r => r.followerID == 1 && r.followedID == 2).Count();
            Assert.Equal(0, size);
        }

        [Fact]
        public void DeleteFollowed_RelationshipDoesNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowed(DbContext, 1, 7);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void DeleteFollowed_UserDoesNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.DeleteFollowed(DbContext, 1, 100);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
