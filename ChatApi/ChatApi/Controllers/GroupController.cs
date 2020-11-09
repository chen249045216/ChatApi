using AutoMapper;
using ChatApi.Common;
using ChatApi.Hubs;
using ChatApi.Models;
using ChatApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("Group")]
    public class GroupController : ChatApiController
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;
        public GroupController(AppDbContext db, IHubContext<ChatHub> hubContext, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _dbContext = db;
        }

        /// <summary>
        /// 根据用户名查询群组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [HttpGet("FindByName")]
        public ResponseContent FindByName(string groupName)
        {
            var groups = _dbContext.ChatGroups.Where(t => t.GroupName.Contains(groupName)).ToList();
            return Success(_mapper.Map<List<ChatGroup>, List<JoinGroupDto>>(groups));
        }
    }
}
