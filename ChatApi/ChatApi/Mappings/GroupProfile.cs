using AutoMapper;
using ChatApi.Models;
using ChatApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Mappings
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<ChatGroup, GroupDto>()
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(t => t.UserId))
                .ForMember(dst => dst.GroupId, opt => opt.MapFrom(t => t.Id))
                .ForMember(dst => dst.Messages, opt => opt.MapFrom(t => t.ChatGroupMessages.OrderBy(x => x.CreateTime)));
            CreateMap<ChatGroup, GroupMessageDto>();
            //.ForMember(dst=>dst.Messages,opt=>opt.MapFrom(t=>t.ChatGroupMaps.))

            CreateMap<ChatGroupMessage, GroupMessageDto>();
            CreateMap<GroupMessageInput, ChatGroupMessage>();
            CreateMap<ChatGroupMessage, GroupMessageInput>();
            CreateMap<ChatGroup, JoinGroupDto>()
                .ForMember(dst => dst.GroupId, opt => opt.MapFrom(t => t.Id));
        }
    }
}
