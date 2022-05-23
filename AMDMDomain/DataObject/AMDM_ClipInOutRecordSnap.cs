using System;
using System.Collections.Generic;
using System.Text;

/*
 * 
 * 
 * 
SELECT
	outp.Id AS OutRecordId,
	outd.id AS OutRecordDetailId,
	outd.MedicineName OutDetailMedicineName,
	outd.Count OutDetailCount,
	outd.MedicineObjectId AS OutObjectId,
	outd.StartTime as OutStartTime,
	outd.EndTime as OutEndTime,
	outd.IsError as OutIsError,
	outd.ErrMsg as OutErrMsg,
	outp.PrescriptionId as OutPrescriptionId,
	outp.Canceled as OutCanceled,
	outp.PatientName as OutPatientName,
	outp.Memo as OutMemo,
	s.ParentType AS OutSnapParentType,
	s.Location AS OutSnapLocation,
	s.Time AS OutSnapTime,
	s.Because AS OutSnapBecause,
	s.LocalFilePath AS OutSnapLocalFilePath
FROM
	amdm_delivery_record_detail AS outd
LEFT JOIN amdm_delivery_record AS outp ON outd.ParentId = outp.Id
RIGHT JOIN amdm_snapshot AS s ON (
	(
		outd.ParentId = s.ParentId
		AND ParentType = 1
	)
	OR (
		outd.Id = s.ParentId
		AND ParentType = 2
	)
)
WHERE
	outd.StockIndex = 0
AND outd.FloorIndex = 3
AND outd.GridIndex = 7;
 */
namespace AMDM_Domain
{
    ///// <summary>
    ///// 2022年2月19日17:12:51  构造格子的使用记录图片类,用于格子错误的时候进行溯源,这个类记录格子的出库记录
    ///// </summary>
    ///// <summary>
    ///// 构造格子的使用记录图片类,用于格子错误的时候进行溯源 这个类记录格子的入库记录 还没构造就作废了
    ///// </summary>
    //public class AMDM_GridInRecordSnap
    //{

    //}

    /// <summary>
    /// 药槽使用记录 或者是弹夹使用记录的日志,虽然使用的都是入库和出库单,但是操作的对象是弹夹,所以这样理解应该是正确的
    /// 以进出库的明细记录为主,如果这一条记录没有图片,截图相关的字段就是空的,如果有,截图相关的字段就不是空的.如果又多条,截图相关的字段不是空的而且是多条记录,但是进出库记录就是重复的了.
    /// 2022年2月19日18:17:42
    /// </summary>
    public class AMDM_ClipInOutRecordSnap : AMDM_Snapshot
    {
        #region 数据库中字段怎么写的
        /*
         * 'in' AS InOrOut,
			ind.InstockTime AS OrderByTime,
			inp.id AS InRecordId,
			ind.id AS InRecordDetailId,
			ind.MedicineName AS InDetailMedicineName,
			ind.Count AS InDetailCount,
			ind.InstockTime AS InDetailTime,
			ind.Memo AS InDetailMemo,
			inp.NurseId AS InAccountId,
			'护士名字信息' AS InAccountName,
			inp.Memo AS InMemo,
			inp.Type AS InType,
			inp.Canceled AS InCanceled,
			NULL AS `OutRecordId`,
			NULL AS `OutRecordDetailId`,
			NULL AS `OutDetailMedicineName`,
			NULL AS `OutDetailCount`,
			NULL AS `OutObjectId`,
			NULL AS `OutStartTime`,
			NULL AS `OutEndTime`,
			NULL AS `OutIsError`,
			NULL AS `OutErrMsg`,
			NULL AS `OutPrescriptionId`,
			NULL AS `OutCanceled`,
			NULL AS `OutPatientName`,
			NULL AS `OutMemo`
         */
        #endregion
        #region 主要的数据
        /// <summary>
        /// 拍照的类型,是为了什么而拍照的.
        /// </summary>
        public SnapshotParentTypeEnum SnapType { get; set; }
        /// <summary>
        /// 该条记录时入库还是出库,如果入库时候访问入库字段 如果出库时候访问出库的字段
        /// 
        /// </summary>
        public InOrOutTypeEnum InOrOut
        {
            //get
            //{
            //    InOrOutTypeEnum ret = InOrOutTypeEnum.UnKnow;
            //    switch (this.SnapType)
            //    {
            //        case SnapshotParentTypeEnum.Unknow:
            //            ret = InOrOutTypeEnum.UnKnow;
            //            break;
            //        case SnapshotParentTypeEnum.DeliveryRecord:
            //            ret = InOrOutTypeEnum.Out;
            //            break;
            //        case SnapshotParentTypeEnum.DeliveryRecordDetail:
            //            ret = InOrOutTypeEnum.Out;
            //            break;
            //        case SnapshotParentTypeEnum.InStockRecord:
            //            ret = InOrOutTypeEnum.In;
            //            break;
            //        case SnapshotParentTypeEnum.InStockRecordDetail:
            //            ret = InOrOutTypeEnum.In;
            //            break;
            //        default:
            //            break;
            //    }
            //    return ret;
            //}
            get;
            set;
        }

