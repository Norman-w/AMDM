using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
/*
 * 2021年11月12日16:45:18  所有plc的控制器
 * 里面应该包含一个主控plc和多个药仓的plc
 * 取药的时候 发送取药信号给主机plc,然后发送给要取药的药仓不同的plc不同的信号,告诉他们要去哪里取药. 都取药完成的时候告诉主控plc完成取药.
 * 当任何一个药仓的plc发生故障的时候通知主plc停止取药.
 * 2021年11月12日16:59:18 改名为 取药控制器
 * 
 * 
 */

namespace AMDM.Manager
{
    #region 取药控制器主类
    /// <summary>
    /// 药机所有plc的控制器,后来改名为 取药控制器
    /// </summary>
    public class MedicinesGettingController : IDisposable, IMedicinesGettingController
    {
        #region 公共全局变量
        public bool MedicinesGetting { get { return medicinesGetting; } set { } }
        /// <summary>
        /// 取药发生的故障
        /// </summary>
        public Nullable<AMDMMedicinesGettingErrorEnum> Error { get; set; }

        public Dictionary<int, List<StockMedicinesGettingErrorEnum>> SubErrors { get; set; }

        /// <summary>
        /// 主控PLC
        /// </summary>
        public PLCCommunicator4Main_台达 MainPLCCommunicator { get { return mainPLCCommunicator; }}
        #endregion
        #region 全局变量
        /// <summary>
        /// 当所有的药仓都已经完成了勾药,等待拍照的时间最长等待多久.
        /// </summary>
        int affterAllStockGettingWaitSnapshotTimeoutMS = 30000;

        /// <summary>
        /// 当等到了拍照信号以后 检测药品被取走的信号 这个信号等多久.
        /// </summary>
        int afterSnapshotWaitMedicinesHasBeenTakedTimeoutMS = 120000;
        /// <summary>
        /// 是否正在取药中  如果当前主控plc没有完成任务 一直都是在取药中.
        /// </summary>
        bool medicinesGetting = false;

        bool allStockMedicinesGettingFinished = false;

        /// <summary>
        /// 是否完成了拍照
        /// </summary>
        bool snapshotCaptured = false;

        /// <summary>
        /// 是否药品已经被完全取走;
        /// </summary>
        bool medicinesHasBeenTaked = false;
        /// <summary>
        /// 当前药单中所有需要使用的药仓和他们的任务集合
        /// </summary>
        //Dictionary<IPLCCommunicator4Stock, List<MedicineGettingSubTask>> currentOrderUsingStocksTasksDic = new Dictionary<IPLCCommunicator4Stock, List<MedicineGettingSubTask>>();

        /// <summary>
        /// 尝试重新获取主寄存器状态的间隔时间毫秒
        /// </summary>
        int reTryGettingStatusDelayMS = 323;


        AMDM_DeliveryRecord currentDeliveryRecord = null;
        //List<MedicineGettingTask> tasks = new List<MedicineGettingTask>();
        /// <summary>
        /// 跟plc进行信号交互时候的订单线程,所有的plc都在这一个里面轮询处理消息
        /// </summary>
        BackgroundWorker bw = new BackgroundWorker();

        /// <summary>
        /// 多个药仓的字典,key是药仓的索引位置 value是药仓的plc
        /// </summary>
        Dictionary<int, IPLCCommunicator4Stock> stocksPLCsDic = new Dictionary<int, IPLCCommunicator4Stock>();
        /// <summary>
        /// 获取指定药仓的plc连接器,主要是用于在清空药槽的时候使用.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPLCCommunicator4Stock GetStocksPLC(int index)
        {
            if (this.stocksPLCsDic.ContainsKey(index) == true)
            {
                return this.stocksPLCsDic[index];
            }
            return null;
        }
        /// <summary>
        /// 主控机的plc设置
        /// </summary>
        MainPLCSettingTD mainPlcSetting = null;

