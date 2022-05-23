using AMDM_Domain;
using MyCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using AMDM;
using AMDM.Manager;
/*
 * 2021年10月31日22:35:27  明天写 删除行 还有 打开页面以后就加载当前的药仓
 * 加载和保存模板以后 可以写药品的拉取 然后药品的绑定 也就是布药 然后上药  然后取药 就完工了
 */
namespace FakeHISClient
{
    public partial class StockEditForm : Form
    {
        #region 变量
        AMDM_Machine machine = new AMDM_Machine();
        int currentStockIndex = 0;
        AMDMHardwareInfoManager amdmInfoManager = null;
        GridMedicineBiddingManager bindingManager;
        StockPLCSettingTD plcSetting = null;
        AMDM_Stock stock = null;
        List<Control> addGridBtns = new List<Control>();
        List<Control> editFloorBtns = new List<Control>();


        FormAutoSizer formAutoSizer = null;
        #endregion
        #region 构造函数和初始化
        public StockEditForm()
        {
            InitializeComponent();
            //this.MaximumSize 根据这个值 自动全屏处理器在初始化的时候直接确认是不是全屏的 如果是全屏的因为vs编辑的窗体都是小的 所以先自动缩放到全屏大小
            //panel开启双缓冲  gridshower关闭双缓冲 看一下全屏切换的时候卡不卡 也看一下格子闪烁时候有没有问题
            this.formAutoSizer = new MyCode.FormAutoSizer(this);
            this.formAutoSizer.TurnOnAutoSize();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            //this.Font = new Font(this.Font.FontFamily, 10);
            //this.StartPosition = FormStartPosition.CenterParent;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.setDefaultGridWidMMBtn.Text = string.Format("↔新药槽默认内径{0}mm", App.Setting.HardwareSetting.Grid.NewGridDefaultWidth - App.Setting.HardwareSetting.Grid.GridWallWidthMM);
            this.setDefaultFloorHeightMMBtn.Text = string.Format("↕ 新层默认层间距{0}mm", App.Setting.HardwareSetting.Floor.NewFloorDefaultHeightMM);

            #region 让随机药槽移动模式的选择框内布满选项
            foreach (TestMedicineGettingModeEnum item in Enum.GetValues(typeof(TestMedicineGettingModeEnum)))
            {
                this.randomMedicineGettingModeCombox.Items.Add(item.ToString());
            }
            #endregion
        }
        public bool Init(StockPLCSettingTD plcSetting, AMDM_Stock stock, AMDMHardwareInfoManager amdmInfoManager, GridMedicineBiddingManager bindingManager)
        {
            this.bindingManager = bindingManager;
            this.amdmInfoManager = amdmInfoManager;
            this.currentStockIndex = stock.IndexOfMachine;
            this.stock = stock;
            this.plcSetting = plcSetting;
            //this.currentStockIndex = stockIndex;

            #region 随机走位线程的初始化
            this.keepMove2RandomGridBW.WorkerSupportsCancellation = true;
            this.keepMove2RandomGridBW.WorkerReportsProgress = true;
            this.keepMove2RandomGridBW.DoWork += keepMove2RandomGridBW_DoWork;
            this.keepMove2RandomGridBW.ProgressChanged += keepMove2RandomGridBW_ProgressChanged;
            this.keepMove2RandomGridBW.RunWorkerCompleted += keepMove2RandomGridBW_RunWorkerCompleted;
            #endregion

            return true;
        }

        void keepMove2RandomGridBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopMove2RandomGrid();
        }

       
        #region 窗体加载
        private void Stock_Load(object sender, EventArgs e)
        {
            
            //this.BackColor = Color.FromArgb(252, 252, 252);
            //this.WindowState = FormWindowState.Maximized;
            
            #region 窗体加载后 从数据库中加载当前给定的stockid的stock信息
            if (this.stock != null)
            {
                this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
            }
            #endregion

            App.MonitorsManager.HardwareMonitor.Pause = true;
        }
        #endregion
        #endregion

        private void 初始化药仓ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void initByAverageBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("该功能仅供测试使用,将按照每层12格药槽,一共12层的方式初始化当前药仓", "测试功能",  MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.SuspendLayout();
            this.clearStockShow();
            int upPartFloorCount = 12;
            int downPartFloorCount = 2;
            int gridCountPerUpPartFloor = 12;
            int gridCountPerDownPartFloor = 8;
            try
            {
                amdmInfoManager.ClearStockFloorAndGirds(this.stock);
                stock = amdmInfoManager.InitStock(ref this.stock,
                    upPartFloorCount,
                    downPartFloorCount,
                    gridCountPerUpPartFloor,
                    gridCountPerDownPartFloor,
                    this.stock.MaxFloorWidthMM,
                    this.stock.MaxFloorWidthMM,
                    App.Setting.HardwareSetting.Floor.UpPartFloorDepthMM,
                    App.Setting.HardwareSetting.Floor.DownPartFloorDepthMM
                    );
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("初始化失败:{0}", err.Message));
                return;
            }
            
            this.showStock(stock,this.showGridsByPercentCheckbox.Checked);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// 显示药仓信息
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="realSizePercentMode">是否使用真实比例模式</param>
        void showStock(AMDM_Stock stock, bool realSizePercentMode)
        {
            int perGridYPadding = 10;
            //int currentGridIndex = 1;
            Dictionary<int, AMDM_Floor> floors = Utils.Z2ADictionary<int, AMDM_Floor>(stock.Floors);
            List<AMDM_Floor> upPartFloors = new List<AMDM_Floor>();
            List<AMDM_Floor> downPartFloors = new List<AMDM_Floor>();
            foreach (var item in floors)
            {
                if (item.Key>=0)
                {
                    upPartFloors.Add(item.Value);
                }
                else
                {
                    downPartFloors.Add(item.Value);
                }
            }
            int currentI = upPartFloors.Count;
            #region 显示上层的
            foreach (var item in upPartFloors)
            {
                currentI--;
                AMDM_Floor currentFloor = item;
                float floorHeight = (this.panel1.Height - perGridYPadding / 2) * 1.0f / upPartFloors.Count;
                #region 如果行的最高高度超过了当前panel的十分之一的话,那就让他变成panel的高度的10分之一
                if (floorHeight > (this.panel1.Height / App.Setting.HardwareSetting.Stock.StockFloorCount))
                {
                    floorHeight = this.panel1.Height / App.Setting.HardwareSetting.Stock.StockFloorCount;
                }
                #endregion
                float floorLocationY = this.panel1.Height - currentI * floorHeight - floorHeight;
                createGridAndEditButtons(this.panel1, currentFloor, realSizePercentMode,
                    //ref currentGridIndex, 
                    floorLocationY, floorHeight, perGridYPadding);
            }
            #endregion
            #region 显示下层的
            currentI = 0;
            //currentGridIndex = 201;
            foreach (var item in downPartFloors)
            {
                //currentGridIndex = ((0 - item.IndexOfStock) + 1) * 100 + 1;
                AMDM_Floor currentFloor = item;
                float floorHeight = (this.panel2.Height - perGridYPadding /2) * 1.0f/ downPartFloors.Count;
                float floorLocationY = currentI * floorHeight + (perGridYPadding / 2);
                createGridAndEditButtons(this.panel2, currentFloor, realSizePercentMode, 
                    //ref currentGridIndex, 
                    floorLocationY, floorHeight, perGridYPadding);
                currentI++;
            }
            #endregion
        }
        void createGridAndEditButtons(Control destPanel, AMDM_Floor currentFloor, bool realSizePercentMode,
            //ref int currentGridIndex,
            float floorLocationY, float floorHeight, int perGridYPadding)
        {
            for (int j = 0; j < currentFloor.Grids.Count; j++)
            {
                AMDM_Grid currentGrid = currentFloor.Grids[j];
                float scallWidthRate = (float)(destPanel.Width / 1.0f / currentFloor.WidthMM);
                float gridWidth = realSizePercentMode ? scallWidthRate * (currentGrid.RightMM - currentGrid.LeftMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM) :
                    destPanel.Width / 1f / currentFloor.Grids.Count;
                ;
                float gridLocationX = scallWidthRate * (currentGrid.LeftMM
                   + App.Setting.HardwareSetting.Grid.GridWallWidthMM / 2);
                #region 创建一个按钮放在里面
                GridShower button = new GridShower(); 
                button.Init(
                    //currentGridIndex++, 
                    currentGrid, (currentGrid.RightMM - currentGrid.LeftMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM));
                button.Tag = currentGrid;

                button.BorderStyle = BorderStyle.FixedSingle;
                button.MouseClick += showGridEditMenu; 
                button.Location = new Point((int)Math.Round(gridLocationX), (int)Math.Round(floorLocationY));
                button.Size = new Size((int)Math.Round(gridWidth), (int)Math.Round(floorHeight) - perGridYPadding); 
                PutColorfulText(ref button, Color.Red, (currentGrid.IndexOfStock+1).ToString(), false);
                destPanel.Controls.Add(button);
                #endregion
            }
            #region 编辑层按钮
            int editFloorBtnWidth = destPanel.Left - this.addFloorBySelectedWidthBtn.Left;
            Button editFloorBtn = new Button();
            editFloorBtn.Text = "操作整层";
            editFloorBtn.Tag = currentFloor;
            editFloorBtn.Click += editFloorBtn_Click;
            editFloorBtn.FlatStyle = FlatStyle.Popup;
            editFloorBtn.Size = new Size(editFloorBtnWidth - 4, (int)Math.Floor(floorHeight) - perGridYPadding);
            editFloorBtn.Location = new Point(this.addFloorBySelectedWidthBtn.Left, (int)Math.Floor(floorLocationY) + destPanel.Top);
            this.editFloorBtns.Add(editFloorBtn);
            this.Controls.Add(editFloorBtn);
            #endregion
            if (currentFloor.IndexOfStock>=0)
            {
                #region 添加格子按钮
                int addGridBtnWidth = this.addFloorBtn.Right - destPanel.Right;
                Button addGridBtn = new Button();
                addGridBtn.Text = "+";
                addGridBtn.Tag = currentFloor;
                addGridBtn.Click += addGridBtn_Click;
                addGridBtn.FlatStyle = FlatStyle.Popup;
                addGridBtn.Size = new Size(addGridBtnWidth - 12, (int)Math.Floor(floorHeight) - perGridYPadding);
                addGridBtn.Location = new Point(destPanel.Right + 6, (int)Math.Floor(floorLocationY) + (destPanel.Top));
                this.addGridBtns.Add(addGridBtn);
                this.Controls.Add(addGridBtn);
                #endregion
            }
            //下半部分的药槽不需要使用添加格子按钮.
        }

