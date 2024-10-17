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
) ENGINE=InnoDB AUTO_INCREMENT=146 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gear`
--

LOCK TABLES `gear` WRITE;
/*!40000 ALTER TABLE `gear` DISABLE KEYS */;
INSERT INTO `gear` VALUES (116,1,'main_weapon',189),(117,1,'sub_weapon',190),(118,1,'armor',191),(119,1,'backpack',192),(120,1,'pocket_first',193),(121,1,'pocket_third',194),(122,2,'main_weapon',198),(123,2,'sub_weapon',199),(124,2,'armor',200),(125,2,'backpack',201),(126,2,'pocket_first',202),(127,2,'pocket_third',203),(128,3,'main_weapon',207),(129,3,'sub_weapon',208),(130,3,'armor',209),(131,3,'backpack',210),(132,3,'pocket_first',211),(133,3,'pocket_third',212),(134,4,'main_weapon',216),(135,4,'sub_weapon',217),(136,4,'armor',218),(137,4,'backpack',219),(138,4,'pocket_first',220),(139,4,'pocket_third',221),(140,5,'main_weapon',225),(141,5,'sub_weapon',226),(142,5,'armor',227),(143,5,'backpack',228),(144,5,'pocket_first',229),(145,5,'pocket_third',230);
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
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage`
--

LOCK TABLES `storage` WRITE;
/*!40000 ALTER TABLE `storage` DISABLE KEYS */;
INSERT INTO `storage` VALUES (34,NULL,'backpack'),(35,NULL,'backpack'),(36,NULL,'backpack'),(37,NULL,'backpack'),(38,NULL,'backpack');
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
) ENGINE=InnoDB AUTO_INCREMENT=85 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage_unit`
--

LOCK TABLES `storage_unit` WRITE;
/*!40000 ALTER TABLE `storage_unit` DISABLE KEYS */;
INSERT INTO `storage_unit` VALUES (70,34,0,0,0,195),(71,34,0,2,0,196),(72,34,2,0,0,197),(73,35,0,0,0,204),(74,35,0,2,0,205),(75,35,2,0,0,206),(76,36,0,0,0,213),(77,36,0,2,0,214),(78,36,2,0,0,215),(79,37,0,0,0,222),(80,37,0,2,0,223),(81,37,2,0,0,224),(82,38,0,0,0,231),(83,38,0,2,0,232),(84,38,2,0,0,233);
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
) ENGINE=InnoDB AUTO_INCREMENT=234 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unit_attributes`
--

LOCK TABLES `unit_attributes` WRITE;
/*!40000 ALTER TABLE `unit_attributes` DISABLE KEYS */;
INSERT INTO `unit_attributes` VALUES (189,102,0,0,NULL,1),(190,101,0,0,NULL,1),(191,201,100,0,NULL,1),(192,302,0,0,34,1),(193,402,0,0,NULL,5),(194,404,0,0,NULL,15),(195,402,0,0,NULL,5),(196,502,0,0,NULL,10),(197,404,0,0,NULL,15),(198,102,0,0,NULL,1),(199,101,0,0,NULL,1),(200,201,100,0,NULL,1),(201,302,0,0,35,1),(202,402,0,0,NULL,5),(203,404,0,0,NULL,15),(204,402,0,0,NULL,5),(205,502,0,0,NULL,10),(206,404,0,0,NULL,15),(207,102,0,0,NULL,1),(208,101,0,0,NULL,1),(209,201,100,0,NULL,1),(210,302,0,0,36,1),(211,402,0,0,NULL,5),(212,404,0,0,NULL,15),(213,402,0,0,NULL,5),(214,502,0,0,NULL,10),(215,404,0,0,NULL,15),(216,102,0,0,NULL,1),(217,101,0,0,NULL,1),(218,201,100,0,NULL,1),(219,302,0,0,37,1),(220,402,0,0,NULL,5),(221,404,0,0,NULL,15),(222,402,0,0,NULL,5),(223,502,0,0,NULL,10),(224,404,0,0,NULL,15),(225,102,0,0,NULL,1),(226,101,0,0,NULL,1),(227,201,100,0,NULL,1),(228,302,0,0,38,1),(229,402,0,0,NULL,5),(230,404,0,0,NULL,15),(231,402,0,0,NULL,5),(232,502,0,0,NULL,10),(233,404,0,0,NULL,15);
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
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'a_1','a_1',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(2,'a_2','a_2',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(3,'a_3','a_3',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(4,'a_4','a_4',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(5,'a_5','a_5',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(6,'a_6','a_6',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(7,'a_7','a_7',1,0,5,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40','2024-09-29 17:29:47'),(8,'a_8','a_8',1,0,5,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41','2024-09-29 17:29:47'),(9,'a_9','a_9',1,0,5,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41','2024-09-29 17:29:47'),(10,'a_10','a_10',1,0,5,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41','2024-09-29 17:29:47'),(11,'111273423436160748486',NULL,1,0,5,0,'Google','1//0eB5OREZb-rkmCgYIARAAGA4SNwF-L9Irx1Dzyy1HfroA0GW3-1vNWj420SUp11-UMDRBClbmN9yzFtRvtWlMhSSyVAg8cYOdNHM','2024-09-03 15:54:25','2024-09-04 17:48:34','2024-09-29 17:29:47'),(12,'a_4003771818344972651','장송',100,1000,10,0,'Google','1//0eHJmnKKtHgK8CgYIARAAGA4SNwF-L9IrCbicAPbqk9-Xwurgu2ifLckSxIkoTrs4_UiSMq00mPum_eIKFNXo8CXRZ6qMxvvi9hA','2024-09-04 17:54:48','2024-10-15 06:57:31','2024-10-15 06:52:07');
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_level_reward`
--

LOCK TABLES `user_level_reward` WRITE;
/*!40000 ALTER TABLE `user_level_reward` DISABLE KEYS */;
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
INSERT INTO `user_metadata` VALUES (1,0,0,0,0,0,0,0),(2,0,0,0,0,0,0,0),(3,0,0,0,0,0,0,0),(4,0,0,0,0,0,0,0),(5,0,0,0,0,0,0,0),(6,0,0,0,0,0,0,0),(7,0,0,0,0,0,0,0),(8,0,0,0,0,0,0,0),(9,0,0,0,0,0,0,0),(10,0,0,0,0,0,0,0),(11,0,0,0,0,0,0,0),(12,1,0,0,0,3,1,2);
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_register_quest`
--

LOCK TABLES `user_register_quest` WRITE;
/*!40000 ALTER TABLE `user_register_quest` DISABLE KEYS */;
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
INSERT INTO `user_skill` VALUES (1,1500,350,0.06),(2,1500,350,0.06),(3,1500,350,0.06),(4,1500,350,0.06),(5,1500,350,0.06),(6,1500,350,0.06),(7,1500,350,0.06),(8,1500,350,0.06),(9,1500,350,0.06),(10,1500,350,0.06),(11,1500,350,0.06),(12,500,247.4632763208616,0.059998651475871814);
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

-- Dump completed on 2024-10-17 15:09:02
