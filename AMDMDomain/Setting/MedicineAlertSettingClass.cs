using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 药品有效期及库存预警通知设置
    /// <summary>
    /// 药品有效期及库存预警通知设置
    /// </summary>
    public class MedicineAlertSettingClass
    {
        /// <summary>
        /// 药品库存预警及药品有效期预警消息接收人的ID集合
        /// </summary>
        public List<int> LowInventoryAndExpirationAlertReceiveUsers { get; set; }
    }
    #endregion
}
