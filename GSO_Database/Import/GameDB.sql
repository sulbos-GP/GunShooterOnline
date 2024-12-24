-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: game_database
-- ------------------------------------------------------
-- Server version	5.5.5-10.4.19-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `gear`
--

DROP TABLE IF EXISTS `gear`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gear` (
  `gear_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '장비 아이디',
  `uid` int(11) NOT NULL COMMENT '유저 아이디',
  `part` enum('main_weapon','sub_weapon','armor','backpack','pocket_first','pocket_second','pocket_third') COLLATE utf8_bin NOT NULL COMMENT '슬롯 타입',
  `unit_attributes_id` int(11) NOT NULL COMMENT '아이템 속성',
  PRIMARY KEY (`gear_id`),
  UNIQUE KEY `uid` (`uid`,`part`),
  KEY `FK_gear_unit_attributes_id_unit_attributes_unit_attributes_id` (`unit_attributes_id`),
  CONSTRAINT `FK_gear_uid_user_uid` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`),
  CONSTRAINT `FK_gear_unit_attributes_id_unit_attributes_unit_attributes_id` FOREIGN KEY (`unit_attributes_id`) REFERENCES `unit_attributes` (`unit_attributes_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=856 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gear`
--

LOCK TABLES `gear` WRITE;
/*!40000 ALTER TABLE `gear` DISABLE KEYS */;
INSERT INTO `gear` VALUES (778,1,'main_weapon',1200),(779,1,'sub_weapon',1201),(780,1,'armor',1202),(781,1,'backpack',1203),(782,1,'pocket_first',1204),(783,1,'pocket_third',1205),(784,2,'main_weapon',1209),(785,2,'sub_weapon',1210),(786,2,'armor',1211),(787,2,'backpack',1212),(788,2,'pocket_first',1213),(789,2,'pocket_third',1214),(790,3,'main_weapon',1218),(791,3,'sub_weapon',1219),(792,3,'armor',1220),(793,3,'backpack',1221),(794,3,'pocket_first',1222),(795,3,'pocket_third',1223),(796,4,'main_weapon',1227),(797,4,'sub_weapon',1228),(798,4,'armor',1229),(799,4,'backpack',1230),(800,4,'pocket_first',1231),(801,4,'pocket_third',1232),(802,5,'main_weapon',1236),(803,5,'sub_weapon',1237),(804,5,'armor',1238),(805,5,'backpack',1239),(806,5,'pocket_first',1240),(807,5,'pocket_third',1241),(808,6,'main_weapon',1245),(809,6,'sub_weapon',1246),(810,6,'armor',1247),(811,6,'backpack',1248),(812,6,'pocket_first',1249),(813,6,'pocket_third',1250),(814,7,'main_weapon',1254),(815,7,'sub_weapon',1255),(816,7,'armor',1256),(817,7,'backpack',1257),(818,7,'pocket_first',1258),(819,7,'pocket_third',1259),(820,8,'main_weapon',1263),(821,8,'sub_weapon',1264),(822,8,'armor',1265),(823,8,'backpack',1266),(824,8,'pocket_first',1267),(825,8,'pocket_third',1268),(826,9,'main_weapon',1272),(827,9,'sub_weapon',1273),(828,9,'armor',1274),(829,9,'backpack',1275),(830,9,'pocket_first',1276),(831,9,'pocket_third',1277),(832,10,'main_weapon',1281),(833,10,'sub_weapon',1282),(834,10,'armor',1283),(835,10,'backpack',1284),(836,10,'pocket_first',1285),(837,10,'pocket_third',1286),(838,11,'main_weapon',1290),(839,11,'sub_weapon',1291),(840,11,'armor',1292),(841,11,'backpack',1293),(842,11,'pocket_first',1294),(843,11,'pocket_third',1295),(853,12,'sub_weapon',1312),(854,12,'backpack',1313),(855,12,'pocket_first',1314);
/*!40000 ALTER TABLE `gear` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `storage`
--

DROP TABLE IF EXISTS `storage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `storage` (
  `storage_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '저장소 아이디',
  `uid` int(11) DEFAULT NULL COMMENT '유저 아이디',
  `storage_type` enum('backpack','cabinet') COLLATE utf8_bin NOT NULL COMMENT '저장소 타입(가방/캐비넷)',
  PRIMARY KEY (`storage_id`),
  KEY `FK_storage_uid_user_uid` (`uid`),
  CONSTRAINT `FK_storage_uid_user_uid` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=165 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage`
--

LOCK TABLES `storage` WRITE;
/*!40000 ALTER TABLE `storage` DISABLE KEYS */;
INSERT INTO `storage` VALUES (151,NULL,'backpack'),(152,NULL,'backpack'),(153,NULL,'backpack'),(154,NULL,'backpack'),(155,NULL,'backpack'),(156,NULL,'backpack'),(157,NULL,'backpack'),(158,NULL,'backpack'),(159,NULL,'backpack'),(160,NULL,'backpack'),(161,NULL,'backpack'),(162,NULL,'backpack'),(163,NULL,'backpack'),(164,NULL,'backpack');
/*!40000 ALTER TABLE `storage` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `storage_unit`
--

DROP TABLE IF EXISTS `storage_unit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `storage_unit` (
  `storage_unit_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '저장소 슬롯 아이디',
  `storage_id` int(11) NOT NULL COMMENT '저장소 아이디 (가방 ID 또는 캐비넷 ID)',
  `grid_x` int(11) NOT NULL COMMENT '아이템 x위치',
  `grid_y` int(11) NOT NULL COMMENT '아이템 y위치',
  `rotation` int(11) NOT NULL COMMENT '아이템 회전',
  `unit_attributes_id` int(11) NOT NULL COMMENT '아이템 속성 아이디',
  PRIMARY KEY (`storage_unit_id`),
  KEY `FK_storage_unit_storage_id_storage_storage_id` (`storage_id`),
  KEY `FK_storage_unit_id_att_id_uatt_att_id` (`unit_attributes_id`),
  CONSTRAINT `FK_storage_unit_id_att_id_uatt_att_id` FOREIGN KEY (`unit_attributes_id`) REFERENCES `unit_attributes` (`unit_attributes_id`) ON DELETE CASCADE,
  CONSTRAINT `FK_storage_unit_storage_id_storage_storage_id` FOREIGN KEY (`storage_id`) REFERENCES `storage` (`storage_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=465 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage_unit`
--

LOCK TABLES `storage_unit` WRITE;
/*!40000 ALTER TABLE `storage_unit` DISABLE KEYS */;
INSERT INTO `storage_unit` VALUES (427,151,0,0,0,1206),(428,151,0,2,0,1207),(429,151,2,0,0,1208),(430,152,0,0,0,1215),(431,152,0,2,0,1216),(432,152,2,0,0,1217),(433,153,0,0,0,1224),(434,153,0,2,0,1225),(435,153,2,0,0,1226),(436,154,0,0,0,1233),(437,154,0,2,0,1234),(438,154,2,0,0,1235),(439,155,0,0,0,1242),(440,155,0,2,0,1243),(441,155,2,0,0,1244),(442,156,0,0,0,1251),(443,156,0,2,0,1252),(444,156,2,0,0,1253),(445,157,0,0,0,1260),(446,157,0,2,0,1261),(447,157,2,0,0,1262),(448,158,0,0,0,1269),(449,158,0,2,0,1270),(450,158,2,0,0,1271),(451,159,0,0,0,1278),(452,159,0,2,0,1279),(453,159,2,0,0,1280),(454,160,0,0,0,1287),(455,160,0,2,0,1288),(456,160,2,0,0,1289),(457,161,0,0,0,1296),(458,161,0,2,0,1297),(459,161,2,0,0,1298),(464,164,0,0,0,1315);
/*!40000 ALTER TABLE `storage_unit` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `unit_attributes`
--

DROP TABLE IF EXISTS `unit_attributes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `unit_attributes` (
  `unit_attributes_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '유닛 속성 아이디',
  `item_id` int(11) NOT NULL COMMENT '아이템 아이디',
  `durability` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 내구도',
  `loaded_ammo` int(11) NOT NULL DEFAULT 0,
  `unit_storage_id` int(11) DEFAULT NULL COMMENT '저장소 아이디 (가방 전용)',
  `amount` int(11) NOT NULL COMMENT '아이템 스택 카운터',
  PRIMARY KEY (`unit_attributes_id`),
  KEY `FK_unit_attributes_item_id_master_item_base_item_id` (`item_id`),
  KEY `FK_unit_attributes_unit_storage_id_storage_storage_id` (`unit_storage_id`),
  CONSTRAINT `FK_unit_attributes_item_id_master_item_base_item_id` FOREIGN KEY (`item_id`) REFERENCES `master_database`.`master_item_base` (`item_id`),
  CONSTRAINT `FK_unit_attributes_unit_storage_id_storage_storage_id` FOREIGN KEY (`unit_storage_id`) REFERENCES `storage` (`storage_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=1316 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unit_attributes`
--

LOCK TABLES `unit_attributes` WRITE;
/*!40000 ALTER TABLE `unit_attributes` DISABLE KEYS */;
INSERT INTO `unit_attributes` VALUES (1200,102,0,0,NULL,1),(1201,101,0,0,NULL,1),(1202,201,100,0,NULL,1),(1203,302,0,0,151,1),(1204,402,0,0,NULL,5),(1205,404,0,0,NULL,15),(1206,402,0,0,NULL,5),(1207,502,0,0,NULL,10),(1208,404,0,0,NULL,15),(1209,102,0,0,NULL,1),(1210,101,0,0,NULL,1),(1211,201,100,0,NULL,1),(1212,302,0,0,152,1),(1213,402,0,0,NULL,5),(1214,404,0,0,NULL,15),(1215,402,0,0,NULL,5),(1216,502,0,0,NULL,10),(1217,404,0,0,NULL,15),(1218,102,0,0,NULL,1),(1219,101,0,0,NULL,1),(1220,201,100,0,NULL,1),(1221,302,0,0,153,1),(1222,402,0,0,NULL,5),(1223,404,0,0,NULL,15),(1224,402,0,0,NULL,5),(1225,502,0,0,NULL,10),(1226,404,0,0,NULL,15),(1227,102,0,0,NULL,1),(1228,101,0,0,NULL,1),(1229,201,100,0,NULL,1),(1230,302,0,0,154,1),(1231,402,0,0,NULL,5),(1232,404,0,0,NULL,15),(1233,402,0,0,NULL,5),(1234,502,0,0,NULL,10),(1235,404,0,0,NULL,15),(1236,102,0,0,NULL,1),(1237,101,0,0,NULL,1),(1238,201,100,0,NULL,1),(1239,302,0,0,155,1),(1240,402,0,0,NULL,5),(1241,404,0,0,NULL,15),(1242,402,0,0,NULL,5),(1243,502,0,0,NULL,10),(1244,404,0,0,NULL,15),(1245,102,0,0,NULL,1),(1246,101,0,0,NULL,1),(1247,201,100,0,NULL,1),(1248,302,0,0,156,1),(1249,402,0,0,NULL,5),(1250,404,0,0,NULL,15),(1251,402,0,0,NULL,5),(1252,502,0,0,NULL,10),(1253,404,0,0,NULL,15),(1254,102,0,0,NULL,1),(1255,101,0,0,NULL,1),(1256,201,100,0,NULL,1),(1257,302,0,0,157,1),(1258,402,0,0,NULL,5),(1259,404,0,0,NULL,15),(1260,402,0,0,NULL,5),(1261,502,0,0,NULL,10),(1262,404,0,0,NULL,15),(1263,102,0,0,NULL,1),(1264,101,0,0,NULL,1),(1265,201,100,0,NULL,1),(1266,302,0,0,158,1),(1267,402,0,0,NULL,5),(1268,404,0,0,NULL,15),(1269,402,0,0,NULL,5),(1270,502,0,0,NULL,10),(1271,404,0,0,NULL,15),(1272,102,0,0,NULL,1),(1273,101,0,0,NULL,1),(1274,201,100,0,NULL,1),(1275,302,0,0,159,1),(1276,402,0,0,NULL,5),(1277,404,0,0,NULL,15),(1278,402,0,0,NULL,5),(1279,502,0,0,NULL,10),(1280,404,0,0,NULL,15),(1281,102,0,0,NULL,1),(1282,101,0,0,NULL,1),(1283,201,100,0,NULL,1),(1284,302,0,0,160,1),(1285,402,0,0,NULL,5),(1286,404,0,0,NULL,15),(1287,402,0,0,NULL,5),(1288,502,0,0,NULL,10),(1289,404,0,0,NULL,15),(1290,102,0,0,NULL,1),(1291,101,0,0,NULL,1),(1292,201,100,0,NULL,1),(1293,302,0,0,161,1),(1294,402,0,0,NULL,5),(1295,404,0,0,NULL,15),(1296,402,0,0,NULL,5),(1297,502,0,0,NULL,10),(1298,404,0,0,NULL,15),(1312,101,0,0,NULL,1),(1313,300,0,0,164,1),(1314,402,0,0,NULL,1),(1315,501,0,0,NULL,12);
/*!40000 ALTER TABLE `unit_attributes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `uid` int(11) NOT NULL AUTO_INCREMENT COMMENT '유저 아이디',
  `player_id` varchar(256) COLLATE utf8_bin NOT NULL COMMENT '플레이어 아이디',
  `nickname` varchar(10) COLLATE utf8_bin DEFAULT NULL COMMENT '닉네임',
  `experience` int(11) NOT NULL DEFAULT 100,
  `money` int(11) NOT NULL DEFAULT 0,
  `ticket` int(11) NOT NULL DEFAULT 5 COMMENT '티켓',
  `gacha` int(11) NOT NULL DEFAULT 0,
  `service` varchar(32) COLLATE utf8_bin NOT NULL COMMENT '서비스',
  `refresh_token` varchar(512) COLLATE utf8_bin DEFAULT NULL COMMENT '갱신 토큰',
  `create_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '생성 일시',
  `recent_login_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '최근 로그인 일시',
  `recent_ticket_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '최근 티켓 사용 일시',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `nickname` (`nickname`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'a_1','a_1',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(2,'a_2','a_2',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(3,'a_3','a_3',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(4,'a_4','a_4',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(5,'a_5','a_5',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(6,'a_6','a_6',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(7,'a_7','a_7',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(8,'a_8','a_8',1,0,5,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41','2024-09-29 17:29:47'),(9,'a_9','a_9',1,0,5,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41','2024-09-29 17:29:47'),(10,'a_10','a_10',1,0,5,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41','2024-09-29 17:29:47'),(11,'111273423436160748486',NULL,1,0,5,0,'Google','1//0eB5OREZb-rkmCgYIARAAGA4SNwF-L9Irx1Dzyy1HfroA0GW3-1vNWj420SUp11-UMDRBClbmN9yzFtRvtWlMhSSyVAg8cYOdNHM','2024-09-03 15:54:25','2024-09-04 17:48:34','2024-09-29 17:29:47'),(12,'a_4003771818344972651','이관호',340,1450,10,4,'Google','1//0eHJmnKKtHgK8CgYIARAAGA4SNwF-L9IrCbicAPbqk9-Xwurgu2ifLckSxIkoTrs4_UiSMq00mPum_eIKFNXo8CXRZ6qMxvvi9hA','2024-09-04 17:54:48','2024-12-12 08:48:27','2024-12-12 08:48:27'),(13,'g02653505751708925358','Gwanho',100,0,-1,0,'Google','1//0emzTMwJGIK-KCgYIARAAGA4SNwF-L9IrgCpazGQ6TVe6MfnGNDw38BEP1XC2g2bLA7vWfTQefMZHez7N8cWwap1n1HQC21VnmBU','2024-10-29 20:27:22','2024-10-29 11:38:48','2024-10-29 11:36:47');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_level_reward`
--

DROP TABLE IF EXISTS `user_level_reward`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_level_reward` (
  `reward_level_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '레벨 보상 아이디',
  `uid` int(11) NOT NULL COMMENT '유저 아이디',
  `reward_id` int(11) NOT NULL COMMENT '보상 아이디',
  `received` tinyint(1) NOT NULL DEFAULT 0 COMMENT '수령 확인',
  `received_dt` datetime DEFAULT NULL COMMENT '수령 확인 날짜',
  PRIMARY KEY (`reward_level_id`),
  UNIQUE KEY `uid` (`uid`,`reward_id`),
  KEY `FK_user_level_reward_master_reward_level_id` (`reward_id`),
  CONSTRAINT `FK_user_level_reward_master_reward_level_id` FOREIGN KEY (`reward_id`) REFERENCES `master_database`.`master_reward_level` (`reward_id`) ON DELETE CASCADE,
  CONSTRAINT `FK_user_reward_uid_user_uid` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_level_reward`
--

LOCK TABLES `user_level_reward` WRITE;
/*!40000 ALTER TABLE `user_level_reward` DISABLE KEYS */;
INSERT INTO `user_level_reward` VALUES (1,12,10001,1,'2024-10-19 17:13:27'),(2,12,10002,0,NULL);
/*!40000 ALTER TABLE `user_level_reward` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_metadata`
--

DROP TABLE IF EXISTS `user_metadata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_metadata` (
  `uid` int(11) NOT NULL COMMENT '유저 아이디',
  `total_games` int(11) NOT NULL DEFAULT 0 COMMENT '게임',
  `kills` int(11) NOT NULL DEFAULT 0 COMMENT '킬',
  `deaths` int(11) NOT NULL DEFAULT 0 COMMENT '데스',
  `damage` int(11) NOT NULL DEFAULT 0 COMMENT '대미지',
  `farming` int(11) NOT NULL DEFAULT 0 COMMENT '파밍',
  `escape` int(11) NOT NULL DEFAULT 0 COMMENT '탈출',
  `survival_time` int(11) NOT NULL DEFAULT 0 COMMENT '생존',
  PRIMARY KEY (`uid`),
  CONSTRAINT `FK_user_metadata_uid_user_uid` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_metadata`
--

LOCK TABLES `user_metadata` WRITE;
/*!40000 ALTER TABLE `user_metadata` DISABLE KEYS */;
INSERT INTO `user_metadata` VALUES (1,0,0,0,0,0,0,0),(2,0,0,0,0,0,0,0),(3,0,0,0,0,0,0,0),(4,0,0,0,0,0,0,0),(5,0,0,0,0,0,0,0),(6,0,0,0,0,0,0,0),(7,0,0,0,0,0,0,0),(8,0,0,0,0,0,0,0),(9,0,0,0,0,0,0,0),(10,0,0,0,0,0,0,0),(11,0,0,0,0,0,0,0),(12,1,0,0,0,3,1,2),(13,1,0,0,0,0,1,0);
/*!40000 ALTER TABLE `user_metadata` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_register_quest`
--

DROP TABLE IF EXISTS `user_register_quest`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_register_quest` (
  `register_quest_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '등록된 퀘스트 아이디',
  `uid` int(11) NOT NULL COMMENT '유저 아이디',
  `quest_id` int(11) NOT NULL COMMENT '퀘스트 아이디',
  `progress` int(11) NOT NULL DEFAULT 0 COMMENT '퀘스트 진행도',
  `completed` tinyint(1) NOT NULL DEFAULT 0 COMMENT '퀘스트 완료 여부',
  `register_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '퀘스트 수령 날짜',
  PRIMARY KEY (`register_quest_id`),
  UNIQUE KEY `uid` (`uid`,`quest_id`),
  KEY `FK_user_register_quest_master_quest_base_id` (`quest_id`),
  CONSTRAINT `FK_user_register_quest_master_quest_base_id` FOREIGN KEY (`quest_id`) REFERENCES `master_database`.`master_quest_base` (`quest_id`) ON DELETE CASCADE,
  CONSTRAINT `FK_user_register_quest_uid_user_uid` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=73 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_register_quest`
--

LOCK TABLES `user_register_quest` WRITE;
/*!40000 ALTER TABLE `user_register_quest` DISABLE KEYS */;
INSERT INTO `user_register_quest` VALUES (64,13,20004,0,0,'2024-10-29 11:32:11'),(65,13,30002,0,0,'2024-10-29 11:32:11'),(66,13,40002,1,0,'2024-10-29 11:32:11'),(70,12,20002,0,0,'2024-12-12 08:48:27'),(71,12,30003,0,0,'2024-12-12 08:48:27'),(72,12,40001,0,0,'2024-12-12 08:48:27');
/*!40000 ALTER TABLE `user_register_quest` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_skill`
--

DROP TABLE IF EXISTS `user_skill`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_skill` (
  `uid` int(11) NOT NULL COMMENT '유저 아이디',
  `rating` double NOT NULL DEFAULT 1500 COMMENT '레이팅',
  `deviation` double NOT NULL DEFAULT 350 COMMENT '표준 편차',
  `volatility` double NOT NULL DEFAULT 0.06 COMMENT '변동성',
  PRIMARY KEY (`uid`),
  CONSTRAINT `FK_user_skill_uid_user_uid` FOREIGN KEY (`uid`) REFERENCES `user` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_skill`
--

LOCK TABLES `user_skill` WRITE;
/*!40000 ALTER TABLE `user_skill` DISABLE KEYS */;
INSERT INTO `user_skill` VALUES (1,1500,350,0.06),(2,1500,350,0.06),(3,1500,350,0.06),(4,1500,350,0.06),(5,1500,350,0.06),(6,1500,350,0.06),(7,1500,350,0.06),(8,1500,350,0.06),(9,1500,350,0.06),(10,1500,350,0.06),(11,1500,350,0.06),(12,1500,350,0.06),(13,1500,350,0.06);
/*!40000 ALTER TABLE `user_skill` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-12-13 21:58:30
