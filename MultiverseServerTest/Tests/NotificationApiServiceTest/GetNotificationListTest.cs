using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response.Notification;
using MultiverseServer.ApiServices;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DbServices;
using MultiverseServerTest.Database;
using MultiverseServerTest.TestSetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MultiverseServerTest.Tests.NotificationApiServiceTest
{
    [Collection("TestSetUp")]
    public class GetNotificationListTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public GetNotificationListTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetNotificationList_GetNotifList_Pass()
        {
            NotificationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 10,
                offset = 0,
            };

            ApiResponse response = NotificationApiService.GetNotificationList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(NotificationListResponseModel), response.obj.GetType());
            NotificationListResponseModel model = (NotificationListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(4, model.totalSize);
            Assert.Equal(2, model.userNotificationList.Count);
            Assert.Equal(2, model.conversationNotificationList.Count);

            UserNotificationResponseModel u1 = model.userNotificationList[0];
            Assert.Equal((byte)NotificationType.NEW_FOLLOWER_REQ, u1.notificationType);
            Assert.Equal(2, u1.userID);
            Assert.Equal("firstname_2", u1.userFirstname);
            Assert.Equal("lastname_2", u1.userLastname);
            Assert.False(String.IsNullOrWhiteSpace(u1.notificationDate));

            UserNotificationResponseModel u2 = model.userNotificationList[1];
            Assert.Equal((byte)NotificationType.NEW_FOLLOWED, u2.notificationType);
            Assert.Equal(3, u2.userID);
            Assert.Equal("firstname_3", u2.userFirstname);
            Assert.Equal("lastname_3", u2.userLastname);
            Assert.False(String.IsNullOrWhiteSpace(u2.notificationDate));

            ConversationNotificationResponseModel c1 = model.conversationNotificationList[0];
            Assert.Equal((byte)NotificationType.NEW_CONVERSATION, c1.notificationType);
            Assert.Equal(1, c1.conversationID);
            Assert.Equal("conversation_1", c1.conversationName);
            Assert.False(String.IsNullOrWhiteSpace(c1.notificationDate));

            ConversationNotificationResponseModel c2 = model.conversationNotificationList[1];
            Assert.Equal((byte)NotificationType.NEW_MESSAGE, c2.notificationType);
            Assert.Equal(2, c2.conversationID);
            Assert.Equal("conversation_2", c2.conversationName);
            Assert.False(String.IsNullOrWhiteSpace(c2.notificationDate));
        }

        [Fact]
        public void GetNotificationList_RequestWithOffset_Pass()
        {
            NotificationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 10,
                offset = 1,
            };

            ApiResponse response = NotificationApiService.GetNotificationList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(NotificationListResponseModel), response.obj.GetType());
            NotificationListResponseModel model = (NotificationListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(1, model.offset);
            Assert.Equal(4, model.totalSize);
            Assert.Equal(1, model.userNotificationList.Count);
            Assert.Equal(2, model.conversationNotificationList.Count);

            UserNotificationResponseModel u2 = model.userNotificationList[0];
            Assert.Equal((byte)NotificationType.NEW_FOLLOWED, u2.notificationType);
            Assert.Equal(3, u2.userID);
            Assert.Equal("firstname_3", u2.userFirstname);
            Assert.Equal("lastname_3", u2.userLastname);
            Assert.False(String.IsNullOrWhiteSpace(u2.notificationDate));

            ConversationNotificationResponseModel c1 = model.conversationNotificationList[0];
            Assert.Equal((byte)NotificationType.NEW_CONVERSATION, c1.notificationType);
            Assert.Equal(1, c1.conversationID);
            Assert.Equal("conversation_1", c1.conversationName);
            Assert.False(String.IsNullOrWhiteSpace(c1.notificationDate));

            ConversationNotificationResponseModel c2 = model.conversationNotificationList[1];
            Assert.Equal((byte)NotificationType.NEW_MESSAGE, c2.notificationType);
            Assert.Equal(2, c2.conversationID);
            Assert.Equal("conversation_2", c2.conversationName);
            Assert.False(String.IsNullOrWhiteSpace(c2.notificationDate));
        }

        [Fact]
        public void GetNotificationList_BadListAccess_ContCorrected()
        {
            NotificationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 30,
                offset = 3,
            };

            ApiResponse response = NotificationApiService.GetNotificationList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(NotificationListResponseModel), response.obj.GetType());
            NotificationListResponseModel model = (NotificationListResponseModel)response.obj;
            Assert.Equal(20, model.count);
            Assert.Equal(3, model.offset);
            Assert.Equal(4, model.totalSize);
            Assert.Equal(0, model.userNotificationList.Count);
            Assert.Equal(1, model.conversationNotificationList.Count);

            ConversationNotificationResponseModel c2 = model.conversationNotificationList[0];
            Assert.Equal((byte)NotificationType.NEW_MESSAGE, c2.notificationType);
            Assert.Equal(2, c2.conversationID);
            Assert.Equal("conversation_2", c2.conversationName);
            Assert.False(String.IsNullOrWhiteSpace(c2.notificationDate));

        }

            public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
