using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AMDM_Domain;

namespace AMDM
{
    public partial class GridShower : UserControl
    {
        GridShowerModeEnum ShowMode = GridShowerModeEnum.StockLayoutMode;
        public GridShower()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }
        /// <summary>
        /// 格子的总索引
        /// </summary>
        //public int GridIndexOfStock { get; set; }
        /// <summary>
        /// 所在药仓的序号
        /// </summary>
        //public int StockIndex { get; set; }
        /// <summary>
        /// 格子所在层的索引
        /// </summary>
        //public int IndexOfFloor { get; set; }
        /// <summary>
        /// 层在仓库中的索引,也就是层索引
        /// </summary>
        //public int FloorIndex { get; set; }
        /// <summary>
        /// 布局模式用到的格子宽度毫米
        /// </summary>
        public float WidthMM { get; set; }
        /// <summary>
        /// 布药和库存管理(上药)模式的时候,需要用到的药品名称信息
        /// </summary>
        public AMDM_Medicine BindedMedicine { get; set; }

        public AMDM_Grid Grid { get; set; }

        public AMDM_Clip BindingInfo { get; set; }
        ///// <summary>
        ///// 库存管理模式的时候,需要用到的药品数量信息
        ///// </summary>
        //public int MedicineInventory { get; set; }
        /// <summary>
        /// 最大可装载数量
        /// </summary>
        public int MaxLoadableCount { get; set; }
        /// <summary>
        /// 当前已经装载的药品数量
        /// </summary>
        public int CurrentLoadedCount { get; set; }

        /// <summary>
        /// 当前药槽有没有被选择,当有多个药槽是该药品的时候,如果没有被选择的,就显示的是闪烁的药品的文字,如果选中了 格子变成绿色的闪烁.
        /// </summary>
        public bool Selected { get; set; }

        public enum GridShowerModeEnum {
            /// <summary>
            /// 药仓的药槽布局模式,显示总编号,层和格子位置, 不显示药品名称, 显示格子宽度,   默认的显示白色,当前选中的显示绿色.
            /// </summary>
            StockLayoutMode,
            /// <summary>
            /// 药品和格子的绑定信息模式, 显示总编号,层和格子位置,药品名称 不包含库存.  已经布药的是绿色, 没有布药的是默认的白色.
            /// </summary>
            BindingInfoManageMode,
            /// <summary>
            /// 库存管理和布药模式,显示总编号,层和格子位置,药品名称. 灰色没有药了,默认白色有药,闪烁绿色->需要放在这里.
            /// </summary>
            InventoryManageMode,
        };

        /// <summary>
        /// 初始化格子显示器,使用药仓和药槽的布局模式
        /// </summary>
        /// <param name="gridIndexOfStock"></param>
        /// <param name="indexOfFloor"></param>
        /// <param name="floorIndex"></param>
        /// <param name="widthMM"></param>
        /// <returns></returns>
        public bool Init(
            //int gridIndexOfStock, 
            AMDM_Grid grid, float widthMM)
        {
            this.Grid = grid;
            //this.GridIndexOfStock = gridIndexOfStock;
            //this.IndexOfFloor = indexOfFloor;
            //this.FloorIndex = floorIndex;
            this.WidthMM = widthMM;
            this.ShowMode = GridShowerModeEnum.StockLayoutMode;
            return true;
        }
        /// <summary>
        /// 初始化格子显示器,使用药品的格子绑定模式
        /// </summary>
        /// <param name="gridIndexOfStock"></param>
        /// <param name="indexOfFloor"></param>
        /// <param name="floorIndex"></param>
        /// <param name="bindedMedicineName"></param>
        /// <returns></returns>
        public bool Init(AMDM_Grid grid, AMDM_Medicine bindedMedicine,int maxLoadableCount)
        {
            //this.GridIndexOfStock = grid.IndexOfStock;
            //this.IndexOfFloor = grid.IndexOfFloor;
            //this.FloorIndex = grid.FloorIndex;
            //this.StockIndex = grid.StockIndex;
            this.Grid = grid;
            this.WidthMM = grid.RightMM - grid.LeftMM;
            this.BindedMedicine = bindedMedicine;
            this.MaxLoadableCount = maxLoadableCount;
            this.ShowMode = GridShowerModeEnum.BindingInfoManageMode;
            return true;
        }
        /// <summary>
        /// 初始化格子显示器,使用库存管理和布药模式,包含库存
        /// </summary>
        /// <param name="gridIndexOfStock"></param>
        /// <param name="indexOfFloor"></param>
        /// <param name="floorIndex"></param>
        /// <param name="bindedMedicineName"></param>
        /// <param name="medicineInventory"></param>
        /// <returns></returns>
        public bool Init(AMDM_Grid grid,AMDM_Clip bindingInfo, AMDM_Medicine bindedMedicine, int maxLoadableCount, int medicineInventory)
        {
            this.Grid = grid;
            this.BindingInfo = bindingInfo;
            //this.GridIndexOfStock = grid.IndexOfStock;
            //this.IndexOfFloor = grid.IndexOfFloor;
            //this.FloorIndex = grid.FloorIndex;
            this.BindedMedicine = bindedMedicine;
            this.MaxLoadableCount = maxLoadableCount;
            this.CurrentLoadedCount = medicineInventory;
            this.ShowMode = GridShowerModeEnum.InventoryManageMode;
            return true;
        }


