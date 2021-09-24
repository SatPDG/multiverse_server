using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request.Conversation;
using MultiverseServer.ApiModel.Response;
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
    public class ConversationInfoTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public ConversationInfoTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetConversationInfo_GetConv_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.GetConversationInfo(DbContext, 1, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(ConversationResponseModel), response.obj.GetType());
            ConversationResponseModel model = (ConversationResponseModel)response.obj;
            Assert.Equal("conversation_1", model.name);
            Assert.Equal(ConversationDbContext.date.ToString(), model.lastUpdate);
            Assert.Equal(1, model.conversationID);
            Assert.Equal(3, model.nbrOfUser);
        }

        [Fact]
        public void GetConversationInfo_UserNotInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.GetConversationInfo(DbContext, 5, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);
        }

        [Fact]
        public void GetConversationInfo_ConvNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.GetConversationInfo(DbContext, 1, 6);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);
        }

        [Fact]
        public void GetConversationInfo_UserNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.GetConversationInfo(DbContext, 8, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);
        }

        [Fact]
        public void SetConversationInfo_SetInfo_Pass()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateConversationRequest request = new UpdateConversationRequest()
            {
                name = "newname",
            };

            ApiResponse response = ConversationApiService.SetConversationInfo(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            ConversationDbModel dbModel = DbContext.conversation.Find(1);
            Assert.Equal("newname", dbModel.name);
            Assert.NotEqual(ConversationDbContext.date, dbModel.lastUpdate);
        }

        [Fact]
        public void SetConversationInfo_UserNotInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateConversationRequest request = new UpdateConversationRequest()
            {
                name = "newname",
            };

            ApiResponse response = ConversationApiService.SetConversationInfo(DbContext, 5, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);
        }

        [Fact]
        public void SetConversationInfo_ConvNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateConversationRequest request = new UpdateConversationRequest()
            {
                name = "newname",
            };

            ApiResponse response = ConversationApiService.SetConversationInfo(DbContext, 1, 10, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);
        }

        [Fact]
        public void SetConversationInfo_UserNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateConversationRequest request = new UpdateConversationRequest()
            {
                name = "newname",
            };

            ApiResponse response = ConversationApiService.SetConversationInfo(DbContext, 10, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);
        }

        [Fact]
        public void SetConversationInfo_BadJson_Failed()
        {
            ConversationDbContext.SetUp(DbContext);
            UpdateConversationRequest request = new UpdateConversationRequest()
            {
                name = "",
            };

            ApiResponse response = ConversationApiService.SetConversationInfo(DbContext, 1, 1, request);

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
