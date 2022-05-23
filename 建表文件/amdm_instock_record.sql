/*
Navicat MySQL Data Transfer

Source Server         : z
Source Server Version : 50616
Source Host           : qpmysqlserver.mysql.zhangbei.rds.aliyuncs.com:3306
Source Database       : amdm_local

Target Server Type    : MYSQL
Target Server Version : 50616
File Encoding         : 65001

Date: 2021-11-27 10:06:04
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for amdm_instock_record
-- ----------------------------
DROP TABLE IF EXISTS `amdm_instock_record`;
CREATE TABLE `amdm_instock_record` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '入库单ID',
  `CreateTime` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '入库单的创建时间,也就是护士是从什么时候上药的',
  `MachineId` bigint(20) DEFAULT NULL COMMENT '给哪个机器进行的上药',
  `StockId` bigint(20) DEFAULT NULL COMMENT '给哪个药仓执行的上药',
  `NurseID` bigint(20) DEFAULT NULL COMMENT '上药的护士的id',
  `EntriesCount` int(10) DEFAULT NULL COMMENT '入库单上一共有多少条数据',
  `TotalMedicineCount` int(10) DEFAULT NULL COMMENT '入库单一共有多少件数',
  `Memo` varchar(255) DEFAULT NULL COMMENT '对整个入库单的一个备注',
  `Type` varchar(255) DEFAULT NULL COMMENT '入库单的类型 比如是 正常上药,报损,记录遗失等等',
  `FinishTime` datetime DEFAULT NULL COMMENT '完成的时间,就是本次上药完成,关舱门的时间',
  `Canceled` tinyint(4) DEFAULT '0' COMMENT '是否作废',
  PRIMARY KEY (`Id`),
  KEY `indexInStockBillId` (`Id`),
  KEY `indexTime` (`CreateTime`),
  KEY `indexCanceled` (`Canceled`),
  KEY `indexType` (`Type`),
  KEY `indexUserId` (`MachineId`),
  KEY `indexCreatorId` (`NurseID`)
) ENGINE=InnoDB AUTO_INCREMENT=15819 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC COMMENT='入库单';
