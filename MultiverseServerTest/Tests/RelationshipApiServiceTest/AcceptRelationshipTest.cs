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
    public class AcceptRelationshipTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public AcceptRelationshipTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void AcceptFollowerRequest_Accept_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.AcceptFollowerRequest(DbContext, 1, 13);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.relationshipRequest.Where(rr => rr.followerID == 1 && rr.followedID == 13).Count();
            Assert.Equal(0, size);

            size = DbContext.relationship.Where(r => r.followerID == 1 && r.followedID == 13).Count();
            Assert.Equal(1, size);
        }

        [Fact]
        public void AcceptFollowerRequest_RelationDoesNotExist_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.AcceptFollowerRequest(DbContext, 1, 2);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void AcceptFollowerRequest_UserNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.AcceptFollowerRequest(DbContext, 1, 100);

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
