using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 广告视频
    /// <summary>
    /// 广告视频设置类
    /// </summary>
    public class AdvertVideosSettingClass
    {
        /// <summary>
        /// 播放速度的倍速.一般都是在测试的时候才会用到.
        /// </summary>
        public float SpeedRate { get; set; }
        /*
         * 后续可以加上一些设置,目前没有想到
         */
        /// <summary>
        /// 空闲时间播放的广告的总目录
        /// </summary>
        public string SpareTimeADVideosDir { get; set; }

        /// <summary>
        /// 正在取药中的广告视频的总目录
        /// </summary>
        public string MedicinesGettingADVideosDir { get; set; }

        /// <summary>
        /// 药品已经出仓等待用户取走药品时候播放的视频的文件夹
        /// </summary>
        public string MedicinesWaitBeenTakeADVideosDir { get; set; }

        /// <summary>
        /// 药品已经被取走,提示请核对药品时候的文件夹
        /// </summary>
        public string MedicinesHasBeenTakedAdVideosDir { get; set; }
    }
    #endregion
}
