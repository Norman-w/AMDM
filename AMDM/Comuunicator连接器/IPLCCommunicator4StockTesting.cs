using System;
namespace AMDM.Manager
{
    interface IPLCCommunicator4StockTesting
    {
        bool SendClearMainStatusWaitNextMedicineGettingCommandTest();
        bool SendGrabberPositioningTestCommandTest(float xMM, float yMM, AMDM_Domain.WhichGrabberEnum grabber, int times);
        bool SendStartMedicineGettingCommandTest(int spacilFloorIndex, int spacilGridIndex, int times);
        bool SendStartMedicineGettingCommandTest(float xMM, float yMM, AMDM_Domain.WhichGrabberEnum grabber, int times);
    }
}
