using AMDM.Manager;
using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM
{
    public class HardwareMonitor
    {
        IPLCCommunicator4Main mainPlc = null;
        private OnMonnitorDetectedErrorEventHandler onError;
        public HardwareMonitor(OnMonnitorDetectedErrorEventHandler onError)
        {
            this.onError = onError;
            mainPlc = App.medicinesGettingController.MainPLCCommunicator;
        }
        /// <summary>
        /// PLC连接是否正常
        /// </summary>
        public bool PLCConnectionValid { get; set; }

        /// <summary>
        /// PLC信号不良(接收到了null的status);
        /// </summary>
        public bool PLCSignalBad { get; set; }

        /// <summary>
        /// 取药斗被遮挡,里面有药
        /// </summary>
        public bool MedicinesBulketCoverd { get; set; }

        /// <summary>
        /// 药品掉落光栅记录器错误,被长时间遮挡
        /// </summary>
        public bool CounterCoverd { get; set; }

        /// <summary>
        /// 正在归位中
        /// </summary>
        public bool Resetting { get; set; }

        /// <summary>
        /// 打印机可用
        /// </summary>
        public bool PrinterValid { get; set; }

        /// <summary>
        /// 监控器可用
        /// </summary>
        public bool CCTVValid { get; set; }
        #region 外设状态
        ///// <summary>
        ///// 外设状态
        ///// </summary>
        //public AMDMPeripheralsStatus PeripheralsStatus { get; set; }
        ///// <summary>
        ///// 刷新外设状态
        ///// </summary>
        ///// <returns></returns>
        //public bool RefreshHardwareStatus()
        //{
        //    #region 空调状态

        //    PeripheralsStatus.WarehousesACStatus = new List<WarehouseACStatus>();
        //    var currents = App.medicinesGettingController.MainPLCCommunicator.SendGetAllStockACCurrentTemperature(App.machine.Stocks.Count);
        //    var dests = App.medicinesGettingController.MainPLCCommunicator.SendGetAllStockACDestTemperature(App.machine.Stocks.Count);
        //    foreach (var s in App.machine.Stocks)
        //    {
        //        WarehouseACStatus ac = new WarehouseACStatus();
        //        if (currents != null && currents.ContainsKey(s.IndexOfMachine) == true)
        //        {
        //            ac.CurrentTemperature = currents[s.IndexOfMachine] == null ? 0 : currents[s.IndexOfMachine].Value;
        //        }
        //        if (dests != null && dests.ContainsKey(s.IndexOfMachine) == true)
        //        {
        //            ac.DestTemperature = dests[s.IndexOfMachine] == null ? 0 : dests[s.IndexOfMachine].Value;
        //        }
        //        ac.WarehouseIndexId = s.IndexOfMachine;
        //        this.PeripheralsStatus.WarehousesACStatus.Add(ac);
        //    }

        //    #endregion
        //    #region 获取紫外线灯状态
        //    bool on = App.medicinesGettingController.MainPLCCommunicator.SendGetUVLampStatus();
        //    this.PeripheralsStatus.UVLampIsWorking = on;
        //    #endregion
        //    return true;
        //}
        #endregion

        private DateTime lastRefreshStatusTime = DateTime.MinValue;
        private bool lastCheckFinished = true;

        /// <summary>
        /// 是否暂停刷新,如果网页浏览器那边发来设置温度等情况的时候需要先暂停这边的刷新,不要独占PLC的使用,然后等设置完了以后再刷新
        /// </summary>
        public bool Pause { get; set; }
        public void RefreshStatus()
        {
            if (Pause)
            {
                //Utils.LogInfo("硬件检测器跳过检测,因外部暂停了硬件检测器的刷新");
                return;
            }
            if ((DateTime.Now - lastRefreshStatusTime).TotalMilliseconds < App.Setting.TimeSignalGeneratorSetting.HardwareDetectionPerTimeIntervalMS || lastCheckFinished == false)
            {
                return;
            }
            lastCheckFinished = false;
            try
            {
                if (App.ControlPanel.GetShowingPage != null)
                {
                    if (App.ControlPanel.GetShowingPage() != ShowingPageEnum.空闲播放广告中)
                    {
                        //Utils.LogInfo("正在取药过程跳过执行硬件状态检测");
                        lastCheckFinished = true;
                        return;
                    }
                }
                PLCConnectionValid = mainPlc.Connect();
                var status = mainPlc.GetMedicineGettingStatus();
                if (status == null)
                {
                    PLCSignalBad = true;
                }
                else
                {
                    MedicinesBulketCoverd = status.MedicinesBulketCoverd;
                    CounterCoverd = status.CounterCoverdError;
                    Resetting = status.Resetting;
                }

                PrinterValid = App.DeliveryRecordPaperPrinter.CheckPrinterIsOnline(App.Setting.DevicesSetting.Printer58MMSetting.PrinterName);
                CCTVValid = App.CameraSnapshotCapturer.Connected;

                ReloadPeripheralsStatus();
            }
            catch (Exception err)
            {
                Utils.LogError("检测硬件状态时发生错误:", err.Message);
            }
            lastRefreshStatusTime = DateTime.Now;
            lastCheckFinished = true;


            #region 推送 只推送第一个错误即可
            if (onError!= null)
            {
                if (PLCConnectionValid == false)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.硬件PLC连接失败);
                    return;
                }
                if (PLCSignalBad == true)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.硬件PLC信号不良);
                    return;
                }
                if (MedicinesBulketCoverd)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.硬件取药斗有残留);
                    return;
                }
                if (CounterCoverd)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.硬件数量检测光栅遮挡);
                    return;
                }
                if (Resetting)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.硬件机械手复位中);
                    return;
                }
                if (!PrinterValid)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.硬件打印机异常);
                    return;
                }
                if (!CCTVValid)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.硬件监控器异常);
                    return;
                }
            }
            
            #endregion
            //Utils.LogSuccess("硬件检查完成,无故障");
        }

        /// <summary>
        /// 重新读取状态 通过跟plc的通讯,获取药仓中的温度等的信息
        /// </summary>
        /// <returns></returns>
        public bool ReloadPeripheralsStatus()
        {
            var currents = mainPlc.SendGetAllStockACCurrentTemperature(App.machine.Stocks.Count);
            List<WarehouseACStatus> acs = new List<WarehouseACStatus>();
            var dests = mainPlc.SendGetAllStockACDestTemperature(App.machine.Stocks.Count);
            foreach (var s in App.machine.Stocks)
            {
                WarehouseACStatus ac = new WarehouseACStatus();
                if (currents != null && currents.ContainsKey(s.IndexOfMachine) == true)
                {
                    ac.CurrentTemperature = currents[s.IndexOfMachine] == null ? 0 : currents[s.IndexOfMachine].Value;
                }
                if (dests != null && dests.ContainsKey(s.IndexOfMachine) == true)
                {
                    ac.DestTemperature = dests[s.IndexOfMachine] == null ? 0 : dests[s.IndexOfMachine].Value;
                }
                ac.WarehouseIndexId = s.IndexOfMachine;
                acs.Add(ac);
            }
            #region 获取紫外线灯状态
            bool on = mainPlc.SendGetUVLampStatus();
            #endregion
            App.ControlPanel.PeripheralsStatus.WarehousesACStatus = acs;
            App.ControlPanel.PeripheralsStatus.UVLampIsWorking = on;
            return true;
        }
    }
}
