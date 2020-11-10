using AutoMapper;
using ChatApi.Common;
using ChatApi.Models;
using ChatApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ChatUser, ChatUsersDto>()
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(x => x.Id))
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.UserName))
                .ForMember(dst => dst.Tag, opt => opt.MapFrom(x => x.Tag))
                .ForMember(dst => dst.Role, opt => opt.MapFrom(x => x.Role))
                .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.Avatar.ToImageUrl()));

            CreateMap<ChatUsersDto, ChatUser>();
            CreateMap<ChatFriendMap, FriendDto>()
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(x => x.FriendId))
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.ChatFriend.UserName))
                .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.ChatFriend.Avatar.ToImageUrl()))
                .ForMember(dst => dst.Role, opt => opt.MapFrom(x => x.ChatFriend.Role))
                .ForMember(dst => dst.Tag, opt => opt.MapFrom(x => x.ChatFriend.Tag))
                .ForMember(dst => dst.CreateTime, opt => opt.MapFrom(x => x.ChatFriend.CreateTime))
                //.ForMember(dst => dst.Messages, opt => opt.MapFrom(x => x.ChatFriend.FriendFromMessage.Where(t => (t.FriendId == x.UserId && t.UserId == x.FriendId) || (t.FriendId
                //       == x.FriendId && t.UserId == x.UserId))))
                .ForMember(dst => dst.Messages, opt => opt.MapFrom(x => x.ChatFriend.FriendFromMessage.Where(t => (t.FriendId == x.UserId && t.UserId == x.FriendId) || (t.FriendId
                       == x.FriendId && t.UserId == x.UserId)).Union(x.ChatFriend.FriendToMessage.Where(t => (t.FriendId == x.UserId && t.UserId == x.FriendId) || (t.FriendId
                       == x.FriendId && t.UserId == x.UserId))).OrderBy(t => t.CreateTime)));

            CreateMap<ChatFriendMessage, FriendMessageDto>();
            CreateMap<FriendMessageDto, ChatFriendMessage>();

            CreateMap<ChatUser, FriendDto>()
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(x => x.Id))
                .ForMember(dst=>dst.Avatar,opt=>opt.MapFrom(x=>x.Avatar.ToImageUrl()));

            CreateMap<FriendDto, ChatUsersDto>();
        }
    }
}
