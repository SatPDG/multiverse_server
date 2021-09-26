using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request.Conversation;
using MultiverseServer.ApiServices;
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
