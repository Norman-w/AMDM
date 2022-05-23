using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{

    /// <summary>
    /// 维护状态
    /// </summary>
    public enum MaintenanceStatusEnum
    {
        运行正常,
        硬件故障,
        软件故障,
        系统维护中,
        紫外线杀菌中,
        紫外线杀菌准备中
    }

    /// <summary>
    /// 检测器检测到的故障枚举
    /// </summary>
    public enum MonitorDetectedErrorTypeEnum
    {
        硬件PLC连接失败,
        硬件PLC信号不良,
        硬件取药斗有残留,
        硬件数量检测光栅遮挡,
        /// <summary>
        /// 这也许不是个错误
        /// </summary>
        硬件机械手复位中,
        硬件打印机异常,
        硬件监控器异常,
        软件组件看门狗异常,
        软件组件数据库异常,
        软件组件HIS系统连接异常,
    }
}
