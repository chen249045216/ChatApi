﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class GroupMessageInput
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string Content { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public MessageTypeEnum MessageType { get; set; }
    }
}
