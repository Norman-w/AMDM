using System;
namespace AMDM.Manager
{
    /// <summary>
    /// 取药控制器
    /// </summary>
    interface IMedicinesGettingController
    {
        void AbortMedicinesGetting(bool transferMedicinesToRecyler);
        void Dispose();
        AMDMMedicinesGettingErrorEnum? Error { get; set; }
        IPLCCommunicator4Stock GetStocksPLC(int index);
        /// <summary>
        /// 初始化取药控制器
        /// </summary>
        /// <param name="stocks">药仓集合</param>
        /// <param name="useSinglePLCMode">是否使用多PLC模式,西门子PLC具有良好的通讯能力,可以通过上位机控制多线程取药,所以用多PLC通讯模式就是为每一个药仓建立一个PLC连接
        /// 否则是使用单PLC模式</param>
        /// <returns></returns>
        bool Init(System.Collections.Generic.List<AMDM_Domain.AMDM_Stock> stocks, bool useSinglePLCMode);
        bool MedicinesGetting { get; set; }
        event OnStocks_AllMedicinesDeliveryFinishedEventHandler OnAllMedicinesDeliveryFinished;
        event OnAMDM_MedicinesGettingErrorEventHandler OnMedicinesGettingError;
        event OnAMDM_MedicinesHasBeenTakedEventHandler OnMedicinesHasBeenTaked;
        StartMedicinesGettingResult StartMedicinesGetting(AMDM_Domain.AMDM_MedicineOrder order);
        System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<StockMedicinesGettingErrorEnum>> SubErrors { get; set; }
    }
}