        /// <summary>
        /// 主控plc的交互器
        /// </summary>
        PLCCommunicator4Main_台达 mainPLCCommunicator;
        #endregion
        #region 初始化
        public bool Init(List<AMDM_Stock> stocks, bool useSinglePLCMode = true)
        {
            this.mainPlcSetting = App.Setting.PlcSetting_台达.MainPLC;
            #region 初始化主控PLC
            mainPLCCommunicator = new PLCCommunicator4Main_台达(this.mainPlcSetting);
	        #endregion
            //Console.WriteLine("主控PLC设置:\r\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(App.Setting.PlcSetting_台达.MainPLC, Newtonsoft.Json.Formatting.Indented));
            //mainPLCCommunicator = new PLCCommunicator4Main(this.mainPlcSetting);
            for (int i = 0; i < stocks.Count; i++)
            {
                AMDM_Stock currentStock = stocks[i];
                //this.stockAndPlcDic.Add(currentStock.IndexOfMachine, currentParam);
                PLCCommunicator4Stock_台达 plc = new PLCCommunicator4Stock_台达(
                    currentStock.IndexOfMachine,
                    App.Setting.PlcSetting_台达.StocksPLC[currentStock.IndexOfMachine], 
                    currentStock.CenterDistanceBetweenTwoGrabbers, 
                    currentStock.XOffsetFromStartPointMM, 
                    currentStock.YOffsetFromStartPointMM,
                    App.Setting.PlcSetting_台达.UseMainPLCSerialPort? mainPLCCommunicator : null
                    );

                //plc.OnOneMedicineGettingFinished += plc_OnStockOneMedicineGettingFinished;
                //plc.OnOneMedicineGettingStarting += plc_OnOneMedicineGettingStarting;
                //plc.OnOneMedicineGettingStarted += plc_OnOneMedicineGettingStarted;
                plc.OnOneClipGettingFinished += plc_OnStockOnClipGettingFinished;
                plc.OnOneClipGettingStarting += plc_OnOneClipGettingStarting;
                plc.OnOneClipGettingStarted += plc_OnOnClipGettingStarted;
                plc.OnAllMedicinesGettingStoped += plc_OnStock_AllMedicinesGettingStoped;
                //plc.OnStock_GettingMedicinceError += plc_OnStock_GettingMedicinceError;
                this.stocksPLCsDic.Add(currentStock.IndexOfMachine, plc);
            }
            
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            return true;
        }

        void plc_OnOnClipGettingStarted(MedicinesGroupGettingTask task)
        {
            //throw new NotImplementedException();
        }

        void plc_OnOneClipGettingStarting(MedicinesGroupGettingTask task)
        {
            long id = task.DeliveryRecordId;
            if (task.RecordDetailRef != null)
            {
                id = task.RecordDetailRef.Id;
            }
            #region 位置1
            if (App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos1!= null)
            {
                string destPathGrabber1 = getCameraDestFilePath(id, SnapshotLocationEnum.GrabbersArea1, DateTime.Now, "jpg");
                App.CameraSnapshotCapturer.CaptureSync(App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos1, destPathGrabber1, (img, path) =>
                {
                    Utils.LogFinished("取药即将开始,开始时机械手摄像位置1拍照任务已完成,存放位置:", path);
                });
                App.sqlClient.AddSnapshot(AMDM_Domain.SnapshotParentTypeEnum.DeliveryRecordDetail, id,
                    SnapshotTimePointEnum.BeforeAction,
               AMDM_Domain.SnapshotLocationEnum.GrabbersArea1
               , DateTime.Now, "已经开始取药触发机械手1镜头位拍照", destPathGrabber1);
            }
            #endregion

            #region 位置2
            if (App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos2!= null)
            {
                string destPathGrabber2 = getCameraDestFilePath(id, SnapshotLocationEnum.GrabbersArea2, DateTime.Now, "jpg");
                App.CameraSnapshotCapturer.CaptureSync(App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos2, destPathGrabber2, (img, path) =>
                {
                    Utils.LogFinished("取药即将开始,开始时机械手摄像位置2拍照任务已完成,存放位置:", path);
                });
                App.sqlClient.AddSnapshot(AMDM_Domain.SnapshotParentTypeEnum.DeliveryRecordDetail, id,
                    SnapshotTimePointEnum.BeforeAction,
               AMDM_Domain.SnapshotLocationEnum.GrabbersArea2
               , DateTime.Now, "已经开始取药触发机械手2镜头位拍照", destPathGrabber2);
            }
            
            #endregion
        }
        /// <summary>
        /// 获取文件的路径
        /// </summary>
        /// <returns></returns>
        string getCameraDestFilePath(long parentDeliveryRecordId, AMDM_Domain.SnapshotLocationEnum location, DateTime time, string imgType)
        {
            string filePath = string.Format("{0}\\{1}_{2}_{3}.{4}",
                App.Setting.DevicesSetting.CCTVCaptureSetting.SavingDictionary.TrimEnd('\\'),
                parentDeliveryRecordId,
                location.ToString(), time.Ticks,
                 imgType);
            return filePath;
        }
        #endregion

