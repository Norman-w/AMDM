/*
Navicat MySQL Data Transfer

Source Server         : z
Source Server Version : 50616
Source Host           : qpmysqlserver.mysql.zhangbei.rds.aliyuncs.com:3306
Source Database       : amdm_local

Target Server Type    : MYSQL
Target Server Version : 50616
File Encoding         : 65001

Date: 2021-11-27 10:06:13
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for amdm_instock_record_detail
-- ----------------------------
DROP TABLE IF EXISTS `amdm_instock_record_detail`;
CREATE TABLE `amdm_instock_record_detail` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自动生成的',
  `ParentId` bigint(11) DEFAULT NULL COMMENT '入库单的ID,也就是这一条条目的所属ID',
  `Index` int(10) DEFAULT NULL COMMENT '在整个入库单中的索引位置',
  `MedicineId` bigint(20) DEFAULT NULL COMMENT '药品的id',
  `MedicineName` varchar(255) DEFAULT NULL COMMENT '药品的名称,记录的是药品当时的名称',
  `MedicineBarcode` varchar(30) DEFAULT NULL COMMENT '药品的条码,记录的是当时的药品的条码',
  `Count` int(10) DEFAULT NULL COMMENT '采购的数量,因为考虑到可能有散货的采集可能性,比如1.1吨,此字段设置为浮点型,正常用的话浮点型的整数部分存储我们日常见过的数量足够了',
  `InstockTime` datetime DEFAULT NULL COMMENT 'RecordTime',
  `StockIndex` int(11) DEFAULT NULL COMMENT '药品放在了哪个药仓当中',
  `FloorIndex` int(11) DEFAULT NULL COMMENT '药品放在了哪一层',
  `GridIndex` int(11) DEFAULT NULL COMMENT '药品放在了哪个格子索引',
  `Memo` varchar(50) DEFAULT NULL COMMENT '对这一条信息的备注',
  PRIMARY KEY (`Id`),
  KEY `indexItemId` (`MedicineId`),
  KEY `indexInstockTime` (`InstockTime`),
  KEY `indexInStockBillId` (`ParentId`),
  KEY `indexCount` (`Count`)
) ENGINE=InnoDB AUTO_INCREMENT=38504 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='入库单的具体的每一条的相关信息';
