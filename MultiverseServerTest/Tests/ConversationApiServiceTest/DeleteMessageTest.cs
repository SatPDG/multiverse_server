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
    public class DeleteMessageTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public DeleteMessageTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void DeleteMessage_DeleteMessage_Pass()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteMessage(DbContext, 1, 1, 3);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(EmptyResult), response.obj.GetType());

            int size = DbContext.message.Count();
            Assert.Equal(14, size);

            MessageDbModel dbModel = DbContext.message.Find(3);
            Assert.Null(dbModel);
        }

        [Fact]
        public void DeleteMessage_DeleteMessageInConvButUserIsNotAuthor_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteMessage(DbContext, 1, 1, 14);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);

            int size = DbContext.message.Where(m => m.conversationID == 1).Count();
            Assert.Equal(6, size);
        }

        [Fact]
        public void DeleteMessage_DeleteMessageInConvButUserIsNotInConv_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteMessage(DbContext, 4, 1, 3);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);

            int size = DbContext.message.Where(m => m.conversationID == 1).Count();
            Assert.Equal(6, size);
        }

        [Fact]
        public void DeleteMessage_MessageNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteMessage(DbContext, 1, 1, 100);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.IllegalAction, model.errorType);

            int size = DbContext.message.Where(m => m.conversationID == 1).Count();
            Assert.Equal(6, size);
        }

        [Fact]
        public void DeleteMessage_ConvNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteMessage(DbContext, 1, 100, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);

            int size = DbContext.message.Where(m => m.conversationID == 1).Count();
            Assert.Equal(6, size);
        }

        [Fact]
        public void DeleteMessage_UserNotExist_Failed()
        {
            ConversationDbContext.SetUp(DbContext);

            ApiResponse response = ConversationApiService.DeleteMessage(DbContext, 100, 1, 1);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.BadIdentificationNumber, model.errorType);

            int size = DbContext.message.Where(m => m.conversationID == 1).Count();
            Assert.Equal(6, size);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
