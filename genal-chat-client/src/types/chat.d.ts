declare module 'socket.io-client';
declare module '@aspnet/signalr';

// 所有群的群信息
interface GroupGather {
  [groupId: string]: Group;
}

// 群组
interface Group {
  groupId: string;
  userId: string; // 群主id
  groupName: string;
  notice: string;
  messages?: GroupMessage[];
  createTime: number;
}

// 群与用户关联表
interface GroupMap {
  groupId: string;
  userId: string;
}

// 群消息
interface GroupMessage {
  userId: string;
  groupId: string;
  content: string;
  messageType: MessageType;
  createTime: number;
}

// 所有好友的好友信息
interface FriendGather {
  [userId: string]: Friend;
}

// 好友
interface Friend {
  userId: string;
  userName: string;
  avatar: string;
  role?: string;
  tag?: string;
  messages?: FriendMessage[];
  createTime: number;
}

// 用户与好友关联表
interface UserMap {
  friendId: string;
  userId: string;
}

// 好友消息
interface FriendMessage {
  userId: string;
  friendId: string;
  content: string;
  messageType: MessageType;
  createTime: Date;
  type?: string;
}

interface SendMessage {
  type: string;
  message: string | File;
  width?: number;
  height?: number;
  messageType: MessageType.text | MessageType.image;
}

// 消息类型
declare enum MessageType {
  text = 0,
  image = 1,
}

// 图片尺寸
interface ImageSize {
  width: number;
  height: number;
}

// 服务端返回值格式
interface ServerRes {
  code: number;
  msg: string;
  data: any;
}

// 所有群的在线用户合集
interface ActiveGroupUser {
  [key: string]: User;
}

// 未读消息对象
interface UnReadGather {
  [key: string]: number;
}

// 获取群分页消息参数
interface PagingParams {
  groupId?: string;
  userId?: string;
  friendId?: string;
  current: number;
  pageSize: number;
}

// 群分页消息返回值
interface PagingResponse {
  messageArr: GroupMessage[];
  userArr: User[];
}
