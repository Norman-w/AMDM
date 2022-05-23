using System;
namespace AMDM_Domain
{
    public enum PLCLibEnum { Unknow, 
        /// <summary>
        /// 使用S7客户端
        /// </summary>
        S7Net,
        /// <summary>
        /// 使用EasyModbusTCP客户端透传模式(自己定义的)
        /// </summary>
        EasyModbusTCP, 
        /// <summary>
        /// 使用EasyModbus串口客户端
        /// </summary>
        EasyModbusSerialPort,
        /// <summary>
        /// 使用NModbusTCP客户端
        /// </summary>
        NModbusTCP, 
        /// <summary>
        /// 使用NModbus串口客户端,正常生产环境使用这个
        /// </summary>
        NModbusSerialPort }
    interface IAMDM_PLCSetting
    {
        PLCLibEnum PLCLib { get; set; }
        int PerMMPulseCount { get; set; }
    }
}
