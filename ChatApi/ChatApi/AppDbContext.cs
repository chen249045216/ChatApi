using ChatApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi
{
    public class AppDbContext : DbContext
    {
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatGroupMap> ChatGroupMaps { get; set; }
        public DbSet<ChatGroupMessage> ChatGroupMessages { get; set; }
        public DbSet<ChatFriendMessage> ChatFriendMessages { get; set; }
        public DbSet<ChatFriendMap> ChatFriendMaps { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChatUser>(entity =>
            {
                entity.HasKey(m => new { m.Id });
                entity.Property(m => m.UserName).HasMaxLength(20);
                entity.Property(m => m.PassWord).HasMaxLength(20);
                entity.Property(m => m.NickName).HasMaxLength(20);
                entity.HasMany(t => t.UserFriends).WithOne(t => t.ChatUser).HasForeignKey(t => t.UserId);
                entity.HasMany(t => t.FriendUsers).WithOne(t => t.ChatFriend).HasForeignKey(t => t.FriendId);
                entity.HasMany(t => t.ChatGroupMaps).WithOne(t => t.ChatUser).HasForeignKey(t => t.UserId);
                entity.HasMany(t => t.ChatGroups).WithOne(t => t.ChatUser).HasForeignKey(t => t.UserId);
                entity.HasMany(t => t.ChatGroupMessages).WithOne(t => t.ChatUser).HasForeignKey(t => t.UserId);
                entity.HasMany(t => t.FriendFromMessage).WithOne(t => t.FromUser).HasForeignKey(t => t.UserId);
                entity.HasMany(t => t.FriendToMessage).WithOne(t => t.ToUser).HasForeignKey(t => t.FriendId);
            });
            modelBuilder.Entity<ChatGroup>(entity =>
            {
                entity.HasKey(m => new { m.Id });
                entity.HasOne(m => m.ChatUser);
                entity.HasMany(t => t.ChatGroupMaps).WithOne(t => t.ChatGroup).HasForeignKey(t => t.GroupId);
                entity.HasMany(t => t.ChatGroupMessages).WithOne(t => t.ChatGroup).HasForeignKey(t => t.GroupId);
            });
            modelBuilder.Entity<ChatFriendMap>(entity =>
            {
                entity.HasKey(m => new { m.Id });
            });
            modelBuilder.Entity<ChatGroupMap>(entity =>
            {
                entity.HasKey(m => new { m.Id });
            });
            modelBuilder.Entity<ChatGroupMessage>(entity =>
            {
                entity.HasKey(m => new { m.Id });
                entity.HasOne(m => m.ChatUser).WithMany(t => t.ChatGroupMessages).HasForeignKey(t => t.UserId);
            });
        }
    }
}
