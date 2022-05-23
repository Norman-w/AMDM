using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 布药仓的层信息
    /// </summary>
    public class AMDM_Floor_data
    {
        Int32 _id = 0;
        /// <summary>
        /// 流水号
        /// </summary>
        public Int32 Id { get { return _id; } set { _id = value; } }

        Int32 _stockid = 0;
        /// <summary>
        /// 所在药仓的药仓编号
        /// </summary>
        public Int32 StockId { get { return _stockid; } set { _stockid = value; } }

        Int32 _indexofstock = 0;
        /// <summary>
        /// 在药仓中的层索引,从下到上从0开始,就是floor的意思
        /// </summary>
        public Int32 IndexOfStock { get { return _indexofstock; } set { _indexofstock = value; } }

        float _widthmm = 0;
        /// <summary>
        /// 层宽度,该宽度多数情况下应该为下面的所有格子的总宽度,但是如果在初始化或者是两边或者格子中间有留空的时候,不等于所有格子的总宽度
        /// </summary>
        public float WidthMM { get { return _widthmm; } set { _widthmm = value; } }

        String _type = null;
        public String Type { get { return _type; } set { _type = value; } }

        /// <summary>
        /// 层的起点高度,是指层板的中心点的高度,不包含层的本身厚度
        /// </summary>
        public float BottomMM { get; set; }

        /// <summary>
        /// 层的顶点的高度毫米,是指层板的中心位的高度,不包含层的本身厚度
        /// </summary>
        public float TopMM { get; set; }

        /// <summary>
        /// 深度毫米,通过深度来计算能放多少个药,每个层板的深度是固定的.所有的格子的深度跟层板的深度是一致的
        /// </summary>
        public float DepthMM { get; set; }
    }

    public class AMDM_Floor:AMDM_Floor_data
    {
        public List<AMDM_Grid> Grids { get; set; }
    }
}
