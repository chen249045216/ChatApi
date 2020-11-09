using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class FriendDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public int? Role { get; set; }
        public string Tag { get; set; }
        public List<FriendMessageDto> Messages { get; set; }
        public double CreateTime { get; set; }
    }
}
