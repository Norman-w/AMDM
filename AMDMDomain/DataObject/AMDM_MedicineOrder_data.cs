using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 给付药机系统使用的付药单信息,2021年11月20日13:16:30 目前设计的不进行改数据的保存,直接在程序内流转
    /// </summary>
    public class AMDM_MedicineOrder_data : IAMDM_PatientInfo
    {
        Int64 _id = 0;
        public Int64 Id { get { return _id; } set { _id = value; } }

        long _creatorid = 0;
        public long CreatorId { get { return _creatorid; } set { _creatorid = value; } }

        string _patientid = null;
        /// <summary>
        /// 患者的id
        /// </summary>
        public string PatientId { get { return _patientid; } set { _patientid = value; } }

        Int32 _pharmacyid = 0;
        public Int32 PharmacyId { get { return _pharmacyid; } set { _pharmacyid = value; } }

        Nullable<DateTime> _createtime = null;
        public Nullable<DateTime> CreateTime { get { return _createtime; } set { _createtime = value; } }

        Nullable<DateTime> _modifiedtime = null;
        public Nullable<DateTime> ModifiedTime { get { return _modifiedtime; } set { _modifiedtime = value; } }

        Nullable<DateTime> _balancetime = null;
        public Nullable<DateTime> BalanceTime { get { return _balancetime; } set { _balancetime = value; } }

        Nullable<DateTime> _fulfilledtime = null;
        public Nullable<DateTime> FulfilledTime { get { return _fulfilledtime; } set { _fulfilledtime = value; } }

        Int32 _entriescount = 0;
        public Int32 EntriesCount { get { return _entriescount; } set { _entriescount = value; } }

        Int32 _totalcount = 0;
        public Int32 TotalCount { get { return _totalcount; } set { _totalcount = value; } }

        bool _balance = false;
        public bool Balance { get { return _balance; } set { _balance = value; } }

        bool _fulfilled = false;
        public bool Fulfilled { get { return _fulfilled; } set { _fulfilled = value; } }

        Int64 _fulfillmentnurseid = 0;
        public Int64 FulfillmentNurseId { get { return _fulfillmentnurseid; } set { _fulfillmentnurseid = value; } }

        bool _canceled = false;
        public bool Canceled { get { return _canceled; } set { _canceled = value; } }

        #region 2021年11月20日13:05:31 加入一些患者的信息,用于展示在显示屏上.之前没想过弄这些信息,经过沟通,医院能给出患者和就诊的相关信息
        /// <summary>
        /// 就诊科室
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 诊断信息
        /// </summary>
        public string DiagnositicInfo { get; set; }
        /// <summary>
        /// 就诊时间
        /// </summary>
        public DateTime VisitTime { get; set; }
        /// <summary>
        /// 诊断医师
        /// </summary>
        public string DoctorName { get; set; }

        ///// <summary>
        ///// 患者id 2021年12月22日21:05:55  好像用不上
        ///// </summary>
        //public string PatientId { get; set; }

        /// <summary>
        /// 患者名称
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 患者性别
        /// </summary>
        public string PatientSex { get; set; }

        /// <summary>
        /// 患者年龄
        /// </summary>
        public int PatientAge { get; set; }
        #endregion
    }


    /// <summary>
    /// 给付药机系统使用的处方或者是付药单记录,不进行数据的存储,只在系统内进行流转和显示.
    /// </summary>
    public class AMDM_MedicineOrder : AMDM_MedicineOrder_data
    {
        public List<AMDM_MedicineOrderDetail> Details { get; set; }
    }
}
