using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.ViewModels
{
    public class JoinGroupDto
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public string GroupName { get; set; }
        public string Notice { get; set; }
        public double CreateTime { get; set; }
    }
}
