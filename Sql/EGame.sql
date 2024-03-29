-- phpMyAdmin SQL Dump
-- version 4.8.3
-- https://www.phpmyadmin.net/
--
-- 主机： 9.3.211.35:14674
-- 生成日期： 2019-12-13 10:54:36
-- 服务器版本： 5.7.18-txsql-log
-- PHP 版本： 5.6.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 数据库： `EGame`
--
CREATE DATABASE IF NOT EXISTS `EGame` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `EGame`;

-- --------------------------------------------------------

--
-- 表的结构 `RankInfo`
--

CREATE TABLE `RankInfo` (
  `Id` int(11) NOT NULL,
  `UserId` bigint(20) NOT NULL COMMENT '用户Id',
  `UserNick` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '用户昵称',
  `UserFans` int(11) NOT NULL DEFAULT '0' COMMENT '粉丝/关注/订阅人数',
  `RoomTitle` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '房间标题',
  `RoomOnline` int(11) DEFAULT '0' COMMENT '房间在线人数',
  `AppName` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '直播分类',
  `AppPercent` int(11) NOT NULL DEFAULT '0' COMMENT '分类所占百分比',
  `LogTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- 转储表的索引
--

--
-- 表的索引 `RankInfo`
--
ALTER TABLE `RankInfo`
  ADD PRIMARY KEY (`Id`);

--
-- 在导出的表使用AUTO_INCREMENT
--

--
-- 使用表AUTO_INCREMENT `RankInfo`
--
ALTER TABLE `RankInfo`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
