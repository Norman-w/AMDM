using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM.Manager
{
    #region 单个药仓获取药品的时候发生的错误枚举
    /// <summary>
    /// 当单个药仓取药时,发生了何种的取药错误.
    /// </summary>
    public enum StockMedicinesGettingErrorEnum
    {
        Success,
        Unknown,
        光电计数与实际已抓取药品次数不符,
        PLC连接失败,
        PLC连接超时,
        获取PLC状态失败,
        获取PLC状态超时,
        传送带计数器光栅传感器被长时间遮挡,
        药仓忙碌时发送的取药信号不能被处理,
        单次取药已超过最大等待时间,
        机械手多次取药后未检测到药品掉落,
        //2022年3月2日16:49:57 新增 弃药回收超时
        弃药回收超时,
    }
    #endregion
    #region 药机发生了取药故障的时候的枚举
    /// <summary>
    /// 药机发生了取药故障时候的枚举,如果是分仓取药发生故障,详细的状态详见StockMedicinesGettingErrorEnum的值
    /// </summary>
    public enum AMDMMedicinesGettingErrorEnum
    {
        Unknow,
        分仓取药发生故障,
        付药单打印错误,
        付药凭图截取信号获取超时,
        等待药斗被清空信号获取超时,
        连接主控PLC超时,
        连接主控PLC错误,
        获取PLC状态失败,
        获取PLC状态超时,
        传送带计数器光栅传感器被长时间遮挡,
    }
    #endregion
}
