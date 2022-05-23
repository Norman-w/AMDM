using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 药品交付记录的明细,记录出药的机器,层,槽,以及药品名称,id,条码,时间,错误信息等
    /// </summary>
    public class AMDM_DeliveryRecordDetail_data
    {
        Int64 _id = 0;
        public Int64 Id { get { return _id; } set { _id = value; } }

        Int64 _parentid = 0;
        /// <summary>
        /// 交付记录单的id编号
        /// </summary>
        public Int64 ParentId { get { return _parentid; } set { _parentid = value; } }

        long _medicineid = 0;
        public long MedicineId { get { return _medicineid; } set { _medicineid = value; } }

        String _medicinename = null;
        public String MedicineName { get { return _medicinename; } set { _medicinename = value; } }

        String _medicinebarcode = null;
        public String MedicineBarcode { get { return _medicinebarcode; } set { _medicinebarcode = value; } }

        Int32 _count = 0;
        public Int32 Count { get { return _count; } set { _count = value; } }

        Int32 _stockindex = 0;
        public Int32 StockIndex { get { return _stockindex; } set { _stockindex = value; } }

        Int32 _floorindex = 0;
        public Int32 FloorIndex { get { return _floorindex; } set { _floorindex = value; } }

        Int32 _gridindex = 0;
        public Int32 GridIndex { get { return _gridindex; } set { _gridindex = value; } }

        DateTime _starttime;
        public DateTime StartTime { get { return _starttime; } set { _starttime = value; } }

        DateTime _endtime = DateTime.Now;
        public DateTime EndTime { get { return _endtime; } set { _endtime = value; } }

        bool _iserror = false;
        public bool IsError { get { return _iserror; } set { _iserror = value; } }

        String _errmsg = null;
        public String ErrMsg { get { return _errmsg; } set { _errmsg = value; } }

        #region 新增的  药品实体的id
        ///// <summary>
        ///// 药品实体记录的id  2022年1月16日10:56:08 添加
        ///// </summary>
        //public Nullable<long> MedicineObjectId { get; set; }
        #endregion
    }

    /// <summary>
    /// 2022年1月6日10:21:13 取药记录中可以包含相关的取药记录图片
    /// </summary>
    public class AMDM_DeliveryRecordDetail : AMDM_DeliveryRecordDetail_data
    {
        #region 2022年1月6日10:04:54  新增拓展字段,记录取药动作时捕获的图像
        /// <summary>
        /// 机械手取药动作执行之前,捕获的药仓后面的画面
        /// </summary>
        public Dictionary<long,AMDM_Snapshot> SnapFilesOfBeforeGetting { get; set; }
        /// <summary>
        /// 机械手取药动作执行之后,捕获的药仓后面的画面
        /// </summary>
        public Dictionary<long, AMDM_Snapshot> SnapFilesOfAfterGetting { get; set; }
        #endregion
    }
}
