using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MultiverseServer.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.TestSetUp
{
    class DatabaseSetUp
    {
        private DatabaseSetUp()
        {

        }

        public static MultiverseDbContext SetUp()
        {
            MultiverseDbContext dbContext = null;
            try
            {
                // Connect to the sql test server
                string sqlServerAddress = "192.168.1.224";
                string sqlServerPort = "3306";
                string sqlServerUsername = "DevOps";
                string sqlServerPassword = "1234qwer";
                string mySqlConnectionStr = "server=" + sqlServerAddress + ";user=" + sqlServerUsername + ";database=multiverseTest;port=" + sqlServerPort + ";password=" + sqlServerPassword;

                DbContextOptionsBuilder<MultiverseDbContext> dbContextOptionBuilder = new DbContextOptionsBuilder<MultiverseDbContext>().UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr), b => b.UseNetTopologySuite());
                dbContext = new MultiverseDbContext(dbContextOptionBuilder.Options);

                // This section is commented because it causes error...
                // A fix should be done to ensure that the test database is the same format as the principal database.

                //// Drop all the table from the db
                //string tableNameList = "authentication, conversation, conversationUser, message, relationship, relationshipRequest, user";
                //dbContext.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;" +
                //    "DROP TABLE IF EXISTS " + tableNameList + ";" +
                //    "SET FOREIGN_KEY_CHECKS = 1;");

                //// Force creation of the tables
                //RelationalDatabaseCreator databaseCreator = (dbContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator);
                //string createString = databaseCreator.GenerateCreateScript();//CreateTables();
                //dbContext.Database.ExecuteSqlRaw(createString);

                dbContext.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;" + 
                    "TRUNCATE TABLE user;" +
                    "TRUNCATE TABLE authentication;" +
                    "TRUNCATE TABLE relationship;" +
                    "TRUNCATE TABLE relationshipRequest;" +
                    "TRUNCATE TABLE conversation;" +
                    "TRUNCATE TABLE conversationUser;" +
                    "TRUNCATE TABLE message;" +
                    "TRUNCATE TABLE notification;" +
                    "SET FOREIGN_KEY_CHECKS = 1");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // An error occur in the database creation. Stop the tests.
                Environment.Exit(1);
            }
            return dbContext;
        } 
    }
}
