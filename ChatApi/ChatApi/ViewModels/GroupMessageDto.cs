using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class GroupMessageDto
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string Content { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        public double CreateTime { get; set; }
    }
}
