using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi
{
    public enum MessageTypeEnum
    {
        文字 = 0,
        图片 = 1,
        语音 = 2
    }
    public enum IsOnlineEnum
    {
        online = 0,
        notOnline = 1
    }
    public enum RoleEnum
    {
        普通用户,
        管理员
    }
    public enum StatusEnum
    {
        正常,
        锁定
    }
}
