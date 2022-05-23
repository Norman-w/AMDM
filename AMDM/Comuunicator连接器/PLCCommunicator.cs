using AMDM;
using AMDM_Domain;
using Newtonsoft.Json;
using Sharp7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NModbus;
using System.IO.Ports;
using NModbus.Serial;
using System.Net.Sockets;
using System.Threading.Tasks;
using EasyModbus;

namespace AMDM.Manager
{
    #region 西门子s7client
    //public abstract class PLCCommunicator:IPLCCommunicator
    //{
    //    protected AMDM_PLCSetting setting = null;
    //    protected S7Client s7client = new S7Client();
    //    #region 内部函数 快速的访问和写寄存器
    //    protected bool Read<T>(PLCDataBufferInfo plcBuffer, out object value)
    //    {
    //        string stringRet = null;
    //        int intRet = 0;
    //        long longRet = 0;
    //        string x2StringRet = null;
    //        int readRet = -1;


    //        StringBuilder readRetSB = new StringBuilder();
    //        StringBuilder x2StringSBFull = new StringBuilder();
    //        StringBuilder readBufferAsString = new StringBuilder();

    //        byte[] buffer = null;
    //        switch (plcBuffer.DataType)
    //        {
    //            case PLCDataTypeEnum.Bit16Int:
    //                buffer = new byte[2];
    //                break;
    //            case PLCDataTypeEnum.Bit32Long:
    //                buffer = new byte[4];
    //                break;
    //            default:
    //                break;
    //        }
    //        readRet = s7client.DBRead(this.setting.DBNumber, plcBuffer.Index, buffer.Length, buffer);
    //        if (readRet != 0)
    //        {
    //            value = null;
    //            return false;
    //        }

    //        for (int i = 0; i < buffer.Length; i++)
    //        {
    //            if (i >= buffer.Length)
    //            {
    //                break;
    //            }
    //            if (readRetSB.Length > 0)
    //            {
    //                readRetSB.Append("\r\n");
    //            }
    //            readRetSB.AppendFormat("{0} ({1}):{2}", i, i + plcBuffer.Index, buffer[i]);
    //            string currentX2 = Convert.ToString(Convert.ToInt32(buffer[i]), 2).PadLeft(8, '0');
    //            x2StringSBFull.Append(currentX2);
    //            readBufferAsString.Append(Convert.ToChar(buffer[i]));
    //        }
    //        stringRet = readRetSB.ToString();

    //        #region 显示二进制和通过8位一组重新组合到10进制的信息

    //        x2StringRet = x2StringSBFull.ToString();

    //        #endregion

