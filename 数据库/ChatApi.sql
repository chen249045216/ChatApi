/*
 Navicat Premium Data Transfer

 Source Server         : ChatApi
 Source Server Type    : MySQL
 Source Server Version : 50649
 Source Host           : 139.129.116.139:3306
 Source Schema         : ChatApi

 Target Server Type    : MySQL
 Target Server Version : 50649
 File Encoding         : 65001

 Date: 10/11/2020 11:42:33
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for chat_friend_map
-- ----------------------------
DROP TABLE IF EXISTS `chat_friend_map`;
CREATE TABLE `chat_friend_map` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL COMMENT '用户ID',
  `FriendId` varchar(255) NOT NULL COMMENT '好友ID',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of chat_friend_map
-- ----------------------------
BEGIN;
COMMIT;

-- ----------------------------
-- Table structure for chat_friend_message
-- ----------------------------
DROP TABLE IF EXISTS `chat_friend_message`;
CREATE TABLE `chat_friend_message` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL COMMENT '用户ID',
  `FriendId` varchar(255) NOT NULL COMMENT '好友ID',
  `Content` text CHARACTER SET utf8mb4 NOT NULL COMMENT '内容',
  `MessageType` int(11) NOT NULL COMMENT '消息类型',
  `CreateTime` double(16,0) NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of chat_friend_message
-- ----------------------------
BEGIN;
COMMIT;

-- ----------------------------
-- Table structure for chat_group
-- ----------------------------
DROP TABLE IF EXISTS `chat_group`;
CREATE TABLE `chat_group` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL COMMENT '用户ID',
  `GroupName` varchar(255) NOT NULL COMMENT '群名称',
  `Notice` varchar(500) NOT NULL COMMENT '公告',
  `CreateTime` double(16,0) NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of chat_group
-- ----------------------------
BEGIN;
INSERT INTO `chat_group` VALUES ('小兰聊天室', '1', '小兰聊天室', '小兰聊天室', 1604932934431);
COMMIT;

-- ----------------------------
-- Table structure for chat_group_map
-- ----------------------------
DROP TABLE IF EXISTS `chat_group_map`;
CREATE TABLE `chat_group_map` (
  `Id` varchar(255) NOT NULL,
  `GroupId` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of chat_group_map
-- ----------------------------
BEGIN;
COMMIT;

-- ----------------------------
-- Table structure for chat_group_message
-- ----------------------------
DROP TABLE IF EXISTS `chat_group_message`;
CREATE TABLE `chat_group_message` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) DEFAULT NULL COMMENT '用户ID',
  `GroupId` varchar(255) DEFAULT NULL COMMENT '聊组ID',
  `Content` text CHARACTER SET utf8mb4 COMMENT '内容',
  `MessageType` int(11) DEFAULT NULL COMMENT '消息类型',
  `CreateTime` double(16,0) DEFAULT NULL COMMENT '创建时间',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of chat_group_message
-- ----------------------------
BEGIN;
COMMIT;

-- ----------------------------
-- Table structure for chat_user
-- ----------------------------
DROP TABLE IF EXISTS `chat_user`;
CREATE TABLE `chat_user` (
  `Id` varchar(255) NOT NULL,
  `UserName` varchar(255) NOT NULL COMMENT '用户名',
  `PassWord` varchar(255) NOT NULL COMMENT '密码',
  `NickName` varchar(255) NOT NULL COMMENT '昵称',
  `Avatar` varchar(255) NOT NULL COMMENT '头像',
  `Role` int(11) NOT NULL DEFAULT '0' COMMENT '权限',
  `Status` int(11) NOT NULL DEFAULT '0' COMMENT '状态',
  `Tag` varchar(255) DEFAULT NULL COMMENT '标签',
  `IsOnline` int(1) DEFAULT '0' COMMENT '是否在线',
  `CreateTime` double(16,0) DEFAULT NULL COMMENT '创建时间',
  `LastLoginTime` double(16,0) DEFAULT NULL COMMENT '最后登录时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of chat_user
-- ----------------------------
BEGIN;
COMMIT;

SET FOREIGN_KEY_CHECKS = 1;
