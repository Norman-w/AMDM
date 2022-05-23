using System;
namespace AMDM.Manager
{
    public interface IPLCCommunicator
    {
        bool Connect();
        bool Disconnect();
        //AMDM_Domain.PLCStatusData GetMedicineGettingStatus(int stockIndex);
    }
}
