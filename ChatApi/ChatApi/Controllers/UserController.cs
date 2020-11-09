using AutoMapper;
using ChatApi.Common;
using ChatApi.Hubs;
using ChatApi.Models;
using ChatApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : ChatApiController
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;
        public UserController(AppDbContext db, IHubContext<ChatHub> hubContext, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _dbContext = db;
        }
        [HttpGet("FindByName")]
        public ResponseContent FindByName(string userName)
        {
            var res = _dbContext.ChatUsers.Where(t => t.UserName.Contains(userName)).ToList();
            return Success(_mapper.Map<List<ChatUser>, List<ChatUsersDto>>(res));
        }

        [HttpPost("UpdatePassword")]
        public ResponseContent UpdatePassword(
            [Required(ErrorMessage = "密码不能为空")]
        [MinLength(5, ErrorMessage = "密码最少5位")]
        [MaxLength(12, ErrorMessage = "密码最多12位")]
        string passWord)
        {
            try
            {
                ValidateInput();

                var userId = CurrentUserId;
                var user = _dbContext.ChatUsers.FirstOrDefault(t => t.Id.Equals(userId));
                user.PassWord = passWord;
                _dbContext.Update(user);
                _dbContext.SaveChanges();
                return Success("修改密码成功");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

    }
}
