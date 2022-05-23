using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 基础信息的类定义
    public enum PLCDataTypeEnum { Bit16Int, Bit32Long };
    /// <summary>
    /// 总控buffer内的值枚举
    /// </summary>
    public enum MainBufferValuesEnum
    {
        /// <summary>
        /// 空载状态
        /// </summary>
        Empty = 0,
        /// <summary>
        /// 开始在层板上的药槽的取药
        /// </summary>
        StartWorkingGridOnFloorGrabber = 100,
        /// <summary>
        /// 获取,结束一次药品的交付
        /// 2022年2月26日06:47:40 已经该成了 一个药槽的取药完成 2022年3月1日17:05:28又改回来了一个药品的,然后再根据药品数量确认是否完成了药槽
        /// 这个数字是我自己定义的 实际并不是正好的返回2000
        /// </summary>
        FinishedDeliveryOneMedicine = 2000,
        /// <summary>
        /// 这个是我自己定义的 实际并不会返回2222
        /// </summary>
        //FinishedDeliveryOneClip = 2222,
        /// <summary>
        /// 获取,发生错误
        /// </summary>
        Error = 300,
        /// <summary>
        /// 开始在特殊传送槽上的取药动作
        /// </summary>
        StartWorkingSpecialConveyingTrough = 1000,

        /// <summary>
        /// 设定全部取药结束
        /// </summary>
        FinishedDeliveryAll = 800,

        /// <summary>
        /// 新增的信息 2022年3月1日15:22:18   取药过程发生了错误执行弃药操作
        /// </summary>
        ErrorFinishedDeliveryAll = 808,

        /// <summary>
        /// 开始抓取器/手指的定位命令,此命令不会进行取药 只是定位位置
        /// </summary>
        StartGrabberPositioning = 2000,
    }
    /// <summary>
    /// 哪只抓手的枚举,靠近0点的就是Near也就是刘工说的b,靠近终点的就是Far 也就是刘工说的a
    /// </summary>
    public enum WhichGrabberEnum { 
        /// <summary>
        /// b抓手
        /// </summary>
        Near = 2, 
        /// <summary>
        /// a抓手
        /// </summary>
        Far = 1, None = 0 };
    /// <summary>
    /// 发送给plc的数据包
    /// </summary>
    public class PLCDataBufferInfo
    {
        public int Index { get; set; }
        public PLCDataTypeEnum DataType { get; set; }
    }
    public class PLCDataBufferValue : PLCDataBufferInfo
    {
        /// <summary>
        /// plc的buffer的值,用于获取到的时候保存过来.
        /// </summary>
        public object Value { get; set; }
    }
    /// <summary>
    /// 获取到plc的状态后使用本状态保存
    /// </summary>
    public class PLCStatusData
    {
        /// <summary>
        /// 总控制开关
        /// </summary>
        public MainBufferValuesEnum Main { get; set; }
        /// <summary>
        /// 槽子,横轴
        /// </summary>
        public long X { get; set; }
        /// <summary>
        /// 手臂,纵轴
        /// </summary>
        public long Y { get; set; }
        /// <summary>
        /// 用哪个手指
        /// </summary>
        public WhichGrabberEnum Grabber { get; set; }
        /// <summary>
        /// 取药的数量
        /// </summary>
        public int Times { get; set; }


        //CommandCount, out commandCount);
        //CounterCoverdError, out counterCoverdErr);
        //CounterGetedMedicinesCount, out counterGettedMedicinesCount);
        //MedicinesHasBeenTaked, out medicinesHasBeenTaked);
        //PrintAndSnapshot, out printAndSnapshot);
        //Unuse124, out unuse124);
        /// <summary>
        /// plc获取到的命令数量
        /// </summary>
        public int CommandCount { get; set; }
        /// <summary>
        /// 掉落药品数量检测计数器光栅错误,可能被覆盖
        /// </summary>
        public bool CounterCoverdError { get; set; }
        /// <summary>
        /// 掉落药品数量计数器读取到的数量 2022年3月1日16:57:18 改为按仓来分辨掉落数量
        /// </summary>
        public Dictionary<int,int> CounterGetedMedicinesCount { get; set; }
        /// <summary>
        /// 药品是否已经被从药篮中取走
        /// </summary>
        public bool MedicinesHasBeenTaked { get; set; }

        /// <summary>
        /// 取药斗底部光电是否被遮挡,如果被遮挡是不能开始取药任务的.需要根据情况考量是把里面可能的药品放到取药斗,还是直接的锁定机器
        /// </summary>
        public bool MedicinesBulketCoverd { get; set; }
        /// <summary>
        /// 可以打印并且拍照的信号
        /// </summary>
        public bool PrintAndSnapshot { get; set; }
        //public int Unuse124 { get; set; }

        /// <summary>
        /// 主控PLC是否正在复位
        /// </summary>
        public bool Resetting { get; set; }

        /// <summary>
        /// 有机械手超出限制区域(不管哪个超限,都会标记)
        /// </summary>
        public bool GrabberOverflow { get; set; }

        #region 转换成可视化数据

        #endregion
    }
    /// <summary>
    /// 操作plc时的数据索引位置
    /// </summary>
    public class PLCDataIndexInfo
    {
        public PLCDataIndexInfo()
        {
            Main = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 100 };
            Y = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit32Long, Index = 102 };
            X = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit32Long, Index = 106 };
            Grabber = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 110 };
            Times = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 112 };

            #region 2021年11月17日10:20:13  新增的变量,以下变量都是只读,不需要写入
            this.CommandCount = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 120 };
            this.CounterGetedMedicinesCount = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 122 };
            this.Unuse124 = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 124 };
            this.CounterCoverdError = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 126 };
            this.MedicinesHasBeenTaked = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 130 };
            this.PrintAndSnapshot = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 132 };
            #endregion
            #region 2021年11月22日11:49:09  新增的变量  用于检测plc是否正在复位中
            Restting = new PLCDataBufferInfo() { DataType = PLCDataTypeEnum.Bit16Int, Index = 114 };
            #endregion
        }
        /// <summary>
        /// 总控制开关
        /// </summary>
        public PLCDataBufferInfo Main { get; set; }
        /// <summary>
        /// 槽子,横轴
        /// </summary>
        public PLCDataBufferInfo X { get; set; }
        /// <summary>
        /// 手臂,纵轴
        /// </summary>
        public PLCDataBufferInfo Y { get; set; }
        /// <summary>
        /// 用哪个手指
        /// </summary>
        public PLCDataBufferInfo Grabber { get; set; }
        /// <summary>
        /// 取药的数量
        /// </summary>
        public PLCDataBufferInfo Times { get; set; }

        #region 2021年11月17日10:23:14   新加入的 一些只读变量
        /// <summary>
        /// 120 121发送到plc的命令数量
        /// </summary>
        public PLCDataBufferInfo CommandCount { get; set; }

        /// <summary>
        /// 122 123传送带处光栅检测到的药品掉落数量
        /// </summary>
        public PLCDataBufferInfo CounterGetedMedicinesCount { get; set; }

        /// <summary>
        /// 124 125保留字段
        /// </summary>
        public PLCDataBufferInfo Unuse124 { get; set; }

        /// <summary>
        /// 126 127长时间传送带光栅被遮挡 被遮挡为100,否则为0
        /// </summary>
        public PLCDataBufferInfo CounterCoverdError { get; set; }
        /// <summary>
        /// 130 131 主控plc上的信号 其他plc上没有此信号  药品已经被用户取走,可以给下一个用户取药了,也就标志为可以播放广告了.
        /// </summary>
        public PLCDataBufferInfo MedicinesHasBeenTaked { get; set; }
        /// <summary>
        /// 132 133 主控plc上的信号 其他plc上没有此信号  可以打印付药单和进行拍照命令,保持时间也是2秒 可以拍照为100,否则为0
        /// </summary>
        public PLCDataBufferInfo PrintAndSnapshot { get; set; }
        #endregion

        #region 2021年11月22日11:49:51  新增的变量 用于检测plc是否正在复位中.
        /// <summary>
        /// PLC是否正在复位中
        /// </summary>
        public PLCDataBufferInfo Restting { get; set; }
        #endregion

    }
    public class AMDM_PLCSetting_西门子 : IAMDM_PLCTCPSetting
    {
        public PLCLibEnum PLCLib { get; set; }
        /// <summary>
        /// plc的ip地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 机架号
        /// </summary>
        public int RackIndex { get; set; }
        /// <summary>
        /// 槽号
        /// </summary>
        public int SlotIndex { get; set; }
        /// <summary>
        /// 索要读写的缓冲区/寄存器编号
        /// </summary>
        public int DBNumber { get; set; }
        /// <summary>
        /// 每移动1毫米所需要的脉冲数量
        /// </summary>
        public int PerMMPulseCount { get; set; }

        public PLCDataIndexInfo PLCDataIndex { get; set; }
    }
    public abstract class AMDM_PLCSettingTD : IAMDM_PLCModbusSerialPortSetting
    {
        public PLCLibEnum PLCLib { get; set; }

        public string TCPModeIP { get; set; }
        public int TCPModePort { get; set; }
        /// <summary>
        /// 串口号
        /// </summary>
        public string SerialPortName { get; set; }
        /// <summary>
        /// 连接采样率/波特率  19200
        /// </summary>
        public int Baudrate { get; set; }
        /// <summary>
        /// 奇偶校验位 N
        /// </summary>
        public System.IO.Ports.Parity Parity { get; set; }
        /// <summary>
        /// 字节数 8
        /// </summary>
        public int BitsCount { get; set; }
        /// <summary>
        /// 停止位 1
        /// </summary>
        public System.IO.Ports.StopBits StopBits { get; set; }

        /// <summary>
        /// 是否启用RTS信号,台达的PLC这个参数非常的重要,如果不设置这个信号为TRUE的话,可以连接上,只能发不能收,理解为  单工时的收发模式信号
        /// </summary>
        public bool RtsEnable { get; set; }

        /// <summary>
        /// 设备基地址,默认是100
        /// </summary>
        public byte DeviceAddress { get; set; }

        /// <summary>
        /// 数据起始位,HoldingRegisters的起始位 模式4096
        /// </summary>
        public ushort DataBufferStartAddress { get; set; }

        public int PerMMPulseCount { get; set; }

        /// <summary>
        /// PLC的数据位置标识符信息
        /// </summary>
        public PLCDataIndexInfo PLCDataIndex { get; set; }

        /// <summary>
        /// 向PLC传递值时的数据起始位置 模式100
        /// </summary>
        public ushort PuttingBufferStartIndex { get; set; }

        /// <summary>
        /// 从PLC获取状态等返回值的数据起始位置 默认200
        /// </summary>
        public ushort GettingBufferStartIndex { get; set; }

        /// <summary>
        /// 是否是主控台PLC
        /// </summary>
        public bool IsMain { get; set; }


        public AMDM_PLCSettingTD()
        {
            this.SerialPortName = "COM5";
            this.Baudrate = 19200;
            this.Parity = System.IO.Ports.Parity.None;
            this.BitsCount = 8;
            this.StopBits = System.IO.Ports.StopBits.One;
            this.RtsEnable = true;
            this.DeviceAddress = 100;
            this.DataBufferStartAddress = 4096;
            this.PuttingBufferStartIndex = 100;
            this.GettingBufferStartIndex = 200;
            this.PerMMPulseCount = 80;
            this.PLCDataIndex = new PLCDataIndexInfo();
        }
    }
    /// <summary>
    /// 药机主机的plc设置
    /// </summary>
    public class MainPLCSettingTD : AMDM_PLCSettingTD
    {
        public MainPLCSettingTD()
        {
            this.IsMain = true;
            this.UseMainPLCSerialPort = true;
        }
        /// <summary>
        /// 是否使用主机的串口发送数据,也就是上位机只对一个PLC,从机默认是要使用这个的.
        /// </summary>
        public bool UseMainPLCSerialPort { get; set; }
    }
    public class MainPLCSetting西门子 : AMDM_PLCSetting_西门子
    {
        public MainPLCSetting西门子()
        {
            IPAddress = "192.168.4.222";
            DBNumber = 1;
            RackIndex = 0;
            SlotIndex = 1;
            PLCDataIndex = new PLCDataIndexInfo();
            PerMMPulseCount = 80;
            //XOffsetFromStartPointMM = -19;//要想到最左侧0点为是不可能的 因为手默认就已经出去19毫米了.
            //YOffsetFromStartPointMM = 2720 / 80;//34
        }
    }
    /// <summary>
    /// 每一个药仓的plc设置
    /// </summary>
    public class StockPLCSettingTD : AMDM_PLCSettingTD
    {
        public StockPLCSettingTD()
        {
            this.IsMain = false;
        }
    }
    public class StockPLCSetting西门子 : AMDM_PLCSetting_西门子
    {
        public StockPLCSetting西门子()
        {
        }
    }

    /// <summary>
    /// plc的总设置
    /// </summary>
    public class PLCSetting西门子
    {
        public MainPLCSetting西门子 MainPLC { get; set; }
        public Dictionary<int, StockPLCSetting西门子> StocksPLC { get; set; }

        /// <summary>
        /// 2022年2月26日03:56:41 开始正式使用台达的PLC设置
        /// </summary>
        //public AMDM_PLCSetting_台达 台达PLC设置 { get; set; }
    }

    public class PLCSetting<MainPLCSettingTYPE,StockPLCSettingTYPE> : IPLCSetting<MainPLCSettingTYPE,StockPLCSettingTYPE> 
    {
        public PLCSetting()
        {
            this.MainPLC = Activator.CreateInstance<MainPLCSettingTYPE>();
            this.StocksPLC = new Dictionary<int, StockPLCSettingTYPE>();
            this.UseMainPLCSerialPort = true;
        }
        public MainPLCSettingTYPE MainPLC { get; set; }
        public Dictionary<int, StockPLCSettingTYPE> StocksPLC { get; set; }

        /// <summary>
        /// 是否值使用主控PLC的模式,如果是的话,所有的从机(药仓)上的PLC都使用主机的串口
        /// </summary>
        public bool UseMainPLCSerialPort { get; set; }

        /// <summary>
        /// 2022年2月26日03:56:41 开始正式使用台达的PLC设置
        /// </summary>
        //public AMDM_PLCSetting_台达 台达PLC设置 { get; set; }
    }
    #endregion
}
