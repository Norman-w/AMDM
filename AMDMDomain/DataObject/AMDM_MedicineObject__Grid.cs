using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 包含格子信息又包含药品实体信息的结构,用于取药的时候进行操作
    /// </summary>
    public class AMDM_MedicineObject__Grid : IAMDM_Grid_data , IAMDM_MedicineObject_data
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

        /// <summary>
        /// 格子相对于药仓的索引
        /// </summary>
        public int IndexOfStock { get; set; }

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
    }

    
}
