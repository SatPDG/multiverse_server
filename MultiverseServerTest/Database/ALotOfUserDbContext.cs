using GeoCoordinatePortable;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database
{
    class ALotOfUserDbContext
    {
        const double MAX_LONGITUDE = 153;
        const double MIN_LONGITUDE = 146;
        const double MAX_LATITUDE = -31;
        const double MIN_LATITUDE = -36;

        public static IList<UserLocation> locationList;

        private ALotOfUserDbContext()
        {

        }

        public static void SetUp(MultiverseDbContext dbContext)
        {
            locationList = new List<UserLocation>();
            Random random = new Random();
            // Add 30 users
            for (int i = 1; i < 30 +1; i++)
            {
                double longitude = random.NextDouble() * (MAX_LONGITUDE - MIN_LONGITUDE) + MIN_LONGITUDE;
                double latitude = random.NextDouble() * (MAX_LATITUDE - MIN_LATITUDE) + MIN_LATITUDE;
                UserDbModel dbModel = new UserDbModel
                {
                    firstname = "firstname_user" + i.ToString(),
                    lastname = "lastname_user" + i.ToString(),
                    email = "email_user" + i.ToString(),
                    password = "password_user" + i.ToString(),
                    lastLocation = new NetTopologySuite.Geometries.Point(longitude, latitude),
                };
                locationList.Add(new UserLocation
                {
                    userID = i,
                    coordinate = new GeoCoordinate(latitude, longitude),
                });
                dbContext.user.Add(dbModel);
                dbContext.SaveChanges();
            }
        }

        public class UserLocation
        {
            public int userID { get; set; }
            public GeoCoordinate coordinate { get; set; }
        }
    }
}