    //        switch (plcBuffer.DataType)
    //        {
    //            case PLCDataTypeEnum.Bit16Int:
    //                try
    //                {
    //                    intRet = Convert.ToInt32(x2StringSBFull.ToString(), 2);
    //                }
    //                catch (Exception err)
    //                {
    //                    Console.WriteLine(err.Message);
    //                    value = null;
    //                    return false;
    //                }
    //                value = intRet;
    //                return true;
    //            case PLCDataTypeEnum.Bit32Long:
    //                try
    //                {
    //                    longRet = Convert.ToInt64(x2StringSBFull.ToString(), 2);
    //                }
    //                catch (Exception err)
    //                {
    //                    Console.WriteLine(err.Message);
    //                    value = null;
    //                    return false;
    //                }
    //                value = longRet;
    //                return true;
    //            default:
    //                value = null;
    //                return false;
    //        }
    //    }
    //    /// <summary>
    //    /// 从已经读取到的buffer中获取指定的位置的buffer信息
    //    /// </summary>
    //    /// <param name="plcBuffer"></param>
    //    /// <param name="fullBuffer"></param>
    //    /// <param name="mainBufferOffset"></param>
    //    /// <returns></returns>
    //    protected bool PickupDataFromBuffer(PLCDataBufferInfo plcBuffer, byte [] fullBuffer , int mainBufferOffset, out object value)
    //    {
    //        if (fullBuffer == null || plcBuffer == null)
    //        {
    //            Console.WriteLine("给定的完整buffer或要读取的片段buffer的信息为空");
    //            value = null;
    //            return false;
    //        }
    //        int realIndex = plcBuffer.Index - mainBufferOffset;
    //        if (realIndex <0)
    //        {
    //            Console.WriteLine("将要读取的真实起始地址小于0");
    //            value = null;
    //            return false;
    //        }
    //        int readCount = 0;
    //        switch (plcBuffer.DataType)
    //        {
    //            case PLCDataTypeEnum.Bit16Int:
    //                readCount = 2;
    //                break;
    //            case PLCDataTypeEnum.Bit32Long:
    //                readCount = 4;
    //                break;
    //            default:
    //                break;
    //        }
    //        if ((realIndex + readCount) > fullBuffer.Length)
    //        {
    //            Console.WriteLine("将要读取的数据超出源数据的宽度");
    //            value = null;
    //            return false;
    //        }
    //        StringBuilder x2Sb = new StringBuilder();
    //        for (int i = 0; i < readCount; i++)
    //        {
    //            int currentRealIndex = realIndex + i;
    //            byte currentByte = fullBuffer[currentRealIndex];
    //            string currentX2 = Convert.ToString(currentByte, 2);
    //            string currentX2Full = currentX2.PadLeft(8, '0');
    //            x2Sb.Append(currentX2Full);
    //        }
    //        if (x2Sb.Length == 0)
    //        {
    //            value = null;
    //            return false;
    //        }
    //        else
    //        {
    //            switch (plcBuffer.DataType)
    //            {
    //                case PLCDataTypeEnum.Bit16Int:
    //                    value = Convert.ToInt32(x2Sb.ToString(), 2);
    //                    break;
    //                case PLCDataTypeEnum.Bit32Long:
    //                    value = Convert.ToInt64(x2Sb.ToString(), 2);
    //                    break;
    //                default:
    //                    value = null;
    //                    break;
    //            }
    //            return true;
    //        }
    //    }
    //    protected bool Write<T>(PLCDataBufferInfo plcBuffer,object value)
    //    {
    //        string x2stringFull = null;
    //        byte[] buffer = null;
    //        switch (plcBuffer.DataType)
    //        {
    //            case PLCDataTypeEnum.Bit16Int:
    //                //16位的话 用两个byte,把要输入的内容 转换成二进制  然后左边用0补全 再拆分成8位一组,每一组的内容转换成byte 写入到buffer
    //                x2stringFull = Convert.ToString(Convert.ToInt32(value), 2).PadLeft(16, '0');
    //                buffer = new byte[2];
    //                break;
    //            case PLCDataTypeEnum.Bit32Long:
    //                x2stringFull = Convert.ToString(Convert.ToInt64(value), 2).PadLeft(32, '0');
    //                buffer = new byte[4];
    //                break;
    //            default:
    //                break;
    //        }
    //        #region 把字符串每8位一组,变成二进制 然后再把那个二进制转换成byte
    //        for (int i = 0; i < buffer.Length; i++)
    //        {
    //            string current8bit = x2stringFull.Substring(i * 8, 8);
    //            byte val = Convert.ToByte(current8bit, 2);
    //            buffer[i] = val;
    //        }
    //        #endregion
    //        //S7Client.S7DataItem sm = new S7Client.S7DataItem();

    //        //s7client.AsDBWrite()
    //        int writeRet = s7client.DBWrite(setting.DBNumber, plcBuffer.Index, buffer.Length, buffer);
    //        if (writeRet != 0)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //    /// <summary>
    //    /// 获取要写入的buffer的值,用于连接到buffer中.
    //    /// </summary>
    //    /// <param name="src">要连接的目标 如果没有 就填写null 就会生成一个新的buffer数组</param>
    //    /// <param name="plcBuffer"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    protected List<byte> GetWaitWriteBuffer(PLCDataBufferInfo plcBuffer, object value)
    //    {
    //        string x2stringFull = null;
    //        byte[] buffer = null;
    //        switch (plcBuffer.DataType)
    //        {
    //            case PLCDataTypeEnum.Bit16Int:
    //                //16位的话 用两个byte,把要输入的内容 转换成二进制  然后左边用0补全 再拆分成8位一组,每一组的内容转换成byte 写入到buffer
    //                x2stringFull = Convert.ToString(Convert.ToInt32(value), 2).PadLeft(16, '0');
    //                buffer = new byte[2];
    //                break;
    //            case PLCDataTypeEnum.Bit32Long:
    //                x2stringFull = Convert.ToString(Convert.ToInt64(value), 2).PadLeft(32, '0');
    //                buffer = new byte[4];
    //                break;
    //            default:
    //                break;
    //        }
    //        #region 把字符串每8位一组,变成二进制 然后再把那个二进制转换成byte
    //        for (int i = 0; i < buffer.Length; i++)
    //        {
    //            string current8bit = x2stringFull.Substring(i * 8, 8);
    //            byte val = Convert.ToByte(current8bit, 2);
    //            buffer[i] = val;
    //        }
    //        #endregion
    //        List<byte> srcList = new List<byte>(buffer);
    //        return srcList;
    //    }
    //    #endregion
    //    /// <summary>
    //    /// 获取药机当前是否正在付药的状态
    //    /// </summary>
    //    /// <returns></returns>
    //    public PLCStatusData GetMedicineGettingStatus()
    //    {
    //        PLCStatusData status = new PLCStatusData();
    //        try
    //        {
    //            object main, x, y, grabber, times;
    //            //新加入的6个只读变量 全都是int true为100, false为0;
    //            object commandCount, counterCoverdErr, counterGettedMedicinesCount, medicinesHasBeenTaked, printAndSnapshot;// unuse124;
    //            #region 一次从100读到133的方式
    //            int size = 34;
    //            byte[] buffer = new byte[size];
    //            //从s7中读取数据
    //            s7client.DBRead(setting.DBNumber, setting.PLCDataIndex.Main.Index,size, buffer);

