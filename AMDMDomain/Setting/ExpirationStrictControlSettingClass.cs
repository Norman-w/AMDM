using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 严控药品有效期模式设置
    //strict control expiration mode
    /// <summary>
    /// 严控有效期模式开关和相关的参数
    /// </summary>
    public class ExpirationStrictControlSettingClass
    {
        /// <summary>
        /// 严控有效期模式是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 默认的可以装入到药仓中的药品的最小有效期
        /// </summary>
        public int DefaultCanLoadMinExpirationDays { get; set; }
        /// <summary>
        /// 默认的建议装载到药仓中的药品的最小有效期
        /// </summary>
        public int DefaultSuggestLoadMinExpirationDays { get; set; }
        /// <summary>
        /// 默认的低于多少数量时候产生预警消息
        /// </summary>
        public int DefaultDaysThresholdOfExpirationAlert { get; set; }
    }
    #endregion
}
