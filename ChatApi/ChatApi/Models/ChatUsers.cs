using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Models
{
    [Table("chat_user")]
    public class ChatUser
    {
        public ChatUser()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 规则
        /// </summary>
        public RoleEnum Role { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public StatusEnum Status { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 是否在线
        /// </summary>
        public IsOnlineEnum? IsOnline { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public double CreateTime { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public double? LastLoginTime { get; set; }


        public List<ChatGroup> ChatGroups { get; set; }
        public List<ChatFriendMap> UserFriends { get; set; }
        public List<ChatFriendMap> FriendUsers { get; set; }
        public List<ChatGroupMap> ChatGroupMaps { get; set; }
        public List<ChatGroupMessage> ChatGroupMessages { get; set; }
        public List<ChatFriendMessage> FriendFromMessage { get; set; }
        public List<ChatFriendMessage> FriendToMessage { get; set; }
    }
}
