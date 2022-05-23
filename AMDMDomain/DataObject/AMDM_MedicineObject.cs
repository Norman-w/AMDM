using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 2022年1月15日18:14:46  在药仓里面装载的实际可交付的弹药/子弹/可交付的药品
    /// 每一盒药品一条数据
    /// </summary>
    public class AMDM_MedicineObject_data : IAMDM_MedicineObject_data
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AMDM_MedicineObject_data()
        {
            this.InStockTime = DateTime.MinValue;
        }
        /// <summary>
        /// 流水号
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 所在药仓索引
        /// </summary>
        public int StockIndex { get; set; }
        /// <summary>
        /// 所在层索引
        /// </summary>
        public int FloorIndex { get; set; }
        /// <summary>
        /// 所在格子索引
        /// </summary>
        public int GridIndex { get; set; }
        /// <summary>
        /// 药品的id
        /// </summary>
        public long MedicineId { get; set; }
        
        /// <summary>
        /// 入仓的时间,也就是本条记录创建的时间
        /// </summary>
        public DateTime InStockTime { get; set; }

        /// <summary>
        /// 属于哪个入库记录表入库进来的
        /// </summary>
        public long InStockRecordId { get; set; }

        /// <summary>
        /// 属于哪个出库记录表出去的
        /// </summary>
        public Nullable<long> OutStockRecordId { get; set; }
        /// <summary>
        /// 出仓的时间,可以不具备,出仓以后直接删除一条记录
        /// </summary>
        public Nullable<DateTime> OutStockTime { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public Nullable<DateTime> ProductionDate { get; set; }
        /// <summary>
        /// 截止日期/有效期
        /// </summary>
        public Nullable<DateTime> ExpirationDate { get; set; }
    }
    public class AMDM_MedicineObject : AMDM_MedicineObject_data
    {
        /// <summary>
        /// 这个子弹/药品所在的药槽
        /// </summary>
        public AMDM_Grid Grid { get; set; }
        /// <summary>
        /// 这个子弹的药品信息
        /// </summary>
        public AMDM_Medicine Medicine { get; set; }
    }
}
