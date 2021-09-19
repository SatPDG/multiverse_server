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
        public string firstname { get; set; }
        public string lastname { get; set; }
        public LocationApiModel lastLocation { get; set; }

        public UserDbModel ToDbModel()
        {
            UserDbModel dbModel = new UserDbModel();
            dbModel.userID = userID;
            dbModel.email = string.Empty;
            dbModel.password = string.Empty;
            dbModel.firstname = firstname;
            dbModel.lastname = lastname;
            dbModel.lastLocation = lastLocation.ToDbModel();

            return dbModel;
        }

        public static UserApiModel ToApiModel(UserDbModel dbModel)
        {
            UserApiModel apiModel = new UserApiModel
            {
                userID = dbModel.userID,
                firstname = dbModel.firstname,
                lastname = dbModel.lastname,
                lastLocation = LocationApiModel.ToApiModel(dbModel.lastLocation),
            };
            return apiModel;
        }
    }
}