    //            ///从读取到的buffer中读取所有的要用到的数据,转换出来
    //            int offset = setting.PLCDataIndex.Main.Index;
    //            PickupDataFromBuffer(setting.PLCDataIndex.Main, buffer, offset, out main);
    //            PickupDataFromBuffer(setting.PLCDataIndex.X, buffer, offset, out x);
    //            PickupDataFromBuffer(setting.PLCDataIndex.Y, buffer, offset, out y);
    //            PickupDataFromBuffer(setting.PLCDataIndex.Grabber, buffer, offset, out grabber);
    //            PickupDataFromBuffer(setting.PLCDataIndex.Times, buffer, offset, out times);
    //            PickupDataFromBuffer(setting.PLCDataIndex.CommandCount, buffer, offset, out commandCount);
    //            PickupDataFromBuffer(setting.PLCDataIndex.CounterCoverdError, buffer, offset, out counterCoverdErr);
    //            PickupDataFromBuffer(setting.PLCDataIndex.CounterGetedMedicinesCount, buffer, offset, out counterGettedMedicinesCount);
    //            PickupDataFromBuffer(setting.PLCDataIndex.MedicinesHasBeenTaked, buffer, offset, out medicinesHasBeenTaked);
    //            PickupDataFromBuffer(setting.PLCDataIndex.PrintAndSnapshot, buffer, offset, out printAndSnapshot);
    //            //PickupDataFromBuffer(setting.PLCDataIndex.Main, buffer, setting.PLCDataIndex.Unuse124.Index, out unuse124);
    //            #endregion
    //            #region 逐一读取方式
    //            //Read<int>(setting.PLCDataIndex.Main, out main);
    //            //Read<long>(setting.PLCDataIndex.X, out x);
    //            //Read<long>(setting.PLCDataIndex.Y, out y);
    //            //Read<int>(setting.PLCDataIndex.Grabber, out grabber);
    //            //Read<int>(setting.PLCDataIndex.Times, out times);
    //            //#region 新加入的6个变量
    //            //Read<int>(setting.PLCDataIndex.CommandCount, out commandCount);
    //            //Read<int>(setting.PLCDataIndex.CounterCoverdError, out counterCoverdErr);
    //            //Read<int>(setting.PLCDataIndex.CounterGetedMedicinesCount, out counterGettedMedicinesCount);
    //            //Read<int>(setting.PLCDataIndex.MedicinesHasBeenTaked, out medicinesHasBeenTaked);
    //            //Read<int>(setting.PLCDataIndex.PrintAndSnapshot, out printAndSnapshot);
    //            //Read<int>(setting.PLCDataIndex.Unuse124, out unuse124);
    //            //#endregion
    //            #endregion
    //            status.Main = (MainBufferValuesEnum)Enum.Parse(typeof(MainBufferValuesEnum), string.Format("{0}", main));
    //            status.X = (long)x;
    //            status.Y = (long)y;
    //            status.Grabber = (WhichGrabberEnum)Enum.Parse(typeof(WhichGrabberEnum), string.Format("{0}", grabber));
    //            status.Times = (int)times;
    //            //新增6个变量中 除了保留的124以外都转换
    //            status.CommandCount = Convert.ToInt32(commandCount);
    //            status.CounterCoverdError = (Convert.ToInt32(counterCoverdErr)) == 100;
    //            status.CounterGetedMedicinesCount = Convert.ToInt32(counterGettedMedicinesCount);
    //            status.PrintAndSnapshot = (Convert.ToInt32(printAndSnapshot)) == 100;
    //            status.MedicinesHasBeenTaked = (Convert.ToInt32(medicinesHasBeenTaked)) == 100;
    //        }
    //        catch (Exception err)
    //        {
    //            Console.WriteLine(string.Format("获取付药机的状态失败{0}",err.Message));
    //        }

    //        //s7client.Disconnect();
    //        return status;
    //    }

