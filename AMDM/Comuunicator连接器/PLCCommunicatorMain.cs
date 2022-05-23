using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM.Manager
{

    /// <summary>
    /// 总控机PLC的控制器 2021年11月13日13:58:24 经过讨论,已经取消了主控plc的思路
    /// 2021年11月17日12:01:10  经过讨论 又使用主plc的思路,用主plc来读取拍照和打印信号以及药品已经被取走的信号
    /// </summary>
    public class PLCCommunicator4Main_台达 : PLCCommunicatorTD, IPLCCommunicator4Main
    {
        //MainPLCSettingTD setting = null;
        //public PLCCommunicatorTD plc = null;
        /// <summary>
        /// 初始化plc,主要是用来初始化主plc控制器,让他能够发送开始和技术信号,可以获取plc状态等.注意不能用这个plc来发送位置信息
        /// </summary>
        /// <param name="setting"></param>
        public PLCCommunicator4Main_台达(MainPLCSettingTD setting):base(setting, null)
        {
            this.setting = setting;
            //plc = new PLCCommunicatorTD(setting);
            //this.GetSerialPort();
            //this.GetModbusClient(this.modbus485Port);
        }
        #region 全局私有变量

        #endregion
        #region 全局公共变量

        #endregion

        /// <summary>
        /// 获取药机当前是否正在付药的状态
        /// 2022年3月1日16:53:41 新增DestCount 这个值如果给定的话 如果想要返回具体
        /// </summary>
        /// <returns></returns>
        public PLCStatusData GetMedicineGettingStatus()
        {
            PLCStatusData status = new PLCStatusData();
            ushort readCount = 23;
            ushort[] ret = new ushort[readCount];
            try
            {
                ret = ReadHoldingRegisters(readCount, 1);
            }
            catch (Exception err)
            {
                Utils.LogError("在执行PLC获取状态时发生错误:", err.Message);
                return null;
            }
            if (ret == null || ret.Length != readCount)
            {
                return null;
            }
            //第一位保存1号柜的情况
            ushort stock1Status = ret[0];
            ProcessStatusInfo(stock1Status, 0, ref status);
            //第二位保存2号柜的情况
            ushort stock2Status = ret[1];
            ProcessStatusInfo(stock2Status, 1, ref status);
            //第三位保存3号柜的情况
            ushort stock3Status = ret[2];
            ProcessStatusInfo(stock3Status, 2, ref status);
            //第四位暂时没有用
            //20年2月20日31122:04:56今天下午启动了 是用来检测光电信号的 取药斗底部的光电 如果被遮挡的话说明里面有药,不能开始取药任务.要在取药之前进行检查
            ushort medicinesBulketCoverd = ret[3];
            if (medicinesBulketCoverd == 100)
            {
                status.MedicinesBulketCoverd = true;
            }
            //第五位保存的是100信号,下一个可以取药了
            ushort allFinished = ret[4];
            if (allFinished == 100)
            {
                //status.Main = MainBufferValuesEnum.FinishedDeliveryAll;
                status.MedicinesHasBeenTaked = true;
            }
            //第六位保存的是可以拍照了
            ushort canSnap = ret[5];
            if (canSnap == 100)
            {
                status.PrintAndSnapshot = true;
            }
            //第七位保存的是柜子1的温度
            ushort stock1Temperature = ret[6];
            //第八位第九位保存的是柜子2,3的温度
            ushort stock2Temperature = ret[7];
            ushort stock3Temperature = ret[8];

            //D215也就是15位,获取到的是药仓1的实际已经掉落的数量
            ushort stock1GetedCount = ret[14];
            ushort stock2GetedCount = ret[15];
            ushort stock3GetedCount = ret[16];
            if (stock1GetedCount > 0 || stock2GetedCount > 0 || stock3GetedCount > 0)
            {

            }

            //D218 219 220分别代表三个药柜的极限状态.
            //第一个药仓,在218的寄存器位置,到极限的时候会发109
            //第二个药仓,在219的寄存器位置,219到极限的时候会发209
            //第三个药仓,在220的寄存器位置,220到极限的时候会发309
            ushort stock1GrabberOverflow = ret[17];
            ushort stock2GrabberOverflow = ret[18];
            ushort stock3GrabberOverflow = ret[19];

            if (
                stock1GrabberOverflow == 109
                || stock2GrabberOverflow == 209
                || stock3GrabberOverflow == 309
                )
            {
                status.GrabberOverflow = true;
            }


            //D211 212 213分别代表三个药仓的回原点状态.如果是1 正在回原点 如果是0则没有在回原点
            ushort stock1GrabberResting = ret[10];
            ushort stock2GrabberResting = ret[11];
            ushort stock3GrabberResting = ret[12];
            if (stock1GrabberResting == 1
                || stock2GrabberResting == 1
                || stock3GrabberResting == 1
                )
            {
                status.Resetting = true;
            }

            return status;
        }

        public static void ProcessStatusInfo(ushort bufferRet, int stockIndex, ref PLCStatusData status)
        {
            if (bufferRet / 1000 == 2)
            {
                if ((bufferRet - 2000) / 100 == stockIndex + 1)
                {
                    //取药时成功的,已经取药的数量是
                    var gotMedicinesCount = bufferRet - 2000 - (stockIndex + 1) * 100;
                    Utils.LogInfo("已经取药完成数量为:", stockIndex, gotMedicinesCount, bufferRet);
                    status.Main = MainBufferValuesEnum.FinishedDeliveryOneMedicine;
                    setCount(status, stockIndex, gotMedicinesCount);
                }
                else
                {
                    Utils.LogBug("2000系信号,从药仓状态寄存器上获取到了其他药仓的状态数据,下位机程序bug");
                }
            }
            else if (bufferRet / 1000 == 3)
            {
                if ((bufferRet - 3000) / 100 == stockIndex + 1)
                {
                    //取药时成功的,已经取药的数量是
                    var gotMedicinesCount = bufferRet - 3000 - (stockIndex + 1) * 100;
                    Utils.LogInfo("失败时,已经检测到的取药完成数量为:", stockIndex, gotMedicinesCount, bufferRet);
                    status.Main = MainBufferValuesEnum.Error;
                    setCount(status, stockIndex, gotMedicinesCount);
                }
                else
                {
                    Utils.LogBug("3000系信号,从药仓状态寄存器上获取到了其他药仓的状态数据,下位机程序bug");
                }
            }
            else if (bufferRet < 500 && bufferRet >= 400)
            {
                //400系列 属于光栅遮挡
                Utils.LogError("药机光栅长时间被遮挡故障,药机编号:", bufferRet - 400);
                status.CounterCoverdError = true;
            }
            else
            {

            }
        }
        static void setCount(PLCStatusData status, int stockIndex, int gotMedicinesCount)
        {
            if (status.CounterGetedMedicinesCount == null)
            {
                status.CounterGetedMedicinesCount = new Dictionary<int, int>();
            }
            if (status.CounterGetedMedicinesCount.ContainsKey(stockIndex) == false)
            {
                status.CounterGetedMedicinesCount.Add(stockIndex, 0);
            }
            status.CounterGetedMedicinesCount[stockIndex] = gotMedicinesCount;
        }

        public bool SendAllFinishedHaveARestCommand()
        {
            if (this.Connected == false)
            {
                Console.WriteLine("plc未连接 不能发送数据");
                return false;
            }
            //return this.plc.Write<int>(setting.PLCDataIndex.Main, MainBufferValuesEnum.FinishedDeliveryAll);
            List<ushort> values = new List<ushort>() { (ushort)MainBufferValuesEnum.FinishedDeliveryAll };
            base.WriteMultipleRegisters(values.ToArray());
            var writeRet = ReadHoldingRegisters(10, -100);
            return true;
        }

        public bool SendAllFinishedHasErrorNeedRecyleCommand()
        {
            if (this.Connected == false)
            {
                Console.WriteLine("plc未连接 不能发送数据");
                return false;
            }
            //return this.plc.Write<int>(setting.PLCDataIndex.Main, MainBufferValuesEnum.FinishedDeliveryAll);
            List<ushort> values = new List<ushort>() { (ushort)MainBufferValuesEnum.ErrorFinishedDeliveryAll };
            WriteMultipleRegisters(values.ToArray());
            var writeRet = ReadHoldingRegisters(10, -100);
            return true;
        }

        //#region 检查PLC状态是否空闲,是否可以继续执行取药任务
        ///// <summary>
        ///// 检查PLC状态是否空闲,是否可以继续执行取药任务
        ///// </summary>
        ///// <returns></returns>
        //public bool CheckPLCStatusCanContinue()
        //{
            
        //}
        //#endregion

        /// <summary>
        /// 获取药机当前是否正在付药的状态
        /// </summary>
        /// <returns></returns>
        /// 
        public bool SendSetACTemperature(int stockIndex, float destTemperature)
        {
            List<ushort> values = new List<ushort>() { };
            var real = (ushort)Math.Round(destTemperature * 10);
            values.Add(real);
            WriteMultipleRegisters(values.ToArray(), (ushort)(11 + stockIndex));
            //var ret = SendGetAllStockACDestTemperature(1);
            return true;
        }
        public bool SendShowGridNumberAt485ShowerOnStock(int stockIndex, Nullable<int> number)
        {
            List<ushort> values = new List<ushort>();
            if (number == null)
            {
                values.Add((ushort)((stockIndex + 1) * 1000 + 0));
            }
            else
            {
                values.Add((ushort)((stockIndex + 1) * 1000 + number.Value + 1));
            }
            WriteMultipleRegisters(values.ToArray(), 7);
            return true;
        }


        public bool SendGetUVLampStatus()
        {
            //List<ushort> values = new List<ushort>() { };
            //if (unlock)
            //{
            //    values.Add(1);
            //}
            //else
            //{
            //    values.Add(0);
            //}
            //WriteMultipleRegisters(values.ToArray(), 10);
            //var writeRet = ReadHoldingRegisters(10, -100);
            //return true;
            ///寄存器位置是111 但是读的时候默认是从200开始读的 所以减去100以后 再加上11 就是111了
            var read = ReadHoldingRegisters(1, -100 + 10);

            if (read != null && read.Length >= 1)
            {
                return read[0] == 1;
            }
            else
            {
                return false;
            }
        }
        public bool SendLockerControlCommand(bool unlock)
        {
            List<ushort> values = new List<ushort>() { };
            if (unlock)
            {
                values.Add(15);
            }
            else
            {
                values.Add(0);
            }
            WriteMultipleRegisters(values.ToArray(), 8);
            var writeRet = ReadHoldingRegisters(10, -100);
            return true;
        }

        public bool SendACControlCommand(bool unlock)
        {
            List<ushort> values = new List<ushort>() { };
            if (unlock)
            {
                values.Add(1);
            }
            else
            {
                values.Add(0);
            }
            WriteMultipleRegisters(values.ToArray(), 9);
            var writeRet = ReadHoldingRegisters(10, -100);
            return true;
        }

        public bool SendUVLampControlCommand(bool unlock)
        {
            List<ushort> values = new List<ushort>() { };
            if (unlock)
            {
                values.Add(1);
            }
            else
            {
                values.Add(0);
            }
            WriteMultipleRegisters(values.ToArray(), 10);
            var writeRet = ReadHoldingRegisters(10, -100);
            return true;
        }
        public Nullable<float> SendGetACTemperature(int stockIndex)
        {
            var read = ReadHoldingRegisters(1, (ushort)(7 + stockIndex));

            if (read != null && read.Length >= 1)
            {
                return read[0] / 10f;
            }
            else
            {
                return null;
            }
        }
        public Dictionary<int, Nullable<float>> SendGetAllStockACCurrentTemperature(int stockCount)
        {
            if (stockCount < 1)
            {
                return null;
            }
            var read = ReadHoldingRegisters((ushort)stockCount, 7);

            if (read != null && read.Length >= 1)
            {
                var ret = new Dictionary<int, Nullable<float>>();
                for (int i = 0; i < stockCount; i++)
                {
                    if (i + 1 > read.Length)
                    {
                        ret.Add(i, null);
                    }
                    else
                    {
                        ret.Add(i, read[0] / 10f);
                    }
                }
                return ret;
            }
            else
            {
                return null;
            }
        }

        #region 获取所有药仓的目标设定温度
        public Dictionary<int, Nullable<float>> SendGetAllStockACDestTemperature(int stockCount)
        {
            if (stockCount < 1)
            {
                return null;
            }
            ///寄存器位置是111 但是读的时候默认是从200开始读的 所以减去100以后 再加上11 就是111了
            var read = ReadHoldingRegisters((ushort)stockCount, -100 + 11);

            if (read != null && read.Length >= 1)
            {
                var ret = new Dictionary<int, Nullable<float>>();
                for (int i = 0; i < stockCount; i++)
                {
                    if (i + 1 > read.Length)
                    {
                        ret.Add(i, null);
                    }
                    else
                    {
                        ret.Add(i, read[0] / 10f);
                    }
                }
                return ret;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
