-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: master_database
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
-- Table structure for table `master_item_backpack`
--

DROP TABLE IF EXISTS `master_item_backpack`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_item_backpack` (
  `item_id` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 아이디',
  `total_scale_x` int(11) NOT NULL DEFAULT 0 COMMENT '가방 x크기',
  `total_scale_y` int(11) NOT NULL DEFAULT 0 COMMENT '가방 y크기',
  `total_weight` double NOT NULL DEFAULT 0 COMMENT '가방 무게',
  PRIMARY KEY (`item_id`),
  CONSTRAINT `FK_master_item_backpack_item_id_master_item_base_item_id` FOREIGN KEY (`item_id`) REFERENCES `master_item_base` (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_item_backpack`
--

LOCK TABLES `master_item_backpack` WRITE;
/*!40000 ALTER TABLE `master_item_backpack` DISABLE KEYS */;
INSERT INTO `master_item_backpack` VALUES (301,3,4,10),(302,5,5,15),(303,7,6,20);
/*!40000 ALTER TABLE `master_item_backpack` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_item_base`
--

DROP TABLE IF EXISTS `master_item_base`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_item_base` (
  `item_id` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 아이디',
  `code` varchar(50) NOT NULL DEFAULT '' COMMENT '아이템 코드',
  `name` varchar(50) NOT NULL DEFAULT '' COMMENT '아이템 이름',
  `weight` double NOT NULL DEFAULT 0 COMMENT '아이템 무게',
  `type` varchar(50) NOT NULL DEFAULT '' COMMENT '아이템 타입',
  `description` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 설명',
  `scale_x` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 가로 크기',
  `scale_y` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 세로 크기',
  `purchase_price` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 구매 가격',
  `inquiry_time` double NOT NULL DEFAULT 0 COMMENT '아이템 조회 시간',
  `sell_price` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 판매 가격',
  `amount` int(11) NOT NULL DEFAULT 0 COMMENT '수량',
  `icon` varchar(50) NOT NULL DEFAULT '' COMMENT '아이템 아이콘 경로',
  PRIMARY KEY (`item_id`),
  UNIQUE KEY `code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_item_base`
--

LOCK TABLES `master_item_base` WRITE;
/*!40000 ALTER TABLE `master_item_base` DISABLE KEYS */;
INSERT INTO `master_item_base` VALUES (101,'ITEM_W001','Colt45',2,'Weapone',101,2,2,400,1.2,100,1,'IconW_colt'),(102,'ITEM_W002','Ak47',7,'Weapone',102,4,2,2200,1.8,500,1,'IconW_ak'),(103,'ITEM_W003','Aug',6,'Weapone',103,4,2,2300,1.8,440,1,'IconW_aug'),(201,'ITEM_D001','경찰조끼',3,'Defensive',201,2,3,1100,1.5,550,1,'IconD_police'),(202,'ITEM_D002','방탄조끼',4,'Defensive',202,2,3,1500,1.5,750,1,'IconD_proof'),(203,'ITEM_D003','군용조끼',5,'Defensive',203,2,3,1700,1.5,850,1,'IconD_military'),(301,'ITEM_B001','의약품가방',3,'Bag',301,2,2,500,1.4,250,1,'IconB_medical'),(302,'ITEM_B002','군대가방',3,'Bag',302,2,2,1500,1.4,750,1,'IconB_army'),(303,'ITEM_B003','군용더블백',4,'Bag',303,2,2,2500,1.4,1300,1,'IconB_double'),(401,'ITEM_R001','의약품상자',1,'Recovery',401,1,1,500,0.7,200,1,'IconR_medical'),(402,'ITEM_R002','밴드',0.2,'Recovery',402,1,1,100,0.7,50,64,'IconR_band'),(403,'ITEM_R003','아드레날린',0.2,'Recovery',403,1,1,1000,0.7,600,1,'IconR_adrenaline'),(404,'ITEM_R004','알약',0.2,'Recovery',404,1,1,500,0.7,300,64,'IconR_pill'),(501,'ITEM_E001','5.59mm',0.3,'Bullet',501,1,1,10,0.4,0,64,'IconE_5.59'),(502,'ITEM_E002','7.29mm',0.3,'Bullet',502,1,1,10,0.4,0,64,'IconE_7.29'),(601,'ITEM_S001','타이어휠',10,'Spoil',601,4,4,0,2.2,2000,1,'IconS_spoil'),(602,'ITEM_S002','1.5볼트건전지',2,'Spoil',602,1,2,0,1.2,600,1,'IconS_battery'),(603,'ITEM_S003','밧줄한묶음',6,'Spoil',603,1,3,0,1.3,500,1,'IconS_rope'),(604,'ITEM_S004','은도금톱니바퀴',8,'Spoil',604,3,3,0,1.8,200,1,'IconS_wheel'),(605,'ITEM_S005','금괴',5,'Spoil',605,2,1,0,1.2,2000,1,'IconS_goldbar'),(606,'ITEM_S006','통나무',3,'Spoil',606,2,2,0,1.2,250,1,'IconS_solidwood');
/*!40000 ALTER TABLE `master_item_base` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_item_use`
--

DROP TABLE IF EXISTS `master_item_use`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_item_use` (
  `item_id` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 아이디',
  `energy` int(11) NOT NULL DEFAULT 0 COMMENT '회복 아이템의 회복량',
  `active_time` double NOT NULL DEFAULT 0 COMMENT '효과발동 시간',
  `duration` double NOT NULL DEFAULT 0 COMMENT '회복 아이템의 지속시간',
  `effect` enum('immediate','buff') NOT NULL DEFAULT 'immediate' COMMENT '해당 아이템의 효과 타입',
  `cool_time` double NOT NULL DEFAULT 0 COMMENT '재사용 대기시간',
  PRIMARY KEY (`item_id`),
  CONSTRAINT `FK_master_item_use_item_id_master_item_base_item_id` FOREIGN KEY (`item_id`) REFERENCES `master_item_base` (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_item_use`
--

LOCK TABLES `master_item_use` WRITE;
/*!40000 ALTER TABLE `master_item_use` DISABLE KEYS */;
INSERT INTO `master_item_use` VALUES (401,70,0,0,'immediate',4),(402,10,0,0,'immediate',2),(403,3,2,40,'buff',4),(404,1,2,40,'buff',2);
/*!40000 ALTER TABLE `master_item_use` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_item_weapon`
--

DROP TABLE IF EXISTS `master_item_weapon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_item_weapon` (
  `item_id` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 아이디',
  `attack_range` int(11) NOT NULL DEFAULT 0 COMMENT '공격 범위',
  `damage` int(11) NOT NULL DEFAULT 0 COMMENT '공격 데미지',
  `distance` int(11) NOT NULL DEFAULT 0 COMMENT '공격 거리',
  `reload_round` int(11) NOT NULL DEFAULT 0 COMMENT '재장전 수',
  `attack_speed` double NOT NULL DEFAULT 0 COMMENT '공격 속도',
  `reload_time` int(11) NOT NULL DEFAULT 0 COMMENT '재장전 시간',
  `bullet` varchar(20) NOT NULL DEFAULT '' COMMENT '사용 탄환',
  PRIMARY KEY (`item_id`),
  CONSTRAINT `FK_master_item_weapon_item_id_master_item_base_item_id` FOREIGN KEY (`item_id`) REFERENCES `master_item_base` (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_item_weapon`
--

LOCK TABLES `master_item_weapon` WRITE;
/*!40000 ALTER TABLE `master_item_weapon` DISABLE KEYS */;
INSERT INTO `master_item_weapon` VALUES (101,8,10,6,8,1,2,'7.29mm'),(102,20,18,10,40,0.4,4,'5.59mm'),(103,15,13,10,40,0.5,4,'7.29mm');
/*!40000 ALTER TABLE `master_item_weapon` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_reward_base`
--

DROP TABLE IF EXISTS `master_reward_base`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_reward_base` (
  `reward_id` int(11) NOT NULL DEFAULT 0 COMMENT '보상 아이디',
  `money` int(11) NOT NULL DEFAULT 0 COMMENT '돈',
  `ticket` int(11) NOT NULL DEFAULT 0 COMMENT '티켓',
  `gacha` int(11) NOT NULL DEFAULT 0 COMMENT '가챠',
  `experience` int(11) NOT NULL DEFAULT 0 COMMENT '경험치',
  `reward_box_id` int(11) DEFAULT NULL COMMENT '보상 박스 아이디',
  PRIMARY KEY (`reward_id`),
  KEY `FK_reward_box_id_master_reward_box_id` (`reward_box_id`),
  CONSTRAINT `FK_reward_box_id_master_reward_box_id` FOREIGN KEY (`reward_box_id`) REFERENCES `master_reward_box` (`reward_box_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_reward_base`
--

LOCK TABLES `master_reward_base` WRITE;
/*!40000 ALTER TABLE `master_reward_base` DISABLE KEYS */;
INSERT INTO `master_reward_base` VALUES (10001,1000,0,0,0,NULL),(10002,0,2,0,0,NULL),(10003,0,0,5,0,NULL),(10004,2000,0,0,0,NULL),(10005,0,4,0,0,NULL),(10006,0,0,10,0,NULL),(10007,3000,0,0,0,NULL),(10008,0,6,0,0,NULL),(10009,0,0,15,0,NULL),(20001,1000,1,0,500,NULL),(20002,0,0,0,1200,1),(20003,500,0,0,500,NULL),(20004,0,0,0,2000,2),(20005,500,0,0,500,NULL),(20006,0,0,0,500,3),(20007,2000,0,0,0,NULL),(20008,0,0,0,2000,NULL),(20009,0,1,1,1000,4),(20010,0,1,1,1000,5);
/*!40000 ALTER TABLE `master_reward_base` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_reward_box`
--

DROP TABLE IF EXISTS `master_reward_box`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_reward_box` (
  `reward_box_id` int(11) NOT NULL DEFAULT 0 COMMENT '보상 박스 아이디',
  `box_scale_x` int(11) NOT NULL DEFAULT 0 COMMENT '박스 x크기',
  `box_scale_y` int(11) NOT NULL DEFAULT 0 COMMENT '박스 y크기',
  PRIMARY KEY (`reward_box_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_reward_box`
--

LOCK TABLES `master_reward_box` WRITE;
/*!40000 ALTER TABLE `master_reward_box` DISABLE KEYS */;
INSERT INTO `master_reward_box` VALUES (1,2,2),(2,2,3),(3,1,0),(4,4,2),(5,4,3);
/*!40000 ALTER TABLE `master_reward_box` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_reward_box_item`
--

DROP TABLE IF EXISTS `master_reward_box_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_reward_box_item` (
  `reward_box_item_id` int(11) NOT NULL DEFAULT 0 COMMENT '보상 박스 아이템 아이디',
  `reward_box_id` int(11) NOT NULL DEFAULT 0 COMMENT '보상 박스 아이디',
  `item_code` varchar(50) NOT NULL DEFAULT '' COMMENT '아이템 코드',
  `x` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 x 위치',
  `y` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 y 위치',
  `rotation` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 회전',
  `amount` int(11) NOT NULL DEFAULT 0 COMMENT '아이템 수량',
  PRIMARY KEY (`reward_box_item_id`),
  KEY `FK_master_reward_box_item_master_reward_box_id` (`reward_box_id`),
  KEY `FK_master_reward_box_item_master_item_base_id` (`item_code`),
  CONSTRAINT `FK_master_reward_box_item_master_item_base_id` FOREIGN KEY (`item_code`) REFERENCES `master_item_base` (`code`) ON DELETE CASCADE,
  CONSTRAINT `FK_master_reward_box_item_master_reward_box_id` FOREIGN KEY (`reward_box_id`) REFERENCES `master_reward_box` (`reward_box_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_reward_box_item`
--

LOCK TABLES `master_reward_box_item` WRITE;
/*!40000 ALTER TABLE `master_reward_box_item` DISABLE KEYS */;
INSERT INTO `master_reward_box_item` VALUES (1,1,'ITEM_W001',0,0,0,1),(2,2,'ITEM_D003',0,0,0,1),(3,3,'ITEM_R002',0,0,0,2),(4,3,'ITEM_R003',1,0,0,2),(5,4,'ITEM_W002',0,0,0,1),(6,5,'ITEM_D001',0,0,0,1),(7,5,'ITEM_B001',2,0,0,1);
/*!40000 ALTER TABLE `master_reward_box_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_reward_level`
--

DROP TABLE IF EXISTS `master_reward_level`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_reward_level` (
  `reward_id` int(11) NOT NULL DEFAULT 0 COMMENT '보상 정보',
  `level` int(11) NOT NULL DEFAULT 0 COMMENT '레벨',
  `name` varchar(50) NOT NULL DEFAULT '' COMMENT '보상 이름',
  `icon` varchar(50) NOT NULL DEFAULT '' COMMENT '보상 아이콘',
  PRIMARY KEY (`reward_id`),
  UNIQUE KEY `level` (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_reward_level`
--

LOCK TABLES `master_reward_level` WRITE;
/*!40000 ALTER TABLE `master_reward_level` DISABLE KEYS */;
INSERT INTO `master_reward_level` VALUES (10001,2,'1000 골드','IconS_goldore'),(10002,3,'연료 2개','IconR_fueltank'),(10003,4,'건전지 5개','IconS_battery'),(10004,5,'2000 골드','IconS_goldore'),(10005,6,'연료 4개','IconR_fueltank'),(10006,7,'건전지 10개','IconS_battery'),(10007,8,'3000 골드','IconS_goldore'),(10008,9,'연료 6개','IconR_fueltank'),(10009,10,'건전지 15개','IconS_battery');
/*!40000 ALTER TABLE `master_reward_level` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_version_app`
--

DROP TABLE IF EXISTS `master_version_app`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_version_app` (
  `major` int(11) NOT NULL DEFAULT 0 COMMENT '앱 버전',
  `minor` int(11) NOT NULL DEFAULT 0 COMMENT '신규 기능',
  `patch` int(11) NOT NULL DEFAULT 0 COMMENT '버그 수정',
  UNIQUE KEY `major` (`major`,`minor`,`patch`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_version_app`
--

LOCK TABLES `master_version_app` WRITE;
/*!40000 ALTER TABLE `master_version_app` DISABLE KEYS */;
INSERT INTO `master_version_app` VALUES (1,0,0);
/*!40000 ALTER TABLE `master_version_app` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_version_data`
--

DROP TABLE IF EXISTS `master_version_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_version_data` (
  `major` int(11) NOT NULL DEFAULT 0 COMMENT '데이터 버전',
  `minor` int(11) NOT NULL DEFAULT 0 COMMENT '신규 데이터',
  `patch` int(11) NOT NULL DEFAULT 0 COMMENT '데이터 수정',
  UNIQUE KEY `major` (`major`,`minor`,`patch`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_version_data`
--

LOCK TABLES `master_version_data` WRITE;
/*!40000 ALTER TABLE `master_version_data` DISABLE KEYS */;
INSERT INTO `master_version_data` VALUES (1,0,2);
/*!40000 ALTER TABLE `master_version_data` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-09-23 16:30:10
