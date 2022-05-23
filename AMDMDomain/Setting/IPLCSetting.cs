using System;
namespace AMDM_Domain
{
    interface IPLCSetting<MainPLCSettingTYPE, StockPLCSettingTYPE>
    {
        MainPLCSettingTYPE MainPLC { get; set; }
        System.Collections.Generic.Dictionary<int, StockPLCSettingTYPE> StocksPLC { get; set; }
        bool UseMainPLCSerialPort { get; set; }
    }
}