        #region 主plc的异步处理
        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 99)
            {
                this.OnAllMedicinesDeliveryFinished(this.currentDeliveryRecord);
            }
            else if(e.ProgressPercentage == 100)
            {
                this.OnMedicinesHasBeenTaked(this.currentDeliveryRecord);
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("主控PLC任务完成");
            if (this.allStockMedicinesGettingFinished)
            {
                suc("所有药仓已完成取药");
            }
            else
            {
                string err ="非所有药仓完成取药";
                
                fail(err);
            }
            if (this.snapshotCaptured)
            {
                suc("已完成拍照");
            }
            else
            {
                fail("未完成拍照");
            }
            if (this.medicinesHasBeenTaked)
            {
                suc("药品已经被用户取走");
            }
            else
            {
                fail("药品没有被用户取走");
            }
            this.medicinesGetting = false;

            if (this.Error != null)
            {
                Utils.LogError(this.Error.Value.ToString());
                this.OnMedicinesGettingError(this.Error.Value, this.SubErrors);
            }
            #region 关闭PLC的连接
            //if (mainPLCCommunicator.Disconnect() == false)
            //{
            //    fail("主控PLC断开连接失败");
            //}
            //else
            //{
            //    suc("主控PLC已断开连接");
            //}
            #region 关闭所有药仓的PLC的连接
            //foreach (var item in this.stocksPLCsDic)
            //{
            //    bool disConnectRet = item.Value.Disconnect();
            //    if (disConnectRet)
            //    {
            //        suc(string.Format("药仓{0}的PLC已断开连接", item.Key));
            //    }
            //    else
            //    {
            //        fail(string.Format("药仓{0}的PLC断开连接失败", item.Key));
            //    }
            //}
            #endregion
            #endregion
        }
        void suc(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        void fail(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 主控PLC线程启动后就等待取药完成信号
            //先等待一秒,让状态都可以完整的清空.然后再继续获取信息
            System.Threading.Thread.Sleep(3000);

            while (this.bw.CancellationPending == false)
            {
                //等待所有的药仓取药完成
                if (this.allStockMedicinesGettingFinished)
                {
                    break;
                }
                else if(this.Error != null)
                {//主控PLC刚要开始监听状态的时候,药仓中的PLC就触发了单个PLC的错误,所以就直接取消当前任务了.
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(17);
                }
            }


            #endregion

            #region 药仓已经完成取药以后 等待拍照信号

            Console.WriteLine("所有药仓的取药信完成信号已经收到.准备等待拍照信号");
            DateTime startGetPrintAndSnapshotCommandTime = DateTime.Now;
            //等待接收拍照和打印信号
            while (this.bw.CancellationPending == false)
            {
                if ((DateTime.Now - startGetPrintAndSnapshotCommandTime).TotalMilliseconds > this.affterAllStockGettingWaitSnapshotTimeoutMS)
                {
                    fail("所有药仓均已完成取药,等待拍照信号超时");
                    this.Error = AMDMMedicinesGettingErrorEnum.付药凭图截取信号获取超时;
                    return;
                }
                //等待顾客把药品取走
                PLCStatusData status = this.mainPLCCommunicator.GetMedicineGettingStatus();
                //Console.ForegroundColor = ConsoleColor.DarkYellow;
                //Console.WriteLine("主控plc获取拍照状态信号:");
                //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(status, Newtonsoft.Json.Formatting.Indented));
                //Console.ResetColor();
                if (status == null || status.PrintAndSnapshot == false)
                {
                    System.Threading.Thread.Sleep(reTryGettingStatusDelayMS);
                    continue;
                }
                if (status.PrintAndSnapshot == true)
                {
                    this.snapshotCaptured = true;
                    this.bw.ReportProgress(99);
                    break;
                }
            }

            #endregion
            #region 完成拍照后 等待药品被取走信号

            Console.WriteLine("完成所有药仓取药完成的函数的回调,当前应已记录出库并完成拍照,正在等待取药斗发送顾客已取走药品信号");
            DateTime startGetMedicinesHasBeenTakedCommandTime = DateTime.Now;
            while(this.bw.CancellationPending == false)
            {
                if ((DateTime.Now - startGetMedicinesHasBeenTakedCommandTime).TotalMilliseconds > this.afterSnapshotWaitMedicinesHasBeenTakedTimeoutMS)
                {
                    fail("所有药仓已经完成取药,已经完成拍照,等待顾客将药品取走信号超时");
                    this.Error = AMDMMedicinesGettingErrorEnum.等待药斗被清空信号获取超时;
                    return;
                }
                //等待顾客把药品取走
                PLCStatusData status = mainPLCCommunicator.GetMedicineGettingStatus();
                if (status == null || status.MedicinesHasBeenTaked == false)
                {
                    System.Threading.Thread.Sleep(reTryGettingStatusDelayMS);
                    continue;
                }
                if (status.MedicinesHasBeenTaked == true)
                {
                    this.medicinesHasBeenTaked = true;
                    this.bw.ReportProgress(100);
                    break;
                }
            }

            #endregion
            Console.WriteLine("当前药品已经被顾客取走,dowork结束,即将触发 complete");
        }
        #endregion

        #region 当plc那边返回取药状态以后
        /// <summary>
        /// 单个plc的所有取药任务都完成,注意 所有plc取药都完成的时候这个状态才是完成
        /// </summary>
        /// <param name="success"></param>
        /// <param name="canceled"></param>
        /// <param name="gettedMedicinesIndexAndCountDic"></param>
        void plc_OnStock_AllMedicinesGettingStoped(
            int stockIndex,
            bool success, 
            Nullable<StockMedicinesGettingErrorEnum> error, 
            bool canceled, 
            Dictionary<string, int> gettedMedicinesIndexAndCountDic
            )
        {
            Console.WriteLine("单个plc的所有取药任务完成success:{0}, canceled:{1}", success, canceled);
            bool allStockFinishedAndSuccess = true;
            foreach (var item in this.stocksPLCsDic)
            {
                if (item.Value.AllMedicineGettingFinished == false)
                {
                    allStockFinishedAndSuccess = false;
                    break;
                }
            }
            if (error != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                //记录当前的付药单已经发生了错误,然后终止所有的药品的交付
                //this.currentOrderUsingStocksTasksDic.Clear();
                Console.WriteLine("有药仓发生了取药错误,已清空当前付药单所需要使用的所有PLC和他们的待处理任务");
                AbortMedicinesGetting(true);
               
                Console.WriteLine("已经发送取消当前取药任务的命令");
                this.allStockMedicinesGettingFinished = true;
                this.Error = AMDMMedicinesGettingErrorEnum.分仓取药发生故障;
                if (this.SubErrors == null)
                {
                    this.SubErrors = new Dictionary<int, List<StockMedicinesGettingErrorEnum>>();
                }
                if (this.SubErrors.ContainsKey(stockIndex) == false)
                {
                    this.SubErrors.Add(stockIndex, new List<StockMedicinesGettingErrorEnum>());
                }
                this.SubErrors[stockIndex].Add(error.Value);

                if (App.inventoryManager.FinishDeliveryRecord(ref this.currentDeliveryRecord, false, true, "取药控制器完成任务"))
                {
                    Console.WriteLine("getting controller 记录取药记录的总表已完结状态完成");
                }
                else
                {
                    Console.WriteLine("记录取药记录为失败并且已经取消时,发生了错误,请检查数据连接");
                }
                Console.ResetColor();
                return;
            }
            #region 如果所有药仓都已经取药完成并且成功了.没有错误的时候,记录所有药仓的取药任务完成,完成付药单.
            if (allStockFinishedAndSuccess)
            {
                //currentOrderUsingStocksTasksDic.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("所有药仓取药完成");
                Console.ResetColor();
                if (App.inventoryManager.FinishDeliveryRecord(ref this.currentDeliveryRecord, true, canceled, "所有药仓取药完成"))
                {
                    Console.WriteLine("getting controller 记录取药记录的总表已完结状态完成");
                    #region 每一个plc发送一个800信号
                    //foreach (var item in this.stocksPLCsDic)
                    //{
                    //    bool sendHaveARestCommandFinished = item.Value.SendAllFinishedHaveARestCommand();
                    //    if (sendHaveARestCommandFinished)
                    //    {
                    //        Console.ForegroundColor = ConsoleColor.Green;
                    //        Console.WriteLine("所有药仓已经完成取药动作,全部进入休息状态! 当前索引:{0}", item.Key);
                    //    }
                    //    else
                    //    {
                    //        Console.ForegroundColor = ConsoleColor.Red;
                    //        Console.WriteLine("药机所有同事药仓取药完成,发送休息信号失败,当前药仓:{0}", item.Key);
                    //        Console.ResetColor();
                    //    }
                    //}
                    #endregion
                    #region 2022年2月26日15:23:33 更新后  要判断一下是否为使用主控plc的方式 如果是的话 只给主控的PLC发一个信号即可
                    SendALlPLCFinishedCommandRestPLC();
                    #endregion
                    this.allStockMedicinesGettingFinished = true;
                }
                else
                {
                    Console.WriteLine("所有药仓取药完成但是记录取药记录失败,请检查数据连接");
                }
            }
            #endregion
            #region 如果不是所有的都完成了,或者是当前的药仓取药信号发生了错误,终止所有的药仓的取药,把已经取到的药品放到废弃药品回收仓内
            else
            {
                //不是所有的都完成了
            }
            #endregion
        }
        /// <summary>
        /// 发送所有plc执行动作完成并且让PLC休息的信号
        /// </summary>
        void SendALlPLCFinishedCommandRestPLC()
        {
            if (App.Setting.PlcSetting_台达.UseMainPLCSerialPort)
            {
                this.mainPLCCommunicator.SendAllFinishedHaveARestCommand();
            }
            else
            {
                foreach (var item in this.stocksPLCsDic)
                {
                    bool sendHaveARestCommandFinished = item.Value.SendAllFinishedHaveARestCommand();
                    if (sendHaveARestCommandFinished)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("所有药仓已经完成取药动作,全部进入休息状态! 当前索引:{0}", item.Key);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("药机所有同事药仓取药完成,发送休息信号失败,当前药仓:{0}", item.Key);
                        Console.ResetColor();
                    }
                }
            }
        }
        //#region 当单个药仓取药发生错误的时候 告诉所有的药仓都取消任务
        //void plc_OnStock_GettingMedicinceError(AMDM_DeliveryRecord record, StockMedicinesGettingErrorEnum error)
        //{
        //    //throw new NotImplementedException();
        //}
        //#endregion
        
