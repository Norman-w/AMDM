using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM.Manager
{

    /// <summary>
    /// 用于测试时使用的plc取药控制器
    /// </summary>
    public class PLCCommunicator4StockTesting_台达 : PLCCommunicator4Stock_台达//  IPLCCommunicator4StockTesting
    {
        //PLCCommunicator4Stock_台达 plc = null;
        public PLCCommunicator4StockTesting_台达(int stockIndex, StockPLCSettingTD setting, float centerDistanceBetweenTwoGrabbers, float xOffsetFromStartPointMM, float yOffsetFromStartPointMM, PLCCommunicator4Main_台达 mainPlc = null)
            : base(stockIndex, setting, centerDistanceBetweenTwoGrabbers, xOffsetFromStartPointMM, yOffsetFromStartPointMM, mainPlc)
        {
            //plc = new PLCCommunicatorTD(setting);
        }
        public bool SendClearMainStatusWaitNextMedicineGettingCommandTest()
        {
            return base.SendClearMainStatusWaitNextMedicineGettingCommand();
        }
        public bool SendGrabberPositioningTestCommandTest(float xMM, float yMM, WhichGrabberEnum grabber, int times)
        {
            return base.SendGrabberPositioningTestCommand(xMM, yMM, grabber, times);
        }
        public bool SendStartMedicineGettingCommandTest(float xMM, float yMM, WhichGrabberEnum grabber, int times)
        {
            return base.SendStartMedicineGettingCommand(xMM, yMM, grabber, times, 0);
        }
        public bool SendStartMedicineGettingCommandTest(int spacilFloorIndex, int spacilGridIndex, int times)
        {
            return base.SendStartMedicineGettingCommand(spacilFloorIndex, spacilGridIndex, times, 0);
        }
    }
}
