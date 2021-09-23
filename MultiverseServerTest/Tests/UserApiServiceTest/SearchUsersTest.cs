using GeoCoordinatePortable;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Request.User;
using MultiverseServer.ApiModel.Response.User;
using MultiverseServer.ApiServices;
using MultiverseServer.DatabaseContext;
using MultiverseServerTest.Database;
using MultiverseServerTest.Database.DatabaseContext;
using MultiverseServerTest.TestSetUp;
using NetTopologySuite.Geometries;
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
    public class SearchUsersTest : IDisposable
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public SearchUsersTest(TestFixture fixture)
        {
            this.Config = fixture.Config;
            this.DbContext = fixture.DbContext;
        }

        [Fact]
        public void GetUserList_SendRequest_Pass()
        {
            ALotOfUserDbContext.SetUp(DbContext);
            GeoCoordinate userLocation = ALotOfUserDbContext.locationList[0].coordinate;
            ApiResponse response = UserApiService.GetUserList(DbContext, 1);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(20, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(20, model.totalSize);
            Assert.NotNull(model.users);
            Assert.Equal(20, model.users.Count);

            IList<int> userSorted = ALotOfUserDbContext.locationList.OrderBy(l => l.coordinate.GetDistanceTo(userLocation)).Select(l => l.userID).ToList();
            for(int i = 0; i < 20; i++)
            {
                Assert.Equal(userSorted[i], model.users[i].userID);
            }
        }

        [Fact]
        public void SearchForUser_SimpleNameSearch_Pass()
        {
            ALotOfUserDbContext.SetUp(DbContext);
            UserSearchRequestModel request = new UserSearchRequestModel
            {
                count = 10,
                offset = 0,
                nameSearch = "firstname",
                locationSearch = null,
            };

            ApiResponse response = UserApiService.SearchForUsers(DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(-1, model.totalSize);
            Assert.Equal(10, model.users.Count);
        }

        [Fact]
        public void SearchForUser_ComplexeNameSearch_Pass()
        {
            UserDbContext.SetUp(DbContext);
            UserSearchRequestModel request = new UserSearchRequestModel
            {
                count = 10,
                offset = 0,
                nameSearch = "i",
                locationSearch = null,
            };

            ApiResponse response = UserApiService.SearchForUsers(DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(-1, model.totalSize);
            Assert.Equal(3, model.users.Count);
            Assert.True(model.users.Where(u => u.userID == 1).Count() == 1);
            Assert.True(model.users.Where(u => u.userID == 3).Count() == 1);
            Assert.True(model.users.Where(u => u.userID == 4).Count() == 1);
        }

        [Fact] void SearchForUser_LocationSearch_Pass()
        {
            ALotOfUserDbContext.SetUp(DbContext);
            GeoCoordinate user1Location = ALotOfUserDbContext.locationList[0].coordinate;
            UserSearchRequestModel request = new UserSearchRequestModel
            {
                count = 10,
                offset = 0,
                nameSearch = "",
                locationSearch = new MultiverseServer.ApiModel.Model.LocationApiModel(user1Location.Longitude, user1Location.Latitude),
            };

            ApiResponse response = UserApiService.SearchForUsers(DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(-1, model.totalSize);
            Assert.Equal(10, model.users.Count);

            IList<int> userSorted = ALotOfUserDbContext.locationList.OrderBy(l => l.coordinate.GetDistanceTo(user1Location)).Select(l => l.userID).ToList();
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(userSorted[i], model.users[i].userID);
            }
        }

        [Fact]
        public void SearchForUser_LocationSearchWithOffset_Pass()
        {
            ALotOfUserDbContext.SetUp(DbContext);
            GeoCoordinate user1Location = ALotOfUserDbContext.locationList[0].coordinate;
            UserSearchRequestModel request = new UserSearchRequestModel
            {
                count = 10,
                offset = 10,
                nameSearch = "",
                locationSearch = new MultiverseServer.ApiModel.Model.LocationApiModel(user1Location.Longitude, user1Location.Latitude),
            };

            ApiResponse response = UserApiService.SearchForUsers(DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(10, model.count);
            Assert.Equal(10, model.offset);
            Assert.Equal(-1, model.totalSize);
            Assert.Equal(10, model.users.Count);

            IList<int> userSorted = ALotOfUserDbContext.locationList.OrderBy(l => l.coordinate.GetDistanceTo(user1Location)).Select(l => l.userID).ToList();
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(userSorted[i + 10], model.users[i].userID);
            }
        }

        [Fact]
        public void SearchForUser_CountToBig_CountIsRestricted()
        {
            ALotOfUserDbContext.SetUp(DbContext);
            GeoCoordinate user1Location = ALotOfUserDbContext.locationList[0].coordinate;
            UserSearchRequestModel request = new UserSearchRequestModel
            {
                count = 30,
                offset = 0,
                nameSearch = "",
                locationSearch = new MultiverseServer.ApiModel.Model.LocationApiModel(user1Location.Longitude, user1Location.Latitude),
            };

            ApiResponse response = UserApiService.SearchForUsers(DbContext, request);

            Assert.Equal(200, response.code);
            Assert.Equal(typeof(UserListResponseModel), response.obj.GetType());
            UserListResponseModel model = (UserListResponseModel)response.obj;
            Assert.Equal(20, model.count);
            Assert.Equal(0, model.offset);
            Assert.Equal(-1, model.totalSize);
            Assert.Equal(20, model.users.Count);

            IList<int> userSorted = ALotOfUserDbContext.locationList.OrderBy(l => l.coordinate.GetDistanceTo(user1Location)).Select(l => l.userID).ToList();
            for (int i = 0; i < 20; i++)
            {
                Assert.Equal(userSorted[i], model.users[i].userID);
            }
        }

        [Fact]
        public void SearchForUser_BadJson_Failed()
        {
            UserSearchRequestModel request = new UserSearchRequestModel
            {
                count = 10,
                offset = 10,
                nameSearch = "",
                locationSearch = null,
            };

            ApiResponse response = UserApiService.SearchForUsers(DbContext, request);

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
