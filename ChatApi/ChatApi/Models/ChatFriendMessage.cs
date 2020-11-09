using ChatApi.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    [Table("chat_friend_message")]
    public class ChatFriendMessage
    {
        public ChatFriendMessage()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public string Content { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        public double CreateTime { get; set; }

        public virtual ChatUser FromUser { get; set; }
        public virtual ChatUser ToUser { get; set; }
    }
}
