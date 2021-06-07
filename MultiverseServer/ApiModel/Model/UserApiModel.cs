using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Model
{
    public class UserApiModel
    {
        public int userID { get; set; }
        public string email { get; set; }
        public string password  { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public LocationApiModel lastLocation { get; set; }

        public UserDbModel ToDbModel()
        {
            UserDbModel dbModel = new UserDbModel();
            dbModel.userID = userID;
            dbModel.email = email;
            dbModel.password = password;
            dbModel.firstname = firstname;
            dbModel.lastname = lastname;
            dbModel.lastLocation = new NetTopologySuite.Geometries.Point(lastLocation.longitude, lastLocation.latitude);

            return dbModel;
        }

        public static UserApiModel ToApiModel(UserDbModel dbModel)
        {
            UserApiModel apiModel = new UserApiModel
            {
                userID = dbModel.userID,
                email = dbModel.email,
                password = dbModel.password,
                firstname = dbModel.firstname,
                lastname = dbModel.lastname,
                lastLocation = new LocationApiModel(dbModel.lastLocation.X, dbModel.lastLocation.Y)
            };
            return apiModel;
        }
    }
}
