using Microsoft.EntityFrameworkCore;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.DatabaseContext
{
    public class MultiverseDbContext : DbContext
    {
        public DbSet<UserDbModel> user { get; set; }
        public DbSet<AuthenticationDbModel> authentication { get; set; }
        public DbSet<RelationshipDbModel> relationship { get; set; }
        public DbSet<RelationshipRequestDbModel> relationshipRequest { get; set; }
        public DbSet<ConversationDbModel> conversation { get; set; }
        public DbSet<ConversationUserDbModel> conversationUser { get; set; }
        public DbSet<MessageDbModel> message { get; set; }

        public MultiverseDbContext(DbContextOptions<MultiverseDbContext> options) : base(options)
        {
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User table
            {
                modelBuilder.Entity<UserDbModel>().ToTable("user");

                modelBuilder.Entity<UserDbModel>().HasKey(u => u.userID).HasName("userID");
                modelBuilder.Entity<UserDbModel>().Property(u => u.userID).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
                modelBuilder.Entity<UserDbModel>().Property(u => u.email).HasColumnType("nvarchar(50)").IsRequired();
                modelBuilder.Entity<UserDbModel>().Property(u => u.password).HasColumnType("nvarchar(100)").IsRequired();
                modelBuilder.Entity<UserDbModel>().Property(u => u.firstname).HasColumnType("nvarchar(50)").IsRequired();
                modelBuilder.Entity<UserDbModel>().Property(u => u.lastname).HasColumnType("nvarchar(50)").IsRequired();
                modelBuilder.Entity<UserDbModel>().HasIndex(u => u.lastLocation).IsSpatial();
                modelBuilder.Entity<UserDbModel>().Property(u => u.lastLocation).IsRequired();
                modelBuilder.Entity<UserDbModel>().HasIndex(u => new { u.email, u.firstname, u.lastname }).HasDatabaseName("user_idx");
            }

            // Authentication table
            {
                modelBuilder.Entity<AuthenticationDbModel>().ToTable("authentication");

                modelBuilder.Entity<AuthenticationDbModel>().HasKey(a => a.userID);
                modelBuilder.Entity<AuthenticationDbModel>().Property(a => a.userID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<AuthenticationDbModel>().Property(a => a.token).HasColumnType("char(88)").IsRequired();
                modelBuilder.Entity<AuthenticationDbModel>().Property(a => a.expireTime).HasColumnType("datetime").IsRequired();
            }

            // Relationships Tables
            {
                modelBuilder.Entity<RelationshipDbModel>().ToTable("relationship");
                modelBuilder.Entity<RelationshipDbModel>().HasKey(f => f.relationshipID);
                modelBuilder.Entity<RelationshipDbModel>().Property(f => f.relationshipID).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
                modelBuilder.Entity<RelationshipDbModel>().Property(f => f.followerID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<RelationshipDbModel>().Property(f => f.followedID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<RelationshipDbModel>().HasOne<UserDbModel>().WithMany().HasPrincipalKey(u => u.userID).HasForeignKey(f => f.followerID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_relationshipFollower_user");
                modelBuilder.Entity<RelationshipDbModel>().HasOne<UserDbModel>().WithMany().HasPrincipalKey(u => u.userID).HasForeignKey(f => f.followedID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_relationshipFollowed_user");
                modelBuilder.Entity<RelationshipDbModel>().HasIndex(f => new { f.followerID, f.followedID }).HasDatabaseName("relationship_idx");

                modelBuilder.Entity<RelationshipRequestDbModel>().ToTable("relationshipRequest");
                modelBuilder.Entity<RelationshipRequestDbModel>().HasKey(f => f.relationshipRequestID);
                modelBuilder.Entity<RelationshipRequestDbModel>().Property(f => f.relationshipRequestID).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
                modelBuilder.Entity<RelationshipRequestDbModel>().Property(f => f.followerID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<RelationshipRequestDbModel>().Property(f => f.followedID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<RelationshipRequestDbModel>().HasOne<UserDbModel>().WithMany().HasPrincipalKey(u => u.userID).HasForeignKey(f => f.followerID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_relationshipRequestFollower_user");
                modelBuilder.Entity<RelationshipRequestDbModel>().HasOne<UserDbModel>().WithMany().HasPrincipalKey(u => u.userID).HasForeignKey(f => f.followedID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_relationshipRequestFollowed_user");
                modelBuilder.Entity<RelationshipRequestDbModel>().HasIndex(f => new { f.followerID, f.followedID }).HasDatabaseName("relationshipRequest_idx");
            }

            // Conversation tables
            {
                modelBuilder.Entity<ConversationDbModel>().ToTable("conversation");
                modelBuilder.Entity<ConversationDbModel>().HasKey(c => c.conversationID).HasName("conversationID");
                modelBuilder.Entity<ConversationDbModel>().Property(c => c.conversationID).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
                modelBuilder.Entity<ConversationDbModel>().Property(c => c.name).HasColumnType("nvarchar(20)").HasDefaultValue("conversation");
                modelBuilder.Entity<ConversationDbModel>().Property(c => c.lastUpdate).HasColumnType("datetime");

                modelBuilder.Entity<ConversationUserDbModel>().ToTable("conversationUser");
                modelBuilder.Entity<ConversationUserDbModel>().HasKey(cu => cu.conversationUserID);
                modelBuilder.Entity<ConversationUserDbModel>().Property(cu => cu.conversationUserID).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
                modelBuilder.Entity<ConversationUserDbModel>().Property(cu => cu.conversationID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<ConversationUserDbModel>().Property(cu => cu.userID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<ConversationUserDbModel>().HasOne<ConversationDbModel>().WithMany().HasPrincipalKey(c => c.conversationID).HasForeignKey(cu => cu.conversationID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_conversationUser_conversation");
                modelBuilder.Entity<ConversationUserDbModel>().HasOne<UserDbModel>().WithMany().HasPrincipalKey(u => u.userID).HasForeignKey(cu => cu.userID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_conversationUser_user");
                modelBuilder.Entity<ConversationUserDbModel>().HasIndex(cu => new { cu.conversationID, cu.userID }).HasDatabaseName("conversationUser_idx");

                modelBuilder.Entity<MessageDbModel>().ToTable("message");
                modelBuilder.Entity<MessageDbModel>().HasKey(m => m.messageID).HasName("messageID");
                modelBuilder.Entity<MessageDbModel>().Property(m => m.messageID).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
                modelBuilder.Entity<MessageDbModel>().Property(m => m.conversationID).HasColumnType("int").IsRequired();
                modelBuilder.Entity<MessageDbModel>().Property(m => m.authorID).HasColumnType("int").HasDefaultValue(null);
                modelBuilder.Entity<MessageDbModel>().Property(m => m.publishedTime).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
                modelBuilder.Entity<MessageDbModel>().Property(m => m.messageType).HasColumnType("tinyint").HasDefaultValue(0).IsRequired();
                modelBuilder.Entity<MessageDbModel>().Property(m => m.message).HasColumnType("nvarchar(250)");
                modelBuilder.Entity<MessageDbModel>().HasOne<ConversationDbModel>().WithMany().HasPrincipalKey(c => c.conversationID).HasForeignKey(m => m.conversationID).OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_message_conversation");
                modelBuilder.Entity<MessageDbModel>().HasOne<UserDbModel>().WithMany().HasPrincipalKey(u => u.userID).HasForeignKey(m => m.conversationID).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_message_user");
                modelBuilder.Entity<MessageDbModel>().HasIndex(m => m.conversationID).HasDatabaseName("message_idx");
            }
        }
    }
}