        /// <summary>
        /// 用于进行排序的时间字段,入库单明细使用的是入库单的创建时间,出库单明细使用的是出库单的开始时间.
        /// </summary>
        public DateTime OrderByTimee { get; set; }


        #region 联合snap表得到的数据,直接继承与SnapShotCapture

        #endregion
        #endregion
        #region 出库操作时候的数据

        /// <summary>
        /// 出库记录的id
        /// </summary>
        public Nullable<long> OutRecordId { get; set; }
        /// <summary>
        /// 出库记录的明细ID
        /// </summary>
        public Nullable<long> OutRecordDetailId { get; set; }
        /// <summary>
        /// 出具记录时候记录的药品名称
        /// </summary>
        public string OutDetailMedicineName { get; set; }
        /// <summary>
        /// 出库记录时候记录的出库数量
        /// </summary>
        public Nullable<int> OutDetailCount { get; set; }
        /// <summary>
        /// 出库的那个目标药品的唯一id
        /// </summary>
        public Nullable<long> OutObjectId { get; set; }
        /// <summary>
        /// 出库的动作开始时间,也就是detail的开始时间
        /// </summary>
        public Nullable<DateTime> OutStartTime { get; set; }
        /// <summary>
        /// 出库的detail动作开始时间
        /// </summary>
        public Nullable<DateTime> OutEndTime { get; set; }
        /// <summary>
        /// 出库的detail动作是否失败
        /// </summary>
        public Nullable<bool> OutIsError { get; set; }
        /// <summary>
        /// 出库的detail动作的失败原因
        /// </summary>
        public string OutErrMsg { get; set; }
        /// <summary>
        /// 出库的处方的编号
        /// </summary>
        public string OutPrescriptionId { get; set; }
        /// <summary>
        /// 出库单 不是出库单明细 有没有被取消
        /// </summary>
        public Nullable<bool> OutCanceled { get; set; }
        /// <summary>
        /// 出库的患者的名字
        /// </summary>
        public string OutPatientName { get; set; }
        /// <summary>
        /// 出库时候的备注,如果是校验库存等情况才会有的 一般的正常患者取药的时候是不会有的
        /// </summary>
        public string OutMemo { get; set; }
        #endregion

        #region 入库操作时候的数据
        /*
         * `inp.id AS InRecordId,
	ind.id AS InRecordDetailId,
	ind.MedicineName AS InDetailMedicineName,
	ind.Count AS InDetailCount,
	ind.InstockTime AS InDetailTime,
	ind.Memo AS InDetailMemo,
	inp.NurseId AS InAccountId,
	'护士名字信息' AS InAccountName,
	inp.Memo AS InMemo,
	inp.Type AS InType,
	inp.Canceled AS InCanceled,
	`s`.`ParentType` AS `SnapType`,
	`s`.`Location` AS `InSnapLocation`,
	`s`.`Time` AS `InSnapTime`,
	`s`.`Because` AS `InSnapBecause`,
	`s`.`LocalFilePath` AS `InSnapLocalFilePath`
         */
        /// <summary>
        /// 入库记录编号
        /// </summary>
        public Nullable<long> InRecordId { get; set; }
        /// <summary>
        /// 入库记录明细编号
        /// </summary>
        public Nullable<long> InRecordDetailId { get; set; }
        /// <summary>
        /// 入库记录明细产生时候的药品名字
        /// </summary>
        public string InDetailMedicineName { get; set; }
        /// <summary>
        /// 入库记录明细产生时候的药品的数量
        /// </summary>
        public int InDetailCount { get; set; }
        /// <summary>
        /// 入库记录明细产生时候的时间
        /// </summary>
        public Nullable<DateTime> InDetailTime { get; set; }
        /// <summary>
        /// 入库记录明细产生时候的备注
        /// </summary>
        public string InDetailMemo { get; set; }
        /// <summary>
        /// 入库发起人 也就是护士或者账号的id
        /// </summary>
        public Nullable<long> InAccountId { get; set; }
        /// <summary>
        /// 入库记录发起人的护士的名字
        /// </summary>
        public string InAccountName { get; set; }
        /// <summary>
        /// 入库的时候对入库单进行的一个备注.
        /// </summary>
        public string InMemo { get; set; }
        /// <summary>
        /// 入库的类型 字符串保存的
        /// </summary>
        public string InType { get; set; }
        /// <summary>
        /// 入库单是否已经取消或者是作废.
        /// </summary>
        public Nullable<bool> InCanceled { get; set; }
        #endregion
    }
}
