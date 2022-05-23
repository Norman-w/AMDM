using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 药仓中的格子信息
    /// </summary>
    public class AMDM_Grid_data : IAMDM_Grid_data
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
        /// 该药槽是否已经禁用,当有特殊情况需要禁止使用这个药槽的时候,就设置为未启用.
        /// </summary>
        public bool Disable { get; set; }

        #region 2022年2月18日10:00:38把格子相对于药仓的索引记录到这里面来了
        /// <summary>
        /// 格子的编号,在药仓中的,这个排序是 y轴最大的左侧第一个为0
        /// </summary>
        public int IndexOfStock { get; set; }
        #endregion
    }
    public class AMDM_Grid:AMDM_Grid_data
    {
        
    }
}
