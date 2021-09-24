using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request.Conversation;
using MultiverseServer.ApiServices;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServerTest.Database;
using MultiverseServerTest.Database.DatabaseContext;
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
    public class CreateConversationTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public CreateConversationTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void CreateConversation_CreateConv_Pass()
        {
            UserDbContext.SetUp(DbContext);
            CreateConversationRequest request = new CreateConversationRequest()
            {
                name = "myConv",
                users = new List<int> { 1, 2, 3, 4 },
            };

            ApiResponse response = ConversationApiService.CreateConversation(DbContext, 1, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(ConversationApiModel), response.obj.GetType());
            ConversationApiModel model = (ConversationApiModel)response.obj;
            Assert.Equal(1, model.conversationID);
            Assert.Equal("myConv", model.name);
            Assert.False(String.IsNullOrEmpty(model.lastUpdate));

            ConversationDbModel dbModel = DbContext.conversation.Find(1);
            Assert.Equal("myConv", dbModel.name);
            Assert.Equal(model.lastUpdate, dbModel.lastUpdate.ToString());

            IList<ConversationUserDbModel> userList = DbContext.conversationUser.Where(cu => cu.conversationID == dbModel.conversationID).OrderBy(cu => cu.userID).ToList();
            Assert.Equal(4, userList.Count);
            Assert.Equal(1, userList[0].userID);
            Assert.Equal(2, userList[1].userID);
            Assert.Equal(3, userList[2].userID);
            Assert.Equal(4, userList[3].userID);
        }

        [Fact]
        public void CreateConversation_BadJson1_Failed()
        {
            UserDbContext.SetUp(DbContext);
            CreateConversationRequest request = new CreateConversationRequest()
            {
                name = "",
                users = new List<int> { 1, 2, 3, 4 },
            };

            ApiResponse response = ConversationApiService.CreateConversation(DbContext, 1, request);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);
        }

        [Fact]
        public void CreateConversation_BadJson2_Failed()
        {
            UserDbContext.SetUp(DbContext);
            CreateConversationRequest request = new CreateConversationRequest()
            {
                name = "conv",
                users = null,
            };

            ApiResponse response = ConversationApiService.CreateConversation(DbContext, 1, request);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);
        }

        [Fact]
        public void CreateConversation_UserNotExist_Failed()
        {
            UserDbContext.SetUp(DbContext);
            CreateConversationRequest request = new CreateConversationRequest()
            {
                name = "myConv",
                users = new List<int> { 1, 2, 3, 100 },
            };

            ApiResponse response = ConversationApiService.CreateConversation(DbContext, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);
        }

        [Fact]
        public void CreateConversation_NoUser_Failed()
        {
            UserDbContext.SetUp(DbContext);
            CreateConversationRequest request = new CreateConversationRequest()
            {
                name = "myConv",
                users = new List<int> { },
            };

            ApiResponse response = ConversationApiService.CreateConversation(DbContext, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        [Fact]
        public void CreateConversation_CreatorUserNotInList_Failed()
        {
            UserDbContext.SetUp(DbContext);
            CreateConversationRequest request = new CreateConversationRequest()
            {
                name = "myConv",
                users = new List<int> {2, 3, 4 },
            };

            ApiResponse response = ConversationApiService.CreateConversation(DbContext, 1, request);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
