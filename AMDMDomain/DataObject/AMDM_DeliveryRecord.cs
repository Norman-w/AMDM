using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 药品(对于处方)的交付记录_数据
    /// </summary>
    public class AMDM_DeliveryRecord_data : IAMDM_PatientInfo
    {
        Int64 _id = 0;
        /// <summary>
        /// 流水号自增量
        /// </summary>
        public Int64 Id { get { return _id; } set { _id = value; } }

        String _prescriptionid = null;
        /// <summary>
        /// 处方编号
        /// </summary>
        public String PrescriptionId { get { return _prescriptionid; } set { _prescriptionid = value; } }

        DateTime _starttime = DateTime.Now;
        /// <summary>
        /// 交付药品的开始时间
        /// </summary>
        public DateTime StartTime { get { return _starttime; } set { _starttime = value; } }

        Nullable<DateTime> _endtime = null;
        /// <summary>
        /// 交付药品的完成时间
        /// </summary>
        public Nullable<DateTime> EndTime { get { return _endtime; } set { _endtime = value; } }

        Int32 _totalkindcount = 0;
        /// <summary>
        /// 该处方中一共有多少种药品
        /// </summary>
        public Int32 TotalKindCount { get { return _totalkindcount; } set { _totalkindcount = value; } }

        Int32 _totalmedicinecount = 0;
        /// <summary>
        /// 该处方中一共有多少件药品
        /// </summary>
        public Int32 TotalMedicineCount { get { return _totalmedicinecount; } set { _totalmedicinecount = value; } }

        bool _finished = false;
        /// <summary>
        /// 是否完成作业
        /// </summary>
        public bool Finished { get { return _finished; } set { _finished = value; } }

        bool _canceled = false;
        /// <summary>
        /// 是否已经取药作业
        /// </summary>
        public bool Canceled { get { return _canceled; } set { _canceled = value; } }

        String _snapshotimagefile = null;
        /// <summary>
        /// 存储快照图像文件的地址
        /// </summary>
        public String SnapshotImageFile { get { return _snapshotimagefile; } set { _snapshotimagefile = value; } }

        Byte[] _snapshotimage = null;
        /// <summary>
        /// 存储记录的图片对象,不一定使用 如果使用 会影响数据库的性能
        /// </summary>
        public Byte[] SnapshotImage { get { return _snapshotimage; } set { _snapshotimage = value; } }

        String _memo = null;
        /// <summary>
        /// 对于该付药记录的备注
        /// </summary>
        public String Memo { get { return _memo; } set { _memo = value; } }



        #region 2022年2月19日12:16:07  继承接口后需要再付药单上保存患者信息,院方可以没有可以不提供 但是咱们得有这个记录的能力.
        /// <summary>
        /// 患者年龄
        /// </summary>
        public int PatientAge { get; set; }

        /// <summary>
        /// 患者在his系统中的id
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 患者的姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 患者的性别
        /// </summary>
        public string PatientSex { get; set; }
        #endregion
        
    }
    /// <summary>
    /// 药品(对于处方)的交付记录,实体
    /// </summary>
    public class AMDM_DeliveryRecord:AMDM_DeliveryRecord_data
    {
        /// <summary>
        /// 交付记录明细
        /// </summary>
        public List<AMDM_DeliveryRecordDetail_data> Details { get; set; }

        #region 2022年1月6日11:05:21 新增取药相关图片的结构
        /// <summary>
        /// 取药结束以后,取药斗的照片
        /// </summary>
        public AMDM_Snapshot SnapshotOfMedicineBucket { get; set; }

        /// <summary>
        /// 取药结束以后,打印的付药单的图片的备份.由打印组件生成
        /// </summary>
        public AMDM_Snapshot SnapshotOfDeliveryRecordBill { get; set; }

        /// <summary>
        /// 取药结束后,用户交互区域的照片,由在仓外的相机捕捉
        /// </summary>
        public AMDM_Snapshot SnapshotOfInteractiveArea { get; set; }      
        #endregion
    }
}
