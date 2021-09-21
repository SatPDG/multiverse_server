using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiServices;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.RequestModel;
using MultiverseServer.Security.Token;
using MultiverseServerTest.Database;
using MultiverseServerTest.Database.DatabaseContext;
using MultiverseServerTest.TestSetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MultiverseServerTest.Tests.AuthenticationControllerTest
{
    [Collection("TestSetUp")]
    public class LoginTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public LoginTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void Login_CredentialOK_Pass()
        {
            UserDbContext.SetUp(DbContext);

            LoginRequestModel request = new LoginRequestModel();
            request.email = "pitpit@hotmail.com";
            request.password = "feather";

            ApiResponse response = AuthenticationApiService.Login(Config, DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(LoginResponseModel), response.obj.GetType());
            LoginResponseModel model = (LoginResponseModel)response.obj;
            Assert.NotNull(model.token);
            JwtTokenService tokenService = new JwtTokenService(Config);
            Assert.True(tokenService.ValidateJwtTokenWithoutLifetime(model.token));
            Assert.Equal(3, int.Parse(tokenService.GetJwtClaim(model.token, "userID")));
            Assert.NotNull(model.refreshToken);

            UserDbModel dbModel = DbContext.user.Find(3);
            Assert.Equal("Bird", dbModel.firstname);
            Assert.Equal("Man", dbModel.lastname);
            Assert.Equal("pitpit@hotmail.com", dbModel.email);
            Assert.NotEqual("feather", dbModel.password);
            Assert.Equal(0, dbModel.lastLocation.X);
            Assert.Equal(0, dbModel.lastLocation.Y);

            AuthenticationDbModel authDbModel = DbContext.authentication.Where(a => a.userID == 3).Take(1).First();
            Assert.Equal(model.refreshToken, authDbModel.token);
            Assert.True(authDbModel.expireTime > DateTime.Now);

            UserDbContext.TearDown(DbContext); 
        }

        [Fact]
        public void Login_BadPassword_Failed()
        {
            UserDbContext.SetUp(DbContext);

            LoginRequestModel request = new LoginRequestModel();
            request.email = "pitpit@hotmail.com";
            request.password = "featherj";

            ApiResponse response = AuthenticationApiService.Login(Config, DbContext, request);

            Assert.Equal(401, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.CredentialNotValid, model.errorType);

            UserDbContext.TearDown(DbContext);
        }

        [Fact]
        public void Login_BadEmail_Failed()
        {
            UserDbContext.SetUp(DbContext);

            LoginRequestModel request = new LoginRequestModel();
            request.email = "pitpdsit@hotmail.com";
            request.password = "feather";

            ApiResponse response = AuthenticationApiService.Login(Config, DbContext, request);

            Assert.Equal(401, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.CredentialNotValid, model.errorType);

            UserDbContext.TearDown(DbContext);
        }

        [Fact]
        public void Login_BadJson_Failed()
        {
            UserDbContext.SetUp(DbContext);
            LoginRequestModel request = new LoginRequestModel();
            request.email = "pitpdsit@hotmail.com";
            request.password = "";

            ApiResponse response = AuthenticationApiService.Login(Config, DbContext, request);

            Assert.Equal(400, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);

            UserDbContext.TearDown(DbContext);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
