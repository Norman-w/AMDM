using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 入库单数据对象
    /// </summary>
    public class AMDM_InstockRecord_data
    {
        Int64 _id = 0;
        /// <summary>
        /// 入库单ID
        /// </summary>
        public Int64 Id { get { return _id; } set { _id = value; } }

        DateTime _createtime = DateTime.Now;
        /// <summary>
        /// 入库单的创建时间,也就是护士是从什么时候上药的
        /// </summary>
        public DateTime CreateTime { get { return _createtime; } set { _createtime = value; } }

        Int64 _machineid = 0;
        /// <summary>
        /// 给哪个机器进行的上药
        /// </summary>
        public Int64 MachineId { get { return _machineid; } set { _machineid = value; } }

        Int64 _stockid = 0;
        /// <summary>
        /// 给哪个药仓执行的上药
        /// </summary>
        public Int64 StockId { get { return _stockid; } set { _stockid = value; } }

        Int64 _nurseid = 0;
        /// <summary>
        /// 上药的护士的id
        /// </summary>
        public Int64 NurseID { get { return _nurseid; } set { _nurseid = value; } }

        Int32 _entriescount = 0;
        /// <summary>
        /// 入库单上一共有多少条数据
        /// </summary>
        public Int32 EntriesCount { get { return _entriescount; } set { _entriescount = value; } }

        Int32 _totalmedicinecount = 0;
        /// <summary>
        /// 入库单一共有多少件数
        /// </summary>
        public Int32 TotalMedicineCount { get { return _totalmedicinecount; } set { _totalmedicinecount = value; } }

        String _memo = null;
        /// <summary>
        /// 对整个入库单的一个备注
        /// </summary>
        public String Memo { get { return _memo; } set { _memo = value; } }

        String _type = null;
        /// <summary>
        /// 入库单的类型 比如是 正常上药,报损,记录遗失等等
        /// </summary>
        public String Type { get { return _type; } set { _type = value; } }

        DateTime _finishtime = DateTime.Now;
        /// <summary>
        /// 完成的时间,就是本次上药完成,关舱门的时间
        /// </summary>
        public DateTime FinishTime { get { return _finishtime; } set { _finishtime = value; } }

        bool _canceled = false;
        /// <summary>
        /// 是否作废
        /// </summary>
        public bool Canceled { get { return _canceled; } set { _canceled = value; } }
    }

    /// <summary>
    /// 入库单实体
    /// </summary>
    public class AMDM_InstockRecord: AMDM_InstockRecord_data
    {
        public List<AMDM_InstockRecordDetail> Details { get; set; }
    }
}
