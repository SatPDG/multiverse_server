using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiServices;
using MultiverseServer.Controllers;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.ApiModel.Error;
using MultiverseServerTest.Database;
using MultiverseServerTest.TestSetUp;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using MultiverseServerTest.Database.DatabaseContext;
using MultiverseServer.Security.Token;

namespace MultiverseServerTest.AuthenticationControllerTest
{
    [Collection("TestSetUp")]
    public class RegisterTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public RegisterTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void Register_userRegister_OK()
        {
            RegisterRequestModel request = new RegisterRequestModel();
            request.firstname = "Mike";
            request.lastname = "Ward";
            request.email = "mike@hotmail.com";
            request.password = "FuckUJeremy";
            request.lastLocation = new LocationApiModel(1, 2);

            ApiResponse response = AuthenticationApiService.Register(Config, DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(RegisterResponseModel), response.obj.GetType());
            RegisterResponseModel model = (RegisterResponseModel)response.obj;
            Assert.NotNull(model.token);
            JwtTokenService tokenService = new JwtTokenService(Config);
            Assert.True(tokenService.ValidateJwtTokenWithoutLifetime(model.token));
            Assert.Equal(model.userID, int.Parse(tokenService.GetJwtClaim(model.token, "userID")));
            Assert.NotNull(model.refreshToken);

            UserDbModel dbModel = DbContext.user.Find(model.userID);
            Assert.Equal("Mike", dbModel.firstname);
            Assert.Equal("Ward", dbModel.lastname);
            Assert.Equal("mike@hotmail.com", dbModel.email);
            Assert.NotEqual("FuckUJeremy", dbModel.password);
            Assert.Equal(1, dbModel.lastLocation.X);
            Assert.Equal(2, dbModel.lastLocation.Y);

            AuthenticationDbModel authDbModel = DbContext.authentication.Where(a => a.userID == dbModel.userID).Take(1).First();
            Assert.Equal(model.refreshToken, authDbModel.token);
            Assert.True(authDbModel.expireTime > DateTime.Now);

            DbContext.user.Remove(dbModel);
            DbContext.SaveChanges();
        }

        [Fact]
        public void Register_BadJson_Failed()
        {
            UserDbContext.SetUp(DbContext);
            RegisterRequestModel request = new RegisterRequestModel();
            request.firstname = "Mike123";
            request.lastname = "";
            request.email = "mikeward@hotmail.com";
            request.password = "FuckUJeremy";
            request.lastLocation = new LocationApiModel(1, 2);

            ApiResponse response = AuthenticationApiService.Register(Config, DbContext, request);

            Assert.Equal(400, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.JsonNotValid, model.errorType);

            int size = DbContext.user.Count();
            Assert.Equal(4, size);

            UserDbContext.TearDown(DbContext);
        }

        [Fact]
        public void Register_EmailAlreadyExist_Failed()
        {
            UserDbContext.SetUp(DbContext);
            RegisterRequestModel request = new RegisterRequestModel();
            request.firstname = "Mike123";
            request.lastname = "Ward123";
            request.email = "mikeward@hotmail.com";
            request.password = "FuckUJeremy";
            request.lastLocation = new LocationApiModel(1, 2);

            ApiResponse response = AuthenticationApiService.Register(Config, DbContext, request);

            Assert.Equal(401, response.code);
            Assert.Equal(typeof(ErrorApiModel), response.obj.GetType());
            ErrorApiModel model = (ErrorApiModel)response.obj;
            Assert.Equal((int)ErrorType.UsernameAlreadyTaken, model.errorType);

            int size = DbContext.user.Count();
            Assert.Equal(4, size);

            UserDbContext.TearDown(DbContext);
        }

        [Fact]
        public void Register_SameFirstAndLastName_OK()
        {
            UserDbContext.SetUp(DbContext);
            RegisterRequestModel request = new RegisterRequestModel();
            request.firstname = "Mike";
            request.lastname = "Ward";
            request.email = "qwerew@hotmail.com";
            request.password = "FuckUJeremy";
            request.lastLocation = new LocationApiModel(1, 2);

            ApiResponse response = AuthenticationApiService.Register(Config, DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(RegisterResponseModel), response.obj.GetType());
            RegisterResponseModel model = (RegisterResponseModel)response.obj;
            Assert.NotNull(model.token);
            JwtTokenService tokenService = new JwtTokenService(Config);
            Assert.True(tokenService.ValidateJwtTokenWithoutLifetime(model.token));
            Assert.Equal(model.userID, int.Parse(tokenService.GetJwtClaim(model.token, "userID")));
            Assert.NotNull(model.refreshToken);

            UserDbModel dbModel = DbContext.user.Find(model.userID);
            Assert.Equal("Mike", dbModel.firstname);
            Assert.Equal("Ward", dbModel.lastname);
            Assert.Equal("qwerew@hotmail.com", dbModel.email);
            Assert.NotEqual("FuckUJeremy", dbModel.password);
            Assert.Equal(1, dbModel.lastLocation.X);
            Assert.Equal(2, dbModel.lastLocation.Y);

            AuthenticationDbModel authDbModel = DbContext.authentication.Where(a => a.userID == dbModel.userID).First();
            Assert.Equal(model.refreshToken, authDbModel.token);
            Assert.True(authDbModel.expireTime > DateTime.Now);

            UserDbContext.TearDown(DbContext);
        }

        public void Dispose()
        {
            UtilDatabaseContext.ClearTables(DbContext);
        }
    }
}
