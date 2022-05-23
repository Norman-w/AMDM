using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 当监测器监测到故障的时候发生的回调函数/事件
    /// </summary>
    /// <param name="monitor"></param>
    /// <param name="errType"></param>
    public delegate void OnMonnitorDetectedErrorEventHandler(object monitor, MonitorDetectedErrorTypeEnum errType);

    /// <summary>
    /// 当控制面板那边接收过来清空主状态上的错误信息时触发
    /// </summary>
    public delegate void OnControlPanelClearErrorOfMaintenanceStatusEventHandler();
    /// <summary>
    /// 当药品监测器检测到药品实体的有效期达到提醒阈值时触发
    /// </summary>
    /// <param name="medicines"></param>
    public delegate void OnMedicineObjectExpirationAlertEventHandler(Dictionary<long, AMDM_MedicineObject__Grid__Medicine> medicines);
    /// <summary>
    /// 当药品的库存数量警报时触发
    /// </summary>
    /// <param name="medicinesInventory"></param>
    public delegate void OnMedicineCountAlertEventHandler(Dictionary<long,AMDM_MedicineInventory> medicinesInventory);
}
