using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 日志服务器(看门狗的设置)
    /// <summary>
    /// 日志服务器(看门狗的设置类)
    /// </summary>
    public class LogServerServiceSettingClass
    {
        /// <summary>
        /// 每一次心跳的间隔时间多久
        /// </summary>
        public int PerHeartbeatsDelayMS { get; set; }

        /// <summary>
        /// 管道服务器的地址 默认的是.
        /// </summary>
        public string PipeServerLocation { get; set; }

        /// <summary>
        /// 管道服务器的名称,默认是LogServerService
        /// </summary>
        public string PipeServerName { get; set; }
    }
    #endregion
}
