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
    public class GetConversationListTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public GetConversationListTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetConversationList_GetConv_Pass()
        {
            ConversationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 2,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetConversationList(DbContext, 3, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(ConversationListResponseModel), response.obj.GetType());
            ConversationListResponseModel model = (ConversationListResponseModel)response.obj;
            Assert.Equal(2, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(3, model.totalSize);
            Assert.Equal(2, model.conversations.Count);
            Assert.Equal(1, model.conversations[0].conversationID);
            Assert.Equal(2, model.conversations[1].conversationID);
        }

        [Fact]
        public void GetConversationList_GetConvWithOffset_Pass()
        {
            ConversationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 2,
                offset = 2,
            };

            ApiResponse response = ConversationApiService.GetConversationList(DbContext, 3, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(ConversationListResponseModel), response.obj.GetType());
            ConversationListResponseModel model = (ConversationListResponseModel)response.obj;
            Assert.Equal(2, model.count);
            Assert.Equal(2, model.offset);
            Assert.Equal(3, model.totalSize);
            Assert.Equal(1, model.conversations.Count);
            Assert.Equal(3, model.conversations[0].conversationID);
        }

        [Fact]
        public void GetConversationList_GetConvWithTooBigOffset_Pass()
        {
            ConversationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 2,
                offset = 100,
            };

            ApiResponse response = ConversationApiService.GetConversationList(DbContext, 3, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(ConversationListResponseModel), response.obj.GetType());
            ConversationListResponseModel model = (ConversationListResponseModel)response.obj;
            Assert.Equal(2, model.count);
            Assert.Equal(100, model.offset);
            Assert.Equal(3, model.totalSize);
            Assert.Equal(0, model.conversations.Count);
        }

        [Fact]
        public void GetConversationList_GetConvOfUserWithNoConversation_Pass()
        {
            ConversationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 2,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetConversationList(DbContext, 6, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(ConversationListResponseModel), response.obj.GetType());
            ConversationListResponseModel model = (ConversationListResponseModel)response.obj;
            Assert.Equal(2, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(0, model.totalSize);
            Assert.Equal(0, model.conversations.Count);
        }

        [Fact]
        public void GetConversationList_BadListSettings_CountIsRestrained()
        {
            ConversationDbContext.SetUp(DbContext);
            ListRequestModel request = new ListRequestModel()
            {
                count = 50,
                offset = 0,
            };

            ApiResponse response = ConversationApiService.GetConversationList(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(ConversationListResponseModel), response.obj.GetType());
            ConversationListResponseModel model = (ConversationListResponseModel)response.obj;
            Assert.Equal(20, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(1, model.totalSize);
            Assert.Equal(1, model.conversations.Count);
            Assert.Equal(1, model.conversations[0].conversationID);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
