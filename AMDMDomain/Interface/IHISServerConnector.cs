using System;
using System.Collections.Generic;
using AMDM_Domain;

/// <summary>
/// 付药机到HIS系统的连接器,每个医院的对接方式不同,但是必须要实现该接口内的全部方法.
/// </summary>
public interface IHISServerConnector
{
    /// <summary>
    /// 分页获取所有的药品信息
    /// </summary>
    /// <param name="onGetedMedicineInfoFromHisServer"></param>
    /// <returns></returns>
    List<AMDM_Medicine> GetAllMedicines(OnGetedMedicineInfoFromHisServerEventHandler onGetedMedicineInfoFromHisServer);
    /// <summary>
    /// 从HIS系统根据处方编号获取付药单订单,每个处方都只会有一个付药单
    /// </summary>
    /// <returns></returns>
    AMDM_MedicineOrder GetMedicineOrderByPrescriptionId(string prescriptionId);
    /// <summary>
    /// 从HIS系统根据id卡号或者是患者的编号获取付药单
    /// </summary>
    /// <returns></returns>
    List<AMDM_MedicineOrder> GetMedicineOrderByPatientId(string patientId);

    /// <summary>
    /// 2021年11月17日22:38:15  向付HIS系统推送信息,告诉HIS系统,某个付药单(处方)已经完成了付药
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    bool PutMedicineOrderFinished(AMDM_MedicineOrder order,string repoter);

    /// <summary>
    /// 通知his系统库存已经不足,这时候his接收到消息后可能会调拨药品信息.
    /// </summary>
    /// <param name="medicinesInventoryDic"></param>
    /// <returns></returns>
    bool NoticeInsufficientInventory(List<AMDM_MedicineInventory> inventories, string reporerInfo, string message, List<string> receiverMobileList);

    /// <summary>
    /// 告知HIS系统药机已经发生故障,让his系统通知护士或者是我们的工作人员 这样好排查故障
    /// </summary>
    /// <param name="msgContent"></param>
    /// <returns></returns>
    bool PushMachineErrorMsg(string msgContent, string repoter, List<string> receiverMobileList);

    /// <summary>
    /// 把本药机的库存推送给his系统.
    /// </summary>
    /// <param name="inventories"></param>
    /// <returns></returns>
    bool NoticeMedicinesInventory(List<AMDM_MedicineInventory> inventories, string repoter, List<long> receiverMobileList);

    /// <summary>
    /// 通知药机给定参数中的药品有效期预警信息.
    /// </summary>
    /// <param name="medicineObjects"></param>
    /// <returns></returns>
    bool NoticeMedicinesExpirationDateAlert(List<AMDM_MedicineObject__Grid__Medicine> medicineObjects, string reporterInfo, string message, List<string> receiverMobileList);
    /// <summary>
    /// 增量获取已经修改的药品数据
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="onGetedMedicineInfoFromHisServer"></param>
    /// <returns></returns>
    List<AMDM_Medicine> GetModifiedMedicines(DateTime start, DateTime end, OnGetedMedicineInfoFromHisServerEventHandler onGetedMedicineInfoFromHisServer);
    bool Init();
    /// <summary>
    /// 是否支持增量获取药品的形式
    /// </summary>
    bool GetIsSupportModifiedMedicines();
    ///// <summary>
    ///// 推送药品已经交付的信息
    ///// </summary>
    ///// <param name="order"></param>
    ///// <returns></returns>
    //string PushMedicinesDeliveriedMsg(AMDM_Domain.AMDM_MedicineOrder order);
    
    /// <summary>
    /// 用于检测HIS系统的当前连接状况,通常是在空闲时间检测HIS系统有没有维护等,如果在维护了.将同步锁定药机的使用.因为那时候就读取不出来处方了,扫了也没有用.如果可以了的时候再根据需要自动恢复
    /// 另外每次取药之前也要先检测一下HIS系统可否连接,如果不能连接,直接给用户提示.并且把药机至于维护等状态
    /// </summary>
    /// <returns></returns>
    bool CheckConnect();
}

/// <summary>
/// 当从his系统服务器获取到了一页数据的时候,使用这个回调接口返回
/// </summary>
/// <param name="medicinesInfo">一页药品数据,返回的格式直接就是每一个药品的对象,可以是每一个药品的json数据</param>
/// <param name="currentPage">当前获取到药品的页</param>
/// <param name="totalPage">一共有多少页药品需要获取</param>
public delegate void OnGetedMedicineInfoFromHisServerEventHandler(List<object> medicinesInfo, int currentPage,int totalPage);
