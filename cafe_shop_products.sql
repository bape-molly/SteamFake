-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: cafe_shop
-- ------------------------------------------------------
-- Server version	8.0.36

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
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `ProductID` varchar(50) NOT NULL,
  `ProductName` varchar(100) NOT NULL,
  `Type` enum('Meal','Drink') NOT NULL,
  `Stock` int NOT NULL,
  `Price` decimal(10,2) NOT NULL,
  `Status` varchar(50) DEFAULT 'Available',
  `Image` varchar(255) DEFAULT NULL,
  `DateInsert` datetime NOT NULL,
  `DateUpdate` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ProductID` (`ProductID`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (13,'PROD-1','Tiramisu','Meal',46,5.00,'Available','https://vietlotusfoods.com.vn/wp-content/uploads/2020/09/Lam-banh-tiramisu-chuan-Y-h4.jpg','2024-07-04 01:52:25','2024-07-03 21:03:05'),(14,'PROD-2','Macaron','Meal',30,6.00,'Available','https://www.southernliving.com/thmb/dnsycw_-mf35yKRkq3cBsThVzrY=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/Southern-Living_Macarons_025-0e05e0cd226d44609f55ed8bc9cd3a3e.jpg','2024-07-04 01:53:13','2024-07-03 19:18:34'),(15,'PROD-3','Mousse Cake','Meal',39,5.00,'Available','https://cdn.apartmenttherapy.info/image/upload/f_jpg,q_auto:eco,c_fill,g_auto,w_1500,ar_4:3/k%2FPhoto%2FRecipes%2F2024-01-triple-chocolate-mousse-cake%2Ftriple-chocolate-mousse-cake-0739','2024-07-04 01:53:38','2024-07-03 20:39:31'),(16,'PROD-4','Muffin','Meal',49,3.00,'Available','https://richanddelish.com/wp-content/uploads/2022/03/coffee-cake-muffins-5.jpg','2024-07-04 01:54:00','2024-07-03 20:23:16'),(17,'PROD-5','Chocolate Tart','Meal',35,3.00,'Available','https://inbloombakery.com/wp-content/uploads/2020/06/chocolate-tart-FI.jpg','2024-07-04 01:54:39','2024-07-03 20:09:07'),(18,'PROD-6','Espresso','Drink',99,4.00,'Available','https://baristaschool.vn/wp-content/uploads/2021/03/espressotieuchuanlagi.png','2024-07-04 01:55:19','2024-07-03 19:18:34'),(19,'PROD-7','Latte','Drink',99,4.00,'Available','https://www.caffesociety.co.uk/assets/recipe-images/latte-small.jpg','2024-07-04 01:56:40','2024-07-03 20:39:31'),(20,'PROD-8','Capuchino','Drink',100,4.00,'Available','https://vmass.vn/wp-content/uploads/2020/04/ca-phe-capuchino-1.jpg','2024-07-04 01:56:56','2024-07-03 19:18:34'),(21,'PROD-9','Cherry Bomb Blend','Drink',100,4.00,'Available','https://visty.vn/wp-content/uploads/2021/12/ca-phe-espresso-cherry-bomb-2.jpg','2024-07-04 01:57:19','2024-07-03 19:18:34'),(22,'PROD-10','Moka','Drink',99,4.00,'Available','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQECcea0dvJE7UYgWPEcLxzdX07cdnVBVsp2Q&s','2024-07-04 01:57:35','2024-07-03 21:13:16'),(23,'PROD-11','Blue Mountain','Drink',99,4.00,'Available','https://www.lottemart.vn/media/catalog/product/cache/0x0/8/9/8936122200276-1-1.jpg.webp','2024-07-04 01:59:02','2024-07-03 21:19:51'),(24,'PROD-12','Strawberry Smoothie','Drink',99,4.00,'Available','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR1WIxUVzP9dMP0r8OVC-lhmjNghg1A7CP_gA&s','2024-07-04 01:59:44','2024-07-03 20:23:16'),(25,'PROD-13','Peanut Butter Banana Smoothie','Drink',98,3.00,'Available','https://hips.hearstapps.com/hmg-prod/images/peanut-butter-banana-smoothie-index-64e6355f938fd.jpg?crop=0.5xw:1xh;center,top&resize=1200:*','2024-07-04 02:00:30','2024-07-03 21:34:24'),(26,'PROD-14','Peach Smoothie','Drink',95,3.00,'Available','https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQogYnSMm2WEs9NBviFcIOvIBeGyUFrJQffKQ&s','2024-07-04 02:01:00','2024-07-03 20:09:07');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-07-04  5:50:02
