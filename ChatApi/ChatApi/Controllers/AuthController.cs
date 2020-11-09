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
    [Route("Auth")]
    public class AuthController : ChatApiController
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;
        public AuthController(AppDbContext db, IHubContext<ChatHub> hubContext, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _dbContext = db;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public ResponseContent Login([FromBody]LoginUserInput input)
        {
            try
            {
                var user = _dbContext.ChatUsers.FirstOrDefault(t => t.UserName.Equals(input.UserName));
                if (user == null)
                {
                    return Error("用户名不存在");
                }
                if (user.PassWord != input.Password)
                {
                    return Error("用户密码不正确");
                }

                var userViewModel = _mapper.Map<ChatUser, ChatUsersDto>(user);
                var token = GetToken(user);
                return Success("登录成功", new
                {
                    user = userViewModel,
                    token
                });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public ResponseContent Register([FromBody]RegisterUsersInput input)
        {
            try
            {
                ValidateInput();

                var user = _dbContext.ChatUsers.FirstOrDefault(t => t.UserName.Equals(input.username));
                if (user != null)
                {
                    return Error("用户名重复,请换个账号");
                }

                var avatar = $"/Upload/avatar/avatar({new Random().Next(1, 20)}).png";
                var newUser = new ChatUser()
                {
                    Avatar = avatar,
                    CreateTime = DateTime.Now.ToTimestamp(),
                    IsOnline = IsOnlineEnum.online,
                    NickName = input.username,
                    PassWord = input.password,
                    Role = RoleEnum.普通用户,
                    Status = StatusEnum.正常,
                    UserName = input.username
                };
                _dbContext.Add(newUser);
                _dbContext.Add(new ChatGroupMap()
                {
                    GroupId = "小兰聊天室",
                    UserId = newUser.Id
                });
                _dbContext.SaveChanges();
                var userDto = _mapper.Map<ChatUser, ChatUsersDto>(newUser);
                var userModel = _dbContext.ChatUsers.FirstOrDefault(t => t.UserName.Equals(input.username));
                var token = GetToken(userModel);
                return Success("注册成功", new
                {
                    user = userDto,
                    token
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        private string GetToken(ChatUser user)
        {
            string token = JwtHelper.IssueAppJwt(new ChatUser()
            {
                Id = user.Id,
                UserName = user.UserName
            });
            return token;
        }

    }
}
