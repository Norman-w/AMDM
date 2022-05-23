using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GridAutoLayouter
{
    public class MoveGridParam
    {
        /// <summary>
        /// 将要移动的格子
        /// </summary>
        public AMDM_Grid Src { get; set; }
        /// <summary>
        /// 将要往哪个格子的地方移动
        /// </summary>
        public AMDM_Grid Dest { get; set; }
        /// <summary>
        /// 移动的偏移量毫米
        /// </summary>
        public float OffsetMM { get; set; }
        /// <summary>
        /// 显示移动格子的按钮的窗体
        /// </summary>
        public Form ParantForm { get; set; }
    }
}
