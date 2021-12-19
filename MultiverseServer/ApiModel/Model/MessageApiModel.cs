using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Model
{
    public class MessageApiModel
    {
        public int messageID { get; set; }
        public int conversationID { get; set; }
        public int authorID { get; set; }
        public string publishedTime { get; set; }
        public byte messageType { get; set; }
        public string message { get; set; }

        public static MessageApiModel ToApiModel(MessageDbModel dbModel)
        {
            MessageApiModel apiModel = new MessageApiModel();
            apiModel.messageID = dbModel.messageID;
            apiModel.conversationID = dbModel.conversationID;
            apiModel.authorID = dbModel.authorID;
            apiModel.publishedTime = dbModel.publishedTime.ToString("yyyy-MM-dd HH:mm:ss");
            apiModel.messageType = dbModel.messageType;
            apiModel.message = dbModel.message;

            return apiModel;
        }
    }
}
