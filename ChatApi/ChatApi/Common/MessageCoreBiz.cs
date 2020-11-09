using AutoMapper;
using ChatApi.Models;
using ChatApi.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Common
{
    public class MessageCoreBiz
    {
        public static void SendFriendMessage(AppDbContext _dbContext, IMapper _mapper, string userId, FriendMessageDto input)
        {
            try
            {
                var user = _dbContext.ChatUsers.FirstOrDefault(t => t.Id == userId);
                if (user == null)
                {
                    throw new Exception("你没资格发消息");
                }
                if (input.MessageType == MessageTypeEnum.图片)
                {


                }
                var userRelation = _dbContext.ChatFriendMaps.Where(t => t.UserId.Equals(userId) && t.FriendId.Equals(input.FriendId));
                if (userRelation.Count() == 0)
                {
                    throw new Exception("对方不是你的好友");
                }

                var friendMessage = _mapper.Map<FriendMessageDto, ChatFriendMessage>(input);
                friendMessage.CreateTime = DateTime.Now.ToTimestamp();
                _dbContext.Add(friendMessage);
                _dbContext.SaveChanges();
                

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static void SendGroupMessage(AppDbContext _dbcontext, IMapper _mapper, string userId, GroupMessageInput input)
        {
            var user = _dbcontext.ChatUsers.FirstOrDefault(t => t.Id == userId);
            if (user == null)
            {
                throw new Exception("你没资格发消息");
            }

            var userGroupMap = _dbcontext.ChatGroupMaps.FirstOrDefault(t => t.UserId == input.UserId && t.GroupId == input.GroupId);
            if (userGroupMap == null)
            {
                throw new Exception("群消息发送错误");
            }
            if (input.MessageType == MessageTypeEnum.图片)
            {

            }
            var groupMessage = _mapper.Map<GroupMessageInput, ChatGroupMessage>(input);
            groupMessage.CreateTime = DateTime.Now.ToTimestamp();
            _dbcontext.Add(groupMessage);
            _dbcontext.SaveChanges();

        }
    }
}
