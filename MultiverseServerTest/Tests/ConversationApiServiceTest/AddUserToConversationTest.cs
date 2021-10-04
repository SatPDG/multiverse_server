using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request.Util;
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

namespace MultiverseServerTest.Tests.ConversationApiServiceTest
{
    [Collection("TestSetUp")]
    public class AddUserToConversationTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public AddUserToConversationTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void AddUserToConversation_AddUser_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() {4, 5 },
            };

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            List<ConversationUserDbModel> userList = DbContext.conversationUser.Where(cu => cu.conversationID == 1).OrderBy(cu => cu.userID).ToList();
            Assert.Equal(5, userList.Count);
            for(int i = 0; i < 5; i++)
            {
                Assert.Equal(i+1, userList[i].userID);
            }
        }

        [Fact]
        public void AddUserToConversation_AddUser_TheNotificationIsSend()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 4, 5 },
            };

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.notification.Count();
            Assert.Equal(2, size);

            List<NotificationDbModel> notifList = DbContext.notification.Where(n => n.objectID == 1).OrderBy(n => n.targetUserID).ToList();
            Assert.Equal(4, notifList[0].targetUserID);
            Assert.Equal((byte)NotificationType.ADDED_IN_CONVERSATION, notifList[0].notificationType);
            Assert.NotNull(notifList[0].date);
            Assert.Equal(5, notifList[1].targetUserID);
            Assert.Equal((byte)NotificationType.ADDED_IN_CONVERSATION, notifList[1].notificationType);
            Assert.NotNull(notifList[1].date);
        }

        [Fact]
        public void AddUserToConversation_AddUserWithOneAlreadyInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() {1, 4, 5 },
            };

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, 1, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(3, size);
        }

        [Fact]
        public void AddUserToConversation_UserNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 4, 100 },
            };

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, 1, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(3, size);
        }

        [Fact]
        public void AddUserToConversation_BadJson_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = null,
            };

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, 1, 1, request);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(3, size);
        }

        [Fact]
        public void AddUserToConversation_ConversationNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 4, 5 },
            };

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, 1, 100, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(3, size);
        }

        [Fact]
        public void AddUserToConversation_UserNotInConversation_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 4, 5 },
            };

            ApiResponse response = ConversationApiService.AddUsersToConversation(DbContext, 5, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(3, size);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
