using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 入库单的具体的每一条的相关信息
    /// </summary>
    public class AMDM_InstockRecordDetail
    {
        Int64 _id = 0;
        /// <summary>
        /// 自动生成的
        /// </summary>
        public Int64 Id { get { return _id; } set { _id = value; } }

        Int64 _parentid = 0;
        /// <summary>
        /// 入库单的ID,也就是这一条条目的所属ID
        /// </summary>
        public Int64 ParentId { get { return _parentid; } set { _parentid = value; } }

        Int32 _index = 0;
        /// <summary>
        /// 在整个入库单中的索引位置
        /// </summary>
        public Int32 Index { get { return _index; } set { _index = value; } }

        Int64 _medicineid = 0;
        /// <summary>
        /// 药品的id
        /// </summary>
        public Int64 MedicineId { get { return _medicineid; } set { _medicineid = value; } }

        String _medicinename = null;
        /// <summary>
        /// 药品的名称,记录的是药品当时的名称
        /// </summary>
        public String MedicineName { get { return _medicinename; } set { _medicinename = value; } }

        String _medicinebarcode = null;
        /// <summary>
        /// 药品的条码,记录的是当时的药品的条码
        /// </summary>
        public String MedicineBarcode { get { return _medicinebarcode; } set { _medicinebarcode = value; } }

        Int32 _count = 0;
        /// <summary>
        /// 采购的数量,因为考虑到可能有散货的采集可能性,比如1.1吨,此字段设置为浮点型,正常用的话浮点型的整数部分存储我们日常见过的数量足够了
        /// </summary>
        public Int32 Count { get { return _count; } set { _count = value; } }

        DateTime _instocktime = DateTime.Now;
        /// <summary>
        /// RecordTime
        /// </summary>
        public DateTime InstockTime { get { return _instocktime; } set { _instocktime = value; } }

        int _stockIndex = 0;
        /// <summary>
        /// 药品放在了哪个药仓当中
        /// </summary>
        public int StockIndex { get { return _stockIndex; } set { _stockIndex = value; } }

        /// <summary>
        /// 药品放在了哪一层
        /// </summary>
        public int FloorIndex { get; set; }

        /// <summary>
        /// 药品放在了哪一个格子
        /// </summary>
        public int GridIndex { get; set; }

        String _memo = null;
        /// <summary>
        /// 对这一条信息的备注
        /// </summary>
        public String Memo { get { return _memo; } set { _memo = value; } }
    }
}
