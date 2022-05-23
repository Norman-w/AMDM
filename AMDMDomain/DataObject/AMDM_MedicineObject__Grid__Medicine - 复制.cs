using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 2022年1月18日11:23:24  获取库存时候可用的 弹药实体,包含格子包含弹药信息的结构
    /// </summary>
    public class AMDM_MedicineObject__Grid__Medicine : IAMDM_Grid_data, IAMDM_MedicineObject_data, IAMDM_Medicine
    {
        Int32 _id = 0;
        /// <summary>
        /// 流水号,自动递增
        /// </summary>
        public Int32 Id { get { return _id; } set { _id = value; } }

        Int32 _stockid = 0;
        /// <summary>
        /// 所属药仓的id
        /// </summary>
        public Int32 StockId { get { return _stockid; } set { _stockid = value; } }

        /// <summary>
        /// 所在的药仓 在所在的药机中的索引
        /// </summary>
        public int StockIndex { get; set; }

        Int32 _floorid = 0;
        /// <summary>
        /// 所属层的id
        /// </summary>
        public Int32 FloorId { get { return _floorid; } set { _floorid = value; } }

        /// <summary>
        /// 所在的层, 在其所在药仓中的索引,也就是该格子的y坐标
        /// </summary>
        public Int32 FloorIndex { get; set; }

        Int32 _indexoffloor = 0;
        /// <summary>
        /// 在层中的索引位置,从左到右 从0开始,也就是该格子的x坐标
        /// </summary>
        public Int32 IndexOfFloor { get { return _indexoffloor; } set { _indexoffloor = value; } }

        /// <summary>
        /// 2022年2月18日10:55:17 格子在药仓中的索引
        /// </summary>
        public Int32 IndexOfStock { get; set; }
        float _leftmm = 0;
        /// <summary>
        /// 左边所在的毫米,也就是x轴的起点位置
        /// </summary>
        public float LeftMM { get { return _leftmm; } set { _leftmm = value; } }

        float _rightmm = 0;
        /// <summary>
        /// 右边所在的毫米位置,也就是x轴的终点位置
        /// </summary>
        public float RightMM { get { return _rightmm; } set { _rightmm = value; } }

        #region 2021年11月13日14:42:24  为了取药的时候方便计算位置  把继承自层板的高度的信息也保存在grid中一份
        /// <summary>
        /// 高点毫米位置
        /// </summary>
        public float TopMM { get; set; }
        /// <summary>
        /// 低点毫米位置
        /// </summary>
        public float BottomMM { get; set; }
        #endregion

        DateTime _createtime = DateTime.Now;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get { return _createtime; } set { _createtime = value; } }

        DateTime _modifiedtime = DateTime.Now;
        /// <summary>
        /// 修改时间,数据库内信息有更新时候自动修改
        /// </summary>
        public DateTime ModifiedTime { get { return _modifiedtime; } set { _modifiedtime = value; } }

        long _lastmodifieduserid = 0;
        /// <summary>
        /// 最后修改这个格子的用户的id
        /// </summary>
        public long LastModifiedUserId { get { return _lastmodifieduserid; } set { _lastmodifieduserid = value; } }

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

        public int Max { get; set; }


        String _idofhis = null;
        /// <summary>
        /// 药品在HIS系统中的id编号,从his系统获取到药品的时候要保存这个编号,方便在以后获取库存等信息的时候,定位到药机系统内的编号.但是因为his系统内的id可能不符合本机的逻辑,所以本药品库中有单独的id
        /// </summary>
        public String IdOfHIS { get { return _idofhis; } set { _idofhis = value; } }

        String _name = null;
        /// <summary>
        /// 药品名称
        /// </summary>
        public String Name { get { return _name; } set { _name = value; } }

        String _barcode = null;
        /// <summary>
        /// 药品条码
        /// </summary>
        public String Barcode { get { return _barcode; } set { _barcode = value; } }

        String _company = null;
        /// <summary>
        /// 药品所属公司
        /// </summary>
        public String Company { get { return _company; } set { _company = value; } }

        float _boxlongmm = 0;
        /// <summary>
        /// 药盒的长度毫米
        /// </summary>
        public Single BoxLongMM { get { return _boxlongmm; } set { _boxlongmm = value; } }

        float _boxwidthmm = 0;
        /// <summary>
        /// 药盒的宽度毫米
        /// </summary>
        public float BoxWidthMM { get { return _boxwidthmm; } set { _boxwidthmm = value; } }

        float _boxheightmm = 0;
        /// <summary>
        /// 药盒的高度毫米
        /// </summary>
        public float BoxHeightMM { get { return _boxheightmm; } set { _boxheightmm = value; } }


        #region 新增库存预警阈值的两个可为空的设置字段 2022年1月9日16:17:17
        /// <summary>
        /// CountThresholdOfLowInventoryAlert低库存预警设定值,如果小于或等于这个数值时警报
        /// </summary>
        public Nullable<int> CTOLIA { get; set; }
        /// <summary>
        /// PercentThresholdOfLowInventoryAlert低库存预警设定百分比,如果小于或等于这个数值时警报
        /// </summary>
        public Nullable<float> PTOLIA { get; set; }
        #endregion

        public Nullable<int> CLMED { get; set; }
        public Nullable<int> SLMED { get; set; }
        public Nullable<int> DTOEA { get; set; }
    }
}