        #region 点击编辑层按钮的时候 显示层的信息
        void editFloorBtn_Click(object sender, EventArgs e)
        {
            AMDM_Floor currentFloor = (sender as Control).Tag as AMDM_Floor;
            //if (currentFloor == this.stock.Floors[0])
            //{

            //}
            FloorEditForm fform = new FloorEditForm();
            fform.Init(currentFloor);
            DialogResult ret = fform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Yes)
            {//需要更新数据和保存,不是点了关闭而取消
                #region 删除层
                if (fform.NeedDeleteFloor)
                {
                    //如果是要删除层
                    List<AMDM_Medicine> bindedMedicines = new List<AMDM_Medicine>();
                    for (int i = 0; i < currentFloor.Grids.Count; i++)
                    {
                        AMDM_Grid currentGrid = currentFloor.Grids[i];
                        var bindedMedicine = this.bindingManager.GetBindedMedicine(new Point(currentGrid.IndexOfFloor, currentFloor.IndexOfStock));
                        if (bindedMedicine != null)
                        {
                            bindedMedicines.Add(bindedMedicine);
                        }
                    }
                    if (bindedMedicines.Count > 0)
                    {
                        MessageBox.Show(string.Format("当前层中有{0}个格子已绑定了药品", bindedMedicines.Count));
                        return;
                    }
                    if (amdmInfoManager.RemoveFloorAndGrids(ref this.stock, currentFloor.Id))
                    {
                        MessageBox.Show("当前层板已移除");
                        this.clearStockShow();
                        this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
                    }
                }
                #endregion
                #region 编辑层
                else
                {
                    //一般的编辑格子.
                    if(amdmInfoManager.UpdateFloor(ref currentFloor, fform.WidthMM, fform.TopMM, fform.BottomMM))
                    {
                        MessageBox.Show("已保存");
                    }
                    else
                    {
                        MessageBox.Show("更新层信息失败,请检查数据库连接");
                    }
                }
                #endregion
                this.clearStockShow();
                this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
            }
        }
