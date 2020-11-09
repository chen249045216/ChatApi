-- MySQL dump 10.13  Distrib 5.6.49, for Linux (x86_64)
--
-- Host: localhost    Database: ChatApi
-- ------------------------------------------------------
-- Server version	5.6.49-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `chat_friend_map`
--

DROP TABLE IF EXISTS `chat_friend_map`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chat_friend_map` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL COMMENT '用户ID',
  `FriendId` varchar(255) NOT NULL COMMENT '好友ID',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chat_friend_map`
--

LOCK TABLES `chat_friend_map` WRITE;
/*!40000 ALTER TABLE `chat_friend_map` DISABLE KEYS */;
/*!40000 ALTER TABLE `chat_friend_map` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chat_friend_message`
--

DROP TABLE IF EXISTS `chat_friend_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chat_friend_message` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL COMMENT '用户ID',
  `FriendId` varchar(255) NOT NULL COMMENT '好友ID',
  `Content` text NOT NULL COMMENT '内容',
  `MessageType` int(11) NOT NULL COMMENT '消息类型',
  `CreateTime` double(16,0) NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chat_friend_message`
--

LOCK TABLES `chat_friend_message` WRITE;
/*!40000 ALTER TABLE `chat_friend_message` DISABLE KEYS */;
/*!40000 ALTER TABLE `chat_friend_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chat_group`
--

DROP TABLE IF EXISTS `chat_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chat_group` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL COMMENT '用户ID',
  `GroupName` varchar(255) NOT NULL COMMENT '群名称',
  `Notice` varchar(500) NOT NULL COMMENT '公告',
  `CreateTime` double(16,0) NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chat_group`
--

LOCK TABLES `chat_group` WRITE;
/*!40000 ALTER TABLE `chat_group` DISABLE KEYS */;
INSERT INTO `chat_group` VALUES ('小兰聊天室','1','小兰聊天室','小兰聊天室',1604932934431);
/*!40000 ALTER TABLE `chat_group` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chat_group_map`
--

DROP TABLE IF EXISTS `chat_group_map`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chat_group_map` (
  `Id` varchar(255) NOT NULL,
  `GroupId` varchar(255) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chat_group_map`
--

LOCK TABLES `chat_group_map` WRITE;
/*!40000 ALTER TABLE `chat_group_map` DISABLE KEYS */;
/*!40000 ALTER TABLE `chat_group_map` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chat_group_message`
--

DROP TABLE IF EXISTS `chat_group_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chat_group_message` (
  `Id` varchar(255) NOT NULL,
  `UserId` varchar(255) DEFAULT NULL COMMENT '用户ID',
  `GroupId` varchar(255) DEFAULT NULL COMMENT '聊组ID',
  `Content` text COMMENT '内容',
  `MessageType` int(11) DEFAULT NULL COMMENT '消息类型',
  `CreateTime` double(16,0) DEFAULT NULL COMMENT '创建时间',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chat_group_message`
--

LOCK TABLES `chat_group_message` WRITE;
/*!40000 ALTER TABLE `chat_group_message` DISABLE KEYS */;
/*!40000 ALTER TABLE `chat_group_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chat_user`
--

DROP TABLE IF EXISTS `chat_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chat_user`
--

LOCK TABLES `chat_user` WRITE;
/*!40000 ALTER TABLE `chat_user` DISABLE KEYS */;
/*!40000 ALTER TABLE `chat_user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-11-09 22:58:50