    //    /// <summary>
    //    /// 连接plc
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool Connect()
    //    {
    //        Console.ForegroundColor = ConsoleColor.Yellow;
    //        Console.WriteLine("调用连接函数:ip:{0} rackIndex:{1} slotIndex:{2}", setting.IPAddress, setting.RackIndex, setting.SlotIndex);
    //        Console.ResetColor();
    //        int connectRet = s7client.ConnectTo(setting.IPAddress, setting.RackIndex, setting.SlotIndex);
    //        if (connectRet != 0)
    //        {
    //            throw new NotImplementedException(string.Format("connect to s7 error \r\n{0}", setting.IPAddress));
    //        }
    //        return connectRet == 0;
    //    }
    //    /// <summary>
    //    /// 断开plc的连接
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool Disconnect()
    //    {
    //        return s7client.Disconnect() == 0;
    //    }
    //}
    #endregion
    public class PLCCommunicatorTD : IPLCCommunicator
    {
        public PLCLibEnum PLCLib { get; set; }
        protected AMDM_PLCSettingTD setting = null;
        protected ushort puttingBufferRealStartIndex = 0;
        protected ushort gettingBufferRealStartIndex = 0;
        protected SerialPort modbus485Port = null;
        protected Socket modbusTCPSocketPort = null;
        protected IModbusMaster modbusClient = null;
        //protected IModbusSerialMaster modbusClient = null;

        int maxRetryTime = 5;
        int retryDelay = 997;
        ModbusFactory mf = new ModbusFactory();

        ModbusClient easyClient = null;
        PLCCommunicatorTD proxy = null;

        public PLCCommunicatorTD(AMDM_PLCSettingTD setting, PLCCommunicatorTD proxy)
        {
            if (proxy != null)
            {
                this.proxy = proxy;
                return;
            }
            #region 如果不使用代理(主控PLC)才初始化

            this.setting = setting;
            this.PLCLib = this.setting.PLCLib;
            switch (this.PLCLib)
            {
                case PLCLibEnum.Unknow:
                    break;
                case PLCLibEnum.S7Net:
                    break;
                case PLCLibEnum.EasyModbusTCP:
                    this.easyClient = new ModbusClient(this.setting.TCPModeIP, this.setting.TCPModePort);
                    break;
                case PLCLibEnum.EasyModbusSerialPort:
                    this.easyClient = new ModbusClient(setting.SerialPortName);
                    this.easyClient.Baudrate = setting.Baudrate;
                    this.easyClient.Parity = setting.Parity;
                    this.easyClient.DataBits = 8;
                    this.easyClient.StopBits = setting.StopBits;
                    this.easyClient.RtsEnable = setting.RtsEnable;
                    break;
                case PLCLibEnum.NModbusTCP:
                    //这里一定要进行一个特殊的socket设置,布药使用tcpclient 好像有问题 或者是没设置正确 要使用socket
                    this.modbusTCPSocketPort = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    this.modbusClient = mf.CreateMaster(modbusTCPSocketPort);
                    break;
                case PLCLibEnum.NModbusSerialPort:
                    GetSerialPort();
                    GetModbusClient();
                    break;
                default:
                    break;
            }
            #endregion
        }
        public bool Connected { get {
            if (proxy!=null)
            { return proxy.Connected; }
            switch (this.PLCLib)
            {
                case PLCLibEnum.Unknow:
                    return false;
                case PLCLibEnum.S7Net:
                    return false;
                case PLCLibEnum.EasyModbusTCP:
                    return this.easyClient.Connected;
                case PLCLibEnum.EasyModbusSerialPort:
                    return this.easyClient.Connected;
                case PLCLibEnum.NModbusTCP:
                    return this.modbusTCPSocketPort.Connected;
                case PLCLibEnum.NModbusSerialPort:
                    return this.modbus485Port == null || this.modbus485Port.IsOpen == false;
                default:
                    return false;
            }
        } }
        #region 台达PLC的工具方法
        ushort GetAddress(PLCDataBufferInfo plcBuffer)
        {
            ushort ret = 0;
            ret += this.setting.DataBufferStartAddress;
            ret += Convert.ToUInt16(plcBuffer.Index);
            return ret;
        }
        #endregion
        #region 内部函数 快速的访问和写寄存器
        //public bool Read<T>(PLCDataBufferInfo plcBuffer, out object value)
        //{
        //    string stringRet = null;
        //    int intRet = 0;
        //    long longRet = 0;
        //    string x2StringRet = null;
        //    int readRet = -1;


        //    StringBuilder readRetSB = new StringBuilder();
        //    StringBuilder x2StringSBFull = new StringBuilder();
        //    StringBuilder readBufferAsString = new StringBuilder();

