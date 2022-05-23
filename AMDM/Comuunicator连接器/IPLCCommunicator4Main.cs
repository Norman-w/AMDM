using System;
namespace AMDM.Manager
{
    interface IPLCCommunicator4Main : IPLCCommunicator
    {
        AMDM_Domain.PLCStatusData GetMedicineGettingStatus();
        bool SendACControlCommand(bool open);
        bool SendAllFinishedHasErrorNeedRecyleCommand();
        bool SendAllFinishedHaveARestCommand();
        float? SendGetACTemperature(int stockIndex);
        System.Collections.Generic.Dictionary<int,Nullable<float>> SendGetAllStockACCurrentTemperature(int stockCount);
        System.Collections.Generic.Dictionary<int, Nullable<float>> SendGetAllStockACDestTemperature(int stockCount);
        bool SendLockerControlCommand(bool unlock);
        bool SendSetACTemperature(int stockIndex, float destTemperature);
        bool SendShowGridNumberAt485ShowerOnStock(int stockIndex, int? gridIndexOfStock);
        bool SendUVLampControlCommand(bool open);
        bool SendGetUVLampStatus();
    }
}
