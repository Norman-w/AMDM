using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 故障处置预案/方案类
    /// <summary>
    /// 故障处置方案设置
    /// </summary>
    public class TroubleshootingPlanSettingClass
    {
        /// <summary>
        /// 接收警告消息的用户集合
        /// </summary>
        public List<int> AlertReceiveUsers { get; set; }
        /// <summary>
        /// 在发生取药故障以后是否停用药机/卡药后进入故障状态
        /// </summary>
        public bool DisableAMDMWhenDeliveryFailed { get; set; }
    }
    #endregion
}
