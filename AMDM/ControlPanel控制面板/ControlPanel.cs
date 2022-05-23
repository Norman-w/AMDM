using AMDM.Manager;
using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 2021年12月19日14:59:33
 * 控制面板用于监控设备的状态,对设备的一些组件进行控制
 */
namespace AMDM
{
    /// <summary>
    /// 设备的控制面板
    /// </summary>
    public class ControlPanel
    {
        #region 全局变量
        AMDMPeripheralsStatus peripheralsStatus = new AMDMPeripheralsStatus();
        //IPLCCommunicator4Main mainPlc = null;
        MaintenanceStatusEnum _preMainStatus = MaintenanceStatusEnum.运行正常;
        MaintenanceStatusEnum _mainStatus = MaintenanceStatusEnum.运行正常;
        #endregion

        #region 公共字段
        /// <summary>
        /// 包含全部外设的工作状态的字段对象.如紫外线灯是否开启,当前温度等.
        /// </summary>
        public AMDMPeripheralsStatus PeripheralsStatus { get { return peripheralsStatus; } set {
            Utils.LogBug("不应该通过Status字段来设置类内的私有变量 status(小写)");
        } }
        /// <summary>
        /// 获取正在显示中的页面,调用这个函数,会自动转到取药页面的这个函数.在调用之前要判断是否为null
        /// </summary>
        public Func<ShowingPageEnum> GetShowingPage { get; set; }

        /// <summary>
        /// 全局状态,当设置为运行正常的时候,触发OnControlPanelClearErrorOfMaintenanceStatus
        /// </summary>
        public MaintenanceStatusEnum MaintenanceStatus
        {
            get
            {
                return this._mainStatus;
            }
            private set
            {
                //如果一样就不设置了.
                if (value == this._mainStatus)
                {
                    return;
                }
                //如果不一样的话,记录之前的状态
                this._preMainStatus = this._mainStatus;   
                //然后设置为新状态
                this._mainStatus = value;
            }
        }

        public long TodayPrescriptionCount { get; set; }

        public long TodayMedicineCount { get; set; }


        #endregion

        #region 构造函数
        /// <summary>
        /// 设备控制面板,控制面板的接口服务器通过此面板的对象(全局唯一的)进行于外界的交互
        /// </summary>
        public ControlPanel()
        {
            //this.mainPlc = App.medicinesGettingController.MainPLCCommunicator;
        }
        #endregion

        #region 私有方法
        void pauseHardwareMonitor()
        {
            if (App.MonitorsManager != null && App.MonitorsManager.HardwareMonitor != null)
            {
                App.MonitorsManager.HardwareMonitor.Pause = true;
                Utils.LogStarted("控制面板暂停了硬件检测器的刷新(通讯)");
            }
        }
        void resumeHardwareMonitor()
        {
            if (App.MonitorsManager != null && App.MonitorsManager.HardwareMonitor != null)
            {
                App.MonitorsManager.HardwareMonitor.Pause = false;
                Utils.LogFinished("控制面板恢复了硬件检测器的刷新(通讯)");
            }
        }
        #endregion

        #region 公有方法

        DateTime lastReloadTime = DateTime.MinValue;

        public bool ReloadStatus()
        {
            #region 给一些测试数据:
            //PeripheralsStatus.WarehousesACStatus = new List<WarehouseACStatus>();
            //WarehouseACStatus ac = new WarehouseACStatus();
            //ac.CurrentTemperature = 23 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10);
            //ac.DestTemperature = 18 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10);
            //ac.IsACWorking = true;
            //ac.WarehouseIndexId = 0;

            //WarehouseACStatus ac2 = new WarehouseACStatus()
            //{
            //    WarehouseIndexId = 1,
            //    IsACWorking = false,
            //    DestTemperature = 18 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10),
            //    CurrentTemperature = 18 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10),
            //};

            //WarehouseACStatus ac3 = new WarehouseACStatus()
            //{
            //    WarehouseIndexId = 2,
            //    IsACWorking = false,
            //    DestTemperature = 18 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10),
            //    CurrentTemperature = 18 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10),
            //};
            //WarehouseACStatus ac4 = new WarehouseACStatus()
            //{
            //    WarehouseIndexId = 3,
            //    IsACWorking = false,
            //    DestTemperature = 18 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10),
            //    CurrentTemperature = 18 + Convert.ToSingle(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * 10),
            //};

            //PeripheralsStatus.WarehousesACStatus.Add(ac);
            //PeripheralsStatus.WarehousesACStatus.Add(ac2);
            //PeripheralsStatus.WarehousesACStatus.Add(ac3);
            //PeripheralsStatus.WarehousesACStatus.Add(ac4);
            #endregion

            #region 获取当日总处方数量和总药品数量
            try
            {
                App.ControlPanel.TodayPrescriptionCount = App.sqlClient.GetTodayPrescriptionCount();
                App.ControlPanel.TodayMedicineCount = App.sqlClient.GetTodayMedicineCount();
            }
            catch (Exception err)
            {
                Utils.LogError("获取当日的已取处方数和药品数错误:", err.Message);
            }
            #endregion
            return true;
        }
        
