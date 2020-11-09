using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class GroupDto
    {
        public GroupDto()
        {
            Messages = new List<GroupMessageDto>();
        }
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public string GroupName { get; set; }
        public string Notice { get; set; }
        public List<GroupMessageDto> Messages { get; set; }
        public double CreateTime { get; set; }
    }
}
