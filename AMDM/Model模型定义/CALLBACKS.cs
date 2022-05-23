using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM.Manager
{

    #region 单个药仓内的所有药品取药完成回调函数
    /// <summary>
    /// 当取药完成时的返回
    /// </summary>
    /// <param name="success"></param>
    /// <param name="canceled"></param>
    /// <param name="gettedMedicinesIndexAndCountDic">已经获取的药品的位置的stockindex->floorindex->gridindex格式对应的已经取药的数量的信息</param>
    public delegate void OnStock_MedicinesGettingStopedEventHandler(int stockIndex, bool success, Nullable<StockMedicinesGettingErrorEnum> error, bool canceled, Dictionary<string, int> gettedMedicinesIndexAndCountDic);

    /// <summary>
    /// 当单品取药完成时的返回
    /// </summary>
    /// <param name="gridIndexString">已经取药完成的格子的索引的  stockindex->floorindex->gridindex标识</param>
    //public delegate void OnOneMedicineGettingFinishedEventHandler(string gridIndexString);
    public delegate void OnStock_OneMedicineGettingFinishedEventHandler(MedicineGettingSubTask task);

    public delegate void OnStock_OneClipGettingFinishedEventHandler(MedicinesGroupGettingTask task);

    /// <summary>
    /// 当一个药槽上的所有的药品都取完了以后返回
    /// </summary>
    /// <param name="task"></param>
    public delegate void OnStock_OneClipMedicinesGettingFinishedEventHandler(MedicinesGroupGettingTask task);

    /// <summary>
    /// 当一个药品准备开始取药,通常用于监控截图的抓拍
    /// </summary>
    /// <param name="task"></param>
    public delegate void OnStock_OneMedicineGettingStartingEventHandler(MedicineGettingSubTask task);

    /// <summary>
    /// 当一个药槽的药品批量取药开始,通常用于监控图的抓拍
    /// </summary>
    /// <param name="task"></param>
    public delegate void OnStock_OneClipGettingStartingEventHandler(MedicinesGroupGettingTask task);
    /// <summary>
    /// 当一个药品已经开始取药,通常用于记录药机的当前状态信息
    /// </summary>
    /// <param name="task"></param>
    public delegate void OnStock_OneMedicineGettingStartedEventHandler(MedicineGettingSubTask task);

    /// <summary>
    /// 当一个药槽的批量取药任务已经开始,通常用于记录药机的当前状态信息
    /// </summary>
    /// <param name="task"></param>
    public delegate void OnStock_OneClipGettingStartedEventHandler(MedicinesGroupGettingTask task);

    /// <summary>
    /// 当打印和拍照截图信号被触发以后 通知所有的plc通讯器 已经有plc进行了拍照和打印信号的触发,所有的plc不再进行该信号的处理
    /// </summary>
    /// <param name="plc"></param>
    public delegate void OnAMDM_PrintAndSnapshotCaptureCommandTriggeredEventHandler(int stockIndex);

    /// <summary>
    /// 当药品已经被取走信号被触发以后,通知所有的plc通讯器,已经有plc进行了药品已经被取走信号的触发,所有的plc不再进行该信号的处理
    /// </summary>
    /// <param name="plc"></param>
    public delegate void OnAMDM_MedicinesHasBeenTakedCommandTriggeredEventHandler(int stockIndex);

    /// <summary>
    /// 当一个药仓获取药品的时候发生了错误的时候 2022年2月13日14:32:17  不再使用   finished里面的task只要标记为错误即可
    /// </summary>
    //public delegate void OnStock_GettingMedicineErrorEventHandler(AMDM_DeliveryRecord record, StockMedicinesGettingErrorEnum error);
    #endregion

    #region 整个付药单完成取药的回调函数
    public delegate void OnStocks_AllMedicinesDeliveryFinishedEventHandler(AMDM_DeliveryRecord record);
    #endregion

    #region 所有药品已经取完并且已经触发了拍照,药品已经被用户取走以后的回调
    public delegate void OnAMDM_MedicinesHasBeenTakedEventHandler(AMDM_DeliveryRecord record);
    #endregion

    #region 当药机发生了取药故障的时候的回调函数 通知外部视图处理器发生故障
    public delegate void OnAMDM_MedicinesGettingErrorEventHandler(AMDMMedicinesGettingErrorEnum error, Dictionary<int, List<StockMedicinesGettingErrorEnum>> subErrors);
    #endregion
}
