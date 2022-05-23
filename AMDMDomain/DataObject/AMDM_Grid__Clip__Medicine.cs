using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 以药槽信息为主,包含弹夹信息和药品信息的数据
    /// </summary>
    public class AMDM_Grid__Clip__Medicine:AMDM_Grid_data,IAMDM_Grid_Ex, IAMDM_Clip, IAMDM_Medicine
    {
        /// <summary>
        /// 绑定时间
        /// </summary>
        public DateTime BindingTime { get; set; }
        /// <summary>
        /// 绑定人
        /// </summary>
        public long BindingUserId { get; set; }
        /// <summary>
        /// 上次付药的时间
        /// </summary>
        public Nullable<DateTime> LastDeliveryTime { get; set; }
        /// <summary>
        /// 上次入库的时间
        /// </summary>
        public Nullable<DateTime> LastInstockTime { get; set; }
        /// <summary>
        /// 弹夹是否已经卡药
        /// </summary>
        public bool Stuck { get; set; }
        /// <summary>
        /// 药品的条码
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// 药盒的高度
        /// </summary>
        public float BoxHeightMM { get; set; }
        /// <summary>
        /// 药盒的宽度
        /// </summary>
        public float BoxWidthMM { get; set; }
        /// <summary>
        /// 药盒的长度/进深
        /// </summary>
        public float BoxLongMM { get; set; }
        /// <summary>
        /// 药品的厂商
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// CountThresholdOfLowInventoryAlert低库存预警设定值,如果小于或等于这个数值时警报
        /// </summary>
        public Nullable<int> CTOLIA { get; set; }
        /// <summary>
        /// PercentThresholdOfLowInventoryAlert低库存预警设定百分比,如果小于或等于这个数值时警报
        /// </summary>
        public Nullable<float> PTOLIA { get; set; }

        public Nullable<int> CLMED { get; set; }
        public Nullable<int> SLMED { get; set; }
        public Nullable<int> DTOEA { get; set; }
        /// <summary>
        /// 在his系统中该药品的id
        /// </summary>
        public string IdOfHIS { get; set; }
        /// <summary>
        /// 药品的名称
        /// </summary>
        public string Name { get; set; }

        public float WidthMM { get; set; }

        public float HeightMM { get; set; }

        public float DepthMM { get; set; }

        public int MaxLoadAbleCount { get; set; }
    }
}
