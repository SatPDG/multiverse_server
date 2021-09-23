using Microsoft.EntityFrameworkCore;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.Security.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace MultiverseServer.DatabaseService
{
    public class UserDbService
    {
        private UserDbService()
        {

        }

        public static int CreateNewUser(MultiverseDbContext dbContext, UserDbModel user)
        {
            int newUserID = -1;
            // Make sure the email is unique
            int size = dbContext.user.Where(u => u.email.Equals(user.email)).Count();
            if (size == 0)
            {
                // Create user for database
                UserDbModel userDb = new UserDbModel
                {
                    email = user.email,
                    firstname = user.firstname,
                    lastname = user.lastname,
                    lastLocation = user.lastLocation,
                };

                // Compute the password hash.
                SHA256Hash hash = new SHA256Hash();
                userDb.password = hash.ComputeSha256Hash(user.password);

                dbContext.user.Add(userDb);
                dbContext.SaveChanges();

                newUserID = userDb.userID;
            }
            return newUserID;
        }

        public static bool DeleteUser(MultiverseDbContext dbContext, int userID)
        {
            UserDbModel userDb = new UserDbModel
            {
                userID = userID
            };

            try
            {
                dbContext.user.Remove(userDb);
                dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public static bool UpdateUser(MultiverseDbContext dbContext, int userID, UserDbModel userDb)
        {
            // Set the user ID
            userDb.userID = userID;

            // Crush the password
            userDb.password = null;

            // Update the user in the db.
            try
            {
                dbContext.user.Update(userDb);
                dbContext.Entry(userDb).Property(u => u.password).IsModified = false;
                dbContext.Entry(userDb).Property(u => u.lastLocation).IsModified = false;
                dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public static bool UpdateUserPassword(MultiverseDbContext dbContext, int userID, string oldPassword, string newPassword)
        {
            // Get the user
            UserDbModel userDb = dbContext.user.Find(userID);

            if (userDb != null)
            {
                // Validate the old password
                SHA256Hash hash = new SHA256Hash();
                string hashOldPassword = hash.ComputeSha256Hash(oldPassword);

                if (hashOldPassword.Equals(userDb.password))
                {
                    // Update the user password
                    string hashNewPassword = hash.ComputeSha256Hash(newPassword);

                    userDb.password = hashNewPassword;
                    try
                    {
                        dbContext.user.Update(userDb); 
                        dbContext.Entry(userDb).Property(u => u.email).IsModified = false;
                        dbContext.Entry(userDb).Property(u => u.password).IsModified = true;
                        dbContext.Entry(userDb).Property(u => u.firstname).IsModified = false;
                        dbContext.Entry(userDb).Property(u => u.lastname).IsModified = false;
                        dbContext.Entry(userDb).Property(u => u.lastLocation).IsModified = false;
                        dbContext.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool UpdateUserLocation(MultiverseDbContext dbContext, int userID, Point location)
        {
            UserDbModel userDb = new UserDbModel();
            userDb.userID = userID;
            userDb.lastLocation = location;

            dbContext.user.Update(userDb);
            dbContext.Entry(userDb).Property(u => u.email).IsModified = false;
            dbContext.Entry(userDb).Property(u => u.password).IsModified = false;
            dbContext.Entry(userDb).Property(u => u.firstname).IsModified = false;
            dbContext.Entry(userDb).Property(u => u.lastname).IsModified = false;
            dbContext.Entry(userDb).Property(u => u.lastLocation).IsModified = true;
            dbContext.SaveChanges();

            return false;
        }

        public static UserDbModel GetUser(MultiverseDbContext dbContext, int userID)
        {
            UserDbModel user = dbContext.user.Find(userID);
       
            if (user != null)
            {
                dbContext.Entry(user).State = EntityState.Detached;

                // Crush the password
                user.password = "";
            }
            return user;
        }

        public static UserDbModel GetUser(MultiverseDbContext dbContext, string email, string password)
        {
            // Find the user
            UserDbModel user = null;
            try
            {
                user = dbContext.user.Where(u => u.email.Equals(email)).First();
            }
            catch
            {
                return null;
            }
            if(user != null)
            {
                // Validate the password
                SHA256Hash hash = new SHA256Hash();
                string hashPassword = hash.ComputeSha256Hash(password);

                if(user.password.Equals(hashPassword))
                {
                    return user;
                }
            }

            return null;
        }

        public static bool CheckIfUserExists(MultiverseDbContext dbContext, int userID)
        {
            int size = dbContext.user.Where(u => u.userID == userID).Count();
            if(size != 1)
            {
                return false;
            }

            return true;
        }

        public static bool CheckIfAllUsersExist(MultiverseDbContext dbContext, IList<int> userList)
        {
            int size = dbContext.user.Where(u => userList.Contains(u.userID)).Count();
            if(size != userList.Count)
            {
                return false;
            }

            return false;
        }

        public static IList<UserDbModel> SearchUserByName(MultiverseDbContext dbContext, string searchStr, int offset, int count)
        {
            // Search in the database
            IList<UserDbModel> userList = dbContext.user.Where(u => searchStr.Contains(u.firstname) || 
                                                                    searchStr.Contains(u.lastname) || 
                                                                    u.firstname.Contains(searchStr) || 
                                                                    u.lastname.Contains(searchStr)).Skip(offset).Take(count).ToList();
            return userList;
        }

        public static IList<UserDbModel> SearchUserByLocation(MultiverseDbContext dbContext, Point location, int offset, int count)
        {
            IList<UserDbModel> userList = dbContext.user.OrderBy(u => MultiverseDbContext.st_distance_sphere(u.lastLocation, location)).Skip(offset).Take(count).ToList();

            return userList;
        }
    }
}
