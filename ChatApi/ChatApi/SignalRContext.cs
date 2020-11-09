using ChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi
{
    public class SignalRContext
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<ChatUser> Users { get; set; }

        /// <summary>
        /// 群聊
        /// </summary>
        public List<ChatGroup> Groups { get; set; }

        public SignalRContext()
        {
            Users = new List<ChatUser>();
            Groups = new List<ChatGroup>();
        }
    }
}
