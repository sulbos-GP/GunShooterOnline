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
  CONSTRAINT `FK_gear_unit_attributes_id_unit_attributes_unit_attributes_id` FOREIGN KEY (`unit_attributes_id`) REFERENCES `unit_attributes` (`unit_attributes_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gear`
--

LOCK TABLES `gear` WRITE;
/*!40000 ALTER TABLE `gear` DISABLE KEYS */;
INSERT INTO `gear` VALUES (1,1,'backpack',1),(2,2,'backpack',2),(3,3,'backpack',3),(4,4,'backpack',4),(5,5,'backpack',5),(6,6,'backpack',6),(7,7,'backpack',7),(8,8,'backpack',8),(9,9,'backpack',9),(10,10,'backpack',10);
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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage`
--

LOCK TABLES `storage` WRITE;
/*!40000 ALTER TABLE `storage` DISABLE KEYS */;
INSERT INTO `storage` VALUES (1,NULL,'backpack'),(2,NULL,'backpack'),(3,NULL,'backpack'),(4,NULL,'backpack'),(5,NULL,'backpack'),(6,NULL,'backpack'),(7,NULL,'backpack'),(8,NULL,'backpack'),(9,NULL,'backpack'),(10,NULL,'backpack');
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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `storage_unit`
--

LOCK TABLES `storage_unit` WRITE;
/*!40000 ALTER TABLE `storage_unit` DISABLE KEYS */;
INSERT INTO `storage_unit` VALUES (1,1,4,2,0,11),(3,1,0,0,0,13),(4,1,3,3,0,14);
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
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unit_attributes`
--

LOCK TABLES `unit_attributes` WRITE;
/*!40000 ALTER TABLE `unit_attributes` DISABLE KEYS */;
INSERT INTO `unit_attributes` VALUES (1,8,0,1,1),(2,8,0,2,1),(3,8,0,3,1),(4,8,0,4,1),(5,8,0,5,1),(6,8,0,6,1),(7,8,0,7,1),(8,8,0,8,1),(9,8,0,9,1),(10,8,0,10,1),(11,11,0,NULL,6),(12,11,0,NULL,11),(13,1,0,NULL,1),(14,11,0,NULL,8);
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
  `service` varchar(32) COLLATE utf8_bin NOT NULL COMMENT '서비스',
  `refresh_token` varchar(512) COLLATE utf8_bin DEFAULT NULL COMMENT '갱신 토큰',
  `create_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '생성 일시',
  `recent_login_dt` datetime NOT NULL DEFAULT current_timestamp() COMMENT '최근 로그인 일시',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `nickname` (`nickname`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'a_1','a_1','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(2,'a_2','a_2','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(3,'a_3','a_3','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(4,'a_4','a_4','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(5,'a_5','a_5','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(6,'a_6','a_6','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(7,'a_7','a_7','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(8,'a_8','a_8','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(9,'a_9','a_9','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42'),(10,'a_10','a_10','Google',NULL,'2024-08-27 10:35:42','2024-08-27 10:35:42');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
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
INSERT INTO `user_metadata` VALUES (1,0,0,0,0,0,0,0),(2,0,0,0,0,0,0,0),(3,0,0,0,0,0,0,0),(4,0,0,0,0,0,0,0),(5,0,0,0,0,0,0,0),(6,0,0,0,0,0,0,0),(7,0,0,0,0,0,0,0),(8,0,0,0,0,0,0,0),(9,0,0,0,0,0,0,0),(10,0,0,0,0,0,0,0);
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
INSERT INTO `user_skill` VALUES (1,1490.114444205644,295.5321525181943,0.0666537690317759),(2,1450.5598476627467,314.8896301303894,0.051425133069441784),(3,1411.6981390972605,211.8025321623334,0.05435891366038371),(4,1570.7355605165806,292.1827991885904,0.06023453191875902),(5,1542.9954053303184,205.95580642666278,0.05107191663335256),(6,1429.772612107706,287.52917576351126,0.059421000824701975),(7,1520.9333443277826,291.5275272834371,0.054738348838629526),(8,1470.807338944457,208.91467678259562,0.054700916786399593),(9,1599.3871383363262,241.9311456772789,0.05813795623196099),(10,1439.1732350356629,313.7956207746599,0.05411177728708068);
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

-- Dump completed on 2024-08-27 21:43:05
