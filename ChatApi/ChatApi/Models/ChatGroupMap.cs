using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    [Table("chat_group_map")]
    public class ChatGroupMap
    {
        public ChatGroupMap()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }


        public virtual ChatUser ChatUser { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }
    }
}
