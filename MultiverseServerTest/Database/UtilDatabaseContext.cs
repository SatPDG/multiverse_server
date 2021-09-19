using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using MultiverseServer.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.Database
{
    class UtilDatabaseContext
    {

        private UtilDatabaseContext()
        {

        }

        public static void ClearTables(MultiverseDbContext dbContext)
        {
            dbContext.ChangeTracker.Clear();
            dbContext.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;" +
                    "TRUNCATE TABLE user;" +
                    "TRUNCATE TABLE authentication;" +
                    "TRUNCATE TABLE relationship;" +
                    "TRUNCATE TABLE relationshipRequest;" +
                    "TRUNCATE TABLE conversation;" +
                    "TRUNCATE TABLE conversationUser;" +
                    "TRUNCATE TABLE message;" +
                    "SET FOREIGN_KEY_CHECKS = 1");
        }

        private static object GetDependencies()
        {
            throw new NotImplementedException();
        }
    }
}
