using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 硬件基础信息设置类
    /// </summary>
    public class HardwareSettingClass
    {
        public HardwareSettingClass()
        {
            this.Stock = new StockSettingClass();
            this.Floor = new FloorSettingClass();
            this.Grid = new GridSettingClass();
        }
        public StockSettingClass Stock { get; set; }
        public FloorSettingClass Floor { get; set; }
        public GridSettingClass Grid { get; set; }
        public class StockSettingClass
        {
            public StockSettingClass()
            {
                XOffsetFromStartPointMM = -34;// -19;
                YOffsetFromStartPointMM = 34;
                FloorFixingsHeightMM = 20;
                StockFloorCount = 12;
                MaxPerMedicineDepthMM = 200;
                MinPerMedicineDepthMM = 30;
                MinPerMedicineHeightMM = 5;
                MaxFloorsHeightMM = 960;
                MaxFloorWidthMM = 975;
                CenterDistanceBetweenTwoGrabbers = 310;
            }
            /// <summary>
            /// 支持的安装所有层板的总高度,也就是机械手动作面可以动作的总高度了,太高了的话机械手就取不了了.这也就决定了能放多少层拍子,一层是80,12层正好是960
            /// </summary>
            public float MaxFloorsHeightMM { get; set; }
            /// <summary>
            /// 可以安装的层板的最宽的宽度是多少
            /// </summary>
            public float MaxFloorWidthMM { get; set; }
            public float XOffsetFromStartPointMM { get; set; }
            public float YOffsetFromStartPointMM { get; set; }
            public float CenterDistanceBetweenTwoGrabbers { get; set; }
            /// <summary>
            /// 在层板下房的层板固定件高度
            /// </summary>
            public float FloorFixingsHeightMM { get; set; }
            //float defaultStockFloorWidthMM = 12*80;
            /// <summary>
            /// 每一个药仓默认能放多少个层板
            /// </summary>
            public int StockFloorCount = 12;
            /// <summary>
            /// 可以放到药机内的药品的最长长度信息 这取决于机器的传送带等相关的参数 只能固定设置, 虽然长度很长的药品能够从层板上挑下来 但是不一定能被传送带和给药槽很好的接收
            /// </summary>
            public float MaxPerMedicineDepthMM { get; set; }
            /// <summary>
            /// 可以放到药机内的药品的最小长度信息 最下层的特殊药品层不做限制
            /// </summary>
            public float MinPerMedicineDepthMM { get; set; }
            /// <summary>
            /// 药盒的最小的尺寸 这个尺寸如果太小了 可能药品就非常轻,不适合放在上面的药槽中.
            /// </summary>
            public float MinPerMedicineHeightMM { get; set; }
        }

        public class FloorSettingClass
        {
            public FloorSettingClass()
            {

                DownPartFloorDepthMM = 500;
                //DefaultFloorDepthMM = 960;
                FloorHeightMM = 80;
                FloorWidthMM = 975;
                UpPartFloorDepthMM = 760;
                FloorPanelHeightMM = 3;
                FloorSlopeAngle = 27;
                MinGridPaddingHeighMM = 3;
                NewFloorDefaultHeightMM = 80;
                //PerFloorMaxHeightMM = 80;
                //PerFloorMinHeightMM = 50;
                //PerFloorMoveStepHeightMM = 5;
            }
            /// <summary>
            /// 层板的高度,用于计算一个药仓中可以放多少个层板
            /// </summary>
            public float FloorHeightMM { get; set; }
            /// <summary>
            /// 层板的宽度,用于计算可以放多少个格子
            /// </summary>
            public float FloorWidthMM { get; set; }
            /// <summary>
            /// 层板的进深尺寸,用于计算可以放多少盒药
            /// </summary>
            //public float DefaultFloorDepthMM { get; set; }
            /// <summary>
            /// 层板那个电木板的高度
            /// </summary>
            public float FloorPanelHeightMM { get; set; }
            /// <summary>
            /// 层板的倾斜角度/坡度 当前默认27°
            /// </summary>
            public float FloorSlopeAngle { get; set; }

            ///// <summary>
            ///// 每一个层板的最小高度是多少,包含框架连接件的高度.目前框架连接件的厚度是20mm,电板的厚度是3mm所以一个80mm的层板,会向下影响23mm的空间不可用.
            ///// </summary>
            //public float PerFloorMinHeightMM = 30;
            ///// <summary>
            ///// 每个层板的最大高度
            ///// </summary>
            //public float PerFloorMaxHeightMM = 200;
            ///// <summary>
            ///// 每个层板每次调整高度的时候能移动多少毫米
            ///// </summary>
            //public int PerFloorMoveStepHeightMM = 5;
            /// <summary>
            /// 新的层板的默认高度毫米
            /// </summary>
            public float NewFloorDefaultHeightMM = 80;
            /// <summary>
            /// 层板上方和药槽之间的最小间隙
            /// </summary>
            public float MinGridPaddingHeighMM { get; set; }

            public int UpPartFloorDepthMM { get; set; }

            /// <summary>
            /// 默认的下部分区域的层的深度
            /// </summary>
            public int DownPartFloorDepthMM { get; set; }
        }
        public class GridSettingClass
        {
            public GridSettingClass()
            {
                GridWallWidthMM = 10;
                GridWallFixtureFullWidthMM = 30;
                PerGridMinWidthMM = 60;
                PerGridMaxWidthMM = 170;
                PerGridMoveStepWidthMM = 5; NewGridDefaultWidth = 100;
                MaxGridPaddingWidthMM = 7;
                MinGridPaddingWidthMM = 2;
            }
            /// <summary>
            /// 槽之间的隔断铝条墙的宽度 药槽的边缘档条的宽度是多少.一个药槽两边都有档条,但是药槽实际被分派的宽度,可以视为0.5档条+内径+0.5档条.
            /// </summary>
            public float GridWallWidthMM { get; set; }
            /// <summary>
            /// 默认的安装在层板上的格子格栅固定件的总宽度,就是3d打印件那个30mm的
            /// </summary>
            public float GridWallFixtureFullWidthMM { get; set; }

            /// <summary>
            /// 每个格子的最小宽度(实际尺寸,包含两侧各半个墙板)
            /// </summary>
            public float PerGridMinWidthMM = 60;//这是外径,实际内径是50

            /// <summary>
            /// 每个格子的最大宽度毫米(实际尺寸,包含两侧各半个墙板)
            /// </summary>
            public float PerGridMaxWidthMM = 170;//这是外径,实际内径是160;
            /// <summary>
            /// 每个格子左右移动墙板的时候每一次能移动多少毫米的步伐
            /// </summary>
            public int PerGridMoveStepWidthMM = 5;
            /// <summary>
            /// 新的格子的默认宽度
            /// </summary>
            public float NewGridDefaultWidth = 100;
            #region 药槽和药盒之间的最小间隙和最大间隙信息
            /// <summary>
            /// 药槽和药盒之间的最大间隙
            /// </summary>
            public float MaxGridPaddingWidthMM { get; set; }
            /// <summary>
            /// 药槽和药盒之间的最小间隙
            /// </summary>
            public float MinGridPaddingWidthMM { get; set; }
            #endregion
        }
    }

}