        Font gridNumFont = new Font("Gulim", 16, FontStyle.Bold);
        Font locationFont = new Font("宋体", 9);
        Font widthMMFont = new Font("宋体", 9);
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            string detailStr = string.Format("内径{0}mm", WidthMM);
            Color detailColor = Color.Black;
            Color indexOfStockColor = Color.DarkRed;
            switch (this.ShowMode)
            {
                #region 药仓的药槽布局模式
                case GridShowerModeEnum.StockLayoutMode:
                    detailStr = string.Format("内径{0}mm", WidthMM);
                    if (this.Grid.FloorIndex <0 )
                    {
                        detailStr = "特殊药槽";
                    }
                    break;
                #endregion
                #region 药品和格子的绑定信息模式
                case GridShowerModeEnum.BindingInfoManageMode:
                    float minWidth = WidthMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM - App.Setting.HardwareSetting.Grid.MaxGridPaddingWidthMM;
                    float maxWidth = WidthMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM-  App.Setting.HardwareSetting.Grid.MinGridPaddingWidthMM;
                    string sizeSpanStr = string.Format("最窄{0}mm~最宽{1}mm", minWidth, maxWidth);
                    if (this.Grid.FloorIndex == -1)
                    {
                        sizeSpanStr = "特殊药槽";
                    }
                    else if (this.Grid.FloorIndex < 0)
                    {
                        sizeSpanStr = "未安装";
                    }
                    detailStr = this.BindedMedicine == null ? sizeSpanStr : this.BindedMedicine.Name;//2021年11月15日11:02:23  替换"未绑定药品" 为显示尺寸 不然不知道放多大的
                    detailColor = this.BindedMedicine == null ? Color.Gray : Color.Black;
                    break;
                #endregion
                #region 库存管理和布药模式
                case GridShowerModeEnum.InventoryManageMode:
                    detailStr = this.BindedMedicine == null ? "未启用的药槽" : this.BindedMedicine.Name;//string.Format("{0} * {1}", this.CurrentLoadedCount, this.BindedMedicine == null ? "" : this.BindedMedicine.Name
                    detailColor = this.BindedMedicine == null ? Color.Gray : Color.Black;
                    indexOfStockColor = this.BindedMedicine == null ? Color.Gray : Color.DarkRed;
                    #region 如果是未安装的药槽 显示的颜色为灰色
                    if (this.Grid.Disable)
                    {
                        detailStr = "未启用的药槽";
                        indexOfStockColor = Color.Gray;
                    }
                    #endregion
                    break;
                #endregion
                default:
                    break;
            }

