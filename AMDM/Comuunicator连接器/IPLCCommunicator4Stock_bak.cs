using System;
namespace AMDM.Manager
{
    public interface IPLCCommunicator4Stock : IPLCCommunicator
    {
        void AbortMedicinesGetting();
        bool AllMedicineGettingFinished { get; set; }
        bool SendAllFinishedHaveARestCommand();
        void Dispose();
        StockMedicinesGettingErrorEnum? Error { get; set; }
        bool IsBusy { get; set; }
        event OnStock_MedicinesGettingStopedEventHandler OnAllMedicinesGettingStoped;
        event OnStock_OneMedicineGettingFinishedEventHandler OnOneMedicineGettingFinished;
        event OnStock_OneClipGettingFinishedEventHandler OnOneClipGettingFinished;
        event OnStock_OneMedicineGettingStartedEventHandler OnOneMedicineGettingStarted;
        event OnStock_OneClipGettingStartedEventHandler OnOneClipGettingStarted;
        event OnStock_OneMedicineGettingStartingEventHandler OnOneMedicineGettingStarting;
        event OnStock_OneClipGettingStartingEventHandler OnOneClipGettingStarting;
        
        bool StartGridClear(AMDM_Domain.AMDM_Grid grid, int maxTimes, Action<int> onClearFinished);
        //bool StartMedicinesGetting(System.Collections.Generic.List<MedicineGettingSubTask> tasks);
        bool StartMedicinesGetting(System.Collections.Generic.List<MedicinesGroupGettingTask> tasks);
        int StockIndex { get; set; }
    }
}
