using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 调试设置
    /// <summary>
    /// 调试设置
    /// </summary>
    public class DebugSettingClass
    {
        public DebugSettingClass()
        {
            this.MedicineGettingTestPerGridGrubTimes = 1;
        }
        /// <summary>
        /// 忽略处方是否已经完成取药的检测
        /// </summary>
        public bool IgnoreDeliveriedChecking { get; set; }
        /// <summary>
        /// 忽略取药斗被遮挡检测,测试过程中由于药品不能及时被取出,所以药忽略这个字段
        /// </summary>
        public bool IgnoreMedicinesBulketCoverChecking { get; set; }
        /// <summary>
        /// 不执行真正的付药单打印,调试时为了省纸不用打印
        /// </summary>
        public bool SkipDeliveryRecordPaperRealPrinting { get; set; }
        /// <summary>
        /// 执行取药测试的时候,每一个格子取多少个药品
        /// </summary>
        public int MedicineGettingTestPerGridGrubTimes { get; set; }

        public bool LogVLCDebugInfo { get; set; }

        /// <summary>
        /// 是否隐藏初始化面板,如果是的话,初始化完了以后就隐藏黑屏幕,这样就能看到桌面,如果不隐藏,在取药等页面进行切换时,有可能会因为时间问题看到桌面.???
        /// </summary>
        public bool HidInitialzerFormOnFinishedInit { get; set; }

        /// <summary>
        /// 是否自动点击开始取药的按钮.
        /// </summary>
        public bool AutoClickStartMedicineGettingBtn { get; set; }
    }
    #endregion
}
