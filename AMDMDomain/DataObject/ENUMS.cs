using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 获取药品实体的方式
    /// </summary>
    public enum GetMedicinesObjectSortModeEnum
    {
        /// <summary>
        /// 截止日期/到期日 升序排列
        /// </summary>
        ExpirationDateAsc,
        /// <summary>
        /// 截止日期/到期日 倒序排列
        /// </summary>
        ExpirationDateDesc,
        /// <summary>
        /// 入库时间排序 升序排列
        /// </summary>
        ObjectIdAsc,
        /// <summary>
        /// 入库时间排序 降序排列
        /// </summary>
        ObjectIdDesc,
    }
    /// <summary>
    /// 入库还是出库
    /// </summary>
    public enum InOrOutTypeEnum { UnKnow = 0, In = 1, Out = -1 };
    /// <summary>
    /// 快照属于哪个应用表中的枚举
    /// </summary>
    public enum SnapshotParentTypeEnum
    {
        /// <summary>
        /// 表示未知,初始化时使用
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// 付药单基础信息,也就表示parentId为付药单记录的编号
        /// </summary>
        DeliveryRecord = 1,
        /// <summary>
        /// 付药单详情记录图,也就表示parentId应该为付药单详细记录的编号
        /// </summary>
        DeliveryRecordDetail = 2,

        #region 目前还没有使用到,但是添加进来了准备以后可能使用到的字段.到时候入库的时候也要拍照记录信息.
        /// <summary>
        /// 入库单记录用到的图,也就表示ParentId应该为入库单的编号
        /// </summary>
        InStockRecord = 3,
        /// <summary>
        /// 入库单明细记录用到的图,也就是表示ParentId应该为入库单明细的编号.
        /// </summary>
        InStockRecordDetail = 4,
        #endregion
    }

    /// <summary>
    /// 拍照机位的枚举
    /// </summary>
    public enum SnapshotLocationEnum
    {
        /// <summary>
        /// 付药单据凭证图片
        /// </summary>
        DeliveryRecordPaper = 0,
        /// <summary>
        /// 取药斗
        /// </summary>
        MedicineBucket = 1,
        /// <summary>
        /// 交互区
        /// </summary>
        InteractiveArea = 2,
        /// <summary>
        /// 取药机械手相机点位1
        /// </summary>
        GrabbersArea1 = 3,
        /// <summary>
        /// 取药机械手相机点位2
        /// </summary>
        GrabbersArea2 = 4,
    }
    /// <summary>
    /// 执行拍照抓图的时间点,实在动作之前还是动作之后,还是在动作时
    /// </summary>
    public enum SnapshotTimePointEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// 在动作之前
        /// </summary>
        BeforeAction = 1,
        /// <summary>
        /// 在动作之后
        /// </summary>
        AfterAction = 2,

        /// <summary>
        /// 动作正在执行的时候
        /// </summary>
        OnAction = 3,
    }
}
