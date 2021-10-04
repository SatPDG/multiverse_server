using Microsoft.EntityFrameworkCore;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseService
{
    public class ConversationDbService
    {
        private ConversationDbService()
        {

        }

        public static ConversationDbModel GetConversation(MultiverseDbContext dbContext, int conversationID)
        {
            ConversationDbModel conversation = null;

            // Get the conversation from the db
            conversation = dbContext.conversation.Find(conversationID);

            return conversation;
        }

        public static IList<int> GetConversationUser(MultiverseDbContext dbContext, int conversationID)
        {
            IList<int> userList = null;

            // Get the conversation user
            userList = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID).Select(c => (int)c.conversationUserID).ToList();
            return userList;
        }

        public static IList<int> GetConversationUser(MultiverseDbContext dbContext, int conversationID, int offset, int count)
        {
            IList<int> userList = null;

            // Get the conversation user
            userList = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID).Select(c => (int)c.conversationUserID).Skip(offset).Take(count).ToList();
            return userList;
        }

        public static ConversationDbModel CreateConversation(MultiverseDbContext dbContext, ConversationDbModel conversation, IList<int> userList)
        {
            ConversationDbModel newConversation = null;

            // Make sure the users exist
            bool exist = UserDbService.CheckIfAllUsersExist(dbContext, userList);
            if(exist)
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

        public static bool AddUsersToConversation(MultiverseDbContext dbContext, IList<int> userIDList, int conversationID)
        {
            // Check that the conversation exist
            ConversationDbModel conversation = dbContext.conversation.Find(conversationID);
            if(conversation != null)
            {
                // Make sure the users exists
                int size = dbContext.user.Where(u => userIDList.Contains(u.userID)).Count();
                if(size == userIDList.Count)
                {
                    // Make sure the users are not already in the conversation
                    size = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID && userIDList.Contains(cu.userID)).Count();
                    if(size == 0)
                    {
                        // Add all the users to the conversation
                        IList<ConversationUserDbModel> dbList = new List<ConversationUserDbModel>();
                        foreach(int userID in userIDList)
                        {
                            dbList.Add(new ConversationUserDbModel
                            {
                                conversationID = conversationID,
                                userID = userID,
                            });
                        }

                        dbContext.conversationUser.AddRange(dbList);
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
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool RemoveUsersFromConversation(MultiverseDbContext dbContext, IList<int> userIDList, int conversationID)
        {
            // Check that the conversation exist
            ConversationDbModel conversation = dbContext.conversation.Find(conversationID);
            if (conversation != null)
            {
                // Get the users to remove
                IList<ConversationUserDbModel> userList = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID && userIDList.Contains(cu.userID)).ToList();
                
                // Make sure all the user are in the conversation
                if(userList.Count != userIDList.Count)
                {
                    return false;
                }
                
                dbContext.conversationUser.RemoveRange(userList);
                dbContext.SaveChanges();

                // Make sure that there is still some user in the conversation. Otherwise delete it.
                int size = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID).Count();
                if(size == 0)
                {
                    // Delete the conversation
                    dbContext.conversation.Remove(conversation);
                    dbContext.SaveChanges();
                }
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

        public static bool IsUserInConversation(MultiverseDbContext dbContext, int userID, int conversationID)
        {
            int size = dbContext.conversationUser.Where(cu => cu.conversationID == conversationID && cu.conversationUserID == userID).Count();
            return size == 1;
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

        public static bool DeleteMessage(MultiverseDbContext dbContext, int messageID)
        {
            MessageDbModel dbModel = new MessageDbModel()
            {
                messageID = messageID,
            };
            dbContext.message.Attach(dbModel);
            dbContext.message.Remove(dbModel);
            dbContext.SaveChanges();
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

        public static MessageDbModel GetMessage(MultiverseDbContext dbContext, int messageid)
        {
            MessageDbModel message = dbContext.message.Find(messageid);
            return message;
        }

        public static bool IsAuthorOfMessage(MultiverseDbContext dbContext, int userID, int messageID)
        {
            int size = dbContext.message.Where(m => m.messageID == messageID && m.authorID == userID).Count();
            if(size <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
