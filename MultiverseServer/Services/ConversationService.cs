using Microsoft.EntityFrameworkCore;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseService
{
    public class ConversationService
    {
        private ConversationService()
        {

        }

        public static ConversationDbModel CreateConversation(MultiverseDbContext dbContext, ConversationDbModel conversation, IList<int> userList)
        {
            ConversationDbModel newConversation = null;

            // Make sure the user exist
            int size = dbContext.user.Where(u => userList.Contains(u.userID)).Count();
            if(size == userList.Count())
            {
                // Create the conversation
                newConversation = new ConversationDbModel
                {
                    name = conversation.name,
                    lastUpdate = DateTime.Now,
                };

                dbContext.conversation.Add(newConversation);
                dbContext.SaveChanges();

                // Add the user to the conversation
                foreach (int user in userList)
                {
                    ConversationUserDbModel conversationUser = new ConversationUserDbModel
                    {
                        conversationID = newConversation.conversationID,
                        userID = user,
                    };
                    dbContext.conversationUser.Add(conversationUser);
                }
                dbContext.SaveChanges();
            }

            return newConversation;
        }

        public static bool DeleteConversation(MultiverseDbContext dbContext, int conversationID)
        {
            // Delete the conversation from the table
            ConversationDbModel conversation = new ConversationDbModel
            {
                conversationID = conversationID
            };
            try
            {
                dbContext.conversation.Remove(conversation);
                //dbContext.conversationUser.RemoveRange(dbContext.conversationUser.Where(cu => cu.conversationID.Equals(conversationID)));
                dbContext.SaveChanges();
            }catch(DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public static bool UpdateConversation(MultiverseDbContext dbContext, int conversationID, ConversationDbModel conversationDb)
        {
            conversationDb.conversationID = conversationID;

            // Update the conversation
            try
            {
                dbContext.Update(conversationDb);
                dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public static IList<ConversationDbModel> GetConversationList(MultiverseDbContext dbContext, int userID, int offset, int count)
        {
            IList<ConversationDbModel> list = dbContext.conversationUser.Join(dbContext.conversation,
                                                                              cu => cu.conversationID,
                                                                              c => c.conversationID,
                                                                              (cu, c) => new
                                                                              {
                                                                                  c.conversationID,
                                                                                  c.name,
                                                                                  c.lastUpdate,
                                                                                  cu.userID,
                                                                              }).Where(c => c.userID == userID)
                                                                              .Select(c => new ConversationDbModel
                                                                              {
                                                                                  conversationID = c.conversationID,
                                                                                  name = c.name,
                                                                                  lastUpdate = c.lastUpdate
                                                                              }).OrderBy(c => c.lastUpdate).Skip(offset).Take(count).ToList();
            return list;
        }

        public static bool AddUserToConversation(MultiverseDbContext dbContext, int userID, int conversationID)
        {
            // Check that the conversation exist
            ConversationDbModel conversation = dbContext.conversation.Find(conversationID);
            if(conversation != null)
            {
                // Check that the user is not already in the conversation
                int size = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID && cu.userID == userID).Count();
                if(size >= 1)
                {
                    ConversationUserDbModel conversationUser = new ConversationUserDbModel
                    {
                        conversationID = conversationID,
                        userID = userID,
                    };
                    dbContext.conversationUser.Add(conversationUser);
                    dbContext.SaveChanges();
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

        public static bool RemoveUserFromConversation(MultiverseDbContext dbContext, int userID, int conversationID)
        {
            // Check that the conversation exist
            ConversationDbModel conversation = dbContext.conversation.Find(conversationID);
            if (conversation != null)
            {
                // Make sur that it is not the last user of the conversation
                int size = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID).Count();
                if(size <= 1)
                {
                    // Delete the conversation
                    dbContext.conversation.Remove(new ConversationDbModel
                    {
                        conversationID = conversationID,
                    });
                }
                else
                {
                    // Remove the user
                    dbContext.conversationUser.Remove(new ConversationUserDbModel
                    {
                        conversationID = conversationID,
                        userID = userID,
                    });
                }
                dbContext.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public static int GetNumberOfConversation(MultiverseDbContext dbContext, int userID)
        {
            int size = dbContext.conversationUser.Where(cu => cu.userID == userID).Count();
            return size;
        }

        public static int GetNumberOfUser(MultiverseDbContext dbContext, int conversationID)
        {
            int size = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID).Count();
            return size;
        }

        public static bool SendMessage(MultiverseDbContext dbContext, MessageDbModel messageDb)
        {
            // Make sur the conversation exists
            ConversationDbModel conversation = dbContext.conversation.Find(messageDb.conversationID);
            if(conversation != null)
            {
                // Make sure the author is in the conversation
                int size = dbContext.conversationUser.Where(cu => cu.conversationID == messageDb.conversationID && cu.userID == messageDb.authorID).Count();
                if(size >= 1)
                {
                    // Add the message
                    messageDb.publishedTime = DateTime.Now;
                    dbContext.message.Add(messageDb);
                    dbContext.SaveChanges();
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

        public static bool DeleteMessage(MultiverseDbContext dbContext, int userID, int messageID)
        {
            // Make sur the user is the author
            MessageDbModel messageDb = dbContext.message.Find(messageID);
            if(messageDb != null && messageDb.authorID == userID)
            {
                // Delete the message from the table
                dbContext.message.Remove(new MessageDbModel { messageID = messageID });
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool UpdateMessage(MultiverseDbContext dbContext, int userID, int messageID, MessageDbModel messageDb)
        {
            // Make sure the author is the user
            MessageDbModel message = dbContext.message.Find(messageID);
            if(message != null && message.authorID == userID)
            {
                // Update the message
                messageDb.messageID = messageID;
                messageDb.publishedTime = DateTime.Now;

                try
                {
                    dbContext.message.Update(messageDb);
                    dbContext.Entry(messageDb).Property(m => m.conversationID).IsModified = false;
                    dbContext.Entry(messageDb).Property(m => m.authorID).IsModified = false;
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

            return true; 
        }

        public static IList<MessageDbModel> GetMessageList(MultiverseDbContext dbContext, int userID, int conversationID, int offset, int count)
        {
            IList<MessageDbModel> list = null;
            
            // Make sure the user is in the conversation
            int size = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID && cu.userID == userID).Count();
            if(size >= 1)
            {
                list = dbContext.message.Where(m => m.conversationID == conversationID).OrderBy(m => m.publishedTime).Skip(offset).Take(count).ToList();
            }

            return list;
        }
    }
}
