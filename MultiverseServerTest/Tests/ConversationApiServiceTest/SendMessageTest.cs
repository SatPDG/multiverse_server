using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request.Conversation;
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
    public  class SendMessageTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public SendMessageTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void SendMessage_SendTextMessage_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            SendMessageRequestModel request = new SendMessageRequestModel()
            {
                message = "Mon message!!",
            };

            ApiResponse response = ConversationApiService.SendMessage(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageApiModel), response.obj.GetType());
            MessageApiModel model = (MessageApiModel)response.obj;
            Assert.Equal(request.message, model.message);
            Assert.Equal(0, model.messageType);
            Assert.NotNull(model.publishedTime);
            Assert.NotEqual(0, model.messageID);
            Assert.Equal(1, model.authorID);

            MessageDbModel dbModel = DbContext.message.Find(model.messageID);
            Assert.NotNull(dbModel);
            Assert.Equal(0, dbModel.messageType);
            Assert.Equal(request.message, dbModel.message);
            Assert.Equal(model.publishedTime, dbModel.publishedTime.ToString());
            Assert.Equal(1, dbModel.authorID);
        }

        [Fact]
        public void SendMessage_SendTextMessage_NotificationSent()
        {
            ConversationDbContext.SetUp(DbContext);

            SendMessageRequestModel request = new SendMessageRequestModel()
            {
                message = "Mon message!!",
            };

            ApiResponse response = ConversationApiService.SendMessage(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageApiModel), response.obj.GetType());

            List<NotificationDbModel> notifList = DbContext.notification.Where(n => n.notificationType == (byte)NotificationType.NEW_MESSAGE && n.objectID == 1).OrderBy(n => n.targetUserID).ToList();
            Assert.Equal(2, notifList.Count);
            Assert.Equal(2, notifList[0].targetUserID);
            Assert.Equal((byte)NotificationType.NEW_MESSAGE, notifList[0].notificationType);
            Assert.NotNull(notifList[0].date);
            Assert.Equal(1, notifList[0].objectID);
            Assert.Equal(3, notifList[1].targetUserID);
            Assert.Equal((byte)NotificationType.NEW_MESSAGE, notifList[1].notificationType);
            Assert.NotNull(notifList[1].date);
            Assert.Equal(1, notifList[1].objectID);
        }

        [Fact]
        public void SendMessage_SendTextMessage_NotificationDateAreUpdated()
        {
            ConversationDbContext.SetUp(DbContext);

            SendMessageRequestModel request = new SendMessageRequestModel()
            {
                message = "Mon message!!",
            };
            DateTime date = DateTime.Now.AddMinutes(-15);
            NotificationDbModel notifModel = new NotificationDbModel()
            {
                date = date,
                notificationType = (byte)NotificationType.NEW_MESSAGE,
                targetUserID = 2,
                objectID = 1,
            };
            DbContext.notification.Add(notifModel);
            DbContext.SaveChanges();

            ApiResponse response = ConversationApiService.SendMessage(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageApiModel), response.obj.GetType());

            List<NotificationDbModel> notifList = DbContext.notification.Where(n => n.notificationType == (byte)NotificationType.NEW_MESSAGE && n.objectID == 1).OrderBy(n => n.targetUserID).ToList();
            Assert.Equal(2, notifList.Count);
            Assert.Equal(2, notifList[0].targetUserID);
            Assert.Equal((byte)NotificationType.NEW_MESSAGE, notifList[0].notificationType);
            Assert.NotEqual(date, notifList[0].date);
            Assert.Equal(1, notifList[0].objectID);
            Assert.Equal(3, notifList[1].targetUserID);
            Assert.Equal((byte)NotificationType.NEW_MESSAGE, notifList[1].notificationType);
            Assert.NotNull(notifList[1].date);
            Assert.Equal(1, notifList[1].objectID);
        }

        [Fact]
        public void SendMessage_SendTextMessageTooLong_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            string message = "";
            for (int i = 0; i < 300; i++)
                message += "m";

            SendMessageRequestModel request = new SendMessageRequestModel()
            {
                message = message,
            };

            ApiResponse response = ConversationApiService.SendMessage(DbContext, 1, 1, request);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);

            int size = DbContext.message.Count();
            Assert.Equal(15, size);
        }

        [Fact]
        public void SendMessage_SendMessageToConvUserIsNotIn_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            SendMessageRequestModel request = new SendMessageRequestModel()
            {
                message = "Mon message!!",
            };

            ApiResponse response = ConversationApiService.SendMessage(DbContext, 1, 2, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void SendMessage_ConvNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            SendMessageRequestModel request = new SendMessageRequestModel()
            {
                message = "Mon message!!",
            };

            ApiResponse response = ConversationApiService.SendMessage(DbContext, 1, 10, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void SendMessage_BadJson_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            SendMessageRequestModel request = new SendMessageRequestModel()
            {
                message = "",
            };

            ApiResponse response = ConversationApiService.SendMessage(DbContext, 1, 1, request);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
