using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM.Manager
{

    /// <summary>
    /// 药仓内的PLC信息的交互器
    /// </summary>
    public class PLCCommunicator4Stock_台达 : PLCCommunicatorTD, IDisposable, IPLCCommunicator4Stock
    {
        PLCCommunicator4Main_台达 mainPlc = null;
        #region 外部公共变量
        /// <summary>
        /// 当前药仓的plc取药是否忙线中
        /// </summary>
        public bool IsBusy { get { return this.medicineGettingWaiterBW.IsBusy; } set { } }

        /// <summary>
        /// 是否已经全部完成
        /// </summary>
        public bool AllMedicineGettingFinished { get; set; }

        /// <summary>
        /// 哪个药仓
        /// </summary>
        public int StockIndex { get; set; }
        #endregion

        #region 全局变量
        float centerDistanceBetweenTwoGrabbers, xOffsetFromStartPointMM, yOffsetFromStartPointMM;
        BackgroundWorker medicineGettingWaiterBW = new BackgroundWorker();
        /// <summary>
        /// 每次获取plc状态的超时时间
        /// </summary>
        float perTimeGetStatusTimeoutMS = 200;
        /// <summary>
        /// plc连接超时时间
        /// </summary>
        int plcConnectTimeoutMS = 2000;
        /// <summary>
        /// 当plc单次连接失败后,多久开始再一次连接plc
        /// </summary>
        int reConnectPLCSleepMS = 100;
        /// <summary>
        /// 单个药品取药的超时时间
        /// </summary>
        int medicineGettingTimeoutMS = 60000;

        int medicineRecyleTimeoutMS = 60000;
        bool allMedicineGetttingSuccess = true;
        int afterOneMedicineGettingSleepMS = 100;
        /// <summary>
        /// 外部可以获取到的全局变量,取药过程中发生的错误.当新的任务到来的时候要重置这个状态为null
        /// </summary>
        public Nullable<StockMedicinesGettingErrorEnum> Error { get; set; }

        //PLCCommunicatorTD plc = null;
        //StockPLCSettingTD setting = null;
        float perMMPulse = 80;

        //每次重复获取状态信号的间隔时间
        int reGettingStatusPerTimeDelay = 217;
        #endregion

        #region 析构函数
        public void Dispose()
        {
            this.medicineGettingWaiterBW.DoWork -= this.medicineGettingWaiterBW_DoWork;
            this.medicineGettingWaiterBW.ProgressChanged -= this.medicineGettingWaiterBW_ProgressChanged;
            this.medicineGettingWaiterBW.RunWorkerCompleted -= this.medicineGettingWaiterBW_RunWorkerCompleted;
            this.medicineGettingWaiterBW.Dispose();
        }
        #endregion
        #region 构造函数
        /// <summary>
        /// 初始化plc连接器,主要是用来初始化每个药仓中的plc,让他们能够接收到位置信息.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="centerDistanceBetweenTwoGrabbers"></param>
        /// <param name="xOffsetFromStartPointMM"></param>
        /// <param name="yOffsetFromStartPointMM"></param>
        public PLCCommunicator4Stock_台达(int stockIndex,
            StockPLCSettingTD setting,
            float centerDistanceBetweenTwoGrabbers,
            float xOffsetFromStartPointMM,
            float yOffsetFromStartPointMM,
            PLCCommunicator4Main_台达 mainPLC = null
            ):base(setting, mainPLC)
        {
            this.setting = setting;
            this.perMMPulse = setting.PerMMPulseCount;
            this.StockIndex = StockIndex;
            if (mainPLC != null)
            {
                this.mainPlc = mainPLC;
                //this.plc = mainPLC.plc;
            }
            this.centerDistanceBetweenTwoGrabbers = centerDistanceBetweenTwoGrabbers;
            this.xOffsetFromStartPointMM = xOffsetFromStartPointMM;
            this.yOffsetFromStartPointMM = yOffsetFromStartPointMM;

            this.medicineGettingWaiterBW.WorkerSupportsCancellation = true;
            this.medicineGettingWaiterBW.WorkerReportsProgress = true;
            this.medicineGettingWaiterBW.DoWork += medicineGettingWaiterBW_DoWork;
            this.medicineGettingWaiterBW.ProgressChanged += medicineGettingWaiterBW_ProgressChanged;
            this.medicineGettingWaiterBW.RunWorkerCompleted += medicineGettingWaiterBW_RunWorkerCompleted;
        }
        #endregion

        #region 取药任务的启动
        /// <summary>
        /// 开始循环取药
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        //public bool StartMedicinesGetting(List<MedicineGettingSubTask> tasks)
        //{
        //    if (this.medicineGettingWaiterBW.IsBusy == true)
        //    {
        //        Console.WriteLine("当前药仓取药任务正在进行中");
        //        return false;
        //    }
        //    var groupTasks = SubTasks2GroupTasks(tasks);
        //    this.medicineGettingWaiterBW.RunWorkerAsync(groupTasks);
        //    return true;
        //}
        public bool StartMedicinesGetting(List<MedicinesGroupGettingTask> groupTasks)
        {
            if (this.medicineGettingWaiterBW.IsBusy == true)
            {
                Console.WriteLine("当前药仓取药任务正在进行中");
                return false;
            }
            this.medicineGettingWaiterBW.RunWorkerAsync(groupTasks);
            return true;
        }

        #endregion

        #region 强制清空药槽2021年11月30日14:28:20  一直发送给某个药槽取药的任务,直到他抛出300的异常以后,检测光电获取到的抓取信号数量,确认清空动作执行的时候一共清出来多少盒药品
        /// <summary>
        /// 开始进行药槽清空动作.第一参数是目标药槽,第二参数是一共要最多执行多少次药品的抓取测试,直接传入格子的最大装载数量.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="maxTimes"></param>
        /// <returns></returns>
        public bool StartGridClear(AMDM_Grid grid, int maxTimes, Action<int> onClearFinished)
        {
            if (this.medicineGettingWaiterBW.IsBusy == true)
            {
                Console.WriteLine("当前药仓取药任务正在进行中");
                return false;
            }
            List<MedicinesGroupGettingTask> ts = new List<MedicinesGroupGettingTask>();
            //for (int i = 0; i < maxTimes; i++)
            //{
            //    MedicineGettingTask task = new MedicineGettingTask();
            //    task.Grid = grid;
            //    task.RecordDetailRef = null;
            //    task.Started = true;
            //    task.IsClearGridTask = true;
            //    ts.Add(task);
            //}
            MedicinesGroupGettingTask task = new MedicinesGroupGettingTask();
            task.Grid = grid;
            task.RecordDetailRef = null;
            task.Started = true;
            task.IsClearGridTask = true;
            ts.Add(task);
            //MedicineGettingTask task = new MedicineGettingTask();
            ////task.
            //task.Grid = grid;
            //task.RecordDetailRef = null;
            //task.Started = true;
            //task.IsClearGridTask = true;
            //task.Count = maxTimes;
            //ts.Add(task);
            this.OnClearGridFinished = onClearFinished;
            this.medicineGettingWaiterBW.RunWorkerAsync(ts);
            return true;
        }
        /// <summary>
        /// 当清空药槽的时候,清空完毕以后的回调函数
        /// </summary>
        Action<int> OnClearGridFinished = null;
        int gridClearedTimes = 0;
        #endregion

        #region 取药任务的终止
        /// <summary>
        /// 终止取药
        /// </summary>
        public void AbortMedicinesGetting()
        {
            this.medicineGettingWaiterBW.CancelAsync();
        }
        #endregion

        

        #region 取药任务的异步处理
        void medicineGettingWaiterBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.AllMedicineGettingFinished = this.allMedicineGetttingSuccess;
            if (OnClearGridFinished == null)
            {
                if (this.OnAllMedicinesGettingStoped != null)
                {
                    this.OnAllMedicinesGettingStoped(this.StockIndex, allMedicineGetttingSuccess, this.Error, e.Cancelled, null);
                }
            }
            //药槽清空动作
            else
            {
                OnClearGridFinished(this.gridClearedTimes);
                //执行完了回调以后,释放
                this.gridClearedTimes = 0;
                this.OnClearGridFinished = null;
            }
            #region 如果使用的不是主PLC的串口,关闭串口
            if (App.Setting.PlcSetting_台达.UseMainPLCSerialPort == false)
            {

                if (this.Disconnect())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("当前药仓已经完成线程任务,关闭PLC的连接完成");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("当前药仓已经完成线程任务,关闭PLC的连接失败!");
                    Console.ResetColor();
                }
            }
            #endregion
        }

        void medicineGettingWaiterBW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //启动中
            if (e.ProgressPercentage == 0)
            {
                MedicinesGroupGettingTask task = e.UserState as MedicinesGroupGettingTask;
                if (this.OnOneClipGettingStarting != null)
                {
                    this.OnOneClipGettingStarting(task);
                }
            }
            //已启动
            else if (e.ProgressPercentage == 1)
            {
                MedicinesGroupGettingTask task = e.UserState as MedicinesGroupGettingTask;
                if (this.OnOneClipGettingStarted != null)
                {
                    this.OnOneClipGettingStarted(task);
                }
            }
            //已完成
            else if (e.ProgressPercentage == 100)
            {
                MedicinesGroupGettingTask task = e.UserState as MedicinesGroupGettingTask;
                if (this.OnOneClipGettingFinished != null)
                {
                    this.OnOneClipGettingFinished(task);
                }
            }
            ///单次取药发生故障
            else if (e.ProgressPercentage == -101)
            {
                //这里是发生错误的时候的单次单条记录的报送
                MedicinesGroupGettingTask task = e.UserState as MedicinesGroupGettingTask;
                Utils.LogInfo("故障信息通过Task的IsError确定是否出错,通过Error传递错误");
                if (this.OnOneClipGettingFinished != null)
                {
                    this.OnOneClipGettingFinished(task);
                }
            }
            ///传送带计数器光栅被长时间遮挡
            else if (e.ProgressPercentage == -100)
            {
                string msg = e.UserState as string;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
                if (e.UserState is MedicinesGroupGettingTask)
                {
                    MedicinesGroupGettingTask task = e.UserState as MedicinesGroupGettingTask;
                    if (this.OnOneClipGettingFinished != null)
                    {
                        this.OnOneClipGettingFinished(task);
                    }
                }
                else
                {
                    //this.onone
                }
            }
        }
        List<MedicinesGroupGettingTask> SubTasks2GroupTasks(List<MedicineGettingSubTask> subTasks)
        {
            #region 把子任务组创建成组合任务
            Dictionary<int, List<MedicineGettingSubTask>> subTasksDic = new Dictionary<int, List<MedicineGettingSubTask>>();
            List<MedicinesGroupGettingTask> groupTasks = new List<MedicinesGroupGettingTask>();
            foreach (var task in subTasks)
            {
                if (subTasksDic.ContainsKey(task.Grid.IndexOfStock) == false)
                {
                    var list = new List<MedicineGettingSubTask>();
                    subTasksDic.Add(task.Grid.IndexOfStock, list);
                    var groupTask = new MedicinesGroupGettingTask();
                    groupTask.SubTasks = list;
                    groupTasks.Add(groupTask);
                }
                subTasksDic[task.Grid.IndexOfStock].Add(task);
            }
            #endregion
            return groupTasks;
        }
        void medicineGettingWaiterBW_DoWork(object sender, DoWorkEventArgs e)
        {
            //清空当前的错误状态为空
            this.Error = null;
            List<MedicinesGroupGettingTask> groupTasks = e.Argument as List<MedicinesGroupGettingTask>;
            if (!ConnectPLC())
            {
                return;
            }
            if (!CheckPLCStatusCanContinue())
            {
                return;
            }
            #region 依次发送取药任务,发送完以后循环获取,等待完成以后继续发送下一次取药任务
            //完成了取药的次数
            int finishedGettingTimes = 0;
            while (this.medicineGettingWaiterBW.CancellationPending == false && groupTasks.Count > 0)
            {
                Console.WriteLine("开始一次取药,当前任务数量:{0}", groupTasks.Count);
                MedicinesGroupGettingTask currentTask = groupTasks[0];
                //发送准备启动方法
                this.medicineGettingWaiterBW.ReportProgress(0, currentTask);
                //调用计算和发送取药信号方法
                System.Threading.Thread.Sleep(2000);
                MathAndSendMedicineGettingSignal(currentTask);
                currentTask.Started = true;
                //发送已启动方法,和准备启动方法的区别是 发送给plc启动信号有没有完成
                this.medicineGettingWaiterBW.ReportProgress(1, currentTask);
                if (!WaitMedicineGettingResultSignal(ref finishedGettingTimes, ref  currentTask))
                {
                    if (currentTask.IsClearGridTask)
                    {
                        this.medicineGettingWaiterBW.ReportProgress(100, currentTask);
                    }
                    #region 普通动作直接报送失败
                    else
                    {
                        //报送不成功记录
                        currentTask.IsError = true;
                        currentTask.ErrorType = this.Error == null ? StockMedicinesGettingErrorEnum.Unknown : this.Error.Value;
                        currentTask.ErrMsg = this.Error == null ? "未捕获到的未知错误" : this.Error.Value.ToString();
                        this.medicineGettingWaiterBW.ReportProgress(-101, currentTask);
                    }
                    #endregion
                    return;
                }

                //这里应该是获取药品获取信号是没问题的 也没有err信号  执行到这里时 如果是清空药槽操作的话 要继续.
                if (currentTask.IsClearGridTask)
                {
                    continue;
                }

                #region 这个时候 status就是已经交付单个药品完成的状态 如果后面还有药 清空状态后继续发送其他的取药信号
                currentTask.Finished = true;
                groupTasks.Remove(currentTask);
                Console.WriteLine("移除取药任务,当前取药任务数量:{0}", groupTasks.Count);
                //报送成功记录
                this.medicineGettingWaiterBW.ReportProgress(100, currentTask);
                #endregion
                //SendClearMainStatusWaitNextMedicineGettingCommand();
                if (groupTasks.Count > 0)
                {//如果后面还有药要取,延迟一定的时间再进行下次取药
                    System.Threading.Thread.Sleep(afterOneMedicineGettingSleepMS);
                }
            }
            #endregion

            allMedicineGetttingSuccess = groupTasks.Count == 0;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("单个plc的所有药品取药完成");
            Console.ResetColor();
        }

        #region 连接PLC

        /// <summary>
        /// 连接PLC,如果不能继续,返回false
        /// </summary>
        /// <returns></returns>
        bool ConnectPLC()
        {
            bool canContinue = true;
            bool plcConnected = false;
            #region 连接plc
            while (this.medicineGettingWaiterBW.CancellationPending == false)
            {
                try
                {
                    if (Connect())
                    {
                        Console.WriteLine("PLC连接成功");
                        plcConnected = true;
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(this.reConnectPLCSleepMS);
                        continue;
                    }
                }
                catch (Exception connectPLCError)
                {
                    Console.WriteLine("连接plc失败:{0}", connectPLCError.Message);
                    continue;
                }
            }
            if (plcConnected == false)
            {
                Console.WriteLine("PLC连接超时");
                this.Error = StockMedicinesGettingErrorEnum.PLC连接超时;
                canContinue = false;
            }
            #endregion
            return canContinue;
        }
        #endregion

        

        #region 计算和发送取药信号
        /// <summary>
        /// 计算和发送取药信号
        /// </summary>
        /// <param name="currentTask"></param>
        void MathAndSendMedicineGettingSignal(MedicinesGroupGettingTask currentTask)
        {
            #region 计算要走的位置并发送取药信号
            float xpos = currentTask.Grid.LeftMM + (currentTask.Grid.RightMM - currentTask.Grid.LeftMM) / 2;
            float ypos = currentTask.Grid.BottomMM;
            WhichGrabberEnum grabber = (currentTask.Grid.IndexOfFloor >= 5) ? WhichGrabberEnum.Far : WhichGrabberEnum.Near;
            #endregion
            #region 发送取药信号
            //取药次数
            int times = currentTask.Count;

            if (currentTask.IsClearGridTask)
            {
                times = 50;
            }


            #region 如果是特殊药槽取药
            if (currentTask.Grid.FloorIndex < 0)
            {
                this.SendStartMedicineGettingCommand(currentTask.Grid.FloorIndex, currentTask.Grid.IndexOfFloor, times, currentTask.Grid.IndexOfStock);
            }
            #endregion
            #region 一般药槽的取药
            else
            {
                this.SendStartMedicineGettingCommand(xpos, ypos, grabber, times, currentTask.Grid.IndexOfStock);
            }
            #endregion
            #endregion

            Console.WriteLine(string.Format("药仓:{0}即将到药槽:{1}-{2}取药", currentTask.Grid.StockIndex + 1,
                currentTask.Grid.FloorIndex < 0 ? currentTask.Grid.FloorIndex : (currentTask.Grid.FloorIndex + 1),
                currentTask.Grid.IndexOfFloor + 1));
        }
        #endregion

        PLCStatusData GetMedicineGettingStatus()
        {
            if (this.mainPlc!= null)
            {
                return mainPlc.GetMedicineGettingStatus();
            }
            return null;
        }

        bool CheckPLCStatusCanContinue()
        {
            #region 获取plc当前的状态是否是空,如果不是空就不能取药了
            PLCStatusData currentPlcStatus = this.GetMedicineGettingStatus();
            if (currentPlcStatus == null)
            {
                Console.WriteLine("获取当前药机状态失败,请稍后再试");
                //this.Error = StockMedicinesGettingErrorEnum.获取PLC状态失败;
                return false;
            }
            if (currentPlcStatus.CounterCoverdError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                string msg = "传送带计数器光栅被长时间遮挡!!!!!";
                Console.WriteLine();
                Console.ResetColor();
                this.allMedicineGetttingSuccess = false;
                this.medicineGettingWaiterBW.ReportProgress(-100, msg);
                this.Error = StockMedicinesGettingErrorEnum.传送带计数器光栅传感器被长时间遮挡;
                return false;
            }
            if (currentPlcStatus.Main != MainBufferValuesEnum.Empty)
            {
                Console.WriteLine("当前药仓忙碌状态,不能取药");
                this.Error = StockMedicinesGettingErrorEnum.药仓忙碌时发送的取药信号不能被处理;
                return false;
            }
            #endregion
            return true;
        }

        #region 等待取药完成信号
        bool WaitMedicineGettingResultSignal(
            //ref int finishedGettingTimes, 
            ref int finishedGettingCount,
            //ref MedicineGettingTask task
            ref MedicinesGroupGettingTask taskGroup
            )
        {
            bool thisTimeMedicineGettingTimeout = false;
            bool thisTimeMEdicineGettingError = false;
            PLCStatusData status = null;
            //本次取药的开始时间
            DateTime thisTimeMedicinesGettingStartTime = DateTime.Now;

            //发送完了取药信号以后,就立刻开始检查是否已经掉了药品
            while (this.medicineGettingWaiterBW.CancellationPending == false)
            {
                #region 循环获取药机的返回信号
                DateTime now = DateTime.Now;
                TimeSpan cost = (now - thisTimeMedicinesGettingStartTime);
                if (cost.TotalMilliseconds > this.medicineGettingTimeoutMS)
                {
                    Console.WriteLine("单次取药超时");
                    this.Error = StockMedicinesGettingErrorEnum.单次取药已超过最大等待时间;
                    break;
                }
                status = this.GetMedicineGettingStatus();
                if (status != null)
                {
                    if (status.Main == MainBufferValuesEnum.FinishedDeliveryOneMedicine)
                    {
                        thisTimeMedicineGettingTimeout = false;
                        thisTimeMEdicineGettingError = false;
                        break;
                    }
                    else if (status.Main == MainBufferValuesEnum.Error)
                    {
                        ///300错误
                        thisTimeMedicineGettingTimeout = false;
                        thisTimeMEdicineGettingError = true;
                        this.Error = StockMedicinesGettingErrorEnum.机械手多次取药后未检测到药品掉落;
                        if (this.OnClearGridFinished != null)
                        {
                            this.gridClearedTimes = status.CounterGetedMedicinesCount[this.StockIndex];
                        }
                        break;
                    }
                    else if (status.CounterCoverdError == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("传送带计数器光栅被长时间遮挡");
                        Console.ResetColor();
                        thisTimeMedicineGettingTimeout = false;
                        thisTimeMEdicineGettingError = true;
                        this.Error = StockMedicinesGettingErrorEnum.传送带计数器光栅传感器被长时间遮挡;
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(reGettingStatusPerTimeDelay);
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("获取到了空的plc状态,延迟后继续获取");
                    System.Threading.Thread.Sleep(reGettingStatusPerTimeDelay);
                    continue;
                }
                #endregion
            }
            #region 如果单个药品取药超时了,或者获取到了错误信号.退出
            if (thisTimeMEdicineGettingError)
            {
                Utils.LogError("取药发生错误:", this.Error.Value.ToString());
                return false;
            }
            if (thisTimeMedicineGettingTimeout)
            {
                Console.WriteLine("取药超时,超过了最大单次取药时间限度{0}毫秒", this.perTimeGetStatusTimeoutMS);
                this.Error = StockMedicinesGettingErrorEnum.单次取药已超过最大等待时间;
                return false;
            }
            #endregion


            #region 如果没有获取到status 说明已经掉线了之类的
            if (status == null)
            {
                Utils.LogError("在等待药品获取结果的时候,没有读取到status");
                return false;
            }
            #endregion


            #region 如果是清空药槽的任务的话 不检测光栅数量信息
            if (taskGroup.IsClearGridTask)
            {
                return true;
            }
            #endregion

            #region 但 如果当前任务不是清空药槽任务,那就要检测一下 要取的数量和任务数量是否一致 检测当前取到的药品的数量 跟光栅上的数量是否一致
            if (status.CounterGetedMedicinesCount.ContainsKey(this.StockIndex)
                && status.CounterGetedMedicinesCount[this.StockIndex] != taskGroup.Count
                //&& status.CounterGetedMedicinesCount[this.StockIndex] != finishedGettingTimes
                )
            {
                thisTimeMEdicineGettingError = true;
                //Console.ForegroundColor = ConsoleColor.Red;
                string msg = string.Format("光电计数器检测到掉落的药品数量为:{0},实际当前应完成的取药数量为:{1}", status.CounterGetedMedicinesCount, finishedGettingCount);
                taskGroup.ErrMsg = msg;
                //Console.WriteLine(msg);
                this.medicineGettingWaiterBW.ReportProgress(-100, taskGroup);
                allMedicineGetttingSuccess = false;
                this.medicineGettingWaiterBW.CancelAsync();
                Console.ResetColor();
                this.Error = StockMedicinesGettingErrorEnum.光电计数与实际已抓取药品次数不符;
                return false;
            }
            else if (status.CounterGetedMedicinesCount.ContainsKey(this.StockIndex))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("光电检测掉落药品数量与实际已发送取药数量一致,均为:{0}", status.CounterGetedMedicinesCount[this.StockIndex]);
                Console.ResetColor();
                //finishedGettingTimes++;
                finishedGettingCount += status.CounterGetedMedicinesCount[this.StockIndex];
            }
            #endregion
            return true;
        }
        #endregion
        #endregion



        #region 内部函数 计算x 和y轴的脉冲数
        float GetXPulseCount(float mmCount, WhichGrabberEnum grabber)
        {
            float ret = (mmCount + this.xOffsetFromStartPointMM
                - (grabber == WhichGrabberEnum.Far ? this.centerDistanceBetweenTwoGrabbers : 0)
                ) * this.perMMPulse;// setting.PerMMPulseCount;
            return ret;
        }
        float GetYPulseCount(float mmCount)
        {
            return (mmCount + this.yOffsetFromStartPointMM) * this.perMMPulse;// setting.PerMMPulseCount;
        }
        /// <summary>
        /// 用远点抓手时候的x轴偏移量
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="grabber"></param>
        /// <returns></returns>
        float GetXOffsetWhenUsingFarGrabber(float centerDistanceBetweenTwoGrabbers, WhichGrabberEnum grabber)
        {
            float ret = 0;
            switch (grabber)
            {
                case WhichGrabberEnum.Near:
                    //用近处的手的时候不需要偏移量
                    break;
                case WhichGrabberEnum.Far:
                    //用远处的手的时候需要用到抓手一半距离的偏移量(减少)
                    //ret = centerDistanceBetweenTwoGrabbers / 2;
                    ret = -centerDistanceBetweenTwoGrabbers;
                    break;
                default:
                    break;
            }
            return ret;
        }
        #endregion

        #region 外部调用的公共函数
        /// <summary>
        /// 获取药机当前是否正在付药的状态
        /// </summary>
        /// <returns></returns>



        /// <summary>
        /// 发送清空main开关等待下一次取药命令,不会清空位置数据
        /// </summary>
        /// <returns></returns>
        protected bool SendClearMainStatusWaitNextMedicineGettingCommand()
        {
            if (Connected == false)
            {
                Console.WriteLine("plc未连接 不能发送数据");
                return false;
            }
            List<ushort> values = new List<ushort>() { (ushort)MainBufferValuesEnum.Empty };
            WriteMultipleRegisters(values.ToArray());
            return true;
            //return plc.Write<int>(setting.PLCDataIndex.Main, MainBufferValuesEnum.Empty);
        }


        /// <summary>
        /// 发送药机需要开始取药命令(使用机械手)
        /// </summary>
        /// <returns></returns>
        protected bool SendStartMedicineGettingCommand(float xMM, float yMM, WhichGrabberEnum grabber, int times, int gridIndexOfStock)
        {
            //Console.WriteLine(  "发送取药信号内容:xMM:{0} yMM:{1} grabber:{2} times:{3}, setting:{4}\r\n\r\n\r\n",xMM, yMM, grabber, times, JsonConvert.SerializeObject(setting));
            float yMMPulse = GetYPulseCount(yMM);
            float xMMPulse = GetXPulseCount(xMM, grabber);

            ushort location = (ushort)(this.setting.PuttingBufferStartIndex + this.StockIndex + 1);

            List<ushort> buffer = new List<ushort>();
            //100
            buffer.Add(location);
            //101 102是y坐标
            uint yMMPulseUI = Convert.ToUInt32(yMMPulse);
            uint highY = yMMPulseUI >> 16 & 0xffff;
            uint lowY = yMMPulseUI & 0xffff;
            //buffer.Add((ushort)highY);
            buffer.Add((ushort)lowY);
            buffer.Add((ushort)highY);
            //103 104是x坐标
            uint xMMPluseUI = Convert.ToUInt32(xMMPulse);
            uint highX = xMMPluseUI >> 16 & 0xffff;
            uint lowX = xMMPluseUI & 0xffff;
            //buffer.Add((ushort)highX);
            buffer.Add((ushort)lowX);
            buffer.Add((ushort)highX);
            //105是哪个机械手
            buffer.Add((ushort)grabber);
            //106是取药数量
            buffer.Add((ushort)times);
            //107是药仓上方显示的数码管的数据
            //buffer.Add((ushort)((this.StockIndex + 1) * 1000 + gridIndexOfStock + 1));
            WriteMultipleRegisters(buffer.ToArray(), 0);
            #region 发送后再次读取同样位置的缓冲区

            ushort[] readBuffer = new ushort[buffer.Count];
            readBuffer = ReadHoldingRegisters((ushort)readBuffer.Length, (this.setting.PuttingBufferStartIndex - this.setting.GettingBufferStartIndex));

            if (readBuffer == null)
            {
                Utils.LogError("发送后立刻读取缓冲区内容没有读取到");
            }
            else
            {
                StringBuilder readSB = new StringBuilder();
                foreach (ushort u in readBuffer)
                {
                    string c = u.ToString("X2");
                    readSB.Append(c);
                    readSB.Append(" ");
                }
                Utils.LogInfo("发送后读取缓冲区:", readSB);
            }

            #endregion
            return true;
        }
        /// <summary>
        /// 发送药机需要开始取药命令(使用特殊药槽)
        /// </summary>
        /// <param name="spacilFloorIndex"></param>
        /// <param name="spacilGridIndex"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        protected bool SendStartMedicineGettingCommand(int spacilFloorIndex, int spacilGridIndex, int times, int gridIndexOfStock)
        {
            if (Connected == false)
            {
                Console.WriteLine("plc未连接 不能发送数据");
                return false;
            }
            ushort location = (ushort)(Math.Abs(spacilFloorIndex) * 1000 + this.StockIndex + 1);
            List<ushort> buffer = new List<ushort>();
            //100
            buffer.Add(location);
            //101 102是y坐标
            buffer.Add(0);
            buffer.Add(0);
            //103 104是x坐标
            buffer.Add((ushort)(spacilGridIndex + 1));
            buffer.Add(0);
            //105是哪个机械手
            buffer.Add(0);
            //106是取药数量
            buffer.Add((ushort)times);
            //107是药仓上方显示的数码管的数据
            //buffer.Add((ushort)((this.StockIndex + 1) * 1000 + gridIndexOfStock + 1));

            WriteMultipleRegisters(buffer.ToArray());
            return true;
        }

        /// <summary>
        /// 发送取药机抓手位置测试命令(使用机械手)
        /// </summary>
        /// <returns></returns>
        protected bool SendGrabberPositioningTestCommand(float xMM, float yMM, WhichGrabberEnum grabber, int times)
        {
            Utils.LogStarted("开始执行测试使用机械手取药");
            return SendStartMedicineGettingCommand(xMM, yMM, grabber, times, 0);
        }
        /// <summary>
        /// 发送取药机抓手位置测试命令(使用特殊药槽)
        /// </summary>
        /// <param name="spacilFloorIndex"></param>
        /// <param name="spacilGridIndex"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        protected bool SendGrabberPositioningTestCommand(int spacilFloorIndex, int spacilGridIndex, int times)
        {
            Utils.LogStarted("开始执行测试使用特殊药槽取药");
            return SendStartMedicineGettingCommand(spacilFloorIndex, spacilGridIndex, times, 0);
        }
        #endregion

        #region 回调函数
        /// <summary>
        /// 当所有的药品已经取完或者是取药失败了或者是取药终止了的时候发生的回调
        /// </summary>
        public event OnStock_MedicinesGettingStopedEventHandler OnAllMedicinesGettingStoped;
        /// <summary>
        /// 当单品药品取完时候发生的回调,没调用一次这个函数说明已经完成一盒药的取药 而不一定是一种.
        /// </summary>
        public event OnStock_OneMedicineGettingFinishedEventHandler OnOneMedicineGettingFinished;
        public event OnStock_OneClipGettingFinishedEventHandler OnOneClipGettingFinished;

        public event OnStock_OneMedicineGettingStartingEventHandler OnOneMedicineGettingStarting;
        public event OnStock_OneClipGettingStartingEventHandler OnOneClipGettingStarting;

        public event OnStock_OneMedicineGettingStartedEventHandler OnOneMedicineGettingStarted;
        public event OnStock_OneClipGettingStartedEventHandler OnOneClipGettingStarted;

        /// <summary>
        /// 当单个药仓取药时发生了错误时触发 2022年2月13日14:32:46 不再使用,finished的回调函数里面就包含了错误信息
        /// </summary>
        //public event OnStock_GettingMedicineErrorEventHandler OnStock_GettingMedicinceError;
        #endregion


        
        public bool SendForceResetGrubbersCommand()
        {
            if (this.Connected == false)
            {
                Console.WriteLine("plc未连接 不能发送数据");
                return false;
            }
            //return this.plc.Write<int>(setting.PLCDataIndex.Main, MainBufferValuesEnum.FinishedDeliveryAll);
            List<ushort> values = new List<ushort>() { 900 };
            WriteMultipleRegisters(values.ToArray());
            //var writeRet = ReadHoldingRegisters(10, -100);
            return true;
        }

        public bool SendAllFinishedHaveARestCommand()
        {
            return true;
        }
    }
}
