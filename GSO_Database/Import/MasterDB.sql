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
-- Table structure for table `app_version`
--

DROP TABLE IF EXISTS `app_version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `app_version` (
  `major` int(11) NOT NULL COMMENT '앱 버전',
  `minor` int(11) NOT NULL COMMENT '신규 기능',
  `patch` int(11) NOT NULL COMMENT '버그 수정'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `app_version`
--

LOCK TABLES `app_version` WRITE;
/*!40000 ALTER TABLE `app_version` DISABLE KEYS */;
/*!40000 ALTER TABLE `app_version` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `data_version`
--

DROP TABLE IF EXISTS `data_version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `data_version` (
  `major` int(11) NOT NULL COMMENT '데이터 버전',
  `minor` int(11) NOT NULL COMMENT '신규 데이터',
  `patch` int(11) NOT NULL COMMENT '데이터 수정'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `data_version`
--

LOCK TABLES `data_version` WRITE;
/*!40000 ALTER TABLE `data_version` DISABLE KEYS */;
/*!40000 ALTER TABLE `data_version` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_item_backpack`
--

DROP TABLE IF EXISTS `master_item_backpack`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_item_backpack` (
  `item_id` int(11) NOT NULL COMMENT '아이템 아이디',
  `total_scale_x` int(11) NOT NULL COMMENT '가방 x크기',
  `total_scale_y` int(11) NOT NULL COMMENT '가방 y크기',
  `total_weight` int(11) NOT NULL COMMENT '가방 무게',
  PRIMARY KEY (`item_id`),
  CONSTRAINT `FK_master_item_backpack_item_id_master_item_base_item_id` FOREIGN KEY (`item_id`) REFERENCES `master_item_base` (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_item_backpack`
--

LOCK TABLES `master_item_backpack` WRITE;
/*!40000 ALTER TABLE `master_item_backpack` DISABLE KEYS */;
INSERT INTO `master_item_backpack` VALUES (7,3,4,10),(8,5,5,15),(9,7,6,20);
/*!40000 ALTER TABLE `master_item_backpack` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_item_base`
--

DROP TABLE IF EXISTS `master_item_base`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_item_base` (
  `item_id` int(11) NOT NULL COMMENT '아이템 아이디',
  `code` varchar(50) NOT NULL COMMENT '아이템 코드',
  `name` varchar(50) NOT NULL COMMENT '아이템 이름',
  `weight` double NOT NULL COMMENT '아이템 무게',
  `type` varchar(50) NOT NULL COMMENT '아이템 타입',
  `description` int(11) NOT NULL COMMENT '아이템 설명',
  `scale_x` int(11) NOT NULL COMMENT '아이템 가로 크기',
  `scale_y` int(11) NOT NULL COMMENT '아이템 세로 크기',
  `purchase_price` int(11) NOT NULL COMMENT '아이템 구매 가격',
  `inquiry_time` double NOT NULL COMMENT '아이템 조회 시간',
  `sell_price` int(11) NOT NULL COMMENT '아이템 판매 가격',
  `stack_count` int(11) NOT NULL COMMENT '스택 카운터',
  `prefab` varchar(50) NOT NULL COMMENT '아이템 프리펩 경로',
  `icon` varchar(50) NOT NULL COMMENT '아이템 아이콘 경로',
  PRIMARY KEY (`item_id`),
  UNIQUE KEY `code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_item_base`
--

LOCK TABLES `master_item_base` WRITE;
/*!40000 ALTER TABLE `master_item_base` DISABLE KEYS */;
INSERT INTO `master_item_base` VALUES (1,'W001','Colt45',2,'weapone',101,2,2,400,0.5,100,1,'0','0'),(2,'W002','Ak47',7,'weapone',102,4,2,2200,0.6,500,1,'0','0'),(3,'W003','Aug',6,'weapone',103,4,2,2300,0.4,440,1,'0','0'),(4,'D001','경찰조끼',3,'defensive',201,2,3,1100,0.1,550,1,'0','0'),(5,'D002','방탄조끼',4,'defensive',202,2,3,1500,0.2,750,1,'0','0'),(6,'D003','군용조끼',5,'defensive',203,2,3,1700,0.3,850,1,'0','0'),(7,'B001','의약품가방',3,'bag',301,2,2,500,0.4,250,1,'0','0'),(8,'B002','군대가방',3,'bag',302,2,2,1500,0.5,750,1,'0','0'),(9,'B003','군용더블백',4,'bag',303,2,2,2500,0.6,1300,1,'0','0'),(10,'R001','의약품상자',1,'recovery',401,1,1,500,0.7,200,1,'0','0'),(11,'R002','밴드',0.2,'recovery',402,1,1,100,0.8,50,64,'0','0'),(12,'R003','아드레날린',0.2,'recovery',403,1,1,1000,0.9,600,1,'0','0'),(13,'R004','알약',0.2,'recovery',404,1,1,500,0.1,300,64,'0','0'),(14,'E001','5.59mm',0.3,'bullet',501,1,1,10,0.2,0,64,'0','0'),(15,'E002','7.29mm',0.3,'bullet',502,1,1,10,0.3,0,64,'0','0'),(16,'S001','타이어휠',10,'spoil',601,4,4,0,0.4,2000,1,'0','0'),(17,'S002','1.5볼트건전지',2,'spoil',602,1,2,0,0.5,600,1,'0','0'),(18,'S003','밧줄한묶음',6,'spoil',603,1,3,0,0.6,500,1,'0','0'),(19,'S004','은도금톱니바퀴',8,'spoil',604,3,3,0,0.7,200,1,'0','0'),(20,'S005','금괴',5,'spoil',605,2,1,0,0.8,2000,1,'0','0'),(21,'S006','통나무',3,'spoil',606,2,2,0,0.9,250,1,'0','0');
/*!40000 ALTER TABLE `master_item_base` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-08-27 21:44:10
