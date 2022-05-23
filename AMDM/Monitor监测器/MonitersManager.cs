using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM
{
    /// <summary>
    /// 监测器集合管理器.软件监测器,硬件监测器,药品监测器都在这里启动和管理.
    /// 考虑该类不提供监测结果的查询,如果有异常直接报告.也就是眼神好使但是是个聋子,听不到别人说什么,也就不处理别人的请求,但是他看到的都会告诉上级
    /// </summary>
    public class MonitersManager
    {
        private TimeSignalGenerator timeSignalGenerator;
        public HardwareMonitor HardwareMonitor;
        public SoftwarePartMonitor SoftwarePartManager;
        public MedicineMonitor MedicineMonitor;
        public MonitersManager()
        {
            this.timeSignalGenerator = new TimeSignalGenerator();
            this.HardwareMonitor = new HardwareMonitor(reportMonitorDetectedError);
            this.SoftwarePartManager = new SoftwarePartMonitor(reportMonitorDetectedError);
            this.MedicineMonitor = new MedicineMonitor(reportMedicineObjectExpirationAlert, reportMedicineCountAlert);

            timeSignalGenerator.RegisterIntervalAction(this.HardwareMonitor.GetType(),
                App.Setting.TimeSignalGeneratorSetting.HardwareDetectionPerTimeIntervalMS,
                this.HardwareMonitor.RefreshStatus);
            #region 测试执行快速的连续获取药机状态,以确定信号传输的稳定性和测试获取药机状态时是否对正常的其他信号有影响
            //timeSignalGenerator.RegisterIntervalAction(this.HardwareMonitor.GetType(),
            //   53,
            //   this.HardwareMonitor.RefreshStatus);
            #endregion

            timeSignalGenerator.RegisterIntervalAction(this.SoftwarePartManager.GetType(),
                            App.Setting.TimeSignalGeneratorSetting.SoftwarePartDetectionPerTimeIntervalMS,
                            this.SoftwarePartManager.RefreshStatus);

            timeSignalGenerator.RegisterIntervalAction(this.MedicineMonitor.GetType(),
                            App.Setting.TimeSignalGeneratorSetting.MedicineExpirationCheckingPerTimeIntervalMS,
                            this.MedicineMonitor.CheckMedicinesExpiration);

            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            this.Start();
        }

        public void Dispose()
        {
            if (this.timeSignalGenerator!= null)
            {
                this.timeSignalGenerator.UnRegisterIntervalAction(typeof(HardwareMonitor));
                this.timeSignalGenerator.UnRegisterIntervalAction(typeof(SoftwarePartMonitor));
                this.timeSignalGenerator.UnRegisterIntervalAction(typeof(MedicineMonitor));
                this.HardwareMonitor = null;
                this.SoftwarePartManager = null;
                this.MedicineMonitor = null;
            }
            this.Stop();
            if (this.bw != null)
            {
                bw.DoWork -= this.bw_DoWork;
                bw.ProgressChanged -= this.bw_ProgressChanged;
                bw.RunWorkerCompleted -= this.bw_RunWorkerCompleted;
                bw.Dispose();
            }
        }


        #region 全局变量
        BackgroundWorker bw = new BackgroundWorker();
        #endregion
        #region 线程的回调函数
        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (bw.CancellationPending == false)
            {
                if (App.LogServerServicePipeClient!= null)
                {
                    //string msg = string.Format("this is my heartbeats my time is {0}", DateTime.Now.Ticks);
                    EasyNamedPipe.PipeMessage_Heartbeats msg = new EasyNamedPipe.PipeMessage_Heartbeats() { 
                         HeartbeatsTime = DateTime.Now, PackageType=  EasyNamedPipe.PipeMessagePackageType.Heartbeats, OtherMessage = null, SendTime = DateTime.Now, ValidTimeMS = 10000
                    };
                    try
                    {
                        App.LogServerServicePipeClient.Send(msg);
                        ///注意每次心跳的间隔时间不能等于或者大于服务器检测心跳有效的时间.比如这里设定的是4000毫秒,而服务器每次心跳的有效期是5000毫秒
                        ///如果不提前发的话,服务器没检查到心跳(浪费到传输上的时间等),就会把这个app关掉,导致程序不稳定一直被关闭.
                    }
                    catch (Exception err)
                    {
                        if (App.DebugCommandServer!= null && App.DebugCommandServer.DebuggerConnected)
                        {
                            Utils.LogError("发送心跳包发生错误:", err.Message, msg);
                        }
                    }
                    System.Threading.Thread.Sleep(App.Setting.LogServerServiceSetting.PerHeartbeatsDelayMS);
                }
                else
                {
                    System.Threading.Thread.Sleep(17);//还没有完全启动LogServerService? 通常这是不可能的,因为本类(StatusCheckAndReportor) 的初始化应该是在初始化任务的最后的,这时候管道应该已经初始化了.
                }
                #region 使用循环多久执行一次任务的模式,如果使用这种模式,可以处理更多的需要时间间隔的任务,如果不使用,就只能没隔多久执行一次任务
                //if ((DateTime.Now - lastHeartBeatsTime).TotalMilliseconds >  this.perHeartbeatsDelayMS)
                //{
                //    //需要发送一次心跳了
                //}
                //else
                //{
                //    //休息一下
                //}
                #endregion
            }
        }
        #endregion
        #region 启动
        bool Start()
        {
            if (this.bw.IsBusy == false)
            {
                this.bw.RunWorkerAsync();
                return true;
            }
            return false;
        }
        #endregion
        #region 停止
        bool Stop()
        {
            if (this.bw != null && this.bw.IsBusy == true)
            {
                this.bw.CancelAsync();
                return true;
            }
            return false;
        }
        #endregion

        //void log(LogLevel level, string title, string message)
        //{
        //    if (App.LogServerServicePipeClient != null)
        //    {
        //        //string msg = string.Format("this is my heartbeats my time is {0}", DateTime.Now.Ticks);
                
        //        try
        //        {
        //            App.LogServerServicePipeClient.Log(level, title, message);
        //        }
        //        catch (Exception err)
        //        {
        //            if (App.DebugCommandServer != null && App.DebugCommandServer.DebuggerConnected)
        //            {
        //                Utils.LogError("MonitersManager中发送log消息错误:", err.Message, title,message);
        //            }
        //        }
        //        System.Threading.Thread.Sleep(App.Setting.LogServerServiceSetting.PerHeartbeatsDelayMS);
        //    }
        //    else
        //    {
        //        System.Threading.Thread.Sleep(17);//还没有完全启动LogServerService? 通常这是不可能的,因为本类(StatusCheckAndReportor) 的初始化应该是在初始化任务的最后的,这时候管道应该已经初始化了.
        //    }
        //}

        void reportMonitorDetectedError(object sender, MonitorDetectedErrorTypeEnum error)
        {
            #region 推送到控制面板
            if (error.ToString().StartsWith("硬件"))
            {
                if (App.ControlPanel != null)
                {
                    App.ControlPanel.SetNewMaintenanceStatus(MaintenanceStatusEnum.硬件故障);
                    App.AlertManager.AlertHardwareError("监测到硬件故障", error.ToString());
                }
            }
            else if (error.ToString().StartsWith("软件"))
            {
                if (App.ControlPanel != null)
                {
                    App.ControlPanel.SetNewMaintenanceStatus(MaintenanceStatusEnum.软件故障);
                    App.AlertManager.AlertSortwarePartError("监测到软件故障", error.ToString());
                }
            }
            #endregion
            #region 告诉其他关注事件的地方(比如前端页面)
            if (App.MonitorsManager.OnMonitorDetectedError != null)
            {
                App.MonitorsManager.OnMonitorDetectedError(sender, error);
            }
            #endregion
        }
        void reportMedicineObjectExpirationAlert(Dictionary<long, AMDM_MedicineObject__Grid__Medicine> medicines)
        {
            if (medicines == null || medicines.Count<1)
            {
                return;
            }
            #region 构建消息
            StringBuilder msgContent = new StringBuilder();
            //StringBuilder objIds = new StringBuilder();
            foreach (var item in medicines)
            {
                if (item.Value.ExpirationDate == null) continue;
                var days = (item.Value.ExpirationDate.Value - DateTime.Now).TotalDays;
                if (msgContent.Length > 0)
                {
                    msgContent.Append("\r\n");
                }
                //objIds.Append(item.Value.Id);
                msgContent.AppendFormat("在{0}号药槽的[{1}]剩余有效期{2}天", item.Value.IndexOfStock + 1, item.Value.Name, days);
            }
            #endregion
            App.AlertManager.AlertMedicineExpiration(medicines, "有效期预警", msgContent.ToString());
            #region 告诉其他关注事件的地方(比如前端页面)
            if (this.OnMedicineObjectExpirationAlert != null)
            {
                this.OnMedicineObjectExpirationAlert(medicines);
            }
            #endregion
        }
        void reportMedicineCountAlert(Dictionary<long, AMDM_MedicineInventory> medicinesInventory)
        {
            if (medicinesInventory == null && medicinesInventory.Count<1)
            {
                return;
            }
            #region 构建消息
            StringBuilder msgContent = new StringBuilder();
            foreach (var item in medicinesInventory)
            {
                if (item.Value == null) continue;
                if (msgContent.Length > 0)
                {
                    msgContent.Append("\r\n");
                }
                msgContent.AppendFormat("药品[{0}]的当前库存{1}", item.Value.Name + 1, item.Value.Count);
            }
            #endregion
            App.AlertManager.AlertMedicineInventory(medicinesInventory, "库存预警", msgContent.ToString());
            #region 告诉其他关注事件的地方(比如前端页面)
            if (this.OnMedicineCountAlert!= null)
            {
                this.OnMedicineCountAlert(medicinesInventory);
            }
            #endregion
        }

        /// <summary>
        /// 当检测器检测到故障的时候,哪个地方注册了这个方法哪个地方就能收到消息.
        /// </summary>
        public event OnMonnitorDetectedErrorEventHandler OnMonitorDetectedError;
        public event OnMedicineObjectExpirationAlertEventHandler OnMedicineObjectExpirationAlert;
        public event OnMedicineCountAlertEventHandler OnMedicineCountAlert;
    }
}
