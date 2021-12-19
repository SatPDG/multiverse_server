using MultiverseServer.ApiModel.Model;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response.Conversation
{
    public class MessageListResponseModel
    {
        public IList<MessageApiModel> messages { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
        public int totalSize { get; set; }

        public static MessageListResponseModel ToApiModel(IList<MessageDbModel> messageList, int count, int offset, int totalSize)
        {
            MessageListResponseModel apiModel = new MessageListResponseModel();
            apiModel.messages = new List<MessageApiModel>();
            foreach (MessageDbModel dbModel in messageList)
            {
                apiModel.messages.Add(MessageApiModel.ToApiModel(dbModel));
            }

            apiModel.count = count;
            apiModel.offset = offset;
            apiModel.totalSize = totalSize;

            return apiModel;
        }
    }
}
