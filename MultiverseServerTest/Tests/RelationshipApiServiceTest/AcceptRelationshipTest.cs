using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiServices;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServerTest.Database;
using MultiverseServerTest.Database.DatabaseContext;
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

            ApiResponse response = RelationshipApiService.AcceptFollowedRequest(DbContext, 1, 13);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.relationshipRequest.Where(rr => rr.followerID == 1 && rr.followedID == 13).Count();
            Assert.Equal(0, size);

            size = DbContext.relationship.Where(r => r.followerID == 1 && r.followedID == 13).Count();
            Assert.Equal(1, size);
        }

        [Fact]
        public void AcceptFollowedRequest_Accept_NotifSent()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.AcceptFollowedRequest(DbContext, 1, 13);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.notification.Count();
            Assert.Equal(1, size);

            NotificationDbModel dbModel = DbContext.notification.Where(n => n.objectID == 1 && n.targetUserID == 13).First();
            Assert.NotNull(dbModel);
            Assert.Equal(1, dbModel.objectID);
            Assert.Equal(13, dbModel.targetUserID);
            Assert.Equal((byte)NotificationType.NEW_FOLLOWED, dbModel.notificationType);
            Assert.NotNull(dbModel.date);
        }

        [Fact]
        public void AcceptFollowedRequest_Accept_UserInfoOK()
        {
            UserDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.SendRequest(DbContext, 1, 2);
            RelationshipApiService.AcceptFollowedRequest(DbContext, 1, 2);
            response = UserApiService.GetUserInfo(DbContext, 2, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserResponseModel), response.obj.GetType());
            UserResponseModel model = (UserResponseModel)response.obj;
            Assert.False(model.isFollowerRequestPending);
            Assert.False(model.isFollowedRequestPending);
            Assert.False(model.isAFollower);
            Assert.True(model.isFollowed);
        }

        [Fact]
        public void AcceptFollowedRequest_RelationDoesNotExist_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.AcceptFollowedRequest(DbContext, 1, 2);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void AcceptFollowedRequest_UserNotExists_Failed()
        {
            UserWithALotOfRelationshipDbContext.SetUp(DbContext);

            ApiResponse response = RelationshipApiService.AcceptFollowedRequest(DbContext, 1, 100);

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
