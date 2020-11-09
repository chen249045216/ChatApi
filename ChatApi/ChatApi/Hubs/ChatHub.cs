using AutoMapper;
using ChatApi.Common;
using ChatApi.Models;
using ChatApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApi.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        public readonly AppDbContext _dbContext;
        public readonly IMapper _mapper;
        const string defaultGroup = "小兰聊天室";
        private readonly static Dictionary<string, string> _connections = new Dictionary<string, string>();
        private readonly static object _lock = new object();

        public ChatHub(AppDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        public  override Task OnConnectedAsync()
        {
            try
            {


                AddOnline();
                //Clients.Caller.SendAsync("getProfileInfo", "test", "test");
                Groups.AddToGroupAsync(Context.ConnectionId, defaultGroup);
                GetActiveGroupUser();
                var userId = CurrentUserId;
                lock (_lock)
                {

                    if (_connections.Keys.FirstOrDefault(t => t.Equals(userId)) == null)
                {
                    _connections.Add(userId, Context.ConnectionId);
                }
                else
                {
                    _connections[userId] = Context.ConnectionId;
                }
                }
                return base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                SendError(ex.Message);
                throw ex;
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            RemoveOnline();
            _connections.Remove(CurrentUserId);
            return base.OnDisconnectedAsync(exception);
        }

        #region 用户

        /// <summary>
        /// 加入私聊频道
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task JoinFriendSocket(string userId, string friendId)
        {
            var relation = await _dbContext.ChatFriendMaps.FirstOrDefaultAsync(t => t.UserId.Equals(userId) && t.FriendId.Equals(friendId));
            if (relation != null)
            {
                var roomName = string.Compare(relation.UserId, relation.FriendId) == 1 ? relation.UserId + relation.FriendId : relation.FriendId + relation.UserId;
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                await Clients.Caller.SendAsync("JoinFriendSocket", MessageResult.Instance.Ok("进入私聊频道成功", relation));
            }
        }

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task FriendMessage(FriendMessageDto input)
        {

            try
            {
                var userId = CurrentUserId;

                MessageCoreBiz.SendFriendMessage(_dbContext, _mapper, userId, input);
                input.CreateTime = DateTime.Now.ToTimestamp();
                var roomName = string.Compare(userId, input.FriendId) == 1 ? userId + input.FriendId : input.FriendId + userId;
                await Clients.Group(roomName).SendAsync("FriendMessage", MessageResult.Instance.Ok("", input));
            }
            catch (Exception ex)
            {
                await SendError(ex.Message);
            }
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task AddFriend(string friendId)
        {
            try
            {


                var userId = CurrentUserId;
                if (friendId.Equals(userId))
                {
                    throw new Exception("不能添加自己为好友");
                }
                var relation1 = _dbContext.ChatFriendMaps.FirstOrDefault(t => t.UserId.Equals(userId) && t.FriendId.Equals(friendId));
                var relation2 = _dbContext.ChatFriendMaps.FirstOrDefault(t => t.UserId.Equals(friendId) && t.FriendId.Equals(userId));
                var roomName = string.Compare(userId, friendId) == 1 ? userId + friendId : friendId + userId;
                if (relation1 != null || relation2 != null)
                {
                    throw new Exception("你们已经是好友关系，请勿重复添加");
                }
                var friend = _mapper.Map<ChatUser, FriendDto>(_dbContext.ChatUsers.FirstOrDefault(t => t.Id.Equals(friendId)));
                var user = _mapper.Map<ChatUser, FriendDto>(_dbContext.ChatUsers.FirstOrDefault(t => t.Id.Equals(userId)));
                if (friend == null)
                {
                    throw new Exception("好友不存在");
                }

                _dbContext.ChatFriendMaps.Add(new ChatFriendMap()
                {
                    UserId = userId,
                    FriendId = friendId
                });
                _dbContext.ChatFriendMaps.Add(new ChatFriendMap()
                {
                    UserId = friendId,
                    FriendId = userId
                });
                //如果是删掉的好友重新加, 重新获取一遍私聊消息
                var messages = _dbContext.ChatFriendMessages.Where(t => (t.UserId.Equals(userId) & t.FriendId.Equals(friendId)) || (t.UserId.Equals(friendId) && t.FriendId.Equals(userId))).OrderByDescending(t => t.CreateTime).Take(30).ToList();
                if (messages.Count > 0)
                {
                    friend.Messages = _mapper.Map<List<ChatFriendMessage>, List<FriendMessageDto>>(messages);
                    user.Messages = _mapper.Map<List<ChatFriendMessage>, List<FriendMessageDto>>(messages);
                }
                _dbContext.SaveChanges();
                await SendUserMessage("AddFriend", userId, $"添加好友{friend.UserName}成功", friend);
                await SendUserMessage("AddFriend", friendId, $"{user.UserName}添加你未好友", user);
            }
            catch (Exception ex)
            {
                await SendError(ex.Message);
            }
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task ExitFriend(string friendId)
        {
            var userId = CurrentUserId;
            var map1 = _dbContext.ChatFriendMaps.FirstOrDefault(t => t.UserId.Equals(userId) & t.FriendId.Equals(friendId));
            var map2 = _dbContext.ChatFriendMaps.FirstOrDefault(t => t.UserId.Equals(friendId) & t.FriendId.Equals(userId));
            if (map1 != null && map2 != null)
            {
                _dbContext.Remove(map1);
                _dbContext.Remove(map2);
                _dbContext.SaveChanges();
                await SendCallerMessage("ExitFriend", "删除好友成功", map1);
            }
            else
            {
                await SendError("删除好友失败");
            }

        }

        #endregion

        #region 群组
        /// <summary>
        /// 创建群组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddGroup(AddGroupInput input)
        {
            try
            {
                var user = _dbContext.ChatUsers.FirstOrDefault(t => t.Id == input.UserId);
                if (user == null)
                {
                    throw new Exception("你没资格创建群");
                }
                var group = _dbContext.ChatGroups.FirstOrDefault(t => t.GroupName.Equals(input.GroupName));
                if (group != null)
                {
                    throw new Exception("该群名已存在");
                }
                var chatGroup = new ChatGroup()
                {
                    UserId = input.UserId,
                    GroupName = input.GroupName,
                    Notice = "无公告",
                    CreateTime = DateTime.Now.ToTimestamp()
                };
                var groupMap = new ChatGroupMap()
                {
                    UserId = chatGroup.UserId,
                    GroupId = chatGroup.Id
                };
                _dbContext.Add(chatGroup);
                _dbContext.Add(groupMap);
                _dbContext.Add(new ChatGroupMessage()
                {
                    GroupId = chatGroup.Id,
                    MessageType = MessageTypeEnum.文字,
                    UserId = input.UserId,
                    Content = $"我创建了群【{chatGroup.GroupName}】",
                    CreateTime = DateTime.Now.ToTimestamp()
                });
                _dbContext.SaveChanges();
                await SendCallerMessage("AddGroup", $"成功创建群{input.GroupName}", groupMap);

            }
            catch (Exception ex)
            {
                await SendError(ex.Message);
            }
        }

        /// <summary>
        /// 加入群组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task JoinGroup(JoinGroupInput input)
        {
            try
            {
                var userId = CurrentUserId;
                var group = _dbContext.ChatGroups.FirstOrDefault(t => t.Id.Equals(input.GroupId));
                if (group == null)
                {
                    throw new Exception("群组不存在");
                }
                var userGroup = _dbContext.ChatGroupMaps.FirstOrDefault(t => t.UserId.Equals(userId) && t.GroupId.Equals(input.GroupId));
                var user = _dbContext.ChatUsers.FirstOrDefault(t => t.Id == CurrentUserId);
                if (userGroup != null)
                {
                    throw new Exception("您已在该群组");
                }
                if (userGroup == null)
                {
                    _dbContext.ChatGroupMaps.Add(new ChatGroupMap()
                    {
                        GroupId = input.GroupId,
                        UserId = userId
                    });
                    await Groups.AddToGroupAsync(Context.ConnectionId, group.Id);
                    await Clients.Groups(group.Id).SendAsync("JoinGroup", MessageResult.Instance.Ok($"{user.UserName}加入群{group.GroupName}", new
                    {
                        group = _mapper.Map<ChatGroup, JoinGroupDto>(group),
                        user = _mapper.Map<ChatUser, ChatUsersDto>(user)
                    }));
                    _dbContext.SaveChanges();
                    GetActiveGroupUser();
                }

            }
            catch (Exception ex)
            {
                await SendError(ex.Message);
            }

        }

        /// <summary>
        /// 退出群组
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task ExitGroup(string groupId)
        {
            try
            {
                var userId = CurrentUserId;
                if (groupId.Equals(defaultGroup))
                {
                    throw new Exception("默认群组不能删除");
                }
                var group = _dbContext.ChatGroups.FirstOrDefault(t => t.Id.Equals(groupId));
                var map = _dbContext.ChatGroupMaps.FirstOrDefault(t => t.UserId.Equals(userId) && t.GroupId.Equals(groupId));
                if (group != null && map != null)
                {
                    _dbContext.Remove(map);
                    _dbContext.SaveChanges();
                    await SendCallerMessage("ExitGroup", "退群成功", map);
                    await this.GetActiveGroupUser();
                }
            }
            catch (Exception ex)
            {
                SendError(ex.Message);
            }

        }

        /// <summary>
        /// 加入群聊的连接
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task JoinGroupSocket(string groupId, string userId)
        {
            var group = await _dbContext.ChatGroups.FirstOrDefaultAsync(t => t.Id.Equals(groupId));
            var user = await _dbContext.ChatUsers.FirstOrDefaultAsync(t => t.Id.Equals(userId));
            if (group != null && user != null)
            {
                var res = new
                {
                    group = _mapper.Map<ChatGroup, GroupDto>(group),
                    user = _mapper.Map<ChatUser, ChatUsersDto>(user)
                };
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id);
                await Clients.OthersInGroup(group.Id.ToString()).SendAsync("JoinGroupSocket", MessageResult.Instance.Ok($"({user.UserName})加入群({group.GroupName})", res));
            }
            else
            {
                await Clients.Caller.SendAsync("JoinGroupSocket", MessageResult.Instance.Error("进群失败", ""));
            }
        }

        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public async Task GroupMessage(GroupMessageInput input)
        {
            try
            {
                MessageCoreBiz.SendGroupMessage(_dbContext, _mapper, CurrentUserId, input);
                await Clients.Group(input.GroupId.ToString()).SendAsync("GroupMessage", MessageResult.Instance.Ok("", input));
            }
            catch (Exception ex)
            {
                await SendError(ex.Message);
            }

        }
        #endregion

        /// <summary>
        /// 获取所有群和好友数据
        /// </summary>
        /// <returns></returns>
        public async Task ChatData()
        {
            try
            {
                var userId = CurrentUserId;
                if (userId == null)
                {

                }
                var groupList = _dbContext.ChatGroupMaps.Include(t => t.ChatGroup).ThenInclude(t => t.ChatGroupMessages).ThenInclude(t => t.ChatUser).Where(t => t.UserId == userId).Select(t => t.ChatGroup).ToList();
                var groupDtoList = _mapper.Map<List<ChatGroup>, List<GroupDto>>(groupList);

                var friendMaps = _dbContext.ChatFriendMaps.Include(t => t.ChatFriend).Include(t => t.ChatFriend.FriendFromMessage).Include(t => t.ChatFriend.FriendToMessage).Where(t => t.UserId == userId).ToList();
                var frindDtoList = _mapper.Map<List<ChatFriendMap>, List<FriendDto>>(friendMaps);
                //foreach (var item in frindDtoList)
                //{
                //    item.Messages=_dbContext.ChatFriendMessages.Where(t=>t.UserId.Equals(item.UserId)&&t.FriendId.Equals(item.))
                //}

                List<ChatUsersDto> userList = new List<ChatUsersDto>();
                foreach (var groupMessages in groupList.Select(t => t.ChatGroupMessages))
                {
                    foreach (var item in groupMessages)
                    {
                        if (userList.FirstOrDefault(t => t.UserId == item.UserId) == null)
                        {
                            userList.Add(_mapper.Map<ChatUser, ChatUsersDto>(item.ChatUser));
                        }
                    }
                }
                userList.AddRange(_mapper.Map<List<FriendDto>, List<ChatUsersDto>>(frindDtoList));
                //var userDtoList= _mapper.Map<List<ChatUser>, List<ChatUsersDto>>(chatUsers);

                await Clients.Caller.SendAsync("ChatData", MessageResult.Instance.Ok("获取聊天数据成功", new
                {
                    groupData = groupDtoList,
                    friendData = frindDtoList,
                    userData = userList
                }));

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取群组在线用户
        /// </summary>
        /// <returns></returns>
        public async Task GetActiveGroupUser()
        {
            try
            {
                var userIdArr = _dbContext.ChatUsers.Select(t => t.Id).ToList();
                var activeGroupUserGather = new Dictionary<string, Dictionary<string, ChatUsersDto>>();
                foreach (var userId in userIdArr)
                {
                    var userGroupArr = _dbContext.ChatGroupMaps.Where(t => t.UserId.Equals(userId)).ToList();
                    var user = _dbContext.ChatUsers.FirstOrDefault(t => t.Id == userId);
                    if (user != null && userGroupArr.Count() > 0)
                    {
                        foreach (var item in userGroupArr)
                        {
                            if (activeGroupUserGather.Keys.FirstOrDefault(t => t == item.GroupId) == null)
                            {
                                activeGroupUserGather.Add(item.GroupId, new Dictionary<string, ChatUsersDto>());
                                //activeGroupUserGather[item.GroupId] = new Dictionary<string, ChatUsersDto>();
                            }
                            activeGroupUserGather[item.GroupId].Add(userId, _mapper.Map<ChatUser, ChatUsersDto>(user));
                            //activeGroupUserGather[item.GroupId][userId] = _mapper.Map<ChatUser, ChatUsersDto>(user);
                        }
                    }
                }
                await Clients.Group(defaultGroup).SendAsync("activeGroupUser", MessageResult.Instance.Ok("activeGroupUser", activeGroupUserGather));
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("OnError", MessageResult.Instance.Error(message, null));
        }
        private async Task SendCallerMessage(string method, string message, object data)
        {
            await Clients.Caller.SendAsync(method, MessageResult.Instance.Ok(message, data));
        }
        private async Task SendUserMessage(string method, string userId, string message, object data)
        {
            if (_connections.Keys.FirstOrDefault(t => t.Equals(userId)) != null)
            {
                await Clients.Client(_connections[userId]).SendAsync(method, MessageResult.Instance.Ok(message, data));
            }
        }
        /// <summary>
        /// 添加在线人员
        /// </summary>
        public void AddOnlineUser(string nickName)
        {
            if (!string.IsNullOrWhiteSpace(nickName))
            {
                var uid = Guid.NewGuid().ToString().ToUpper();
                //添加在线人员
                //userInfoList.Add(new UserInfo
                //{
                //    ConnectionId = Context.ConnectionId,
                //    UserID = uid,//随机用户id
                //    UserName = nickName,
                //    LoginTime = DateTime.Now,
                //    JoimTime = DateTime.Now
                //});
                //Clients.Client(Context.ConnectionId).showJoinMessage(uid);
            }
        }

        public string CurrentUserId
        {
            get
            {

                if (Context.GetHttpContext() == null)
                {
                    return null;
                }
                var principal = Context.GetHttpContext().User as ClaimsPrincipal;
                var userIdClaim = principal?.Claims.FirstOrDefault();
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                return userIdClaim.Value;

            }
        }

        private void AddOnline()
        {
            string userId = CurrentUserId.ToString();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }
            using (var scope = new AppDbContextScope())
            {
                var db = scope.AppDb;
                var userInfo = db.ChatUsers.AsTracking().FirstOrDefault(t => t.Id.Equals(userId));
                userInfo.IsOnline = IsOnlineEnum.online;
                userInfo.LastLoginTime = DateTime.Now.ToTimestamp();
                db.SaveChanges();
            }
        }

        private void RemoveOnline()
        {
            string userId = CurrentUserId.ToString();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }
            using (var scope = new AppDbContextScope())
            {
                var db = scope.AppDb;
                var userInfo = db.ChatUsers.AsTracking().FirstOrDefault(t => t.Id.Equals(userId));
                userInfo.IsOnline = IsOnlineEnum.notOnline;
                db.SaveChanges();
            }
        }


    }
}
