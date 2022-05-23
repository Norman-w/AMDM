using System;
namespace AMDM_Domain
{
    interface IAMDM_PLCModbusSerialPortSetting :IAMDM_PLCSetting
    {
        int Baudrate { get; set; }
        int BitsCount { get; set; }
        ushort DataBufferStartAddress { get; set; }
        byte DeviceAddress { get; set; }
        ushort GettingBufferStartIndex { get; set; }
        bool IsMain { get; set; }
        System.IO.Ports.Parity Parity { get; set; }
        ushort PuttingBufferStartIndex { get; set; }
        bool RtsEnable { get; set; }
        string SerialPortName { get; set; }
        System.IO.Ports.StopBits StopBits { get; set; }
    }
}
