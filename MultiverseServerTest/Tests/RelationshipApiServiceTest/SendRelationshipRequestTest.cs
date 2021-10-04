using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiServices;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
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
    public class SendRelationshipRequestTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public SendRelationshipRequestTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void SendRequest_SendRequest_Pass()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.SendRequest(DbContext, 1, 21);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            RelationshipRequestDbModel dbModel = DbContext.relationshipRequest.Where(rr => rr.followerID == 1 && rr.followedID == 21).First();
            Assert.NotNull(dbModel);
            Assert.Equal(1, dbModel.followerID);
            Assert.Equal(21, dbModel.followedID);
        }

        [Fact]
        public void SendRequest_SendRequest_NotificationSent()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.SendRequest(DbContext, 1, 21);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.notification.Count();
            Assert.Equal(1, size);

            NotificationDbModel dbmodel = DbContext.notification.Where(n => n.objectID == 1 && n.targetUserID == 21).First();
            Assert.Equal(21, dbmodel.targetUserID);
            Assert.Equal(1, dbmodel.objectID);
            Assert.NotNull(dbmodel.date);
            Assert.Equal((byte)NotificationType.NEW_FOLLOWER_REQ, dbmodel.notificationType);
        }

        [Fact]
        public void SendRequest_RequestAlreadyExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.SendRequest(DbContext, 1, 13);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void SendRequest_UserDoesNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.SendRequest(DbContext, 1, 100);

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
