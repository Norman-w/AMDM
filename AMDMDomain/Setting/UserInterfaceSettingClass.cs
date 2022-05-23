using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 用户的交互设置
    /// </summary>
    public class UserInterfaceSettingClass
    {
        /// <summary>
        /// 当用户扫描了处方二维码但是没有进行点击取药的操作的话 多久自动消失那个提示框
        /// </summary>
        public int MedicineOrderAutoHideWhenNoActionMS { get; set; }

        /// <summary>
        /// 信息提示框 比如提示  该机药品库存不足之类的  提示完以后的默认关闭时间是多久
        /// </summary>
        public int NoticeShowerAutoHideMS { get; set; }
    }
}
