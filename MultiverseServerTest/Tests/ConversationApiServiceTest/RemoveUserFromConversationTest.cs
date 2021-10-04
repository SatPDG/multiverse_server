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
    public class RemoveUserFromConversationTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public RemoveUserFromConversationTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void RemoveUserFromConversation_RemoveUser_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 2, 3 },
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            List<ConversationUserDbModel> userList = DbContext.conversationUser.Where(cu => cu.conversationID == 1).OrderBy(cu => cu.userID).ToList();
            Assert.Single(userList);
            Assert.Equal(1, userList[0].userID);
        }

        [Fact]
        public void RemoveUserFromConversation_RemoveUser_NotificationsAreDeleted()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 2, 3 },
            };
            List<NotificationDbModel> list = new List<NotificationDbModel>();
            list.Add(new NotificationDbModel()
            {
                notificationType = (byte)NotificationType.NEW_CONVERSATION,
                date = DateTime.Now,
                targetUserID = 2,
                objectID = 1,
            });
            list.Add(new NotificationDbModel()
            {
                notificationType = (byte)NotificationType.ADDED_IN_CONVERSATION,
                date = DateTime.Now,
                targetUserID = 3,
                objectID = 1,
            });
            list.Add(new NotificationDbModel()
            {
                notificationType = (byte)NotificationType.NEW_FOLLOWED,
                date = DateTime.Now,
                targetUserID = 2,
                objectID = 1,
            });
            DbContext.notification.AddRange(list);
            DbContext.SaveChanges();

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.notification.Count();
            Assert.Equal(1, size);
        }

        [Fact]
        public void RemoveUserFromConversation_UserNotInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 2, 3, 5 },
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 1, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void RemoveUserFromConversation_BadJson_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = null,
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 1, 1, request);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);
        }

        [Fact]
        public void RemoveUserFromConversation_UserDoingActionNotInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 2, 3},
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 5, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void RemoveUserFromConversation_ConvNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 2, 3 },
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 1, 100, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void RemoveUserFromConversation_UserNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 2, 3 },
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 100, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void RemoveUserFromConversation_UserCallingFunctionRemoveHimself_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 1 },
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            List<ConversationUserDbModel> userList = DbContext.conversationUser.Where(cu => cu.conversationID == 1).OrderBy(cu => cu.userID).ToList();
            Assert.Equal(2, userList.Count);
            Assert.Equal(2, userList[0].userID);
            Assert.Equal(3, userList[1].userID);
        }

        [Fact]
        public void RemoveUserFromConversation_RemoveAllUserFromConversation_ConversationIsDeleted()
        {
            ConversationDbContext.SetUp(DbContext);

            IDListRequestModel request = new IDListRequestModel()
            {
                idList = new List<int>() { 1, 2, 3 },
            };

            ApiResponse response = ConversationApiService.RemoveUsersFromConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            ConversationDbModel convModel = DbContext.conversation.Find(1);
            Assert.Null(convModel);

            int size = DbContext.message.Where(m => m.conversationID == 1).Count();
            Assert.Equal(0, size);

            size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count(); ;
            Assert.Equal(0, size);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
