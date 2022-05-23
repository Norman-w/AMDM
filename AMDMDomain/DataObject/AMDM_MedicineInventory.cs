using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 药机内的当前库存信息
    /// </summary>
    public class AMDM_MedicineInventory : AMDM_Medicine
    {
        /// <summary>
        /// 药品的库存数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 2022年3月13日19:50:56  这个库存所在的弹夹是否被卡住了
        /// </summary>
        /// 2022年3月29日19:24:47 被注释掉  药槽可以卡住 药品不可以卡住
        //public bool Stuck { get; set; }
    }
}
