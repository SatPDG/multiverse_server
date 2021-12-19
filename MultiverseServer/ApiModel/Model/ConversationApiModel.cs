using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Model
{
    public class ConversationApiModel
    {
        public int conversationID { get; set; }
        public string name { get; set; }
        public string lastUpdate { get; set; }

        public ConversationDbModel ToDbModel()
        {
            ConversationDbModel dbModel = new ConversationDbModel();
            dbModel.conversationID = this.conversationID;
            dbModel.name = this.name;
            dbModel.lastUpdate = DateTime.Parse(this.lastUpdate);

            return dbModel;
        } 

        public static ConversationApiModel ToApiModel(ConversationDbModel dbModel)
        {
            ConversationApiModel apiModel = new ConversationApiModel();
            apiModel.conversationID = dbModel.conversationID;
            apiModel.name = dbModel.name;
            apiModel.lastUpdate = dbModel.lastUpdate.ToString("yyyy-MM-dd HH:mm:ss");

            return apiModel;
        }
    }
}
