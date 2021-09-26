using MultiverseServer.ApiModel.Model;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Response.Conversation
{
    public class ConversationListResponseModel
    {
        public IList<ConversationApiModel> conversations { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
        public int totalSize { get; set; }

        public static ConversationListResponseModel ToApiModel(IList<ConversationDbModel> conversationList, int count, int offset, int totalSize)
        {
            ConversationListResponseModel apiModel = new ConversationListResponseModel();
            apiModel.conversations = new List<ConversationApiModel>();
            foreach(ConversationDbModel dbModel in conversationList)
            {
                apiModel.conversations.Add(ConversationApiModel.ToApiModel(dbModel));
            }

            apiModel.count = count;
            apiModel.offset = offset;
            apiModel.totalSize = totalSize;

            return apiModel;
        }
    }
}