#endregion

        
        #region 添加格子按钮动作
        void addGridBtn_Click(object sender, EventArgs e)
        {
            var destFloor = (AMDM_Floor)((Control)sender).Tag;
            float start = destFloor.Grids.Count == 0? App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM/2: (float)destFloor.Grids[destFloor.Grids.Count-1].RightMM;
            float newGridWidth = App.Setting.HardwareSetting.Grid.NewGridDefaultWidth;
            if ((destFloor.WidthMM - App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM /2 )<(start+newGridWidth))
            {
                if ((destFloor.WidthMM - App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM/2 - start) >= App.Setting.HardwareSetting.Grid.PerGridMinWidthMM)
                {//虽然默认的100的放不进去,但是要是能放下大于或等于50的,那就放这个尺寸的
                    newGridWidth = destFloor.WidthMM - App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM/2 - start;
                }
                else
                {
                    MessageBox.Show(string.Format("至少需要{0}毫米可用空间以添加一个新的药槽\r\n该层可用宽度 {1} 毫米", App.Setting.HardwareSetting.Grid.PerGridMinWidthMM, destFloor.WidthMM -App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM/2 - start));
                    return;
                }
            }
            this.amdmInfoManager.CraeteAndJoinNewGrid(destFloor, null, start, newGridWidth);
            this.clearStockShow();
            this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
        }
        #endregion
        void PutColorfulText(ref GridShower textbox, Color color, string str, bool newLine = false)
        {
            if (newLine)
            {
                str = str + Environment.NewLine;
            }
            //textbox.SelectionStart = textbox.TextLength;
            //textbox.SelectionLength = 0;
            //textbox.SelectionColor = color;
            //textbox.AppendText(str);
            //textbox.SelectionColor = textbox.ForeColor;
        }

        void showGridEditMenu(object sender, MouseEventArgs e)
        {
            if (this.gridEditMenuOwner != null)
            {
                //之前点击的按钮
                Control lastClickButton = this.gridEditMenuOwner as Control;
                lastClickButton.BackColor = this.BackColor;
            }
            Control button = sender as Control;
            button.BackColor = Color.GreenYellow;
            this.gridEditMenuOwner = sender;
            if (e.Button == System.Windows.Forms.MouseButtons.Right || e.Clicks == 2)
            {
                AMDM_Grid gird = ((sender as Control).Tag as AMDM_Grid);
                if (gird.FloorIndex>=0)
                {
                    gridEditMenu.Show(sender as Control, new Point(e.X, e.Y));   
                }
                return;
            }
        }
        void clearStockShow()
        {
            for (int i = 0; i < this.panel1.Controls.Count; i++)
            {
                this.panel1.Controls[i].MouseClick -= this.showGridEditMenu;
                this.panel1.Controls[i].Dispose();
            }
            this.panel1.Controls.Clear();

            for (int i = 0; i < this.panel2.Controls.Count; i++)
            {
                this.panel2.Controls[i].MouseClick -= this.showGridEditMenu;
                this.panel2.Controls[i].Dispose();
            }
            this.panel2.Controls.Clear();

            for (int i = 0; i < this.addGridBtns.Count; i++)
            {
                this.Controls.Remove(addGridBtns[i]);
                addGridBtns[i].Click -= this.addGridBtn_Click;
                addGridBtns[i].Dispose();
            }
            this.addGridBtns.Clear();
            for (int i = 0; i < this.editFloorBtns.Count; i++)
            {
                this.Controls.Remove(editFloorBtns[i]);
                editFloorBtns[i].Click -= this.editFloorBtn_Click;
                editFloorBtns[i].Dispose();
            }
            this.editFloorBtns.Clear();
        }

        private void addFloorBtn_Click(object sender, EventArgs e)
        {
            //if (this.stock == null)
            //{
            //    this.stock = this.amdmInfoManager.CreateAndJoinStock(this.machine,0, 0, this.defaultStockFloorWidthMM);
            //}
            this.SuspendLayout();
            AMDM_Floor newFloor = this.amdmInfoManager.CreateAndJoinNewUpPartFloor(this.stock,
                App.Setting.HardwareSetting.Floor.NewFloorDefaultHeightMM, this.stock.MaxFloorWidthMM,App.Setting.HardwareSetting.Floor.UpPartFloorDepthMM);
            this.amdmInfoManager.CraeteAndJoinNewGrid(newFloor, null, 0, 
                App.Setting.HardwareSetting.Grid.NewGridDefaultWidth);
            this.clearStockShow();
            this.showStock(this.stock,this.showGridsByPercentCheckbox.Checked);
            this.ResumeLayout(false);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {

        }
        #region 左右移动挡板
        private void toLeftMenuItem_Click(object sender, EventArgs e)
        {
            /*
             * 向左移动挡板时,最多可移动宽度为左边药槽的宽度-30毫米(perGridMinWidthMM)
             * 每5毫米一个格,按照可移动的格子数量,分配按钮数量,每个按钮上显示要调整的距离,横向排列成为一个组合,然后把这些按钮显示到一个模态窗口中.点选后确定移动该宽度,确认后
             * 左边的格子减少相应的毫米数,右边的格子左侧的起点也减少相应的毫米数
             */
            AMDM_Grid currentGrid = (AMDM_Grid)(this.gridEditMenuOwner as Control).Tag;
            MoveGridWall(currentGrid, MoveDirectionEnum.LeftWall2Left);
        }
        #region 移动格子参数和函数
        
        bool MoveGridWall(AMDM_Grid currentGrid, MoveDirectionEnum type)
        {
            AMDM_Floor currentFloor = null;
            AMDM_Grid preGrid = null;
            AMDM_Grid nextGrid = null;
            #region 获取当前格子所在的行

                    currentFloor = this.stock.Floors[currentGrid.FloorIndex];
                    int currentIndex = currentFloor.Grids.IndexOf(currentGrid);
                    #region 左挡板向左移动
                    if (type == MoveDirectionEnum.LeftWall2Left)
                    {
                        if (currentIndex > 0)
                        {
                            preGrid = currentFloor.Grids[currentIndex - 1];
                        }
                        else
                        {
                            if (currentGrid.LeftMM <= 0)
                            {
                                MessageBox.Show("当前格在最左侧,且左挡板已经在零位,无法向左移动挡板");
                                return false;
                            }
                            else
                            {
                                //还可以向左移动的位置等于leftmm
                            }
                        }
                    }
                    #endregion
                    #region 右挡板向右移动
                    else if (type == MoveDirectionEnum.RightWall2Right)
                    {
                        if (currentIndex<(currentFloor.Grids.Count-1))
                        {
                            nextGrid = currentFloor.Grids[currentIndex + 1];
                        }
                        else
                        {
                            if (currentGrid.RightMM>= currentFloor.WidthMM)
                            {
                                MessageBox.Show("当前格在最右侧,且右挡板已经在终点位,无法向右移动挡板");
                                return false;
                            }
                        }
                    }
                    #endregion
                    #region 移除左侧的挡板
                    else if (type == MoveDirectionEnum.RemoveLeftWall)
                    {
                        if (currentIndex > 0)
                        {
                            preGrid = currentFloor.Grids[currentIndex - 1];
                        }
                        else
                        {
                            if (currentGrid.LeftMM <= 0)
                            {
                                MessageBox.Show("当前格在最左侧,且此挡板为零位挡板,不可移除");
                                return false;
                            }
                            else
                            {
                                //还可以向左移动的位置等于leftmm
                            }
                        }
                    }
                    #endregion
                    #region 移除右侧的挡板
                    else if (type == MoveDirectionEnum.RemoveWriteWall)
                    {
                        if (currentIndex < (currentFloor.Grids.Count - 1))
                        {
                            nextGrid = currentFloor.Grids[currentIndex + 1];
                        }
                        else
                        {
                            if (currentGrid.RightMM >= currentFloor.WidthMM)
                            {
                                MessageBox.Show("当前格在最右侧,且此挡板为终点位挡板,不可移除");
                                return false;
                            }
                        }
                    }
                    #endregion
                    //已经找到当前行 直接可以退出循环
            #endregion
            if (currentFloor != null
                //&& (preGrid != null || nextGrid!= null)
                //&& type.ToString().Contains("Remove") == false
                )
            {//符合移动条件,也就是检测到了当前的格子和要移动的方向的邻居格子的话.
                #region 生成可移动的距离相关按钮集合
                float maxCanMoveWidth = 0;
                if (type == MoveDirectionEnum.LeftWall2Left)
                {
                    maxCanMoveWidth = preGrid.RightMM - preGrid.LeftMM - App.Setting.HardwareSetting.Grid.PerGridMinWidthMM;
                }
                else if( type == MoveDirectionEnum.RightWall2Right)
                {
                    maxCanMoveWidth = nextGrid == null ? (currentFloor.WidthMM - currentGrid.RightMM) : (nextGrid.RightMM - nextGrid.LeftMM - App.Setting.HardwareSetting.Grid.PerGridMinWidthMM);
                }
                if (maxCanMoveWidth + (currentGrid.RightMM - currentGrid.LeftMM) > App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM)
                {
                    //如果要移动的距离加上当前格子的宽度大于最大的格子宽度的话,把最大可移动空间缩小到  最大格子-当前格子 的大小
                    maxCanMoveWidth = App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM - (currentGrid.RightMM - currentGrid.LeftMM);
                }
                if ((currentGrid.RightMM - currentGrid.LeftMM) >= App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM)
                {
                    MessageBox.Show(string.Format("当前选中的药槽已经达到最大药槽限定宽度\r\n最大宽度应在{0}毫米内", App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM));
                    return false;
                }
                if (maxCanMoveWidth <= 0)
                {
                    MessageBox.Show(string.Format("相邻格子的尺寸太小(小于最小药槽宽度<{0}毫米>),已无法移动", App.Setting.HardwareSetting.Grid.PerGridMinWidthMM));
                    return false;
                }
                else
                {
                    int canMoveStep = ((int)Math.Floor(maxCanMoveWidth)) / App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM;
                    if (canMoveStep < 1)
                    {
                        MessageBox.Show("无法继续左移,没有可移动空间");
                        return false;
                    }
                    int perBtnWidthPixel = Screen.PrimaryScreen.WorkingArea.Width / canMoveStep;
                    int perBtnHeightPixel = perBtnWidthPixel / 2;

                    Form selectSizeForm = new Form();
                    #region 调整宽度的弹窗的标题
                    string selectSizeFormTitle = null;
                    if (type == MoveDirectionEnum.LeftWall2Left)
                    {
                        selectSizeFormTitle = string.Format("调整{0}-{1}的宽度,左挡板向左偏移量选择", currentFloor.IndexOfStock + 1, currentFloor.Grids.IndexOf(currentGrid) + 1);
                    }
                    else if (type == MoveDirectionEnum.RightWall2Right)
                    {
                        selectSizeFormTitle=  string.Format("调整{0}-{1}的宽度,右挡板向右偏移量选择", currentFloor.IndexOfStock + 1, currentFloor.Grids.IndexOf(currentGrid) + 1);
                    }
                    #endregion
                    selectSizeForm.Text = selectSizeFormTitle;
                    int xPadding = 10;
                    int yPadding = 20;
                    selectSizeForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                    selectSizeForm.StartPosition = FormStartPosition.CenterParent;
                    Size panelSize = new Size(0, perBtnHeightPixel);

                    for (int i = 0; i < canMoveStep; i++)
                    {
                        Button stepButton = new Button();
                        #region 移动固定尺寸的标题按钮
                        switch (type)
                        {
                            case MoveDirectionEnum.LeftWall2Left:
                                stepButton.Text = string.Format("向<<-扩展{0}毫米", (canMoveStep - i) * App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM);
                                stepButton.Tag = new MoveGridParam() { Dest = preGrid, Src = currentGrid, OffsetMM = -(canMoveStep - i) * App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM, ParantForm = selectSizeForm };
                                break;
                            case MoveDirectionEnum.RightWall2Right:
                                stepButton.Text = string.Format("向->>扩展{0}毫米", (i + 1) * App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM);
                                stepButton.Tag = new MoveGridParam() { Dest = nextGrid, Src = currentGrid, OffsetMM = (i + 1) * App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM, ParantForm = selectSizeForm };
                                break;
                            case MoveDirectionEnum.LeftWall2Right:
                                break;
                            case MoveDirectionEnum.RightWall2Left:
                                break;
                            case MoveDirectionEnum.RemoveLeftWall:
                                break;
                            case MoveDirectionEnum.RemoveWriteWall:
                                break;
                            default:
                                break;
                        }
                        #endregion
                        stepButton.Location = new Point(i * perBtnWidthPixel + xPadding, yPadding);
                        stepButton.Size = new Size(perBtnWidthPixel, perBtnHeightPixel);
                        stepButton.Click += stepButton_Click;
                        selectSizeForm.Controls.Add(stepButton);
                        panelSize.Width += stepButton.Width;
                    }

                    panelSize.Width += xPadding * 2;
                    panelSize.Height += yPadding * 2;
                    selectSizeForm.ClientSize = panelSize;

                    selectSizeForm.ShowDialog();
                }
                #endregion
            }
            return true;
        }
        
        #endregion
        #region 点击向左向右移动具体毫米数的按钮
        void stepButton_Click(object sender, EventArgs e)
        {
            MoveGridParam param = (sender as Control).Tag as MoveGridParam;
            
            //释放所有的回调函数,关闭窗体
            for (int i = 0; i < param.ParantForm.Controls.Count; i++)
            {
                (param.ParantForm.Controls[i] as Button).Click -= stepButton_Click;
            }
            param.ParantForm.DialogResult = System.Windows.Forms.DialogResult.OK;
            param.ParantForm.Close();

            if (amdmInfoManager.MoveGridWall(param) == false)
            {
                MessageBox.Show("更新药槽数据失败");
                return;
            }

            //移动时,必须开启按实际比例显示模式
            this.showGridsByPercentCheckbox.Checked = true;
            //重新显示格子信息
            this.clearStockShow();
            this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
        }
        #endregion
       

        private void toRightMenuItem_Click(object sender, EventArgs e)
        {
            AMDM_Grid currentGrid = (AMDM_Grid)(this.gridEditMenuOwner as Control).Tag;
            MoveGridWall(currentGrid, MoveDirectionEnum.RightWall2Right);
        }
        #endregion

        #region 移除挡板
        private void removeLeftMenuItem_Click(object sender, EventArgs e)
        {
            RemoveGridWall(sender, e, MoveDirectionEnum.RemoveLeftWall);
        }

        void RemoveGridWall(object sender,EventArgs e, MoveDirectionEnum type)
        {
            AMDM_Grid currentGrid = (AMDM_Grid)(this.gridEditMenuOwner as Control).Tag;
            AMDM_Floor currentFloor = null;
            AMDM_Grid preGrid = null;
            AMDM_Grid nextGrid = null;
            #region 获取药槽的所在层
            for (int i = 0; i < this.stock.Floors.Count; i++)
            {
                if (this.stock.Floors[i].Grids.Contains(currentGrid))
                {
                    currentFloor = this.stock.Floors[i];
                    int index = currentFloor.Grids.IndexOf(currentGrid);
                    if(index >0)
                    {
                        preGrid = currentFloor.Grids[index - 1];
                    }
                    if((index +1 )<currentFloor.Grids.Count)
                    {
                        nextGrid = currentFloor.Grids[index + 1];
                    }
                    break;
                }
            }
            #endregion
            #region 移除药槽时,要检查当前药槽内是否有绑定的药品
            this.bindingManager.GetBindedMedicine(new Point(currentGrid.IndexOfFloor, currentFloor.IndexOfStock));
            #endregion
            
            //如果是移除挡板
            switch (type)
            {
                case MoveDirectionEnum.RemoveLeftWall:
                    if (preGrid == null)
                    {
                        MessageBox.Show("不能移除最左侧药槽的的左侧挡板");
                        return;
                    }
                    if ((currentGrid.RightMM - preGrid.LeftMM) > App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM)
                    {
                        MessageBox.Show(string.Format("移除该挡板后两个药槽合并后的宽度超出最大宽度限制{0}", App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM));
                        return;
                    }
                    break;
                case MoveDirectionEnum.RemoveWriteWall:
                    if (nextGrid == null)
                    {
                        MessageBox.Show("不能移除最右侧药槽的右侧挡板"); ;
                        //return false;
                        return;
                    }
                    if ((nextGrid.RightMM - currentGrid.LeftMM) > App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM)
                    {
                        MessageBox.Show(string.Format("移除该挡板后两个药槽合并后的宽度超出最大宽度限制{0}", App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM));
                        return;
                    }
                    break;
                default:
                    break;
            }
            if (amdmInfoManager.RemoveGridWall(currentGrid, ref currentFloor, type))
            {
                //移动时,必须开启按实际比例显示模式
                this.showGridsByPercentCheckbox.Checked = true;
                //重新显示格子信息
                this.clearStockShow();
                this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
            }
           
        }

        private void removeRightMenuItem_Click(object sender, EventArgs e)
        {
            RemoveGridWall(sender, e, MoveDirectionEnum.RemoveWriteWall);
        }
        #endregion

        

        object gridEditMenuOwner = null;
        private void gridEditMenu_Opening(object sender, CancelEventArgs e)
        {
        }

        #region 添加层,使用选择的宽度的药槽进行填充层
        private void addFloorBySelectedWidthBtn_Click(object sender, EventArgs e)
        {
            SelectGridWidthForm sform = new SelectGridWidthForm();
            sform.Init(App.Setting.HardwareSetting.Grid.GridWallWidthMM,
                App.Setting.HardwareSetting.Grid.NewGridDefaultWidth,
                App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM,
                App.Setting.HardwareSetting.Grid.PerGridMinWidthMM, App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM);
            sform.StartPosition = FormStartPosition.CenterParent;
            DialogResult ret= sform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                //if (this.stock == null)
                //{
                //    this.stock = this.amdmInfoManager.CreateAndJoinStock(this.machine, 0, 0, this.defaultStockFloorWidthMM);
                //}
                this.SuspendLayout();
                AMDM_Floor newFloor = this.amdmInfoManager.CreateAndJoinNewUpPartFloor(this.stock, 
                    App.Setting.HardwareSetting.Floor.NewFloorDefaultHeightMM, 
                    this.stock.MaxFloorWidthMM,App.Setting.HardwareSetting.Floor.UpPartFloorDepthMM);
                //该层可以加入的格子总数是多少
                int maxCanJoinGridCount = (int)Math.Floor(this.stock.MaxFloorWidthMM / sform.SelectedWidthMM);
                for (int i = 0; i < maxCanJoinGridCount; i++)
                {
                    this.amdmInfoManager.CraeteAndJoinNewGrid(newFloor, null, i*sform.SelectedWidthMM, sform.SelectedWidthMM);
                }
                
                this.clearStockShow();
                this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
                this.ResumeLayout(false);
            }
        }
        #endregion

        private void clearStockBtn_Click(object sender, EventArgs e)
        {
           bool clearRet= this.amdmInfoManager.ClearStockFloorAndGirds(this.stock);
           if (clearRet)
           {
               this.clearStockShow();
               this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
           }
           else
           {
               MessageBox.Show("清空失败");
           }
        }

        private void setDefaultGridWidMMBtn_Click(object sender, EventArgs e)
        {
            SelectGridWidthForm sform = new SelectGridWidthForm();
            sform.Init(App.Setting.HardwareSetting.Grid.GridWallWidthMM,
                App.Setting.HardwareSetting.Grid.NewGridDefaultWidth,
                App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM,
                App.Setting.HardwareSetting.Grid.PerGridMinWidthMM,
                App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM);
            sform.StartPosition = FormStartPosition.CenterParent;
            DialogResult ret = sform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                App.Setting.HardwareSetting.Grid.NewGridDefaultWidth = sform.SelectedWidthMM;
            }
            this.setDefaultGridWidMMBtn.Text = string.Format("↔新药槽默认内径{0}mm",
                App.Setting.HardwareSetting.Grid.NewGridDefaultWidth - App.Setting.HardwareSetting.Grid.GridWallWidthMM);
        }

        private void 移除当前层板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //检查当前层板上的所有格子有没有绑定的药品.如果没有绑定的药品,再次确认是否移除后直接移除数据库上的数据和本类中对象中的数据 也就是从stock中的floors的list中移除该floor

            //if (MessageBox.Show("请再次确认是否要移除整层?", "再次确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Cancel)
            //{
            //    return;
            //}

            //Control currentGridButton = this.gridEditMenuOwner as Control;
            //AMDM_Grid currentGrid = currentGridButton.Tag as AMDM_Grid;
            //List<MedicineInfo> bindedMedicines = new List<MedicineInfo>();
            //AMDM_Floor currentFloor = null;
            //for (int i = 0; i < this.stock.Floors.Count; i++)
            //{
            //    currentFloor = this.stock.Floors[i];
            //    if (currentFloor.Grids.Contains(currentGrid) == true)
            //    {
            //        var bindedMedicine = this.bindingManager.GetBindedMedicine(new Point(currentGrid.IndexOfFloor, currentFloor.IndexOfStock));
            //        if (bindedMedicine != null)
            //        {
            //            bindedMedicines.Add(bindedMedicine);
            //        }
            //        break;
            //    }
            //}
            //if (bindedMedicines.Count>0)
            //{
            //    MessageBox.Show(string.Format("当前层内有{0}个药槽有绑定的药品\r\n需清空药槽并接触绑定后再删除当前层", bindedMedicines.Count));
            //}
            //#region 删除层板的格子和层数据
            ////先删除格子信息,再删除删除层信息.
            //for (int i = 0; i < currentFloor.Grids.Count; i++)
            //{
            //    amdmInfoManager.RemoveFloorAndGrids(ref this.stock, currentFloor.Id);
            //}
            //#endregion
            //MessageBox.Show("移除层板完成");
            //this.clearStockShow();
            //this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
        }

        #region 使用模板文件保存或加载层格信息
        private void initByTemplateFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.DefaultExt = "*.*";
            AMDM_Stock tempStock = null;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (System.IO.File.Exists(of.FileName) == true)
                {
                    string json = System.IO.File.ReadAllText(of.FileName);
                    try
                    {
                        tempStock = JsonConvert.DeserializeObject<AMDM_Stock>(json);
                        #region 如果加载文件成功,那就要提示是不是用现有的文件替换掉当前的药库信息.如果是的话 就把现在所有的药库信息清空,然后在导入这个文件.
                        if (MessageBox.Show("此操作将清空当前药仓的所有层与槽信息!确认要执行此操作吗?", "将抹掉现有数据", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                        else
                        {
                            amdmInfoManager.ClearStockFloorAndGirds(this.stock);
                        }
                        #endregion
                    }
                    catch (Exception jsonParseErr)
                    {
                        MessageBox.Show(string.Format("解析模板文件失败!\r\n{0}",jsonParseErr.Message));
                        return;
                    }
                }
            }
            #region 填充完了stock信息以后,只是记录了当前的类,但是没有进行一个数据的保存.
            //要使用client下面的保存stock的方法保存stock的数据到数据库
            bool joinFloorsAndGridsRet = this.amdmInfoManager.JoinFloorsAndGrids(ref this.stock, tempStock.Floors);
            if (joinFloorsAndGridsRet)
            {
                MessageBox.Show("数据导入完成\r\n已重新初始化该药仓的层和药槽数据");
            }
            #endregion
            this.clearStockShow();
            this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
        }

        private void save2TemplateBtn_Click(object sender, EventArgs e)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            string json = JsonConvert.SerializeObject(this.stock, jss);
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = "json";
            sf.FileName = "stockTemplate.json";
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string dir = System.IO.Path.GetDirectoryName(sf.FileName);
                string shortFileName = System.IO.Path.GetFileName(sf.FileName);
                if (System.IO.Directory.Exists(dir) == false)
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                if (System.IO.File.Exists(sf.FileName) == false)
                {
                    System.IO.File.Create(sf.FileName).Close();
                }
                System.IO.File.WriteAllText(sf.FileName, json);
                MessageBox.Show(string.Format("文件已保存到\r\n{0}", sf.FileName),"模板已保存");
            }
        }
        #endregion

        #region 移动到选择的药槽
        private void moveToSelectedGridBtn_Click(object sender, EventArgs e)
        {
            if (this.gridEditMenuOwner == null)
            {
                MessageBox.Show("未选择药槽");
                return;
            }
            var destGrid = (this.gridEditMenuOwner as Control).Tag as AMDM_Grid;
            PLCCommunicator4StockTesting_台达 pc = new PLCCommunicator4StockTesting_台达(this.stock.IndexOfMachine, plcSetting, this.stock.CenterDistanceBetweenTwoGrabbers, this.stock.XOffsetFromStartPointMM, this.stock.YOffsetFromStartPointMM, App.medicinesGettingController.MainPLCCommunicator);
            pc.Connect();


            float xpos = destGrid.LeftMM + (destGrid.RightMM - destGrid.LeftMM) / 2;
            float ypos = this.stock.Floors[destGrid.FloorIndex].BottomMM;
            WhichGrabberEnum grabber = (destGrid.IndexOfFloor >= 5) ? WhichGrabberEnum.Far : WhichGrabberEnum.Near;
            int times = 0;
            //pc.SendGrabberPositioningTestCommandTest(xpos,ypos,grabber, times);
            pc.SendGrabberPositioningTestCommandTest(xpos, ypos, grabber, times);

            System.Threading.Thread.Sleep(100);
            PLCStatusData lastStatus = null;
            while (true)
            {
                var status = App.medicinesGettingController.MainPLCCommunicator.GetMedicineGettingStatus();
                if (status == null )
                {
                    System.Threading.Thread.Sleep(17);
                    continue;
                }
                if ( status.Main == MainBufferValuesEnum.Error)
                {
                    lastStatus = status;
                    break;
                }
                if ( status.Main != MainBufferValuesEnum.FinishedDeliveryOneMedicine)
                {
                    System.Threading.Thread.Sleep(17);
                    continue;
                }
                else
                {
                    lastStatus = status;
                    break;
                }
            }
            if (lastStatus!= null)
            {
                string json = JsonConvert.SerializeObject(lastStatus);
                MessageBox.Show(json);
                pc.Disconnect();
            }
        }
        #endregion

        private void setDefaultFloorHeightMMBtn_Click(object sender, EventArgs e)
        {
            SelectFloorHeightForm sform = new SelectFloorHeightForm();
            sform.Text = "↕ 设置默认层间距";
            sform.Init(App.Setting.HardwareSetting.Floor.NewFloorDefaultHeightMM,
                0,0,0);
            sform.StartPosition = FormStartPosition.CenterParent;
            DialogResult ret = sform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                App.Setting.HardwareSetting.Floor.NewFloorDefaultHeightMM = sform.SelectedHeightMM;
            }
            this.setDefaultFloorHeightMMBtn.Text = string.Format("↕ 新层默认层间距{0}mm", App.Setting.HardwareSetting.Floor.NewFloorDefaultHeightMM);
        }

        private void setStockBaseInfoBtn_Click(object sender, EventArgs e)
        {
            StockBasicInfoEditForm sform = new StockBasicInfoEditForm();
            sform.Init(this.stock);
           DialogResult ret = sform.ShowDialog();
           if (ret == System.Windows.Forms.DialogResult.OK)
           {
               #region 删除药仓的操作
               
               #endregion
               if (sform.NeedDeleteStock)
               {
                  if(amdmInfoManager.RemoveStock(ref this.machine, this.stock.Id))
                  {
                      MessageBox.Show("药仓已删除");
                      this.DialogResult = System.Windows.Forms.DialogResult.OK;
                      this.Close();
                      return;
                  }
               }
               #region 编辑药仓的操作
               else
               {
                   if(amdmInfoManager.UpdateStock(ref this.stock, sform.MaxFloorWidthMM, sform.XOffsetFromStartPointMM, sform.YOffsetFromStartPointMM, sform.CenterDistanceBetweenTwoGrabbers))
                   {
                       MessageBox.Show("已保存");
                   }
                   else
                   {
                       MessageBox.Show("保存药仓信息失败.请检查数据连接");
                   }
               }
               #endregion
               this.clearStockShow();
               this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
           }
        }

        private void keepMove2RandomGridBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.randomMedicineGettingModeCombox.Text) == true)
            {
                Console.WriteLine("先选择测试模式");
                return;
            }
            
            this.statusLabel.Text = "开始测试取药";
            this.randomMedicineGettingModeCombox.Enabled = false;
            string stopText = "停止测试取药";
            string text = keepMove2RandomGridBtn.Text;
            if (text == stopText)
            {
                StopMove2RandomGrid();
            }
            else
            {
                
                #region 点击了开始随机走位
                if (this.keepMove2RandomGridBW.IsBusy == false)
                {
                    keepMove2RandomGridBW.RunWorkerAsync();
                    keepMove2RandomGridBtn.Text = stopText;
                    DialogResult ignore300ErrorRet = MessageBox.Show(this, "是否忽略掉药光栅检测?", "光栅检测", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (ignore300ErrorRet == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.ignore300Error = true;
                    }
                }
                else
                {
                    MessageBox.Show("走位任务正在进行中.请稍后再试");
                } 
                #endregion
            }
        }
        #region 随机走位的线程
        /// <summary>
        /// 是否忽略300类错误信号,如果忽略,机械手将获取到300后继续执行后续的走位动作
        /// </summary>
        bool ignore300Error = false;
        PLCCommunicator4StockTesting_台达 pc = null;
        BackgroundWorker keepMove2RandomGridBW = new BackgroundWorker();
        void keepMove2RandomGridBW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            #region 如果状态小于1 关闭plc的连接
            if (e.ProgressPercentage <0)
            {
                StopMove2RandomGrid();
            }
            #endregion
            
            if(e.ProgressPercentage == -100)
            {
                MessageBox.Show(e.UserState as string);
            }
            else if (e.ProgressPercentage == -1)
            {
                MessageBox.Show("获取plc状态超时");
            }
            else if(e.ProgressPercentage == -2)
            {
                MessageBox.Show("取药超时");
            }
            else if(e.ProgressPercentage == 300)
            {
                StopMove2RandomGrid();
                MessageBox.Show("取药发生错误!可能没有药了");
                this.statusLabel.Text = e.UserState as string;
            }
            //throw new NotImplementedException();
        }
        void StopMove2RandomGrid()
        {
            if (this.keepMove2RandomGridBW.IsBusy)
            {
                this.keepMove2RandomGridBW.CancelAsync();
            }
            this.statusLabel.Text = "已停止测试取药";
            this.randomMedicineGettingModeCombox.Enabled = true;
            string startText = "开始测试取药";
            this.keepMove2RandomGridBtn.Text = startText;
            this.keepMove2RandomGridBW.CancelAsync();
            if (pc != null)
            {
                try
                {
                    pc.Disconnect();
                }
                catch (Exception err)
                {
                    string msg = string.Format("尝试关闭plc连接错误:{0}", err.Message);
                    MessageBox.Show(msg);
                }
            }
        }

        public enum TestMedicineGettingModeEnum { 
            只取最下面一层,
            只取最上面一层,
            只取最右侧药槽,
            只取最左侧药槽,
            随机取药,
            顺时针最外侧绕行,
            逆时针最外侧绕行,
            随机外侧绕行,
            从下到上从左到右Z形,
            从下到上从右到左Z形,
            从上到下从左到右Z形,
            从上到下从右到左Z形,
            从下到上从左到右S形,
            从下到上从右到左S形,
            从上到下从左到右S形,
            从上到下从右到左S形,

        };
        int getUpPartTopestFloorIndex(AMDM_Stock stock)
        {
            int max = 0;
            foreach (var item in stock.Floors)
            {
                if (item.Value.IndexOfStock>max)
                {
                    max = item.Value.IndexOfStock;
                }
            }
            return max;
        }
        void keepMove2RandomGridBW_DoWork(object sender, DoWorkEventArgs e)
        {
           
            TestMedicineGettingModeEnum mode = (TestMedicineGettingModeEnum)Enum.Parse(typeof(TestMedicineGettingModeEnum), this.randomMedicineGettingModeCombox.Text);
            int getStatusTimeoutMS = 5000;
            int getReadyStatusTimeoutMS = 30000;
            //最多取药次数
            int maxGettingTimes = 5000;
            int finishedGettingTimes = 0;
            //取药开始的时间
            DateTime startTime = DateTime.Now;
            #region 先测试连接
            try
            {
                pc = new PLCCommunicator4StockTesting_台达(this.stock.IndexOfMachine, plcSetting, this.stock.CenterDistanceBetweenTwoGrabbers, this.stock.XOffsetFromStartPointMM, this.stock.YOffsetFromStartPointMM, App.medicinesGettingController.MainPLCCommunicator);
                pc.Connect();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("PLC连接完成...");
                Console.ResetColor();
            }
            catch (Exception connectErr)
            {
                string msg = string.Format("plc连接错误:{0}", connectErr.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
                this.keepMove2RandomGridBW.ReportProgress(-100, msg);
                return;
            }
            #endregion
            int currentRoundIndex = 0;
            ///扫描模式的时候偶 将要扫描的格子的index  最大不超过格子数量-1;
            int scanModeIndex = 0;
            ///扫描模式的时候要扫描的格子,根据不同的模式生成
            List<AMDM_Grid> scanModeGrids = null;
            switch (mode)
            {
                case TestMedicineGettingModeEnum.从下到上从左到右Z形:
                    scanModeGrids = GetGridListByBottom2TopLeft2Right();
                    break;
                case TestMedicineGettingModeEnum.从下到上从右到左Z形:
                    scanModeGrids = GetGridListByBottom2TopRight2Left();
                    break;
                case TestMedicineGettingModeEnum.从上到下从左到右Z形:
                    scanModeGrids = GetGridListByTop2BottomLeft2Right();
                    break;
                case TestMedicineGettingModeEnum.从上到下从右到左Z形:
                    scanModeGrids = GetGridListByTop2BottomRight2Left();
                    break;
                case TestMedicineGettingModeEnum.从下到上从左到右S形:
                    scanModeGrids = GetGridListByBottom2TopLeft2Right(false);
                    break;
                case TestMedicineGettingModeEnum.从下到上从右到左S形:
                    scanModeGrids = GetGridListByBottom2TopRight2Left(false);
                    break;
                case TestMedicineGettingModeEnum.从上到下从左到右S形:
                    scanModeGrids = GetGridListByTop2BottomLeft2Right(false);
                    break;
                case TestMedicineGettingModeEnum.从上到下从右到左S形:
                    scanModeGrids = GetGridListByTop2BottomRight2Left(false);
                    break;
                default:
                    break;
            }
            Console.WriteLine("最大取药次数:{0}", maxGettingTimes);
            //扫描模式时 横向索引
            //int scanModeXIndex = 0;
            //扫描模式时 纵向索引
            //int scanModeYIndex = 0;
            while (this.keepMove2RandomGridBW.CancellationPending == false && maxGettingTimes>finishedGettingTimes)
            {
                DateTime thisTimeStartTime = DateTime.Now;
                //Console.WriteLine("准备获取目标格信息");
                #region 获取随机数
                int randomFloorIndex = 0; 
                int randomGridIndex = 0;
                #region 外侧4个格子
                int topestFloorIndex = getUpPartTopestFloorIndex(this.stock);
                Point topLeftGrid = new Point(0, topestFloorIndex);
                Point topRightGrid = new Point(this.stock.Floors[topestFloorIndex].Grids.Count - 1, topestFloorIndex);
                Point bottomLeftGrid = new Point(0, 0);
                Point bottomRightGrid = new Point(this.stock.Floors[0].Grids.Count-1, 0);
                List<Point> points = new List<Point>() {topLeftGrid,topRightGrid,bottomRightGrid, bottomLeftGrid };
                #endregion
                #region 根据不同的方案 随机获取位置
                switch (mode)
                {
                    case TestMedicineGettingModeEnum.只取最下面一层:
                        randomFloorIndex = 0;
                        randomGridIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, this.stock.Floors[randomFloorIndex].Grids.Count);
                        break;
                    case TestMedicineGettingModeEnum.只取最上面一层:
                        randomFloorIndex = topestFloorIndex;
                        randomGridIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, this.stock.Floors[randomFloorIndex].Grids.Count);
                        break;
                    case TestMedicineGettingModeEnum.只取最右侧药槽:
                        randomFloorIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, topestFloorIndex+1);
                        randomGridIndex = this.stock.Floors[randomFloorIndex].Grids.Count - 1;
                        break;
                    case TestMedicineGettingModeEnum.只取最左侧药槽:
                        randomFloorIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, topestFloorIndex+1);
                         randomGridIndex = 0;
                        break;
                    case TestMedicineGettingModeEnum.随机取药:
                        randomFloorIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, topestFloorIndex+1);
                        randomGridIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, this.stock.Floors[randomFloorIndex].Grids.Count);
                        break;
                    case TestMedicineGettingModeEnum.顺时针最外侧绕行:
                        randomFloorIndex = points[currentRoundIndex].Y;
                        randomGridIndex = points[currentRoundIndex].X;
                        currentRoundIndex++;
                        if (currentRoundIndex >3)
                        {
                            currentRoundIndex = 0;
                        }
                        break;
                    case TestMedicineGettingModeEnum.逆时针最外侧绕行:
                        randomFloorIndex = points[3-currentRoundIndex].Y;
                        randomGridIndex = points[3-currentRoundIndex].X;
                        currentRoundIndex--;
                        if (currentRoundIndex < 0)
                        {
                            currentRoundIndex = 3;
                        }
                        break;
                    case TestMedicineGettingModeEnum.随机外侧绕行:
                        int randomRoundPointIndex = new Random(Guid.NewGuid().GetHashCode()).Next(0, 4);
                        randomFloorIndex = points[randomRoundPointIndex].Y;
                        randomGridIndex = points[randomRoundPointIndex].X;
                        break;
                    case TestMedicineGettingModeEnum.从下到上从左到右Z形:
                    case TestMedicineGettingModeEnum.从下到上从右到左Z形:
                    case TestMedicineGettingModeEnum.从上到下从左到右Z形:
                    case TestMedicineGettingModeEnum.从上到下从右到左Z形:
                    case TestMedicineGettingModeEnum.从下到上从左到右S形:
                    case TestMedicineGettingModeEnum.从下到上从右到左S形:
                    case TestMedicineGettingModeEnum.从上到下从左到右S形:
                    case TestMedicineGettingModeEnum.从上到下从右到左S形:
                        randomFloorIndex = scanModeGrids[scanModeIndex].FloorIndex;
                        randomGridIndex = scanModeGrids[scanModeIndex].IndexOfFloor;
                        scanModeIndex++;
                        if (scanModeIndex>= scanModeGrids.Count)
                        {
                            scanModeIndex = 0;
                        }
                        break;
                    default:
                        break;
                }
                #endregion

                //int randomFloorIndex = 0;
                AMDM_Floor randomFloor = this.stock.Floors[randomFloorIndex];

                if (randomFloor.Grids == null || randomFloor.Grids.Count == 0)
                {
                    continue;
                }
                //int randomGridIndex = randomFloor.Grids.Count - 1;
                AMDM_Grid randomGrid = randomFloor.Grids[randomGridIndex];
                #endregion
                //Console.WriteLine("准备计算位置信息");
                #region 计算要走的位置并发送取药信号
                float xpos = randomGrid.LeftMM + (randomGrid.RightMM - randomGrid.LeftMM) / 2;
                float ypos = this.stock.Floors[randomGrid.FloorIndex].BottomMM;
                WhichGrabberEnum grabber = (randomGrid.IndexOfFloor >= 5) ? WhichGrabberEnum.Far : WhichGrabberEnum.Near;
                #endregion
                //Console.WriteLine("准备发送取药信号");
                #region 发送取药信号
                //取药次数
                int times = 1;
                bool sendGettingCommandRet = false;
                try
                {
                    System.Threading.Thread.Sleep(2000);
                    Utils.LogSimulating("每次发送取药命令之前间隔2秒钟");
                    sendGettingCommandRet = pc.SendStartMedicineGettingCommandTest(xpos, ypos, grabber, times);
                }
                catch (Exception err)
                {
                    Console.WriteLine("发送取药信号错误:{0}", err.Message);
                }
                if (sendGettingCommandRet)
                {
                    //Console.WriteLine("发送取药信号完成");
                }
                #endregion
                
                
                
                Console.WriteLine(string.Format("即将到药槽:{0}-{1}取药", randomGrid.FloorIndex+1,randomGrid.IndexOfFloor+1));

                //从plc读的取单次取药的返回值
                PLCStatusData status = null;
                //开始取药的时间
                DateTime startGetMedicineTime = DateTime.Now;
                #region 等待plc返回200信号
                while ((DateTime.Now - startGetMedicineTime).TotalMilliseconds < getReadyStatusTimeoutMS && this.keepMove2RandomGridBW.CancellationPending == false)
                {
                    //Console.WriteLine("已发送开始取药命令,正在等待获取取药完成状态");
                    #region 获取返回状态
                    DateTime startGetStatusTime = DateTime.Now;
                    while ((DateTime.Now - startGetStatusTime).TotalMilliseconds < getStatusTimeoutMS)
                    {
                        System.Threading.Thread.Sleep(997);
                        status = App.medicinesGettingController.MainPLCCommunicator.GetMedicineGettingStatus();
                        Console.WriteLine("尝试获取返回值状态耗时{0}", (DateTime.Now - startGetStatusTime).TotalMilliseconds);
                        if (status != null)
                        {
                            //Console.WriteLine(string.Format("获取到返回状态:{0}", status.Main));
                            break;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("未获取到取药完成状态!!!!!!"));
                            //if (pc.Connect() == false)
                            //{
                            //    MessageBox.Show("PLC已断开连接");
                            //    return;
                            //}
                            continue;
                        }
                    }
                    //如果获取状态为空,超时
                    if (status == null)
                    {
                        Console.WriteLine(string.Format("获取取药完结状态超时"));
                        this.keepMove2RandomGridBW.ReportProgress(-1);
                        return;
                    }
                    #endregion
                    #region 如果单次取药完成,跳出本次取药状态获取
                    if (status.Main == MainBufferValuesEnum.FinishedDeliveryOneMedicine)
                    {
                        finishedGettingTimes++;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(string.Format("完成本次取药,重置状态,已取药品数量:{0}\r\n本次用时:{1},总用时:{2}秒",finishedGettingTimes,(DateTime.Now - thisTimeStartTime).TotalSeconds, (DateTime.Now-startTime).TotalSeconds));
                        Console.ResetColor();
                        //如果状态为取药完成 继续下一次取药,在取药之前,先清空plc的main数据为0
                        pc.SendClearMainStatusWaitNextMedicineGettingCommandTest();
                        break;
                    }
                    #endregion
                    #region 如果返回状态为300
                    else if(status.Main == MainBufferValuesEnum.Error)
                    {
                        if (ignore300Error)
                        {
                            Utils.LogInfo("PLC发来未检测到药品掉落信号,已经设置为忽略此检测,继续执行下次模拟取药");
                            pc.SendClearMainStatusWaitNextMedicineGettingCommandTest();
                            break;
                        }
                        else
                        {
                            string errMsg = string.Format("获取到的取药状态为:发生错误,可能没有药了.状态码:{0}", status.Main);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(errMsg);
                            Console.ResetColor();
                            this.keepMove2RandomGridBW.ReportProgress(300, errMsg);
                            return;
                        }
                    }
                    #endregion
                    #region 如果计数器检测光栅被遮挡
                    else if (status.CounterCoverdError)
                    {
                        string errmsg = "计数器光栅被长时间遮挡!!!!";
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(errmsg);
                        Console.ResetColor();
                        this.keepMove2RandomGridBW.ReportProgress(-100, errmsg);
                        return;
                    }
                    #endregion
                    #region 其他状态
                    else
                    {
                        //Console.WriteLine(string.Format("获取到取药完结状态不是取药完成,当前状态:{0}  继续获取取药状态,当前已取药次数:{1}", status.Main, finishedGettingTimes));
                        continue;
                    }
                    #endregion
                }
                #endregion
                #region 如果是超时跳出,status没有值或者是取药状态不是200的话,那就是取药超时
                if (status == null || status.Main != MainBufferValuesEnum.FinishedDeliveryOneMedicine && ignore300Error == false)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("取药超时,status是否为空?:{0} status:{1}", status == null, status==null? "空":status.Main.ToString()));
                    Console.ResetColor();
                    this.keepMove2RandomGridBW.ReportProgress(-2);
                    return;
                }
                #endregion
            }
            Utils.LogFinished("模拟取药完成,最大取药次数:", maxGettingTimes);
        }
        /// <summary>
        /// 获取从上到下从左到右的方式的格子列表
        /// </summary>
        /// <returns></returns>
        List<AMDM_Grid> GetGridListByTop2BottomLeft2Right(bool useZMode = true)
        {
            List<AMDM_Grid> grids = new List<AMDM_Grid>();
            int currentFloorIndex = 0;
            foreach (var item in this.stock.Floors)
            {
                if (item.Key<0)
                {
                    continue;
                }
                AMDM_Floor currentFloor = item.Value;
                if (!useZMode && (currentFloorIndex % 2) != 0)//单数行也就是索引为 1 3 5 7 9 这样的行
                {
                    for (int j = currentFloor.Grids.Count - 1; j >= 0; j--)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
                else
                {
                    for (int j = 0; j < currentFloor.Grids.Count; j++)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
                currentFloorIndex++;
            }
            return grids;
        }
        /// <summary>
        /// 获取从上到下从右到左的方式的格子列表
        /// </summary>
        /// <returns></returns>
        List<AMDM_Grid> GetGridListByTop2BottomRight2Left(bool useZMode = true)
        {
            List<AMDM_Grid> grids = new List<AMDM_Grid>();
            int currentFloorIndex = 0;
            foreach (var item in this.stock.Floors)
            {
                if (item.Key<0)
                {
                    continue;
                }
                AMDM_Floor currentFloor = item.Value;
                if (!useZMode && (currentFloorIndex % 2 != 0))
                {
                    for (int j = 0; j < currentFloor.Grids.Count; j++)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
                else
                {
                    for (int j = currentFloor.Grids.Count - 1; j >= 0; j--)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
                currentFloorIndex++;
            }
            return grids;
        }
        /// <summary>
        /// 获取从下到上从左到右方式的格子列表
        /// </summary>
        /// <returns></returns>
        List<AMDM_Grid> GetGridListByBottom2TopLeft2Right(bool useZMode = true)
        {
            List<AMDM_Grid> grids = new List<AMDM_Grid>();
            int currentI = -1;
            foreach (var item in this.stock.Floors)
            {
                currentI++;
                if (item.Key<0)
                {
                    continue;
                }
                AMDM_Floor currentFloor = item.Value;
                if (!useZMode && (currentI % 2) != 0)//单数行也就是索引为 1 3 5 7 9 这样的行
                {
                    for (int j = currentFloor.Grids.Count - 1; j >= 0; j--)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
                else
                {
                    for (int j = 0; j < currentFloor.Grids.Count; j++)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
            }
            return grids;
        }
        /// <summary>
        /// 获取从下到上从右到左方式的格子的列表
        /// </summary>
        /// <returns></returns>
        List<AMDM_Grid> GetGridListByBottom2TopRight2Left(bool useZMode = true)
        {
            List<AMDM_Grid> grids = new List<AMDM_Grid>();
            foreach (var f in this.stock.Floors)
            {
                if (f.Key<0)
                {
                    continue;
                }
                AMDM_Floor currentFloor = f.Value;
                if (!useZMode && (f.Key % 2 != 0))
                {
                    for (int j = 0; j < currentFloor.Grids.Count; j++)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
                else
                {
                    for (int j = currentFloor.Grids.Count - 1; j >= 0; j--)
                    {
                        grids.Add(currentFloor.Grids[j]);
                    }
                }
            }
            return grids;
        }
        #endregion

        #region 最大化 最小化 关闭按钮的操作
        private void closeBtn_Click(object sender, EventArgs e)
        {
            if (this.keepMove2RandomGridBW != null && this.keepMove2RandomGridBW.IsBusy == true)
            {
                this.keepMove2RandomGridBW.CancelAsync();
                this.keepMove2RandomGridBW.Dispose();
                this.keepMove2RandomGridBW = null;
            }
            GC.Collect();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void minSizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void maxSizeBtn_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }
        #endregion

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void addFloorAtDownPart_Click(object sender, EventArgs e)
        {
            
                this.SuspendLayout();
                AMDM_Floor newFloor = this.amdmInfoManager.CreateAndJoinNewDownPartFloor(this.stock,
                    this.stock.MaxFloorWidthMM,
                    App.Setting.HardwareSetting.Floor.DownPartFloorDepthMM);
                //默认加入8个格子
                float perGridWidth = this.stock.MaxFloorWidthMM /8;
                for (int i = 0; i < 8; i++)
                {
                    this.amdmInfoManager.CraeteAndJoinNewGrid(newFloor, null, i * perGridWidth, perGridWidth);
                }

                this.clearStockShow();
                this.showStock(this.stock, this.showGridsByPercentCheckbox.Checked);
                this.ResumeLayout(false);
        }

        private void testMedicineGettingBySelectedSpicalGridBtn_Click(object sender, EventArgs e)
        {
            #region 先测试连接
            try
            {
                pc = new PLCCommunicator4StockTesting_台达(this.stock.IndexOfMachine, plcSetting, this.stock.CenterDistanceBetweenTwoGrabbers, this.stock.XOffsetFromStartPointMM, this.stock.YOffsetFromStartPointMM, App.medicinesGettingController.MainPLCCommunicator);
                pc.Connect();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("PLC连接完成...");
                Console.ResetColor();
            }
            catch (Exception connectErr)
            {
                string msg = string.Format("plc连接错误:{0}", connectErr.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
                pc.Disconnect();
                return;
            }
            #endregion

            Control currentSelectedGrid = this.gridEditMenuOwner as Control;
            if (currentSelectedGrid == null)
            {
                pc.Disconnect();
                return;
            }
            AMDM_Grid currentGrid = currentSelectedGrid.Tag == null ? null : currentSelectedGrid.Tag as AMDM_Grid;
            if (currentGrid == null)
            {
                pc.Disconnect();
                return;
            }
            if (currentGrid.FloorIndex>=0)
            {
                MessageBox.Show("请选择特殊药槽");
                pc.Disconnect();
                return;
            }
            //Console.WriteLine("准备发送取药信号");
            #region 发送取药信号
            //取药次数
            int times = 1;
            bool sendGettingCommandRet = false;
            try
            {
                sendGettingCommandRet = pc.SendStartMedicineGettingCommandTest(currentGrid.FloorIndex, currentGrid.IndexOfFloor, times);
            }
            catch (Exception err)
            {
                Console.WriteLine("发送取药信号错误:{0}", err.Message);
            }
            if (sendGettingCommandRet)
            {
                //Console.WriteLine("发送取药信号完成");
            }
            #endregion



            Console.WriteLine(string.Format("即将到特殊药槽:{0}-{1}取药", currentGrid.FloorIndex, currentGrid.IndexOfFloor + 1));

            float getReadyStatusTimeoutMS = 30000;
            float getStatusTimeoutMS = 2000;
            int finishedGettingTimes = 0;
            DateTime thisTimeStartTime = DateTime.Now;
            //从plc读的取单次取药的返回值
            PLCStatusData status = null;
            //开始取药的时间
            
            DateTime startTime = DateTime.Now;
            DateTime startGetMedicineTime = startTime;
            #region 等待plc返回200信号
            while ((DateTime.Now - startGetMedicineTime).TotalMilliseconds < getReadyStatusTimeoutMS && this.keepMove2RandomGridBW.CancellationPending == false)
            {
                //Console.WriteLine("已发送开始取药命令,正在等待获取取药完成状态");
                #region 获取返回状态
                DateTime startGetStatusTime = DateTime.Now;
                while ((DateTime.Now - startGetStatusTime).TotalMilliseconds < getStatusTimeoutMS)
                {
                    System.Threading.Thread.Sleep(17);
                    status = App.medicinesGettingController.MainPLCCommunicator.GetMedicineGettingStatus();
                    if (status != null)
                    {
                        //Console.WriteLine(string.Format("获取到返回状态:{0}", status.Main));
                        break;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("未获取到取药完成状态!!!!!!"));
                        continue;
                    }
                }
                //如果获取状态为空,超时
                if (status == null)
                {
                    Utils.LogError("获取取药完结状态超时");
                    pc.Disconnect();
                    return;
                }
                #endregion
                #region 如果单次取药完成,跳出本次取药状态获取
                if (status.Main == MainBufferValuesEnum.FinishedDeliveryOneMedicine)
                {
                    string msg = string.Format("完成本次取药,重置状态,已取药品数量:{0}\r\n本次用时:{1},总用时:{2}秒", finishedGettingTimes, (DateTime.Now - thisTimeStartTime).TotalSeconds, (DateTime.Now - startTime).TotalSeconds);
                    Utils.LogSuccess(msg);
                    //如果状态为取药完成 继续下一次取药,在取药之前,先清空plc的main数据为0
                    //pc.SendClearMainStatusWaitNextMedicineGettingCommandTest();
                    break;
                }
                #endregion
                #region 如果返回状态为300
                else if (status.Main == MainBufferValuesEnum.Error)
                {
                    string errMsg = string.Format("获取到的取药状态为:发生错误,可能没有药了.状态码:{0}", status.Main);
                    Utils.LogError(errMsg);
                    pc.Disconnect();
                    return;
                }
                #endregion
                #region 如果计数器检测光栅被遮挡
                else if (status.CounterCoverdError)
                {
                    string errmsg = "计数器光栅被长时间遮挡!!!!";
                    Utils.LogError(errmsg);
                    pc.Disconnect();
                    return;
                }
                #endregion
                #region 其他状态
                else
                {
                    //Console.WriteLine(string.Format("获取到取药完结状态不是取药完成,当前状态:{0}  继续获取取药状态,当前已取药次数:{1}", status.Main, finishedGettingTimes));
                    continue;
                }
                #endregion
            }
            #endregion
            #region 如果是超时跳出,status没有值或者是取药状态不是200的话,那就是取药超时
            if (status == null || status.Main != MainBufferValuesEnum.FinishedDeliveryOneMedicine)
            {
                Utils.LogError(string.Format("取药超时,status是否为空?:{0} status:{1}", status == null, status == null ? "空" : status.Main.ToString()));
                pc.Disconnect();
                return;
            }
            #endregion
            pc.Disconnect();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        #region 更新并保存药槽编号2022年2月18日10:10:01
        private void updateAndSaveGridIndexOfStockBtn_Click(object sender, EventArgs e)
        {
            int currentGridIndexOfStock = 0;
            int currentDownPartFloorIndex = 0;
            //当前的格子的索引序号
            Dictionary<int, AMDM_Floor> floors = Utils.Z2ADictionary(stock.Floors);
            List<AMDM_Floor> upPartFloors = new List<AMDM_Floor>();
            List<AMDM_Floor> downPartFloors = new List<AMDM_Floor>();
            #region 分出上层和下层的层板信息
            foreach (var item in floors)
            {
                if (item.Key >= 0)
                {
                    upPartFloors.Add(item.Value);
                }
                else
                {
                    downPartFloors.Add(item.Value);
                }
            }
            #endregion
            //int currentGridIndexOfStock = 1;
            foreach (var item in upPartFloors)
            {
                AMDM_Floor currentFloor = item;
                for (int j = 0; j < currentFloor.Grids.Count; j++)
                {
                    AMDM_Grid currentGrid = currentFloor.Grids[j];
                    currentGrid.IndexOfStock = currentGridIndexOfStock++;
                    string index = string.Format("{0}->{1}->{2}", stock.IndexOfMachine, currentFloor.IndexOfStock, currentGrid.IndexOfFloor);
                }
            }
            #endregion
            #region 下部分的层板的可显示格式
            //下部分空间中的每一个层板的显示高度是多少,包含了padding
            //float downPartYScallRate = this.gridsInDownPartShowerPanel.ClientRectangle.Height * 1f / 40;
            foreach (var item in downPartFloors)
            {
                currentGridIndexOfStock = (0 - item.IndexOfStock + 1) * 100;
                AMDM_Floor currentFloor = item;
                for (int j = 0; j < currentFloor.Grids.Count; j++)
                {
                    AMDM_Grid currentGrid = currentFloor.Grids[j];
                    currentGrid.IndexOfStock = currentGridIndexOfStock++;
                    string index = string.Format("{0}->{1}->{2}", stock.IndexOfMachine, currentFloor.IndexOfStock, currentGrid.IndexOfFloor);
                }
                currentDownPartFloorIndex++;
            }
            #endregion

            foreach (var floor in this.stock.Floors)
            {
                foreach (var grid in floor.Value.Grids)
                {
                    var g = grid;
                    bool updRet = this.amdmInfoManager.UpdateGridIndexOfStock(ref g, g.IndexOfStock, null);
                    if (updRet == false)
                    {
                        Utils.LogError("在药仓管理页面中更新药仓的索引编号发生错误;", g);
                    }
                }
            }
        }

        private void 打开所有电磁锁按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator; 
            pc.Connect();
            pc.SendLockerControlCommand(true);
            pc.Disconnect();
        }

        private void 关闭所有电磁锁按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator; 
            pc.Connect();
            pc.SendLockerControlCommand(false);
            pc.Disconnect();
        }

        private void 打开所有空调按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator;
            pc.Connect();
            pc.SendACControlCommand(true);
            pc.Disconnect();
        }

        private void 关闭所有空调按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator; 
            pc.Connect();
            pc.SendACControlCommand(false);
            pc.Disconnect();
        }

        private void 打开所有紫外线按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator;
            pc.Connect();
            pc.SendUVLampControlCommand(true);
            pc.Disconnect();
        }

        private void 关闭所有紫外线按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator; 
            pc.Connect();
            pc.SendUVLampControlCommand(false);
            pc.Disconnect();
        }

        private void 读取药仓1温度_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator; 
            pc.Connect();
            var ret = pc.SendGetACTemperature(0);
            pc.Disconnect();
            if (ret == null)
            {
                MessageBox.Show("读取温度失败");
            }
            else
            {
                MessageBox.Show(ret.ToString());
            }
        }

        private void 设定药仓1空调温度为25按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator; 
            pc.Connect();
            var ret = pc.SendSetACTemperature(0,25f);
            pc.Disconnect();
        }

        private void 发送完成取药休息信号按钮_Click(object sender, EventArgs e)
        {
            var pc = App.medicinesGettingController.MainPLCCommunicator;
            pc.Connect();
            var ret = pc.SendAllFinishedHaveARestCommand();
            pc.Disconnect();
        }

        private void 机械手强制复位按钮_Click(object sender, EventArgs e)
        {
            PLCCommunicator4StockTesting_台达 pc = new PLCCommunicator4StockTesting_台达(this.stock.IndexOfMachine, plcSetting, this.stock.CenterDistanceBetweenTwoGrabbers, this.stock.XOffsetFromStartPointMM, this.stock.YOffsetFromStartPointMM, App.medicinesGettingController.MainPLCCommunicator);
            pc.Connect();
            var ret = pc.SendForceResetGrubbersCommand();
            Utils.LogSuccess("已发送机械手强制复位信号", ret);
            DateTime start = DateTime.Now;
            Nullable<DateTime> finishedTime = null;
            int perTimeCheckDelay = 1000;
            double timeOutMS = 120000;
            bool resetSuccess = false;
            while ((DateTime.Now-start).TotalMilliseconds< timeOutMS)
            {
                System.Threading.Thread.Sleep(perTimeCheckDelay);
                var status = App.medicinesGettingController.MainPLCCommunicator.GetMedicineGettingStatus();
                if (status != null && status.Resetting)
                {
                    Utils.LogInfo("正在归位");
                }
                else if(status != null && !status.Resetting)
                {
                    Utils.LogSuccess("归位完成");
                    finishedTime = DateTime.Now;
                    resetSuccess = true;
                    break;
                }
            }
            pc.Disconnect();
            MessageBox.Show(string.Format("机械手强制归位结果:{0}", resetSuccess? "成功":"超时"));
            if (finishedTime!= null)
            {
                string msg = (finishedTime.Value - start).TotalMilliseconds.ToString();
                MessageBox.Show(msg);
            }
        }

        private void StockEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            App.MonitorsManager.HardwareMonitor.Pause = false;
        }
    }
}
