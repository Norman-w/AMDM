using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 时间节点控制器的设定 不包含药品库存  因为库存是取药触的报
    /// </summary>
    public class TimeSignalGeneratorSettingClass
    {
        /// <summary>
        /// 硬件的检测时间间隔毫秒
        /// </summary>
        public long HardwareDetectionPerTimeIntervalMS { get; set; }
        /// <summary>
        /// 软件部件 比如看门狗之类的每次检查的间隔时间 毫秒
        /// </summary>
        public long SoftwarePartDetectionPerTimeIntervalMS { get; set; }
        /// <summary>
        /// the interval ms between per time expiration date checking of medicines
        /// 每次检查药有效期的间隔时间 毫秒
        /// </summary>
        public long MedicineExpirationCheckingPerTimeIntervalMS { get; set; }

    }
}
