using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class ChatUsersDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Role { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        //public DateTime createTime { get; set; }
    }
}
