using System;
namespace AMDM_Domain
{
    /// <summary>
    /// 提取接口2022年2月18日12:27:06
    /// </summary>
    interface IAMDM_Clip
    {
        /// <summary>
        /// 药槽和药品的绑定时间
        /// </summary>
        DateTime BindingTime { get; set; }
        /// <summary>
        /// 药槽和药品的绑定用户(谁指派的这个弹夹就一定要装这个药品)
        /// </summary>
        long BindingUserId { get; set; }
        /// <summary>
        /// 上次弹夹出弹的时间
        /// </summary>
        DateTime? LastDeliveryTime { get; set; }
        /// <summary>
        /// 上次弹夹的装弹时间
        /// </summary>
        DateTime? LastInstockTime { get; set; }
        /// <summary>
        /// 弹夹当前是否已经被卡住
        /// </summary>
        bool Stuck { get; set; }
    }
}
