import { ActionTree } from 'vuex';
import { ChatState } from './state';
import { RootState } from '../../index';
import io from 'socket.io-client';

import Vue from 'vue';
import {
  SET_SOCKET,
  SET_SIGNAL,
  SET_DROPPED,
  SET_ACTIVE_GROUP_USER,
  ADD_GROUP_MESSAGE,
  ADD_FRIEND_MESSAGE,
  SET_FRIEND_MESSAGES,
  SET_GROUP_GATHER,
  SET_FRIEND_GATHER,
  SET_USER_GATHER,
  SET_ACTIVE_ROOM,
  DEL_GROUP,
  DEL_FRIEND,
  ADD_UNREAD_GATHER,
} from './mutation-types';
import { DEFAULT_GROUP } from '@/const/index';
import {socketUrl}from '@/api/fetch/config'
import * as signalR from '@aspnet/signalr';

const actions: ActionTree<ChatState, RootState> = {
  // 初始化socket连接和监听socket事件
  async connectSocket({ commit, state, dispatch, rootState }, callback) {
    let user = rootState.app.user;
    let token = rootState.app.token;
    let url=socketUrl+'/chatHub';
    //signalR
    let signal = new signalR.HubConnectionBuilder() //服务器地址
      .withUrl(url, { accessTokenFactory: () => token })
      .build();

    signal
      .start()
      .then(function (data: any) {
        signal.invoke('chatData');
        console.log("已成功连接到signalr服务器");

      // 先保存好socket对象
      commit(SET_SIGNAL, signal);
      })
      .catch(function (error: any) {
        setTimeout("console.log('重连中...');",5000);
        dispatch("connectSocket");
        console.log(error);
      });

    // 获取所有群和好友数据
    signal.on("chatData", (res: ServerRes) => {
      if (res.code) {
        return Vue.prototype.$message.error(res.msg);
      }
      dispatch('handleChatData', res.data);
      commit(SET_DROPPED, false);
    });

    signal.onclose(async()=>{
      
      console.log("监听到连接关闭");
      // setTimeout("console.log('重连中...');",5000);
      dispatch("connectSocket");
    });

    // let socket: SocketIOClient.Socket = io.connect(`/?userId=${user.userId}`, { reconnection: true });

    // socket.on('connect', async () => {
    //   console.log('连接成功');

    //   // 获取聊天室所需所有信息
    //   socket.emit('chatData', user);

    //   // 先保存好socket对象
    //   commit(SET_SOCKET, socket);
    // });

    // 初始化事件监听
    signal.on('ActiveGroupUser', (data: any) => {
      console.log('activeGroupUser', data);
      commit(SET_ACTIVE_GROUP_USER, data.data);
    });

    signal.on('addGroup', (res: ServerRes) => {
      console.log('on addGroup', res);
      if (res.code) {
        return Vue.prototype.$message.error(res.msg);
      }
      Vue.prototype.$message.success(res.msg);
      commit(SET_GROUP_GATHER, res.data);
    });

    signal.on('joinGroup', async (res: ServerRes) => {
      console.log('on joinGroup', res);
      if (res.code) {
        return Vue.prototype.$message.error(res.msg);
      }
      let newUser = res.data.user;
      let group = res.data.group;
      if (newUser.userId != user.userId) {
        commit(SET_USER_GATHER, newUser);
        return Vue.prototype.$message.info(`${newUser.username}加入群${group.groupName}`);
      } else {
        console.log(state.groupGather, group.groupId);
        debugger;
        // 是用户自己 则加入到某个群
        if (!state.groupGather[group.groupId]) {
          commit(SET_GROUP_GATHER, group);
          // 获取群里面所有用户的用户信息
          signal.invoke('chatData');
        }
        Vue.prototype.$message.info(`成功加入群${group.groupName}`);
        commit(SET_ACTIVE_ROOM, state.groupGather[group.groupId]);
      }
    });

    signal.on('JoinGroupSocket', (res: ServerRes) => {
      console.log('on joinGroupSocket', res);
      if (res.code) {
        return Vue.prototype.$message.error(res.msg);
      }
      let newUser: Friend = res.data.user;
      let group: Group = res.data.group;
      let friendGather = state.friendGather;
      if (newUser.userId != user.userId) {
        commit(SET_USER_GATHER, newUser);
        if (friendGather[newUser.userId]) {
          // 当用户的好友更新了用户信息
          let messages;
          if (friendGather[newUser.userId].messages) {
            messages = friendGather[newUser.userId].messages;
          }
          commit(SET_FRIEND_GATHER, newUser);
          commit(SET_FRIEND_MESSAGES, messages);
        }
        // @ts-ignore 解决重复进群消息问题
        if (window.msg === newUser.userId) {
          return;
        }
        // @ts-ignore
        window.msg = newUser.userId;
        return Vue.prototype.$message.info(`${newUser.userName}加入群${group.groupName}`);
      } else {
        if (!state.groupGather[group.groupId]) {
          commit(SET_GROUP_GATHER, group);
        }
        commit(SET_USER_GATHER, newUser);
      }
    });

    signal.on('GroupMessage', (res: ServerRes) => {
      console.log('on groupMessage', res);
      if (!res.code) {
        commit(ADD_GROUP_MESSAGE, res.data);
        let activeRoom = state.activeRoom;
        if (activeRoom && activeRoom.groupId !== res.data.groupId) {
          commit(ADD_UNREAD_GATHER, res.data.groupId);
        }
      } else {
        Vue.prototype.$message.error(res.msg);
      }
    });

    signal.on('addFriend', (res: ServerRes) => {
      console.log('on addFriend', res);
      if (!res.code) {
        commit(SET_FRIEND_GATHER, res.data);
        commit(SET_USER_GATHER, res.data);
        Vue.prototype.$message.info(res.msg);
        signal.invoke('joinFriendSocket', user.userId, res.data.userId);
      } else {
        Vue.prototype.$message.error(res.msg);
      }
    });

    signal.on('JoinFriendSocket', (res: ServerRes) => {
      console.log('on JoinFriendSocket', res);
      if (!res.code) {
        console.log('成功加入私聊房间');
      }
    });

    signal.on('friendMessage', (res: ServerRes) => {
      console.log('on friendMessage', res);
      if (!res.code) {
        if (res.data.friendId === user.userId || res.data.userId === user.userId) {
          console.log('ADD_FRIEND_MESSAGE', res.data);
          commit(ADD_FRIEND_MESSAGE, res.data);
          let activeRoom = state.activeRoom;
          if (activeRoom && activeRoom.userId !== res.data.userId) {
            commit(ADD_UNREAD_GATHER, res.data.userId);
          }
        }
      } else {
        Vue.prototype.$message.error(res.msg);
      }
    });

    signal.on('OnError',(res:ServerRes)=>{
      Vue.prototype.$message.error(res.msg);
    });

    // socket.on('chatData', (res: ServerRes) => {
    //   if (res.code) {
    //     return Vue.prototype.$message.error(res.msg);
    //   }
    //   dispatch('handleChatData', res.data);
    //   commit(SET_DROPPED, false);
    // });

    signal.on('ExitGroup', (res: ServerRes) => {
      if (!res.code) {
        commit(DEL_GROUP, res.data);
        commit(SET_ACTIVE_ROOM, state.groupGather[DEFAULT_GROUP]);
        Vue.prototype.$message.success(res.msg);
      } else {
        Vue.prototype.$message.error(res.msg);
      }
    });

    signal.on('ExitFriend', (res: ServerRes) => {
      if (!res.code) {
        commit(DEL_FRIEND, res.data);
        commit(SET_ACTIVE_ROOM, state.groupGather[DEFAULT_GROUP]);
        Vue.prototype.$message.success(res.msg);
      } else {
        Vue.prototype.$message.error(res.msg);
      }
    });
  },

  async handleChatData({ commit, dispatch, state, rootState }, payload) {
    let user = rootState.app.user;
    let signal = state.signal;
    let groupGather = state.groupGather;
    let groupArr = payload.groupData;
    let friendArr = payload.friendData;
    let userArr = payload.userData;
    if (groupArr.length) {
      for (let group of groupArr) {
        // socket.emit('joinGroupSocket', {
        //   groupId: group.groupId,
        //   userId: user.userId,
        // });
        signal.invoke('JoinGroupSocket',group.groupId,user.userId);
        commit(SET_GROUP_GATHER, group);
      }
    }
    if (friendArr.length) {
      for (let friend of friendArr) {
        signal.invoke('JoinFriendSocket', user.userId,friend.userId);
        commit(SET_FRIEND_GATHER, friend);
      }
    }
    if (userArr.length) {
      for (let user of userArr) {
        commit(SET_USER_GATHER, user);
      }
    }

    /**
     * 由于groupgather和userGather都更新了, 但是activeGather依旧是老对象,
     * 这里需要根据老的activeGather找到最新的gather对象,这样才能使得vue的watch监听新gather
     */

    let activeRoom = state.activeRoom;
    let groupGather2 = state.groupGather;
    let friendGather2 = state.friendGather;
    if (!activeRoom) {
      // 更新完数据没有默认activeRoom设置群为'阿童木聊天室'
      return commit(SET_ACTIVE_ROOM, groupGather[DEFAULT_GROUP]);
    }
    commit(SET_ACTIVE_ROOM, groupGather2[activeRoom.groupId] || friendGather2[activeRoom.userId]);
  },
};

export default actions;