using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Response;
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

namespace MultiverseServerTest.Tests.UserApiServiceTest
{
    [Collection("TestSetUp")]
    public class GetUserInfoTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public GetUserInfoTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetUserInfo_GetUser1Info_Pass()
        {
            UserWithRelationDbContext.TestSetUp(DbContext);

            ApiResponse response = UserApiService.GetUserInfo(DbContext, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserResponseModel), response.obj.GetType());
            UserResponseModel model = (UserResponseModel)response.obj;
            Assert.Equal(1, model.userID);
            Assert.Equal("Mike", model.firstname);
            Assert.Equal("Ward", model.lastname);
            Assert.Equal(1, model.nbrOfFollowed);
            Assert.Equal(1, model.nbrOfFollower);
        }

        [Fact]
        public void GetUserInfo_GetUser2Info_Pass()
        {
            UserWithRelationDbContext.TestSetUp(DbContext);

            ApiResponse response = UserApiService.GetUserInfo(DbContext, 2);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserResponseModel), response.obj.GetType());
            UserResponseModel model = (UserResponseModel)response.obj;
            Assert.Equal(2, model.userID);
            Assert.Equal("John", model.firstname);
            Assert.Equal("Doe", model.lastname);
            Assert.Equal(0, model.nbrOfFollowed);
            Assert.Equal(1, model.nbrOfFollower);
        }

        [Fact]
        public void GetUserInfo_GetInfoFromIdThatDoesNotExists_Failed()
        {
            UserWithRelationDbContext.TestSetUp(DbContext);

            ApiResponse response = UserApiService.GetUserInfo(DbContext, 1000);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal(model.errorType, (int) ErrorType.BadIdentificationNumber);
        }

        [Fact]
        public void GetUserOwnInfo_GetUserOwnInfo_OK()
        {
            UserWithRelationDbContext.TestSetUp(DbContext);

            ApiResponse response = UserApiService.GetUserOwnInfo(DbContext, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserOwnResponseModel), response.obj.GetType());
            UserOwnResponseModel model = (UserOwnResponseModel)response.obj;
            Assert.Equal(1, model.userID);
            Assert.Equal("Mike", model.firstname);
            Assert.Equal("Ward", model.lastname);
            Assert.Equal(1, model.nbrOfFollowed);
            Assert.Equal(1, model.nbrOfFollower);
            Assert.Equal(1, model.nbrOfRequestFollowed);
            Assert.Equal(0, model.nbrOfRequestFollower);
            Assert.Equal(1, model.nbrOfConversation);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
