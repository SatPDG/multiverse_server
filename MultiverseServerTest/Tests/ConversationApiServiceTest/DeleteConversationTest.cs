using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
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
    public class DeleteConversationTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public DeleteConversationTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void DeleteConversation_DeleteConv_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteConversation(DbContext, 1, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            ConversationDbModel dbModel = DbContext.conversation.Find(1);
            Assert.Null(dbModel);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(0, size);

            size = DbContext.message.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(0, size);
        }

        [Fact]
        public void DeleteConversation_UserNotInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteConversation(DbContext, 5, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);

            ConversationDbModel dbModel = DbContext.conversation.Find(1);
            Assert.NotNull(dbModel);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(3, size);

            size = DbContext.message.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(5, size);
        }

        [Fact]
        public void DeleteConversation_UserNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteConversation(DbContext, 100, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);

            ConversationDbModel dbModel = DbContext.conversation.Find(1);
            Assert.NotNull(dbModel);

            int size = DbContext.conversationUser.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(3, size);

            size = DbContext.message.Where(cu => cu.conversationID == 1).Count();
            Assert.Equal(5, size);
        }

        [Fact]
        public void DeleteConversation_ConvNotExists_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteConversation(DbContext, 1, 100);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IdentificationNumberDoNotGrantAccess, model.errorType);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