        //    ushort[] buffer = null;
        //    switch (plcBuffer.DataType)
        //    {
        //        case PLCDataTypeEnum.Bit16Int:
        //            buffer = new ushort[2];
        //            break;
        //        case PLCDataTypeEnum.Bit32Long:
        //            buffer = new ushort[4];
        //            break;
        //        default:
        //            break;
        //    }
        //    buffer = modbusClient.ReadHoldingRegisters(this.setting.DeviceAddress, GetAddress(plcBuffer),Convert.ToUInt16(buffer.Length));
        //    //readRet = s7client.DBRead(this.setting.DBNumber, plcBuffer.Index, buffer.Length, buffer);
        //    if (readRet != 0)
        //    {
        //        value = null;
        //        return false;
        //    }

        //    for (int i = 0; i < buffer.Length; i++)
        //    {
        //        if (i >= buffer.Length)
        //        {
        //            break;
        //        }
        //        if (readRetSB.Length > 0)
        //        {
        //            readRetSB.Append("\r\n");
        //        }
        //        readRetSB.AppendFormat("{0} ({1}):{2}", i, i + plcBuffer.Index, buffer[i]);
        //        string currentX2 = Convert.ToString(Convert.ToInt32(buffer[i]), 2).PadLeft(8, '0');
        //        x2StringSBFull.Append(currentX2);
        //        readBufferAsString.Append(Convert.ToChar(buffer[i]));
        //    }
        //    stringRet = readRetSB.ToString();

        //    #region 显示二进制和通过8位一组重新组合到10进制的信息

        //    x2StringRet = x2StringSBFull.ToString();

        //    #endregion

        //    switch (plcBuffer.DataType)
        //    {
        //        case PLCDataTypeEnum.Bit16Int:
        //            try
        //            {
        //                intRet = Convert.ToInt32(x2StringSBFull.ToString(), 2);
        //            }
        //            catch (Exception err)
        //            {
        //                Console.WriteLine(err.Message);
        //                value = null;
        //                return false;
        //            }
        //            value = intRet;
        //            return true;
        //        case PLCDataTypeEnum.Bit32Long:
        //            try
        //            {
        //                longRet = Convert.ToInt64(x2StringSBFull.ToString(), 2);
        //            }
        //            catch (Exception err)
        //            {
        //                Console.WriteLine(err.Message);
        //                value = null;
        //                return false;
        //            }
        //            value = longRet;
        //            return true;
        //        default:
        //            value = null;
        //            return false;
        //    }
        //}
        /// <summary>
        /// 从已经读取到的buffer中获取指定的位置的buffer信息
        /// </summary>
        /// <param name="plcBuffer"></param>
        /// <param name="fullBuffer"></param>
        /// <param name="mainBufferOffset"></param>
        /// <returns></returns>
        //protected bool PickupDataFromBuffer(PLCDataBufferInfo plcBuffer, byte[] fullBuffer, int mainBufferOffset, out object value)
        //{
        //    if (fullBuffer == null || plcBuffer == null)
        //    {
        //        Console.WriteLine("给定的完整buffer或要读取的片段buffer的信息为空");
        //        value = null;
        //        return false;
        //    }
        //    int realIndex = plcBuffer.Index - mainBufferOffset;
        //    if (realIndex < 0)
        //    {
        //        Console.WriteLine("将要读取的真实起始地址小于0");
        //        value = null;
        //        return false;
        //    }
        //    int readCount = 0;
        //    switch (plcBuffer.DataType)
        //    {
        //        case PLCDataTypeEnum.Bit16Int:
        //            readCount = 2;
        //            break;
        //        case PLCDataTypeEnum.Bit32Long:
        //            readCount = 4;
        //            break;
        //        default:
        //            break;
        //    }
        //    if ((realIndex + readCount) > fullBuffer.Length)
        //    {
        //        Console.WriteLine("将要读取的数据超出源数据的宽度");
        //        value = null;
        //        return false;
        //    }
        //    StringBuilder x2Sb = new StringBuilder();
        //    for (int i = 0; i < readCount; i++)
        //    {
        //        int currentRealIndex = realIndex + i;
        //        byte currentByte = fullBuffer[currentRealIndex];
        //        string currentX2 = Convert.ToString(currentByte, 2);
        //        string currentX2Full = currentX2.PadLeft(8, '0');
        //        x2Sb.Append(currentX2Full);
        //    }
        //    if (x2Sb.Length == 0)
        //    {
        //        value = null;
        //        return false;
        //    }
        //    else
        //    {
        //        switch (plcBuffer.DataType)
        //        {
        //            case PLCDataTypeEnum.Bit16Int:
        //                value = Convert.ToInt32(x2Sb.ToString(), 2);
        //                break;
        //            case PLCDataTypeEnum.Bit32Long:
        //                value = Convert.ToInt64(x2Sb.ToString(), 2);
        //                break;
        //            default:
        //                value = null;
        //                break;
        //        }
        //        return true;
        //    }
        //}
        //public bool Write<T>(PLCDataBufferInfo plcBuffer, object value)
        //{
        //    string x2stringFull = null;
        //    ushort[] buffer = null;
        //    switch (plcBuffer.DataType)
        //    {
        //        case PLCDataTypeEnum.Bit16Int:
        //            //16位的话 用两个byte,把要输入的内容 转换成二进制  然后左边用0补全 再拆分成8位一组,每一组的内容转换成byte 写入到buffer
        //            x2stringFull = Convert.ToString(Convert.ToInt32(value), 2).PadLeft(16, '0');
        //            buffer = new ushort[2];
        //            break;
        //        case PLCDataTypeEnum.Bit32Long:
        //            x2stringFull = Convert.ToString(Convert.ToInt64(value), 2).PadLeft(32, '0');
        //            buffer = new ushort[4];
        //            break;
        //        default:
        //            break;
        //    }
        //    #region 把字符串每8位一组,变成二进制 然后再把那个二进制转换成byte
        //    for (int i = 0; i < buffer.Length; i++)
        //    {
        //        string current8bit = x2stringFull.Substring(i * 8, 8);
        //        byte val = Convert.ToByte(current8bit, 2);
        //        buffer[i] = val;
        //    }
        //    #endregion
        //    //S7Client.S7DataItem sm = new S7Client.S7DataItem();

