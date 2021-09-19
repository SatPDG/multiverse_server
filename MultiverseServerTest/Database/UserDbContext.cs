using Microsoft.EntityFrameworkCore;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.Security.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database.DatabaseContext
{
    class UserDbContext
    {
        private UserDbContext()
        {

        }

        public static void SetUp(MultiverseDbContext dbContext)
        {
            SHA256Hash hash = new SHA256Hash();
            // Create 4 users
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "Mike";
                dbModel.lastname = "Ward";
                dbModel.email = "mikeward@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("fuckUJeremy");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "John";
                dbModel.lastname = "Doe";
                dbModel.email = "doe@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("nothing");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "Bird";
                dbModel.lastname = "Man";
                dbModel.email = "pitpit@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("feather");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            {
                UserDbModel dbModel = new UserDbModel();
                dbModel.firstname = "Morty";
                dbModel.lastname = "Smith";
                dbModel.email = "smith@hotmail.com";
                dbModel.password = hash.ComputeSha256Hash("i8grandpa");
                dbModel.lastLocation = new NetTopologySuite.Geometries.Point(0, 0);
                dbContext.user.Add(dbModel);
            }
            dbContext.SaveChanges();
        }

        public static void TearDown(MultiverseDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;" + "TRUNCATE TABLE user;" + "SET FOREIGN_KEY_CHECKS = 1");
        }
    }
}
