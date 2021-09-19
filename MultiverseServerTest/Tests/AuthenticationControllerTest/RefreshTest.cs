using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiServices;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
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
    public class RefreshTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public RefreshTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void Refresh_NormalRefresh_Pass()
        {
            UserDbContext.SetUp(DbContext);
            JwtTokenService jwt = new JwtTokenService(Config);
            string token = jwt.GenerateToken(2, "doe@hotmail.com", "0");
            string refreshToken = jwt.GenerateRefreshToken();

            System.Threading.Thread.Sleep(100);

            AuthenticationDbModel authDbModel = new AuthenticationDbModel
            {
                userID = 2,
                token = refreshToken,
                expireTime = DateTime.Now.AddMinutes(15),
            };
            DbContext.authentication.Add(authDbModel);
            DbContext.SaveChanges();

            RefreshRequestModel request = new RefreshRequestModel
            {
                token = token,
                refreshToken = refreshToken,
            };

            ApiResponse response = AuthenticationApiService.Refresh(Config, DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(RefreshResponseModel), response.obj.GetType());
            RefreshResponseModel model = (RefreshResponseModel)response.obj;
            Assert.NotNull(model.token);
            Assert.NotEqual(token, model.token);
            JwtTokenService tokenService = new JwtTokenService(Config);
            Assert.True(tokenService.ValidateJwtTokenWithoutLifetime(model.token));
            Assert.Equal(2, int.Parse(tokenService.GetJwtClaim(model.token, "userID")));
            Assert.NotNull(model.refreshToken);
            Assert.NotEqual(refreshToken, model.refreshToken);

            UserDbModel dbModel = DbContext.user.Find(2);
            Assert.Equal("John", dbModel.firstname);
            Assert.Equal("Doe", dbModel.lastname);
            Assert.Equal("doe@hotmail.com", dbModel.email);
            Assert.NotEqual("nothing", dbModel.password);
            Assert.False(string.IsNullOrWhiteSpace(dbModel.password));
            Assert.Equal(0, dbModel.lastLocation.X);
            Assert.Equal(0, dbModel.lastLocation.Y);

            authDbModel = DbContext.authentication.Where(a => a.userID == 2).Take(1).First();
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