            #region 库存管理模式 显示电池电量
            if (this.ShowMode == GridShowerModeEnum.InventoryManageMode)
            {
                Color backColor = Color.LightGray;
                if (this.BindingInfo!= null && this.BindingInfo.Stuck)
                {
                    backColor = Color.PaleVioletRed;
                }
                Color borderColorFramed = Color.LightGray;
                #region 显示背景
                #region 获取该控件的动画状态 如果是动画中,获取动画的不同的颜色 显示一个边框,该动画状态就表述 他当前是一个匹配上了该药品的药槽.
                if (App.ControlAnimationRenderingController.IsAnimating(this))
                {
                    if (!Selected)
                    {
                        //如果没有选中的话 药品的名字是闪烁的.如果选中了 药品的格子是闪烁的.
                        float percent = App.ControlAnimationRenderingController.GetSinPercent();

                        float fps = App.ControlAnimationRenderingController.FPS;
                        Color detailColorFrame = this.GetSingleColorList(
                            Color.Black,
                            Color.Green,
                            Convert.ToInt32(fps),
                            Convert.ToInt32(percent * (fps - 1))
                            );
                        detailColor = detailColorFrame;

                        //Color indexColorFramed = this.GetSingleColorList(
                        //    Color.DarkRed,
                        //    Color.LightGreen,
                        //    Convert.ToInt32(fps),
                        //    Convert.ToInt32(percent * (fps - 1))
                        //    );
                        //indexOfStockColor = indexColorFramed;

                        backColor =
                            //如果是已经卡药了,显示为红色的,如果不是,显示为绿色的
                            this.BindingInfo != null && this.BindingInfo.Stuck ? Color.PaleVioletRed : Color.LightGreen;

                        
                        borderColorFramed = this.GetSingleColorList(
                            Color.LightGray,
                            Color.DarkGreen,
                            Convert.ToInt32(fps),
                            Convert.ToInt32(percent * (fps - 1))
                            );
                    }
                    else
                    {//如果是被选择了的话 字体的颜色就是黑色的 但是背景的颜色就是多变的了.
                        detailColor = Color.Black;


                        float percent = App.ControlAnimationRenderingController.GetSinPercent();

                        float fps = App.ControlAnimationRenderingController.FPS;
                        Color animateColor = this.GetSingleColorList(
                            Color.DarkGreen,
                            Color.LightGreen,
                            Convert.ToInt32(fps),
                            Convert.ToInt32(percent * (fps - 1))
                            );
                        backColor = animateColor;
                    }
                }
                #endregion
                else
                {
                    //不是动画模式保持格子还是灰色的
                }
                #endregion
                e.Graphics.FillRectangle(new SolidBrush(backColor), this.ClientRectangle);

                //if (borderColorFramed != Color.LightGray)
                //{
                //    Pen borderPen = new Pen(borderColorFramed);
                //    borderPen.Width = 5;
                //    e.Graphics.DrawRectangle(borderPen, this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 3, this.ClientRectangle.Height - 5);
                //}
                //this.BackColor = dashColor;
                #region 横向显示所有电量槽子
                float singleWidth = this.ClientRectangle.Width / 1f / MaxLoadableCount;
                for (int i = 0; i < this.MaxLoadableCount; i++)
                {
                    RectangleF currentSigleRectF = new RectangleF(
                        this.ClientRectangle.Left + i * singleWidth + singleWidth * 0.1f,
                        this.ClientRectangle.Height * 0.4f,
                        singleWidth * 0.8f,
                        this.ClientRectangle.Height * 0.2f
                        );
                    if ((i + 1) > this.CurrentLoadedCount)
                    {
                        Pen dashPen = new Pen(CurrentLoadedCount == 0 ? Brushes.DarkRed : Brushes.Gray);
                        dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        e.Graphics.DrawRectangle(dashPen, currentSigleRectF.X, currentSigleRectF.Y, currentSigleRectF.Width, currentSigleRectF.Height);
                    }
                    else
                    {
                        //int loadedPercent = (int)Math.Round(this.CurrentLoadedCount*100f/this.MaxLoadableCount);
                        //int red = 255-(int)(Math.Floor(loadedPercent/100f*255));
                        //int green = (int)Math.Floor(loadedPercent / 100f * 255);
                        //Color notFullLoadColor = Color.FromArgb(red, green, 0);
                        //Color notFullLoadColor = GetSingleColorList(Color.DarkRed,Color.DarkOrange, this.MaxLoadableCount, this.CurrentLoadedCount-1);
                        Color notFullLoadColor = Color.DarkOrange;
                        Brush filledBrushes = (CurrentLoadedCount >= MaxLoadableCount) ? Brushes.Green : new SolidBrush(notFullLoadColor);
                        e.Graphics.FillRectangle(filledBrushes, currentSigleRectF);
                    }
                }
                #endregion
            }
            #endregion
            #region 如果是绑定模式,直接显示可以绑定多少的格子 都用灰色的
            else if (this.ShowMode == GridShowerModeEnum.BindingInfoManageMode)
            {
                
                Color dashColor = Color.DarkGray;
                #region 获取该控件的动画状态 如果是动画中,获取动画的不同的颜色 显示一个边框
                if (App.ControlAnimationRenderingController.IsAnimating(this))
                {
                    float percent = App.ControlAnimationRenderingController.GetSinPercent();

                    float fps = App.ControlAnimationRenderingController.FPS;
                    Color borderColor = this.GetSingleColorList(
                        Color.DarkGreen,
                        Color.LightGreen,
                        Convert.ToInt32(fps),
                        Convert.ToInt32(percent * (fps - 1))
                        );
                    dashColor = borderColor;
                    
                    //e.Graphics.DrawRectangle(new Pen(borderColor), this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
                }
                #endregion
                else
                {
                    if (this.BindedMedicine == null)
                    {//如果没有绑定药品

                    }
                    else
                    {
                        dashColor = Color.DarkGreen;
                    }
                }
                
                //this.BackColor = dashColor;
                e.Graphics.FillRectangle(new SolidBrush(dashColor), this.ClientRectangle);
            }
            #endregion
            //e.Graphics.DrawRectangle(new Pen(Brushes.Black), 2, 2, this.ClientRectangle.Width - 1-4, this.ClientRectangle.Height - 1-4);
            //计算红色药槽编号所需要使用的矩形尺寸
            string gridNumStr = (this.Grid.IndexOfStock+1).ToString().PadLeft(3, '0');
            if (this.BindingInfo!= null && this.BindingInfo.Stuck)
            {
                gridNumStr = string.Format("故障{0}", gridNumStr);
            }
            SizeF grimNumSizeF = e.Graphics.MeasureString(gridNumStr, gridNumFont);
            //渲染药槽编号
            e.Graphics.DrawString(gridNumStr, gridNumFont, new SolidBrush(indexOfStockColor), new PointF(0, 0));
            //渲染药槽位置信息
            string stockLocationStr = this.Grid.FloorIndex >= 0 ? string.Format("{0}-{1}", this.Grid.FloorIndex + 1, this.Grid.IndexOfFloor + 1) :
                string.Format("{0}-{1}", this.Grid.FloorIndex, this.Grid.IndexOfFloor+1);
            e.Graphics.DrawString(stockLocationStr, locationFont, Brushes.Gray, new PointF(grimNumSizeF.Width, 3));
            //毫米位置所显示的内容,如果是布药模式和库存管理(上药)模式,显示的是药品名称

