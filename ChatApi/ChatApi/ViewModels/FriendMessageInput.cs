using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class FriendMessageInput
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public IFormFile Content { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        public double CreateTime { get; set; }
    }
}
