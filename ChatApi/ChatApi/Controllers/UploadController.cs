using AutoMapper;
using ChatApi.Common;
using ChatApi.Hubs;
using ChatApi.Models;
using ChatApi.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("Upload")]
    public class UploadController : ChatApiController
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public UploadController(AppDbContext db, IHubContext<ChatHub> hubContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _dbContext = db;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("SendImageMessage")]
        public ResponseContent SendImageMessage([FromForm]string toId, string type, string width, string height)
        {
            var userId = CurrentUserId;
            var files = HttpContext.Request.Form.Files.ToList();
            if (files == null || files.Count == 0) return Error("请上传文件");
            var maxSize = 3;//默认文件大小3M
            var limitFiles = files.Where(x => x.Length > maxSize * 1024 * 1024);
            if (limitFiles.Count() > 0)
            {
                return Error($"文件大小不能超过：{ maxSize}M,{string.Join(",", limitFiles)}");
            }
            string filePath = $"/Upload/Messages/{CurrentUserId}/";
            string webRootPath = _webHostEnvironment.WebRootPath;//wwwroot位置
            var fullPath = webRootPath + filePath;

            int i = 0;
            try
            {
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                for (i = 0; i < files.Count; i++)
                {
                    var contentType = files[i].ContentType;
                    var suffix = "";
                    switch (contentType)
                    {
                        case "image/jpeg":
                            suffix = ".jpeg";
                            break;
                        case "image/png":
                            suffix = ".png";
                            break;
                        case "image/jpg":
                            suffix = ".jpg";
                            break;
                        case "image/gif":
                            suffix = ".gif";
                            break;
                        default:
                            throw new Exception("请选择jpeg/jpg/png/gif格式的图片!");
                    }
                    var fileName = DateTime.Now.ToString("yyyMMddHHmmsss") + new Random().Next(1000, 9999) + suffix;
                    using (var stream = new FileStream(fullPath + fileName, FileMode.Create))
                    {
                        files[i].CopyTo(stream);
                    }

                    if (type.Equals("group"))
                    {
                        var input = new GroupMessageInput()
                        {
                            Content = filePath + fileName,
                            GroupId = toId,
                            Height = double.Parse(height),
                            Width = double.Parse(width),
                            UserId = userId,
                            MessageType = MessageTypeEnum.图片
                        };
                        MessageCoreBiz.SendGroupMessage(_dbContext, _mapper, CurrentUserId, input);
                        _hubContext.Clients.Group(input.GroupId.ToString()).SendAsync("GroupMessage", MessageResult.Instance.Ok("", input));
                    }
                    else
                    {
                        var input = new FriendMessageDto()
                        {
                            Content = filePath + fileName,
                            FriendId = toId,
                            Height = int.Parse(height),
                            Width = int.Parse(width),
                            UserId = userId,
                            MessageType = MessageTypeEnum.图片
                        };
                        MessageCoreBiz.SendFriendMessage(_dbContext, _mapper, CurrentUserId, input);
                        var roomName = string.Compare(userId, toId) == 1 ? userId + toId : toId + userId;
                        _hubContext.Clients.Group(roomName).SendAsync("FriendMessage", MessageResult.Instance.Ok("", input));
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
            return Success("上传成功");
        }

        /// <summary>
        /// 设置头像
        /// </summary>
        /// <returns></returns>
        [HttpPost("SetUserAvatar")]
        public ResponseContent SetUserAvatar()
        {
            var userId = CurrentUserId;
            var files = HttpContext.Request.Form.Files.ToList();
            if (files == null || files.Count == 0) return Error("请上传文件");
            var maxSize = 3;//默认文件大小3M
            var limitFiles = files.Where(x => x.Length > maxSize * 1024 * 1024);
            if (limitFiles.Count() > 0)
            {
                return Error($"文件大小不能超过：{ maxSize}M,{string.Join(",", limitFiles)}");
            }
            string filePath = $"/Upload/Messages/{CurrentUserId}/";
            string webRootPath = _webHostEnvironment.WebRootPath;//wwwroot位置
            var fullPath = webRootPath + filePath;
            try
            {
                var contentType = files[0].ContentType;
                var suffix = "";
                switch (contentType)
                {
                    case "image/jpeg":
                        suffix = ".jpeg";
                        break;
                    case "image/png":
                        suffix = ".png";
                        break;
                    case "image/jpg":
                        suffix = ".jpg";
                        break;
                    case "image/gif":
                        suffix = ".gif";
                        break;
                    default:
                        throw new Exception("请选择jpeg/jpg/png/gif格式的图片!");
                }
                var fileName = DateTime.Now.ToString("yyyMMddHHmmsss") + new Random().Next(1000, 9999) + suffix;
                using (var stream = new FileStream(fullPath + fileName, FileMode.Create))
                {
                    files[0].CopyTo(stream);
                }
                var user = _dbContext.ChatUsers.FirstOrDefault(t => t.Id == userId);
                user.Avatar = filePath + fileName;
                _dbContext.Update(user);
                _dbContext.SaveChanges();
                return Success("修改成功", _mapper.Map<ChatUser, ChatUsersDto>(user));
            }
            catch (Exception ex)
            {
                return Error("修改失败");
            }

        }
    }
}