        /// <summary>
        /// 单个plc的单个药品取药任务完成
        /// </summary>
        /// <param name="task"></param>
        void plc_OnStockOnClipGettingFinished(MedicinesGroupGettingTask task)
        {
            if (task.IsClearGridTask)
            {
                Utils.LogSuccess("完成一次清空药槽操作");
                SendALlPLCFinishedCommandRestPLC();
            }
            else
            {
                #region 完成一个药品的取药,记录一次付药信息
                if (App.inventoryManager.EndDeliveryOneMedicine(task.RecordDetailRef, task.IsError, task.ErrMsg))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("完成单个药品取药:{0}", task.RecordDetailRef.MedicineName);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("完成单个药品取药失败:{0}", task.ErrMsg);
                    Console.ResetColor();
                }

                #endregion

                #region 如果发生了错误,锁定这个药槽(弹夹) 说明药槽(弹夹)发生了取药错误不能继续使用
                if (task.IsError == true)
                {
                    bool ret = App.bindingManager.SetClipStucked(task.Grid.StockIndex, task.Grid.FloorIndex, task.Grid.IndexOfFloor, true);
                    if (ret)
                    {
                        Utils.LogSuccess("设置弹夹已卡药完成,该弹夹不会再次使用");
                    }
                    else
                    {
                        Utils.LogWarnning("设置弹夹已经卡药失败,下次取药的时候还会尝试从这个弹夹取药的");
                    }
                }
                #endregion
                    //没有错误的话 出库这个药品
                else
                {
                    App.bindingManager.OutMedicineObjects(task.MedicineObjects, task.DeliveryRecordId);
                    //App.bindingManager.InOutMedicineCount(task.Grid, task.RecordDetailRef.MedicineId, currentDeliveryRecord.Id, null, task.Count, null);
                }

                #region 记录一下取完药品以后的图片
                long id = task.DeliveryRecordId;
                if (task.RecordDetailRef!= null)
                {
                    id = task.RecordDetailRef.Id;
                }
                #region 位置1
                if (App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos1 != null)
                {
                    string destPathGrabber1 = getCameraDestFilePath(id, SnapshotLocationEnum.GrabbersArea1, DateTime.Now, "jpg");
                    App.CameraSnapshotCapturer.CaptureSync(App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos1, destPathGrabber1, (img, path) =>
                    {
                        Utils.LogFinished("单次取药完成,机械手摄像位置1拍照任务已完成,存放位置:", path);
                    });
                    App.sqlClient.AddSnapshot(AMDM_Domain.SnapshotParentTypeEnum.DeliveryRecordDetail, id,
                        SnapshotTimePointEnum.AfterAction,
                   AMDM_Domain.SnapshotLocationEnum.GrabbersArea1
                   , DateTime.Now, "已经完成取药触发机械手1镜头位拍照", destPathGrabber1);
                }
                #endregion

                #region 位置2
                if (App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos2 != null)
                {
                    string destPathGrabber2 = getCameraDestFilePath(id, SnapshotLocationEnum.GrabbersArea2, DateTime.Now, "jpg");
                    App.CameraSnapshotCapturer.CaptureSync(App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos2, destPathGrabber2, (img, path) =>
                    {
                        Utils.LogFinished("单次取药完成,机械手摄像位置2拍照任务已完成,存放位置:", path);
                    });
                    App.sqlClient.AddSnapshot(AMDM_Domain.SnapshotParentTypeEnum.DeliveryRecordDetail, id,
                        SnapshotTimePointEnum.AfterAction,
                   AMDM_Domain.SnapshotLocationEnum.GrabbersArea2
                   , DateTime.Now, "已经完成取药触发机械手2镜头位拍照", destPathGrabber2);
                }

                #endregion
                #endregion
            }
            
        }
        #endregion
        #region 开始取药,发送给主控plc启动信号,然后告诉不同的plc到不同的位置上取药.
        /// <summary>
        /// 开始取药
        /// </summary>
        /// <returns></returns>
        public StartMedicinesGettingResult StartMedicinesGetting(AMDM_MedicineOrder order)
        {
            #region 检查当前主控plc的状态
            if (this.bw.IsBusy == true)
            {
                return new StartMedicinesGettingResult() { Success = false, Notice = "请等待当前取药任务完毕后,再执行操作" };
            }
            if (this.medicinesGetting)
            {
                //return new StartMedicinesGettingResult() { Success = false, ErrMsg = "主控PLC线程不在忙碌中,但标记取药状态仍为正在进行中,不能取药" };
                return new StartMedicinesGettingResult() { Success = false, Notice = "当前主机有正在取药的任务正在执行中,请稍后再试" };
            }


            #region 检测主控PLC当前的状态是不是正在自检中
            try
            {
                bool mainPlcConnected = this.mainPLCCommunicator.Connect();
                if (mainPlcConnected == false)
                {
                    this.medicinesGetting = false;
                    return new StartMedicinesGettingResult() { Success = false, ErrMsg = "取药机内部通讯错误!", IsError = true };
                }
                else
                {
                    Utils.LogSuccess("主控PLC已连接,正在等待获取PLC的主控状态和是否复位状态");
                    PLCStatusData status = this.mainPLCCommunicator.GetMedicineGettingStatus();
                    if (status.Resetting)
                    {
                        this.mainPLCCommunicator.Disconnect();
                        Utils.LogFinished("由于主控plc正在属于复位状态,所以断开主控plc的连接,已断开完成");
                        return new StartMedicinesGettingResult() { Success = false, Notice = "取药机正在自维护中,请1分钟后再试" };
                    }
                    else if (status.MedicinesBulketCoverd)
                    {
                        if (App.DebugCommandServer.Setting.IgnoreMedicinesBulketCoverChecking && App.DebugCommandServer.DebuggerConnected

                            )
                        {
                            Utils.LogInfo("调试模式,已取消检测取药斗底部光电");
                        }
                        else
                        {
                            Utils.LogFinished("主控PLC检测到取药斗光电被遮挡,说明内部有药品,不能开始取药.需要护士清空药斗后再提供用户使用");
                            return new StartMedicinesGettingResult() { Success = false, 
                                ErrMsg = "取药斗内有参与药品,请联系工作人员清空后再使用取药机",
                                Notice = "上一用户的药品未取走,请联系工作人员后再次尝试",
                            };
                        }
                    }
                    //else
                    //正常连接并且没有在复位
                }
            }
            catch (Exception err)
            {
                this.medicinesGetting = false;
                return new StartMedicinesGettingResult() { Success = false, ErrMsg = string.Format("主控PLC连接异常:{0}", err.Message), IsError = true };
            }
            #endregion
            #endregion

            #region 检查所有药仓的状态
            foreach (var item in this.stocksPLCsDic)
            {
                var plc = item.Value;
                if (plc.IsBusy)
                {
                    return new StartMedicinesGettingResult() { Success = false, Notice = string.Format("药仓{0}正在忙碌中", item.Key) };
                }
                //bool connectStockRet = plc.Connect();
                //if (connectStockRet == false)
                //{
                //    return new StartMedicinesGettingResult() { Success = false, ErrMsg = string.Format("药仓{0}信号连接错误", item.Key), IsError = true };
                //}
            }
            #endregion
            #region 记录当前付药单信息
            currentDeliveryRecord = App.inventoryManager.CreateDeliveryRecord(order.Id.ToString());
            if (currentDeliveryRecord == null || currentDeliveryRecord.Id < 1)
            {
                this.medicinesGetting = false;
                return new StartMedicinesGettingResult() { Success = false, ErrMsg = "向数据库中插入付药单记录失败", IsError = true };
            }
            #endregion

            //获取最适合的格子的模式
            GetMedicinesObjectSortModeEnum getBestGridMode = App.Setting.ExpirationStrictControlSetting.Enable ? GetMedicinesObjectSortModeEnum.ExpirationDateAsc : GetMedicinesObjectSortModeEnum.ObjectIdAsc;
            #region 把取药单内的信息 换成格子信息 再转换成任务 然后开始执行
            Console.WriteLine("准备获取最合适的药槽");
            Dictionary<IPLCCommunicator4Stock, List<MedicinesGroupGettingTask>> thisOrderUsingStocksTasksDic = new Dictionary<IPLCCommunicator4Stock, List<MedicinesGroupGettingTask>>();
            for (int i = 0; i < order.Details.Count; i++)
            {
                AMDM_MedicineOrderDetail detail = order.Details[i];
                Dictionary<AMDM_Grid_data, List<AMDM_MedicineObject_data>> bestGrids = new Dictionary<AMDM_Grid_data, List<AMDM_MedicineObject_data>>();
                //先获取哪个格子有这个药,然后根据需要的数量安排他在哪个格子里面取
                try
                {
                    bestGrids = App.inventoryManager.GetBestGrids(detail.MedicineId, getBestGridMode, detail.Count);
                    if (bestGrids.Count == 0)
                    {
                        string getBestGrids0ErrMsg = "获取最合适的目标配送药槽后,未获取到有效的结果.";
                        Utils.LogError(getBestGrids0ErrMsg, detail, getBestGridMode.ToString());
                        return new StartMedicinesGettingResult() { Success = false, ErrMsg = getBestGrids0ErrMsg, IsError = true, Notice = "创建取药任务失败,请联系工作人员" };
                    }
                }
                catch (Exception getBestGridErr)
                {
                    string getBestGridErrMsg = "在MedicinesGettingController中,获取最合适的格子时发生了错误";
                    Utils.LogBug(getBestGridErrMsg, getBestGridErr.Message, detail, getBestGridMode.ToString());
                    return new StartMedicinesGettingResult() { Success = false, ErrMsg = getBestGridErrMsg, IsError = true, Notice = "创建取药任务失败,请联系工作人员" };
                }
                //每次只发一个取一个药品的命令时候用的及数字  2022年3月1日16:44:26  改动到台达PLC时此值不再使用.
                int fulfilledCount = 0;
                Console.WriteLine("获取最合适的药槽完成:{0},药品id:{1},内容:{2},\r\n需要的数量是:{3}",
                    i, detail.MedicineId, Newtonsoft.Json.JsonConvert.SerializeObject(bestGrids, Newtonsoft.Json.Formatting.Indented),
                    detail.Count
                    );
                foreach (var grid in bestGrids)
                {
                    #region 2022年3月1日16:41:16  台达的PLC使用一个药槽只发一次命令的方式 所以逻辑有变动
                    //foreach (var o in grid.Value)
                    //{

                    //}
                    MedicinesGroupGettingTask task = new MedicinesGroupGettingTask();
                    #region 创建并开始一个取药记录
                    AMDM_DeliveryRecordDetail_data recordDetail = App.inventoryManager.StartDeliveryOneMedicine(ref currentDeliveryRecord,
                        detail.MedicineId,
                        detail.Name,
                        detail.Barcode,
                        //每条任务记录一个药品对象
                        grid.Value.Count,
                        grid.Key.StockIndex,
                        grid.Key.FloorIndex,
                        grid.Key.IndexOfFloor);
                    task.Grid = grid.Key;
                    task.RecordDetailRef = recordDetail;
                    task.DeliveryRecordId = currentDeliveryRecord.Id;
                    task.Count = grid.Value.Count;
                    task.MedicineObjects = grid.Value;
                    #endregion
                    Console.WriteLine("获取当前格子使用的plc:");
                    var thisTaskPLC = this.stocksPLCsDic[task.Grid.StockIndex];
                    if (thisOrderUsingStocksTasksDic.ContainsKey(thisTaskPLC) == false)
                    {
                        thisOrderUsingStocksTasksDic.Add(thisTaskPLC, new List<MedicinesGroupGettingTask>());
                        Console.WriteLine("当前药单需要使用{0}号药仓", task.Grid.StockIndex);
                    }
                    thisOrderUsingStocksTasksDic[thisTaskPLC].Add(task);
                    #endregion
                    #region 西门子PLC使用一个药品一个药品的发送方式 这是备份之前的逻辑
                    //#region 该药槽有多少药就循环多少次的去创建任务  但是当任务数量已经够的时候就跳出
                    //for (int j = 0; j < grid.Value; j++)
                    //{
                    //    MedicineGettingTask task = new MedicineGettingTask();
                    //    #region 创建并开始一个取药记录
                    //    AMDM_DeliveryRecordDetail_data recordDetail = App.inventoryManager.StartDeliveryOneMedicine(ref currentDeliveryRecord,
                    //        detail.MedicineId,
                    //        detail.Name,
                    //        detail.Barcode,
                    //        detail.Count,
                    //        //1,
                    //        grid.Key.StockIndex,
                    //        grid.Key.FloorIndex,
                    //        grid.Key.IndexOfFloor);
                    //    task.Grid = grid.Key;
                    //    task.RecordDetailRef = recordDetail;
                    //    task.DeliveryRecordId = currentDeliveryRecord.Id;
                    //    #endregion
                    //    Console.WriteLine("获取当前格子使用的plc:");
                    //    var thisTaskPLC = this.stocksPLCsDic[task.Grid.StockIndex];
                    //    if (thisOrderUsingStocksTasksDic.ContainsKey(thisTaskPLC) == false)
                    //    {
                    //        thisOrderUsingStocksTasksDic.Add(thisTaskPLC, new List<MedicineGettingTask>());
                    //        Console.WriteLine("当前药单需要使用{0}号药仓", task.Grid.StockIndex);
                    //    }
                    //    thisOrderUsingStocksTasksDic[thisTaskPLC].Add(task);

                    //    fulfilledCount++;
                    //    //两个格子可能有4个,如果当前就要3个,已经给完3个了 就跳出就可以了.
                    //    if (fulfilledCount >= detail.Count)
                    //    {
                    //        Console.WriteLine("够了 跳出吧,已经能给的数量为:{0}", fulfilledCount);
                    //        break;
                    //    }
                    //}
                    //#endregion
                    #endregion


                    //两个格子可能有4个,如果当前就要3个,已经给完3个了 就跳出就可以了.
                    if (fulfilledCount >= detail.Count)
                    {
                        Console.WriteLine("格子循环检测:够了 跳出吧,已经能给的数量为:{0}", fulfilledCount);
                        break;
                    }
                }
            }
            //Console.ForegroundColor = ConsoleColor.DarkBlue;
            //Console.WriteLine("获取最合适的药槽以及创建任务完成当前任务集合:\r\n{0}",Newtonsoft.Json.JsonConvert.SerializeObject(thisOrderUsingStocksTasksDic, Newtonsoft.Json.Formatting.Indented));
            //Console.ResetColor();
            #endregion
            #region 启动主机
            //Console.WriteLine("主控机已打开取药状态监测,即将分发药仓的取药任务");
            //try
            //{
            //    bool mainPlcConnected = this.mainPLCCommunicator.Connect();
            //    if (mainPlcConnected == false)
            //    {
            //        this.medicinesGetting = false;
            //        return new StartMedicinesGettingResult() { Success = false, ErrMsg = "尝试启动主机时,主控PLC连接失败" };
            //    }
            //    else
            //    {
            //        suc("主控PLC已连接");
            //    }
            //}
            //catch (Exception err)
            //{
            //    string failMsg = string.Format("主控PLC连接异常:{0}", err.Message);
            //    this.medicinesGetting = false;
            //    return new StartMedicinesGettingResult() {Success = false, ErrMsg = failMsg };
            //}
            this.bw.RunWorkerAsync();


            #endregion
            #region 启动所有药仓的取药
            //本次付药的药单需要动作的药仓集合
            #region 启动所有要工作的药仓
            foreach (var item in thisOrderUsingStocksTasksDic)
            {
                var plc = item.Key;
                var tasks = item.Value;
                bool startSyncTask = plc.StartMedicinesGetting(tasks);
                if (startSyncTask == false)
                {
                    this.medicinesGetting = false;
                    return new StartMedicinesGettingResult() { Success = false, ErrMsg = string.Format("错误 启动药仓{0}的取药任务失败", item.Key), IsError = true, };
                }
            }
            #endregion
            #endregion
            #region 清空状态
            this.allStockMedicinesGettingFinished = false;
            this.medicinesGetting = true;
            this.medicinesHasBeenTaked = false;
            this.snapshotCaptured = false;
            this.Error = null;
            this.SubErrors = null;
            #endregion
            ///返回正确无错误的结果
            return new StartMedicinesGettingResult() { Success = true , DeliveryRecordId = currentDeliveryRecord.Id};
        }
        #endregion

        #region 终止取药,标记取药完成以后,把药品放置到废药斗内.
        /// <summary>
        /// 终止取药
        /// </summary>
        /// <param name="transferMedicinesToRecyler">转移已经取到的药品到废弃药品回收仓</param>
        public void AbortMedicinesGetting(bool transferMedicinesToRecyler)
        {
            if (transferMedicinesToRecyler)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("即将转移药品到废弃药品回收仓");
                this.mainPLCCommunicator.SendAllFinishedHasErrorNeedRecyleCommand();
                Utils.LogFinished("发送取药斗清空操作完成");
                Console.ResetColor();
            }
            if (this.bw.IsBusy)
            {
                this.bw.CancelAsync();
            }
            foreach (var item in this.stocksPLCsDic)
            {
                item.Value.AbortMedicinesGetting();
            }
        }
        #endregion

        #region 析构函数
        public void Dispose()
        {
            foreach (var item in this.stocksPLCsDic)
            {
                item.Value.OnAllMedicinesGettingStoped -= this.plc_OnStock_AllMedicinesGettingStoped;
                //item.Value.OnStock_GettingMedicinceError -= this.plc_OnStock_GettingMedicinceError;
                //item.Value.OnOneMedicineGettingFinished -= this.plc_OnStockOneMedicineGettingFinished;
                //item.Value.OnOneMedicineGettingStarting -= this.plc_OnOneMedicineGettingStarting;
                //item.Value.OnOneMedicineGettingStarted -= this.plc_OnOneMedicineGettingStarted;

                item.Value.OnOneClipGettingFinished -= this.plc_OnStockOnClipGettingFinished;
                item.Value.OnOneClipGettingStarting -= this.plc_OnOneClipGettingStarting;
                item.Value.OnOneClipGettingStarted -= this.plc_OnOnClipGettingStarted;

                item.Value.Disconnect();
                item.Value.Dispose();
            }
        }
        #endregion
       
        #region 回调函数
        /// <summary>
        /// 全部的药品已经取完了,在这个回调函数内应该处理拍照.
        /// </summary>
        public event OnStocks_AllMedicinesDeliveryFinishedEventHandler OnAllMedicinesDeliveryFinished;

        /// <summary>
        /// 当顾客把药品已经取走了,就是可以播放广告的地方.
        /// </summary>
        public event OnAMDM_MedicinesHasBeenTakedEventHandler OnMedicinesHasBeenTaked;

        public event OnAMDM_MedicinesGettingErrorEventHandler OnMedicinesGettingError;
        #endregion
    }
    #endregion
}
