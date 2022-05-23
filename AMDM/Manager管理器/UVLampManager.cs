using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 紫外线灯控制器,在控制紫外线的时候需要通过PLC发送信号,但是可能检测器正在使用其他的线程检测
 * 所以要暂停检测器以后,发送紫外线灯的控制命令,然后再进行操作.操作后恢复检测器的检测
     */
namespace AMDM
{
    //public enum UVLampStatusEnum
    //    {
    //        Preparing,
    //        On,
    //        Off
    //    }
    /// <summary>
    /// 当uv灯状态发生改变时触发,参数为在开启紫外线灯之前的状态,只有关闭紫外线灯的时候才会传递
    /// </summary>
    /// <param name="maintenanceStatusBeforeUVLampWorking"></param>
    public delegate void OnUVLampStatusChangedEvnetHandler(Nullable<MaintenanceStatusEnum> maintenanceStatusBeforeUVLampWorking);

    /// <summary>
    /// 紫外线灯控制器
    /// </summary>
    public class UVLampManager
    {
        /// <summary>
        /// 打开状态,默认设置为true,让程序运行起来以后就执行一次关闭的动作.这样的话 防止程序是在上一次开了紫外线灯以后 崩溃了 然后又起来的时候一直都不关闭紫外线灯.
        /// </summary>
        bool on = true;
        /// <summary>
        /// 手动打开模式的时候,开到什么时间关闭
        /// </summary>
        Nullable<DateTime> muanlTurnOnModeAutoTurnOffTime = null;
        /// <summary>
        /// 紫外线杀菌灯开始工作之前,系统是什么状态,比如卡药了,正常等等.等紫外线灯恢复工作以后再给他设置回去
        /// </summary>
        Nullable<MaintenanceStatusEnum> MaintenanceStatusBeforeUVLampWorking = null;
        /// <summary>
        /// 当紫外线灯的状态变更时触发
        /// </summary>
        public event OnUVLampStatusChangedEvnetHandler OnUVLampStatusChanged;
        /// <summary>
        /// 紫外线灯的当前状态
        /// </summary>
        //public UVLampStatusEnum Status { get; set; }
        /// <summary>
        /// 杀菌工作时,该值表示剩余工作时间
        /// </summary>
        public double Remaining { get; set; }
        #region 变量
        /// <summary>
        /// 单独的工作线程
        /// </summary>
        BackgroundWorker bw = new BackgroundWorker();
        #endregion
        /// <summary>
        /// 初始化紫外线灯控制器
        /// </summary>
        public UVLampManager()
        {
            bw.DoWork += bw_DoWork;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync();
        }