        //    //s7client.AsDBWrite()
        //    modbusClient.WriteMultipleRegisters(this.setting.DeviceAddress, GetAddress(plcBuffer), buffer);
        //    //int writeRet = s7client.DBWrite(setting.DBNumber, plcBuffer.Index, buffer.Length, buffer);
        //    //if (writeRet != 0)
        //    //{
        //    //    return false;
        //    //}
        //    return true;
        //}
        /// <summary>
        /// 获取要写入的buffer的值,用于连接到buffer中.
        /// </summary>
        /// <param name="src">要连接的目标 如果没有 就填写null 就会生成一个新的buffer数组</param>
        /// <param name="plcBuffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        List<byte> GetWaitWriteBuffer(PLCDataBufferInfo plcBuffer, object value)
        {
            string x2stringFull = null;
            byte[] buffer = null;
            switch (plcBuffer.DataType)
            {
                case PLCDataTypeEnum.Bit16Int:
                    //16位的话 用两个byte,把要输入的内容 转换成二进制  然后左边用0补全 再拆分成8位一组,每一组的内容转换成byte 写入到buffer
                    x2stringFull = Convert.ToString(Convert.ToInt32(value), 2).PadLeft(16, '0');
                    buffer = new byte[2];
                    break;
                case PLCDataTypeEnum.Bit32Long:
                    x2stringFull = Convert.ToString(Convert.ToInt64(value), 2).PadLeft(32, '0');
                    buffer = new byte[4];
                    break;
                default:
                    break;
            }
            #region 把字符串每8位一组,变成二进制 然后再把那个二进制转换成byte
            for (int i = 0; i < buffer.Length; i++)
            {
                string current8bit = x2stringFull.Substring(i * 8, 8);
                byte val = Convert.ToByte(current8bit, 2);
                buffer[i] = val;
            }
            #endregion
            List<byte> srcList = new List<byte>(buffer);
            return srcList;
        }
        #endregion
        