            //计算毫米信息或者药品名称的显示区域所占用的空间大小
            SizeF widthMMStrSizeF = e.Graphics.MeasureString(detailStr, widthMMFont);
            //渲染毫米信息或者药品名称信息
            e.Graphics.DrawString(detailStr, widthMMFont, new SolidBrush(detailColor), new RectangleF(0, this.ClientRectangle.Height - widthMMStrSizeF.Height, this.ClientRectangle.Width, widthMMStrSizeF.Height), new StringFormat() { Alignment = StringAlignment.Center });

        }

        List<Color> GetSingleColorList(Color src, Color dest, int count)
        {
            List<Color> colorFactorList = new List<Color>();
            int red = dest.R - src.R;
            int g = dest.G - src.G;
            int b = dest.B - src.B;
            for (int i = 0; i < count; i++)
            {
                Color color = Color.FromArgb(
                    src.R + (int)((double)i/count*red),
                    src.G + (int)((double)i/count*g),
                    src.G + (int)((double)i/count*b)
                    );
                colorFactorList.Add(color);
            }
            return colorFactorList;
        }
        /// <summary>
        /// 从两个颜色中间 分成若干段, 取其中的一段的颜色
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="count"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Color GetSingleColorList(Color src, Color dest, int count, int index)
        {
            int alpaha = dest.A - src.A;
            int red = dest.R - src.R;
            int g = dest.G - src.G;
            int b = dest.B - src.B;

            return Color.FromArgb(
                src.A + (int)((double)index/count*alpaha),
                 src.R + (int)((double)index / count * red),
                 src.G + (int)((double)index / count * g),
                 src.B + (int)((double)index / count * b)
                 );
        }
    }
}
