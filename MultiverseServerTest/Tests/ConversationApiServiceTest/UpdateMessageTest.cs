using Microsoft.AspNetCore.Mvc;
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
    public class UpdateMessageTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public UpdateMessageTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void UpdateMessage_UpdateMessage_Pass()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateMessageRequestModel request = new UpdateMessageRequestModel()
            {
                message = "newMessage",
            };

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, 1, 1, 3, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageApiModel), response.obj.GetType());
            MessageApiModel model = (MessageApiModel)response.obj;
            Assert.Equal(3, model.messageID);
            Assert.Equal(1, model.conversationID);
            Assert.Equal(1, model.authorID);
            Assert.Equal("newMessage", model.message);

            MessageDbModel dbModel = DbContext.message.Find(3);
            Assert.Equal(3, dbModel.messageID);
            Assert.Equal(0, dbModel.messageType);
            Assert.Equal("newMessage", dbModel.message);
            Assert.Equal(1, dbModel.authorID);
        }

        [Fact]
        public void UpdateMessage_BadJson_Failed()
        {

            ConversationDbContext.SetUp(DbContext);
            UpdateMessageRequestModel request = new UpdateMessageRequestModel()
            {
                message = "",
            };

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, 1, 1, 3, request);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);

            MessageDbModel dbModel = DbContext.message.Find(3);
            Assert.Equal(3, dbModel.messageID);
            Assert.Equal(0, dbModel.messageType);
            Assert.NotEqual("", dbModel.message);
        }

        [Fact]
        public void UpdateMessage_UserNotAuthorOfMessage_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateMessageRequestModel request = new UpdateMessageRequestModel()
            {
                message = "newMessage",
            };

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, 1, 2, 3, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void UpdateMessage_MessageIDNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateMessageRequestModel request = new UpdateMessageRequestModel()
            {
                message = "newMessage",
            };

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, 1, 1, 100, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void UpdateMessage_ConversationIDNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateMessageRequestModel request = new UpdateMessageRequestModel()
            {
                message = "newMessage",
            };

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, 1, 100, 3, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void UpdateMessage_UserIDNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateMessageRequestModel request = new UpdateMessageRequestModel()
            {
                message = "newMessage",
            };

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, 100, 1, 3, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void UpdateMessage_UpdateWithEmptyTextMessage_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateMessageRequestModel request = new UpdateMessageRequestModel()
            {
                message = "",
            };

            ApiResponse response = ConversationApiService.UpdateMessage(DbContext, 1, 1, 3, request);

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
