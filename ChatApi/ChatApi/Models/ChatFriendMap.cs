using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    [Table("chat_friend_map")]
    public class ChatFriendMap
    {
        public ChatFriendMap()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }

        public virtual ChatUser ChatUser { get; set; }
        public virtual ChatUser ChatFriend { get; set; }
    }
}
