using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 付药机信息.通常只在初始化时保存,并且在实际部署环节修改
    /// </summary>
    public class AMDM_Machine_data
    {
        Int32 _id = 0;
        /// <summary>
        /// 流水编号
        /// </summary>
        public Int32 Id { get { return _id; } set { _id = value; } }

        String _name = null;
        /// <summary>
        /// 付药机的名称
        /// </summary>
        public String Name { get { return _name; } set { _name = value; } }

        String _serialnumber = null;
        /// <summary>
        /// 付药机串号
        /// </summary>
        public String SerialNumber { get { return _serialnumber; } set { _serialnumber = value; } }

        String _hospitalname = null;
        /// <summary>
        /// 所在医院的名称
        /// </summary>
        public String HospitalName { get { return _hospitalname; } set { _hospitalname = value; } }

        Int32 _pharmacyid = 0;
        /// <summary>
        /// 所在药房的编号
        /// </summary>
        public Int32 PharmacyId { get { return _pharmacyid; } set { _pharmacyid = value; } }

        String _pharmacyname = null;
        /// <summary>
        /// 所在药房的名称
        /// </summary>
        public String PharmacyName { get { return _pharmacyname; } set { _pharmacyname = value; } }

        String _setupengineer = null;
        /// <summary>
        /// 部署该机器的工程师信息
        /// </summary>
        public String SetupEngineer { get { return _setupengineer; } set { _setupengineer = value; } }

        Nullable<DateTime> _setuptime = null;
        /// <summary>
        /// 部署时间
        /// </summary>
        public Nullable<DateTime> SetupTime { get { return _setuptime; } set { _setuptime = value; } }

        /// <summary>
        /// 药机的生产日期
        /// </summary>
        public Nullable<DateTime> ProductionTime { get; set; }

        Int32 _indexofpharmacy = 0;
        /// <summary>
        /// 在当前药房中的机器索引号.比如急诊药房的0号机
        /// </summary>
        public Int32 IndexOfPharmacy { get { return _indexofpharmacy; } set { _indexofpharmacy = value; } }

        public string SoftwarePublickKey { get; set; }
    }
    public class AMDM_Machine:AMDM_Machine_data
    {
        /// <summary>
        /// 该药机的所有药仓
        /// </summary>
        public List<AMDM_Stock> Stocks { get; set; }
    }
}