        /// <summary>
        /// 连接plc
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (proxy != null)
            { return proxy.Connect(); }
            switch (this.PLCLib)
            {
                case PLCLibEnum.Unknow:
                    break;
                case PLCLibEnum.S7Net:
                    break;
                case PLCLibEnum.EasyModbusTCP:
                case PLCLibEnum.EasyModbusSerialPort:
                    if (this.easyClient.Connected)
                    {
                        return true;
                    }
                    else
                    {
                        try
                        {
                            this.easyClient.Connect();
                        }
                        catch (Exception err)
                        {
                            Utils.LogError("使用easyclient连接失败:", err.Message);
                        }
                        return this.easyClient.Connected;
                    }
                case PLCLibEnum.NModbusTCP:
                    if (this.modbusTCPSocketPort.Connected)
                    {
                        return true;
                    }
                    else
                    {
                        try
                        {
                            this.modbusTCPSocketPort.Connect(this.setting.TCPModeIP, this.setting.TCPModePort);
                        }
                        catch (Exception err)
                        {
                            Utils.LogError("连接PLC失败:", err.Message);
                            this.modbusTCPSocketPort = new Socket(SocketType.Stream, ProtocolType.Tcp);
                            this.modbusTCPSocketPort.Connect(this.setting.TCPModeIP, this.setting.TCPModePort);
                            this.modbusClient = mf.CreateMaster(modbusTCPSocketPort);
                        }
                        return this.modbusTCPSocketPort.Connected;
                    }
                case PLCLibEnum.NModbusSerialPort:
                    if (this.modbus485Port.IsOpen == true)
                    {
                        return true;
                    }
                    this.modbus485Port.Open();
                    return this.modbus485Port.IsOpen;
                default:
                    break;
            }
            return false;
        }
        /// <summary>
        /// 获取串口对象,没有时自动初始化
        /// </summary>
        /// <returns></returns>
        internal SerialPort GetSerialPort()
        {
            if (this.modbus485Port == null)
            {
                this.modbus485Port = new SerialPort(this.setting.SerialPortName, this.setting.Baudrate, this.setting.Parity, this.setting.BitsCount);
                this.modbus485Port.StopBits = this.setting.StopBits;
                this.modbus485Port.RtsEnable = this.setting.RtsEnable;
            }
            return modbus485Port;
        }
        /// <summary>
        /// 获取modbus对象,没有时自动初始化
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        internal IModbusMaster GetModbusClient(SerialPort exeistPort = null)
        {
            if (this.modbusClient == null)
            {
                ModbusFactory fc = new ModbusFactory();
                modbusClient = ModbusFactoryExtensions.CreateRtuMaster(fc, exeistPort == null? this.modbus485Port: exeistPort);
                this.puttingBufferRealStartIndex = (ushort)(this.setting.PuttingBufferStartIndex + this.setting.DataBufferStartAddress);
                this.gettingBufferRealStartIndex = (ushort)(this.setting.GettingBufferStartIndex + this.setting.DataBufferStartAddress);
            }
            return this.modbusClient;
        }
        /// <summary>
        /// 断开plc的连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (proxy != null)
            { return proxy.Disconnect(); }
            switch (this.PLCLib)
            {
                case PLCLibEnum.Unknow:
                    return false;
                case PLCLibEnum.S7Net:
                    return false;
                case PLCLibEnum.EasyModbusTCP:
                case PLCLibEnum.EasyModbusSerialPort:
                    this.easyClient.Disconnect();
                    return true;
                case PLCLibEnum.NModbusTCP:
                    this.modbusTCPSocketPort.Disconnect(true);
                    return true;
                case PLCLibEnum.NModbusSerialPort:
                    this.modbus485Port.Close();
                    return true;
                default:
                    return false;
            }
        }


        #region nmodbus
        public void WriteMultipleRegisters(ushort[] data, ushort startOffset = 0)
        {
            if (proxy != null)
            { proxy.WriteMultipleRegisters(data,startOffset);
            return;
            }
            if (this.Connected == false)
            {
                this.Connect();
            }
            int currentTime = 0;
            while (currentTime<this.maxRetryTime)
            {
                try
                {
                    var bufferStartAddress = (ushort)(this.setting.DataBufferStartAddress + this.setting.PuttingBufferStartIndex + startOffset);
                    switch (this.PLCLib)
                    {
                        case PLCLibEnum.Unknow:
                            string err = "未指定有效的PLC连接库类型.请检查配置文件";
                            Utils.LogBug(err, this.setting);
                            throw new NotImplementedException();
                        case PLCLibEnum.S7Net:
                            break;
                        case PLCLibEnum.EasyModbusTCP:
                        case PLCLibEnum.EasyModbusSerialPort:
                            List<int> buffer = new List<int>();
                            foreach (var d in data)
                            {
                                buffer.Add(d);
                            }
                            this.easyClient.WriteMultipleRegisters(bufferStartAddress, buffer.ToArray());
                            break;
                        case PLCLibEnum.NModbusTCP:
                        case PLCLibEnum.NModbusSerialPort:
                           var task = this.modbusClient.WriteMultipleRegistersAsync(this.setting.DeviceAddress, bufferStartAddress, data);
                            bool success = task.Wait(523);
                            if (success == true)
                            {
                                return;
                            }
                            else
                            {
                                currentTime++;
                                Utils.LogInfo("写保持寄存器失败,即将休息997毫秒后再尝试.当前尝试次数", currentTime);
                                System.Threading.Thread.Sleep(997);
                            }
                            var ret = this.modbusClient.ReadHoldingRegisters(this.setting.DeviceAddress, bufferStartAddress, 10);
                            break;
                        default:
                            break;
                    }
                    break;
                }
                catch (Exception err)
                {
                    currentTime++;
                    Utils.LogError("在台达PLC写保持寄存器操作中发生错误.错误消息和当前已尝试次数分别为:", err.Message, currentTime);
                    System.Threading.Thread.Sleep(this.retryDelay);
                    continue;
                }
            }
        }
        bool reInitingNModbus = false;
        bool reading = false;
        public ushort[] ReadHoldingRegisters(ushort numberOfPoints, int startOffset = 0)
        {
            if (proxy != null)
            { return proxy.ReadHoldingRegisters(numberOfPoints, startOffset); }
            if (reading)
            {
                Utils.LogError("上一个请求正在进行中");
                throw new NotImplementedException("台达PLC只能通过消息队列形式发送消息.");
            }
            if (reInitingNModbus)
            {
                Utils.LogInfo("正在断开连接重连,本次读取操作跳过");
                return null;
            }
            reading = true;
            if (this.Connected == false)
            {
                this.Connect();
            }
            int currentTime = 0;
            var startAddress = (ushort)(this.setting.DataBufferStartAddress + this.setting.GettingBufferStartIndex + startOffset);
            while (currentTime < this.maxRetryTime)
            {
                
                //return this.modbusClient.ReadHoldingRegisters(100, 4297, numberOfPoints);
                try
                {
                    switch (this.PLCLib)
                    {
                        case PLCLibEnum.Unknow:
                        case PLCLibEnum.S7Net:
                            throw new NotImplementedException("PLCCommunicator读取保持寄存器错误,未能匹配有效的连接库");
                        case PLCLibEnum.EasyModbusTCP:
                        case PLCLibEnum.EasyModbusSerialPort:
                            var retIntArr = this.easyClient.ReadHoldingRegisters(startAddress, numberOfPoints);
                            List<ushort> retUshortArr = new List<ushort>();
                            foreach (var i in retIntArr)
                            {
                                retUshortArr.Add((ushort)i);
                            }
                            reading = false;
                            return retUshortArr.ToArray();
                        case PLCLibEnum.NModbusTCP:
                        case PLCLibEnum.NModbusSerialPort:
                            var task = this.modbusClient.ReadHoldingRegistersAsync(this.setting.DeviceAddress, startAddress, numberOfPoints);
                            bool success = task.Wait(1000);
                            if (success)
                            {
                                reading = false;
                                return task.Result;
                            }
                            else
                            {
                                throw new NotImplementedException("使用NModbus获取数据时未获取到返回结果,触发重连");
                            }
                        default:
                            Utils.LogBug("无法确认使用的PLC连接引擎");
                            reading = false;
                            return null;
                    }
                }
                catch (Exception err)
                {
                    currentTime++;
                    Utils.LogError("在台达PLC读保持寄存器时发生错误,错误消息和已尝试次数分别为:", err.Message, currentTime);
                    System.Threading.Thread.Sleep(this.retryDelay);
                    this.reConnect(97);
                }
            }

            Utils.LogBug("本条命令没有成功执行!!!!!!!!!!!开始地址和读取数量:", startAddress, numberOfPoints);
            reading = false;
            return null;
        }
        #endregion

        private void reConnect(int delay)
        {
            Utils.LogError(string.Format("{0}客户端读取数据发生错误,现在断线,{1}毫秒后重连", this.PLCLib.ToString(), delay));
            reInitingNModbus = true;
            switch (this.PLCLib)
            {
                case PLCLibEnum.Unknow:
                    break;
                case PLCLibEnum.S7Net:
                    break;
                case PLCLibEnum.EasyModbusTCP:
                case PLCLibEnum.EasyModbusSerialPort:
                    this.easyClient.Disconnect();
                    System.Threading.Thread.Sleep(delay);
                    this.easyClient.Connect();
                    break;
                case PLCLibEnum.NModbusTCP:
                    if (this.modbusTCPSocketPort != null)
                    {
                        this.modbusTCPSocketPort.Disconnect(false);
                        this.modbusTCPSocketPort.Close();
                        this.modbusTCPSocketPort.Dispose();
                    }
                    if (this.modbusClient != null)
                    {
                        this.modbusClient.Dispose();
                    }
                    System.Threading.Thread.Sleep(delay);
                    this.modbusTCPSocketPort = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    this.modbusClient = mf.CreateMaster(modbusTCPSocketPort);
                    break;
                case PLCLibEnum.NModbusSerialPort:
                    if (this.modbus485Port != null)
                    {
                        this.modbus485Port.Close();
                        this.modbus485Port.Dispose();
                    }
                    if (this.modbusClient != null)
                    {
                        this.modbusClient.Dispose();
                    }
                    System.Threading.Thread.Sleep(delay);
                    GetSerialPort();
                    GetModbusClient();
                    break;
                default:
                    break;
            }
            reInitingNModbus = false;
        }
    }

}
