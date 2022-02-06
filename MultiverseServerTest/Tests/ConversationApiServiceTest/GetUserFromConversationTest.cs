using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response.User;
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
    public class GetUserFromConversationTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public GetUserFromConversationTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }


        [Fact]
        public void GetUserFromConversation_GetUser_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel
            {
                count = 10,
                offset = 0,
            };
            ApiResponse response = ConversationApiService.GetUserFromConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(3, model.totalSize);
            Assert.NotNull(model.users);
            Assert.Equal(3, model.users.Count);
            Assert.Equal(1, model.users[0].userID);
            Assert.Equal(2, model.users[1].userID);
            Assert.Equal(3, model.users[2].userID);
        }

        [Fact]
        public void GetUserFromConversation_GetUserWithOffset_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel
            {
                count = 10,
                offset = 1,
            };
            ApiResponse response = ConversationApiService.GetUserFromConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(1, model.offset);
            Assert.Equal(3, model.totalSize);
            Assert.NotNull(model.users);
            Assert.Equal(2, model.users.Count);
            Assert.Equal(2, model.users[0].userID);
            Assert.Equal(3, model.users[1].userID);
        }

        [Fact]
        public void GetUserFromConversation_BadListAccess_AccessCorrected()
        {
            ConversationDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel
            {
                count = 1000,
                offset = 1,
            };
            ApiResponse response = ConversationApiService.GetUserFromConversation(DbContext, 1, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(20, model.count);
            Assert.Equal(1, model.offset);
            Assert.Equal(3, model.totalSize);
            Assert.NotNull(model.users);
            Assert.Equal(2, model.users.Count);
            Assert.Equal(2, model.users[0].userID);
            Assert.Equal(3, model.users[1].userID);
        }

        [Fact]
        public void GetUserFromConversation_UserNotInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel
            {
                count = 10,
                offset = 0,
            };
            ApiResponse response = ConversationApiService.GetUserFromConversation(DbContext, 4, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void GetUserFromConversation_UserNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel
            {
                count = 10,
                offset = 0,
            };
            ApiResponse response = ConversationApiService.GetUserFromConversation(DbContext, 1000, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void GetUserFromConversation_ConversationNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ListRequestModel request = new ListRequestModel
            {
                count = 10,
                offset = 0,
            };
            ApiResponse response = ConversationApiService.GetUserFromConversation(DbContext, 1, 1000, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);

        }
    }
}