        /// <summary>
        /// 打开某个药仓的空调
        /// </summary>
        /// <param name="stockIndex">具体是哪个药仓的顺序位置</param>
        /// <returns></returns>
        public bool TurnOnAC(int stockIndex)
        {
            Utils.LogSimulating("模拟打开药仓的空调:药仓索引:", stockIndex);
            return false;
        }
        /// <summary>
        /// 关闭某个药仓的空调
        /// </summary>
        /// <param name="stockIndex">具体是哪个药仓的顺序位置</param>
        /// <returns></returns>
        public bool TurnOffAC(int stockIndex)
        {
            Utils.LogSimulating("模拟关闭药仓的空调:药仓索引:", stockIndex);
            return false;
        }
        /// <summary>
        /// 设置某个药仓的目标温度
        /// </summary>
        /// <param name="stockIndex">具体是哪个药仓的顺序位置</param>
        /// <param name="Temperature">目标温度</param>
        /// <returns></returns>
        public bool SetACDestTemperature(int stockIndex, float Temperature)
        {
            if (Temperature>=40 && Temperature <=40)
            {
                Utils.LogBug("你他妈给的这是什么温度啊?");
                return false;
            }
            pauseHardwareMonitor();
            //Utils.LogSimulating("模拟设置药仓的空调目标温度", stockIndex, Temperature);
            //return false;
            try
            {
                App.medicinesGettingController.MainPLCCommunicator.Connect();
                var ret = App.medicinesGettingController.MainPLCCommunicator.SendSetACTemperature(stockIndex, Temperature);
                resumeHardwareMonitor();
                return ret;
            }
            catch (Exception err)
            {
                Utils.LogError("设置空调温度错误", err.Message);
                resumeHardwareMonitor();
                return false;
            }
        }
        /// <summary>
        /// 打开uv杀菌灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOnUVLamp(double autoTurnOffDelayMM)
        {
            try
            {
                pauseHardwareMonitor();
                var ret = App.UVLampManager.TurnOnUVLamp(autoTurnOffDelayMM);
                this.PeripheralsStatus.UVLampIsWorking = MaintenanceStatus == MaintenanceStatusEnum.紫外线杀菌中;
                resumeHardwareMonitor();
                return ret;
            }
            catch (Exception err)
            {
                Utils.LogError("打开紫外线杀菌灯错误", err.Message);
                resumeHardwareMonitor();
                return false;
            }
        }
        /// <summary>
        /// 关闭uv杀菌灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOffUVLamp()
        {
            try
            {
                pauseHardwareMonitor();
                var ret = App.UVLampManager.TurnOffUVLamp(true);
                this.PeripheralsStatus.UVLampIsWorking = this.MaintenanceStatus == MaintenanceStatusEnum.紫外线杀菌中;
                resumeHardwareMonitor();
                return ret;
            }
            catch (Exception err)
            {
                Utils.LogError("关闭紫外线杀菌灯错误", err.Message);
                resumeHardwareMonitor();
                return false;
            }
        }

        /// <summary>
        /// 关闭所有的电控门及药仓的仓门
        /// </summary>
        /// <returns></returns>
        public bool LockAllDoors()
        {
            try
            {
                pauseHardwareMonitor();
                App.medicinesGettingController.MainPLCCommunicator.Connect();
                var ret= App.medicinesGettingController.MainPLCCommunicator.SendLockerControlCommand(false);
                resumeHardwareMonitor();
                return ret;
            }
            catch (Exception err)
            {
                Utils.LogError("打开所有仓门错误", err.Message);
                resumeHardwareMonitor();
                return false;
            }
        }
        /// <summary>
        /// 打开所有的电控门及药仓的仓门
        /// </summary>
        /// <returns></returns>
        public bool UnlockAllDoors()
        {
            try
            {
                pauseHardwareMonitor();
                App.medicinesGettingController.MainPLCCommunicator.Connect();
                var ret = App.medicinesGettingController.MainPLCCommunicator.SendLockerControlCommand(true);
                resumeHardwareMonitor();
                return ret;
            }
            catch (Exception err)
            {
                Utils.LogError("锁定所有仓门错误", err.Message);
                resumeHardwareMonitor();
                return false;
            }
        }

        public void ClearMaintenanceError()
        {
            #region 清掉错误状态的时候触发
            this.MaintenanceStatus = this._preMainStatus;
            if (this.OnControlPanelClearErrorOfMaintenanceStatus != null)
            {
                this.OnControlPanelClearErrorOfMaintenanceStatus();
            }
            #endregion
        }
        public void SetNewMaintenanceStatus(MaintenanceStatusEnum newStatus)
        {
            this.MaintenanceStatus = newStatus;
        }
        /// <summary>
        /// 恢复之前的状态,当清除故障或者是紫外线杀菌完毕后执行
        /// </summary>
        public void ResumeOldMaintenanceStatus()
        {
            this.MaintenanceStatus = this._preMainStatus;
        }
        #endregion

        #region 回调函数
        public event OnControlPanelClearErrorOfMaintenanceStatusEventHandler OnControlPanelClearErrorOfMaintenanceStatus;
        #endregion
    }
}
