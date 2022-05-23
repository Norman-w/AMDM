using System;
namespace AMDM_Domain
{
    interface IAMDM_PLCTCPSetting :IAMDM_PLCSetting
    {
        int DBNumber { get; set; }
        string IPAddress { get; set; }
        int RackIndex { get; set; }
        int SlotIndex { get; set; }
    }
}
