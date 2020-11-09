import { GetterTree } from 'vuex';
import { ChatState } from './state';
import { RootState } from '../../index';
import { State } from 'vuex-class';

const getters: GetterTree<ChatState, RootState> = {
  socket(state) {
    return state.socket;
  },
  signal(state){
    return state.signal;
  },
  dropped(state) {
    return state.dropped;
  },
  activeGroupUser(state) {
    return state.activeGroupUser;
  },
  activeRoom(state) {
    return state.activeRoom;
  },
  groupGather(state) {
    return state.groupGather;
  },
  friendGather(state) {
    return state.friendGather;
  },
  userGather(state) {
    return state.userGather;
  },
  unReadGather(state) {
    return state.unReadGather;
  },
};

export default getters;