        #region 公共方法
        /// <summary>
        /// 开始控制器
        /// </summary>
        /// <returns></returns>
        public bool StartManager()
        {
            if (this.bw.IsBusy)
            {
                return true;
            }
            else
            {
                this.bw.RunWorkerAsync();
            }
            return true;
        }
        /// <summary>
        /// 停止控制器
        /// </summary>
        /// <returns></returns>
        public bool StopManager()
        {
            if (this.bw.IsBusy)
            {
                this.bw.CancelAsync();
            }
            return true;
        }
        /// <summary>
        /// 打开紫外线灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOnUVLamp(Nullable<double> munalTurnOnModeAutoTurnOffDeleyMM)
        {
            if (App.medicinesGettingController == null || App.medicinesGettingController.MainPLCCommunicator == null)
            {
                Utils.LogError("无法操作紫外线杀菌灯,主控PLC尚未准备就绪");
                return false;
            }
            bool ret = false;
            App.MonitorsManager.HardwareMonitor.Pause = true;
            try
            {
                App.medicinesGettingController.MainPLCCommunicator.Connect();
                ret = App.medicinesGettingController.MainPLCCommunicator.SendUVLampControlCommand(true);
                if (ret)
                {
                    if (this.MaintenanceStatusBeforeUVLampWorking == null)
                    {
                        this.MaintenanceStatusBeforeUVLampWorking = App.ControlPanel.MaintenanceStatus;
                    }
                    App.ControlPanel.SetNewMaintenanceStatus(MaintenanceStatusEnum.紫外线杀菌中);
                    if (this.OnUVLampStatusChanged != null)
                    {
                        this.OnUVLampStatusChanged(MaintenanceStatusBeforeUVLampWorking);
                    }
                    this.on = true;

                    if (munalTurnOnModeAutoTurnOffDeleyMM != null)
                    {
                        if (this.muanlTurnOnModeAutoTurnOffTime != null)
                        {
                            Utils.LogWarnning("当前紫外线灯已经是打开状态,关闭时间预计为:", this.muanlTurnOnModeAutoTurnOffTime);
                            this.muanlTurnOnModeAutoTurnOffTime = DateTime.Now.AddMilliseconds(munalTurnOnModeAutoTurnOffDeleyMM.Value);
                            Utils.LogSuccess("手动开启模式,已经重设到期关闭时间为:", this.muanlTurnOnModeAutoTurnOffTime);
                        }
                        else
                        {
                            this.muanlTurnOnModeAutoTurnOffTime = DateTime.Now.AddMilliseconds(munalTurnOnModeAutoTurnOffDeleyMM.Value);
                            Utils.LogSuccess("手动开启模式,已设置到期关闭时间为:", this.muanlTurnOnModeAutoTurnOffTime);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Utils.LogError("打开紫外线灯捕获到错误:", err.Message);
                throw;
            }
            App.MonitorsManager.HardwareMonitor.Pause = false;
            return ret;
        }
        /// <summary>
        /// 关闭紫外线灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOffUVLamp(bool clearMunalModeTimer)
        {
            if (App.medicinesGettingController == null || App.medicinesGettingController.MainPLCCommunicator == null)
            {
                Utils.LogError("无法操作紫外线杀菌灯,主控PLC尚未准备就绪");
                return false;
            }
            App.MonitorsManager.HardwareMonitor.Pause = true;
            bool ret = false;
            try
            {
                App.medicinesGettingController.MainPLCCommunicator.Connect();
                ret = App.medicinesGettingController.MainPLCCommunicator.SendUVLampControlCommand(false);
                if (ret)
                {
                    App.ControlPanel.SetNewMaintenanceStatus(this.MaintenanceStatusBeforeUVLampWorking == null ? MaintenanceStatusEnum.运行正常 : this.MaintenanceStatusBeforeUVLampWorking.Value);
                    if (this.OnUVLampStatusChanged != null)
                    {
                        this.OnUVLampStatusChanged(this.MaintenanceStatusBeforeUVLampWorking);
                    }
                    this.MaintenanceStatusBeforeUVLampWorking = null;
                    this.on = false;
                    if (clearMunalModeTimer)
                    {
                        this.muanlTurnOnModeAutoTurnOffTime = null;
                        Utils.LogInfo("已经清除了手动模式的计时器");
                    }
                }
            }
            catch (Exception err)
            {
                Utils.LogError("关闭UV灯捕获到错误:", err.Message);
            }
            App.MonitorsManager.HardwareMonitor.Pause = false;
            return ret;
        }
        #endregion

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (bw!= null && bw.CancellationPending == false)
            {
                //每次检查到没到开启或者关闭时间 间隔10秒
                System.Threading.Thread.Sleep(10000);
                var now = DateTime.Now;
                if (App.Setting.DevicesSetting.UVLampSetting.UVLampOnTime!= null && App.Setting.DevicesSetting.UVLampSetting.UVLampOffTime != null)
                {
                    var startTime = getTodayThisTime(now, App.Setting.DevicesSetting.UVLampSetting.UVLampOnTime.Value);
                    DateTime prepareTime = startTime;
                    var endTime = getTodayThisTime(now, App.Setting.DevicesSetting.UVLampSetting.UVLampOffTime.Value);
                    if (App.Setting.DevicesSetting.UVLampSetting.HowEarlyToEnterTheWarningStateSS!= null)
                    {
                        prepareTime = startTime.AddSeconds(-App.Setting.DevicesSetting.UVLampSetting.HowEarlyToEnterTheWarningStateSS.Value);
                    }
                    if (prepareTime == startTime)
                    {
                        Utils.LogWarnning("当前未设置紫外线灯打开前的提醒时间");
                    }
                    if (endTime<startTime)
                    {
                        Utils.LogError("当前紫外线灯开启和关闭时间设置错误,关闭时间必须大于开始时间", startTime, endTime);
                        continue;
                    }
                    //如果需要提前准备,并且当前已经到了准备时间,并且当前在结束时间之前,就开始准备
                    if (prepareTime<startTime && now >= prepareTime && now <= startTime)
                    {
                        enterPrepareMode();
                    }
                        //如果当前已经可以开始了,并且不在结束时间之后,尝试开始
                    else if (now >= startTime && now <= endTime)
                    {
                        enterTurnOnMode();
                    }
                    else if(this.muanlTurnOnModeAutoTurnOffTime!= null && now>=muanlTurnOnModeAutoTurnOffTime)
                    {
                        enterTurnOffMode(true);
                    }
                        //如果当前已经到了结束时间了,结束
                    else if (this.muanlTurnOnModeAutoTurnOffTime == null && now >= endTime)
                    {
                        enterTurnOffMode(false);
                    }
                }
            }
        }
        DateTime getTodayThisTime(DateTime now, DateTime time)
        {
            DateTime ret = new DateTime(now.Year, now.Month, now.Day, time.Hour, time.Minute, time.Second);
            return ret;
        }
        /// <summary>
        /// 进入准备模式
        /// </summary>
        void enterPrepareMode()
        {
            if (App.medicinesGettingController == null || App.medicinesGettingController.MainPLCCommunicator == null)
            {
                Utils.LogError("无法操作紫外线杀菌灯,主控PLC尚未准备就绪");
                return;
            }
            if (App.ControlPanel.MaintenanceStatus == MaintenanceStatusEnum.紫外线杀菌准备中)
            {
                return;
            }

            if (this.MaintenanceStatusBeforeUVLampWorking == null)
            {
                this.MaintenanceStatusBeforeUVLampWorking = App.ControlPanel.MaintenanceStatus;
            }
            App.ControlPanel.SetNewMaintenanceStatus(AMDM_Domain.MaintenanceStatusEnum.紫外线杀菌准备中);
            if (this.OnUVLampStatusChanged!= null)
            {
                this.OnUVLampStatusChanged(this.MaintenanceStatusBeforeUVLampWorking);
            }
            Utils.LogStarted("已自动进入紫外线灯杀菌准备状态");
        }
        bool doingAction = false;
        /// <summary>
        /// 进入打开模式
        /// </summary>
        void enterTurnOnMode()
        {
            if (App.medicinesGettingController == null || App.medicinesGettingController.MainPLCCommunicator == null)
            {
                Utils.LogError("无法操作紫外线杀菌灯,主控PLC尚未准备就绪");
                return;
            }
            if (App.ControlPanel.MaintenanceStatus == MaintenanceStatusEnum.紫外线杀菌中)
            {
                return;
            }
            if (this.doingAction == true)
            {
                return;
            }
            doingAction = true;
            pauseHardwareMonitor();
            if (this.TurnOnUVLamp(null))
            {
                Utils.LogStarted("已自动进入紫外线灯杀菌状态");
            }
            else
            {
                Utils.LogError("自动打开紫外线灯发生错误");
            }
            resumeHardwareMonitor();
            doingAction = false;
        }
        /// <summary>
        /// 进入关闭模式
        /// </summary>
        void enterTurnOffMode(bool clearMunalModeTimer)
        {
            if (App.medicinesGettingController == null || App.medicinesGettingController.MainPLCCommunicator == null)
            {
                Utils.LogError("无法操作紫外线杀菌灯,主控PLC尚未准备就绪");
                return;
            }
            if (this.doingAction == true)
            {
                return;
            }
            //没有之前的状态,说明没有打开紫外线灯,但是有可能是上一次打开了紫外线灯以后,系统崩溃了,然后再打开的程序不会正常关闭紫外线灯.这种情况的时候.
            //通过是否打开的变量来控制
            if (!this.on)
            {
                return;
            }
            doingAction = true;
            pauseHardwareMonitor();
            if (this.TurnOffUVLamp(clearMunalModeTimer))
            {
                Utils.LogFinished("已经自动结束紫外线杀菌");
            }
            else
            {
                Utils.LogError("自动关闭紫外线灯发生错误");
            }
            resumeHardwareMonitor();
            doingAction = false;
        }
        void pauseHardwareMonitor()
        {
            if (App.MonitorsManager != null && App.MonitorsManager.HardwareMonitor != null)
            {
                App.MonitorsManager.HardwareMonitor.Pause = true;
                Utils.LogStarted("紫外线灯控制器暂停了硬件检测器的刷新(通讯),997毫秒后继续执行");
                System.Threading.Thread.Sleep(997);
            }
        }
        void resumeHardwareMonitor()
        {
            if (App.MonitorsManager != null && App.MonitorsManager.HardwareMonitor != null)
            {
                System.Threading.Thread.Sleep(997);
                Utils.LogFinished("紫外线灯控制器将在997毫秒后恢复硬件检测器的刷新(通讯)");
                App.MonitorsManager.HardwareMonitor.Pause = false;
            }
        }
    }
}
