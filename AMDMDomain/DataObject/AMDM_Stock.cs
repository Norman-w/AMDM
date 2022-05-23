using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 药仓信息,一个药机可以有多个药仓
    /// </summary>
    public class AMDM_Stock_data
    {
        Int32 _id = 0;
        /// <summary>
        /// 流水号
        /// </summary>
        public Int32 Id { get { return _id; } set { _id = value; } }

        String _serialnumber = null;
        /// <summary>
        /// 药仓的序列号
        /// </summary>
        public String SerialNumber { get { return _serialnumber; } set { _serialnumber = value; } }

        Int32 _machineid = 0;
        /// <summary>
        /// 所属的药机id
        /// </summary>
        public Int32 MachineId { get { return _machineid; } set { _machineid = value; } }

        Int32 _indexofmachine = 0;
        /// <summary>
        /// 在药机中的序号,从0开始,因为一个药机可以有多个仓
        /// </summary>
        public Int32 IndexOfMachine { get { return _indexofmachine; } set { _indexofmachine = value; } }

        Int32 _firstlayoutmedicineuserid = 0;
        /// <summary>
        /// 初次布药的用户id
        /// </summary>
        public Int32 FirstLayoutMedicineUserId { get { return _firstlayoutmedicineuserid; } set { _firstlayoutmedicineuserid = value; } }

        DateTime _firstlayouttime = DateTime.Now;
        /// <summary>
        /// 初次布药的时间
        /// </summary>
        public DateTime FirstLayoutTime { get { return _firstlayouttime; } set { _firstlayouttime = value; } }

        /// <summary>
        /// 支持的层板的最宽宽度
        /// </summary>
        public float MaxFloorWidthMM { get; set; }
        /// <summary>
        /// 支持的层板的所有板子空间的总高度
        /// </summary>
        public int MaxFloorsHeightMM { get; set; }
        ///// <summary>
        ///// 下部分空间支持的层板的所有板子的空间总高度,目前只是为了视图,实际使用过程中因为结构的不确定,不能只局限于使用此字段的值
        ///// </summary>
        //public int DownPartMaxFloorsTotalHeightMM { get; set; }
        /// <summary>
        /// 横坐标/X轴 从起点到药层左侧第一个格子的起点中心位的偏移量毫米
        /// </summary>
        public float XOffsetFromStartPointMM { get; set; }
        /// <summary>
        /// 纵坐标/Y轴 从起点到药层的第0层的偏移量毫米数
        /// </summary>
        public float YOffsetFromStartPointMM { get; set; }

        /// <summary>
        /// 两个抓取器中间的中心距离
        /// </summary>
        public float CenterDistanceBetweenTwoGrabbers { get; set; }
    }

    public class AMDM_Stock:AMDM_Stock_data
    {
        /// <summary>
        /// 层信息集合
        /// 在2021年11月27日14:27:51做了修改,使用了字典结构,key是层的索引,value是层信息
        /// </summary>
        //public List<AMDM_Floor> Floors { get; set; }
        public Dictionary<int,AMDM_Floor> Floors { get; set; }
    }
}
