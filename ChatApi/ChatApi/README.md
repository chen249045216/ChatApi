# 基于.Net core signalR的IM聊天室

### 开发环境
- VS2019 /VS Code
- .net core 3.1
- MySQL
- Vue 
- Vuex 
- Typescript
### 项目说明
前些天再看到一个不错的IM项目，前后端都是采用typescript开发，界面和功能都比较完善。就拿来主义，前端小改，后端用.net core重新全部实现了一遍。后面计划继续完善项目，开发更多功能。

因为是一套很独立的IM系统，所以很方便集成到现有项目。

### 功能介绍
- 移动端兼容
- 用户信息的修改(头像/密码)
- 群聊/私聊
- 创建群/加入群/退群/模糊搜索群
- 添加好友/删好友/模糊搜索用户
- 消息分页
- 表情包
- 图片发送/图片预览
- 自定义主题
### 计划功能
* [ ] 加密群组
* [ ] 管理员后台
* [ ] 更方便的与现有项目集成方式

### Demo地址
http://chat.apeskill.com/

### 注意
- 创建mysql数据库,并将ChatApi.sql导入
- 修改config配置数据库
- 运行后端
- 运行前端：npm run serve
前端访问：http://localhost:1997/ 后端访问：http://localhost:44383/
### 特别感谢
前端是基于[Genal](https://github.com/genaller)的[genal-chat](https://github.com/genaller/genal-chat)（阿童木聊天室）二次开发的 也是本项目【小兰聊天室】名称的由来

*** 对他表示诚挚的感谢🙏 ***