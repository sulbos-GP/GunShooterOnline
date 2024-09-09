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
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gear`
--

LOCK TABLES `gear` WRITE;
/*!40000 ALTER TABLE `gear` DISABLE KEYS */;
INSERT INTO `gear` VALUES (1,1,'backpack',1),(2,2,'backpack',2),(3,3,'backpack',3),(4,4,'backpack',4),(5,5,'backpack',5),(6,6,'backpack',6),(7,7,'backpack',7),(8,8,'backpack',8),(9,9,'backpack',9),(10,10,'backpack',10),(13,1,'pocket_third',17),(16,1,'pocket_second',28),(17,1,'sub_weapon',29),(25,11,'main_weapon',53),(26,11,'sub_weapon',54),(27,11,'armor',55),(28,11,'backpack',56),(29,11,'pocket_first',57),(30,11,'pocket_second',58),(31,11,'pocket_third',59);
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
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage`
--

LOCK TABLES `storage` WRITE;
/*!40000 ALTER TABLE `storage` DISABLE KEYS */;
INSERT INTO `storage` VALUES (1,NULL,'backpack'),(2,NULL,'backpack'),(3,NULL,'backpack'),(4,NULL,'backpack'),(5,NULL,'backpack'),(6,NULL,'backpack'),(7,NULL,'backpack'),(8,NULL,'backpack'),(9,NULL,'backpack'),(10,NULL,'backpack'),(11,NULL,'backpack');
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
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage_unit`
--

LOCK TABLES `storage_unit` WRITE;
/*!40000 ALTER TABLE `storage_unit` DISABLE KEYS */;
INSERT INTO `storage_unit` VALUES (28,1,0,0,0,52),(29,11,0,0,0,60),(30,11,0,2,0,61),(31,11,2,2,0,62);
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
  `unit_storage_id` int(11) DEFAULT NULL COMMENT '저장소 아이디 (가방 전용)',
  `amount` int(11) NOT NULL COMMENT '아이템 스택 카운터',
  PRIMARY KEY (`unit_attributes_id`),
  KEY `FK_unit_attributes_item_id_master_item_base_item_id` (`item_id`),
  KEY `FK_unit_attributes_unit_storage_id_storage_storage_id` (`unit_storage_id`),
  CONSTRAINT `FK_unit_attributes_item_id_master_item_base_item_id` FOREIGN KEY (`item_id`) REFERENCES `master_database`.`master_item_base` (`item_id`),
  CONSTRAINT `FK_unit_attributes_unit_storage_id_storage_storage_id` FOREIGN KEY (`unit_storage_id`) REFERENCES `storage` (`storage_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=63 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unit_attributes`
--

LOCK TABLES `unit_attributes` WRITE;
/*!40000 ALTER TABLE `unit_attributes` DISABLE KEYS */;
INSERT INTO `unit_attributes` VALUES (1,302,0,1,1),(2,302,0,2,1),(3,302,0,3,1),(4,302,0,4,1),(5,302,0,5,1),(6,302,0,6,1),(7,302,0,7,1),(8,302,0,8,1),(9,302,0,9,1),(10,302,0,10,1),(17,402,0,NULL,15),(28,402,0,NULL,21),(29,101,0,NULL,1),(52,102,0,NULL,1),(53,102,0,NULL,1),(54,101,0,NULL,1),(55,201,100,NULL,1),(56,302,0,11,1),(57,402,0,NULL,5),(58,502,0,NULL,10),(59,602,0,NULL,15),(60,402,0,NULL,5),(61,502,0,NULL,10),(62,602,0,NULL,15);
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
  `ticket` int(11) NOT NULL DEFAULT 0,
  `gacha` int(11) NOT NULL DEFAULT 0,
  `service` varchar(32) COLLATE utf8_bin NOT NULL COMMENT '서비스',
  `refresh_token` varchar(512) COLLATE utf8_bin DEFAULT NULL COMMENT '갱신 토큰',
  `create_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '생성 일시',
  `recent_login_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '최근 로그인 일시',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `nickname` (`nickname`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'a_1','a_1',1,0,0,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40'),(2,'a_2','a_2',1,0,0,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40'),(3,'a_3','a_3',1,0,0,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40'),(4,'a_4','a_4',1,0,0,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40'),(5,'a_5','a_5',1,0,0,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40'),(6,'a_6','a_6',1,0,0,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40'),(7,'a_7','a_7',1,0,0,0,'Google',NULL,'2024-08-29 19:29:40','2024-08-29 19:29:40'),(8,'a_8','a_8',1,0,0,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41'),(9,'a_9','a_9',1,0,0,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41'),(10,'a_10','a_10',1,0,0,0,'Google',NULL,'2024-08-29 19:29:41','2024-08-29 19:29:41'),(11,'111273423436160748486','장송',1,0,0,0,'Google','1//0eB5OREZb-rkmCgYIARAAGA4SNwF-L9Irx1Dzyy1HfroA0GW3-1vNWj420SUp11-UMDRBClbmN9yzFtRvtWlMhSSyVAg8cYOdNHM','2024-09-03 15:54:25','2024-09-04 17:48:34'),(12,'a_4003771818344972651','테스트1',100,0,0,0,'Google','1//0eHJmnKKtHgK8CgYIARAAGA4SNwF-L9IrCbicAPbqk9-Xwurgu2ifLckSxIkoTrs4_UiSMq00mPum_eIKFNXo8CXRZ6qMxvvi9hA','2024-09-04 17:54:48','2024-09-09 20:19:38');
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
INSERT INTO `user_level_reward` VALUES (2,12,10001,1,'2024-09-09 20:18:39');
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
INSERT INTO `user_metadata` VALUES (1,0,0,0,0,0,0,0),(2,0,0,0,0,0,0,0),(3,0,0,0,0,0,0,0),(4,0,0,0,0,0,0,0),(5,0,0,0,0,0,0,0),(6,0,0,0,0,0,0,0),(7,0,0,0,0,0,0,0),(8,0,0,0,0,0,0,0),(9,0,0,0,0,0,0,0),(10,0,0,0,0,0,0,0),(11,0,0,0,0,0,0,0),(12,0,0,0,0,0,0,0);
/*!40000 ALTER TABLE `user_metadata` ENABLE KEYS */;
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
INSERT INTO `user_skill` VALUES (1,1431.093800841918,280.67481394920014,0.054455069661564263),(2,1500.0542334281413,324.96431258038086,0.06329327230648443),(3,1564.8063711494267,218.92526952449722,0.05317493561950972),(4,1483.045709024226,289.98536201192564,0.06507652924868887),(5,1593.8850205334695,287.84692351505805,0.05039911013133741),(6,1468.5678186533673,298.14934660508237,0.05486255275538429),(7,1450.530257495614,280.0810332224528,0.06822827884762388),(8,1591.0898156381118,206.45053918142852,0.05697341537752507),(9,1522.8686182972683,203.85547723048876,0.055709733279151595),(10,1470.0647611823535,334.27386026296193,0.05849647910194144),(11,1500,350,0.06),(12,1500,350,0.06);
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

-- Dump completed on 2024-09-09 20:39:09
