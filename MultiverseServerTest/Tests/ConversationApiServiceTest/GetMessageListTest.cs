using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response.Conversation;
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

namespace MultiverseServerTest.Tests.ConversationApiServiceTest
{
    [Collection("TestSetUp")]
    public class GetMessageListTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public GetMessageListTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetMessageList_GetMsg_Pass()
        {
            ALotOfMessageDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 5,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetMessageList(DbContext, 1, 1, request);
            
            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageListResponseModel), response.obj.GetType());
            MessageListResponseModel model = (MessageListResponseModel)response.obj;
            Assert.Equal(5, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(10, model.totalSize);
            Assert.Equal(5, model.messages.Count);
            Assert.Equal(10, model.messages[0].messageID);
            Assert.Equal(9, model.messages[1].messageID);
            Assert.Equal(8, model.messages[2].messageID);
            Assert.Equal(7, model.messages[3].messageID);
            Assert.Equal(6, model.messages[4].messageID);
        }

        [Fact]
        public void GetMessageList_GetMsgWithOffset_Pass()
        {
            ALotOfMessageDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 5,
                offset = 3,
            };

            ApiResponse response = ConversationApiService.GetMessageList(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageListResponseModel), response.obj.GetType());
            MessageListResponseModel model = (MessageListResponseModel)response.obj;
            Assert.Equal(5, model.count);
            Assert.Equal(3, model.offset);
            Assert.Equal(10, model.totalSize);
            Assert.Equal(5, model.messages.Count);
            Assert.Equal(7, model.messages[0].messageID);
            Assert.Equal(6, model.messages[1].messageID);
            Assert.Equal(5, model.messages[2].messageID);
            Assert.Equal(4, model.messages[3].messageID);
            Assert.Equal(3, model.messages[4].messageID);
        }

        [Fact]
        public void GetMessageList_ConversationWithNoMessage_Pass()
        {
            ALotOfMessageDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 5,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetMessageList(DbContext, 1, 2, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageListResponseModel), response.obj.GetType());
            MessageListResponseModel model = (MessageListResponseModel)response.obj;
            Assert.Equal(5, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(0, model.totalSize);
            Assert.Equal(0, model.messages.Count);
        }

        [Fact]
        public void GetMessageList_ConversationNotExist_Failed()
        {
            ALotOfMessageDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 5,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetMessageList(DbContext, 1, 100, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void GetMessageList_GetMessageOfUserNotInConv_Failed()
        {
            ALotOfMessageDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 5,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetMessageList(DbContext, 2, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void GetConversationList_BadListSettings_CountIsRestrained()
        {
            ALotOfMessageDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 50,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetMessageList(DbContext, 1,  1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(MessageListResponseModel), response.obj.GetType());
            MessageListResponseModel model = (MessageListResponseModel)response.obj;
            Assert.Equal(20, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(10, model.totalSize);
            Assert.Equal(10, model.messages.Count);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
