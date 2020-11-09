using ChatApi.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    [Table("chat_group")]
    public class ChatGroup
    {
        public ChatGroup()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 群名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        public string Notice { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public double CreateTime { get; set; }

        public virtual ChatUser ChatUser { get; set; }
        public List<ChatGroupMap> ChatGroupMaps { get; set; }
        public List<ChatGroupMessage> ChatGroupMessages { get; set; }
    }
}
