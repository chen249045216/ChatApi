using ChatApi.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    [Table("chat_group_message")]
    public class ChatGroupMessage
    {
        public ChatGroupMessage()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string Content { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        public double CreateTime { get; set; }

        public virtual ChatGroup ChatGroup { get; set; }
        public virtual ChatUser ChatUser { get; set; }
    }
}
