using Microsoft.EntityFrameworkCore;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Services
{
    public class AuthenticationDbService
    {

        private AuthenticationDbService()
        {

        }

        public static bool UpdateAuthentication(MultiverseDbContext dbContext, int userID, AuthenticationDbModel authenticationDb)
        {
            // Check if the user have an authentication entry
            AuthenticationDbModel authentication = dbContext.authentication.Find(userID);
            if (authentication == null)
            {
                authenticationDb.userID = userID;
                dbContext.authentication.Add(authenticationDb);
                dbContext.SaveChanges();
            }
            else
            {
                try
                {
                    authenticationDb.userID = userID;
                    dbContext.Entry(authentication).State = EntityState.Detached;
                    dbContext.authentication.Update(authenticationDb);
                    dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool RemoveAuthentication(MultiverseDbContext dbContext, int userID)
        {
            try
            {
                AuthenticationDbModel dbModel = new AuthenticationDbModel { userID = userID };
                dbContext.authentication.Attach(dbModel);
                dbContext.authentication.Remove(dbModel);
                dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public static AuthenticationDbModel GetAuthentication(MultiverseDbContext dbContext, int userID)
        {
            AuthenticationDbModel authenticationDb = dbContext.authentication.Find(userID);
            return authenticationDb;
        }
    }
}
