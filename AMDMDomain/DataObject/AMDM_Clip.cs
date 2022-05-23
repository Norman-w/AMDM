using System;
using System.Collections.Generic;

using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 格子上的药品绑定信息表 
    /// 原来的名字为GridMedicineBindingInfo2022年2月18日12:58:21 更名为
    /// AMDM_Clip_data,以这种可物化的概念来定义绑定关系.就像测算两个车之间的距离可以理解为连接两个车子的线的长度一个意思
    /// </summary>
    public class AMDM_Clip_data : AMDM_Domain.IAMDM_Clip
    {
        Int64 _id = 0;
        /// <summary>
        /// ID,自增量流水号
        /// </summary>
        public Int64 Id { get { return _id; } set { _id = value; } }

        Int32 _stockindex = 0;
        /// <summary>
        /// 所在药仓的索引
        /// </summary>
        public Int32 StockIndex { get { return _stockindex; } set { _stockindex = value; } }

        Int32 _floorindex = 0;
        /// <summary>
        /// 所在层索引
        /// </summary>
        public Int32 FloorIndex { get { return _floorindex; } set { _floorindex = value; } }

        Int32 _gridindex = 0;
        /// <summary>
        /// 所在格子的索引
        /// </summary>
        public Int32 GridIndex { get { return _gridindex; } set { _gridindex = value; } }

        long _medicineid = 0;
        /// <summary>
        /// 药品id
        /// </summary>
        public long MedicineId { get { return _medicineid; } set { _medicineid = value; } }

        String _medicinename = null;
        /// <summary>
        /// 药品名称
        /// </summary>
        public String MedicineName { get { return _medicinename; } set { _medicinename = value; } }

        String _medicinebarcode = null;
        /// <summary>
        /// 药品条码
        /// </summary>
        public String MedicineBarcode { get { return _medicinebarcode; } set { _medicinebarcode = value; } }

        DateTime _bindingtime = DateTime.Now;
        /// <summary>
        /// 绑定时间
        /// </summary>
        public DateTime BindingTime { get { return _bindingtime; } set { _bindingtime = value; } }

        long _bindinguserid = 0;
        /// <summary>
        /// 绑定人的ID
        /// </summary>
        public long BindingUserId { get { return _bindinguserid; } set { _bindinguserid = value; } }

        #region 2022年1月16日17:14:07 此刻起不使用此字段记录库存信息
        //Int32 _currentinventory = 0;
        ///// <summary>
        ///// 当前库存数量
        ///// </summary>
        //public Int32 CurrentInventory { get { return _currentinventory; } set { _currentinventory = value; } }
        #endregion


        //2021年11月12日19:09:15新增加的两个字段是 最后入库时间和最后交付药品的时间  方便用做药品交付的时候做负载均衡和 先进先出

        /// <summary>
        /// 最后的入库时间
        /// </summary>
        public Nullable<DateTime> LastInstockTime { get; set; }
        /// <summary>
        /// 最后的出库/付药时间
        /// </summary>
        public Nullable<DateTime> LastDeliveryTime { get; set; }

        /// <summary>
        /// 2022年2月16日10:34:40这个表已经可以理解为弹夹,该字段表示弹夹是否被卡住,是否卡药,如果取药过程中发生了卡药的现象,就不能再继续使用该药槽了.并且在前端也要显示这个药槽卡住了,需要重置后才能继续使用
        /// </summary>
        public bool Stuck { get; set; }
    }

    ///// <summary>
    ///// 药品的绑定信息,包含绑定的目标格子,目标层,目标药仓,目标药品
    ///// </summary>
    //public class GridMedicineBindingInfo : GridMedicineBindingInfo_data
    //{
    //    public AMDM_Grid DestGridRef { get; set; }
    //    public AMDM_Floor DestFloorRef { get; set; }
    //    public AMDM_Stock DestStockRef { get; set; }
    //    public MedicineInfo DestMedicineRef { get; set; }
    //}
    /// <summary>
    /// GridMedicineBindingInfo改名为 AMDM_Clip
    /// 2022年2月18日12:59:21
    /// </summary>
    public class AMDM_Clip : AMDM_Clip_data
    {
        List<AMDM_MedicineObject_data> _medicineobjects = new List<AMDM_MedicineObject_data>();
        /// <summary>
        /// 实体的药品信息列表(该格子下的)
        /// </summary>
        public List<AMDM_MedicineObject_data> MedicineObjects { get { return _medicineobjects; } set { _medicineobjects = value; } }
    }
}
