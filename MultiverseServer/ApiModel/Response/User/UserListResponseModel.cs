using MultiverseServer.ApiModel.Model;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response.User
{
    public class UserListResponseModel
    {
        public IList<UserApiModel> users { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
        public int totalSize { get; set; }

        public static UserListResponseModel ToApiModel(IList<UserDbModel> userList, int count, int offset, int totalSize)
        {
            UserListResponseModel apiModel = new UserListResponseModel();

            apiModel.users = new List<UserApiModel>();
            foreach (UserDbModel user in userList)
            {
                apiModel.users.Add(UserApiModel.ToApiModel(user));
            }

            apiModel.count = count;
            apiModel.offset = offset;
            apiModel.totalSize = totalSize;

            return apiModel;
        }
    }
}
