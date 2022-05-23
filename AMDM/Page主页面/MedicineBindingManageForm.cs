using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AMDM_Domain;
using AMDM;
using MyCode;
//using FakeHISClient.Component;
/*
 * 药槽和药品的绑定信息管理页面 2021年11月4日14:38:31 
 * 已经布药的是绿色,没有布药的是灰色
 * 
 * 显示的内容有 药槽编号 红色数字, 绑定的药品名称
 * 
 * //如果药槽中已经绑定的药品如果有数量的话 是不能清空绑定的 需要清除槽子内的药品以后才可以操作.
                //    提示清除槽子内的药品以后,多确认一下是否已经在机器内没有药品了.如果没有了.可以继续绑定了.
                //        另外 布药时要求输入盒子的长宽高信息,展示一个页面提供调节,如果盒子已经有了长宽高信息,弹出提示框确认是否为该形状
                //        这里需要比较多的算法,计算出来一个盒子的样子然后展示出来.
 * 
 * 
 * 2021年11月6日20:54:59
 * 写完了对盒子信息输入的部分以后,写一下 专门用来多线程显示控件的控制器.在程序开始以后,就初始化一个含有backgroundworker的类
 * 如果在他的内部标记了一个或者多个需要动画渲染的控件,就需要开启线程,然后在线程内计算 0-100的值 提供给控件使用 然后根据值来确认显示的模式.
 * 
 * 同时还会在计算0-100的时候,把sin值的曲线算出来 防止每个控件都借用0-100来算曲线 浪费性能.
 * 
 * 这个类供外界获取数值 其他不需要什么功能,没有控件需要动态渲染的时候 就把线程关闭. 提供一个 AddAnimationControl 方法 然后把内容添加到自身保存的
 * List里面,然后再检查线程是否被启动.如果没启动,就启动.
 * 
 * 2021年11月7日13:46:46  已经完成了药盒尺寸的添加,覆盖,并且在修改时如果已经有药槽绑定了该药品  提示清空后才可以修改药槽.不然容易不适配.
 * 
 * 接下来写一个动画组件 就是昨晚提到的要动态展示的.命名为  ControlAnimationRenderingController
 */

namespace AMDM
{
    public partial class MedicineBindingManageForm : Form
    {
        #region 全局变量
        FormAutoSizer autoSizer;
        Dictionary<AMDM_Grid, AMDM_Medicine> gridsBindedMedicineDic = new Dictionary<AMDM_Grid, AMDM_Medicine>();
        AMDM_Stock stock = null;
        BackgroundWorker getMedicinesBW = new BackgroundWorker();
        //List<GridShowerInfo> showers = new List<GridShowerInfo>();
        /// <summary>
        /// 当前已经扫描读取到的药品
        /// </summary>
        AMDM_Medicine currentScanedMedicine = null;
        long localCurrentMedicineKindCount = 0;
        #endregion

        #region 构造函数于初始化
        public MedicineBindingManageForm()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            autoSizer = new FormAutoSizer(this);
            autoSizer.TurnOnAutoSize();

            //this.gridsShowerPanel.Dock = DockStyle.Fill;
            //this.gridsShowerPanel.Height += 100;
            //this.Height += 100;
            #region 默认显示的信息的清空
            this.currentMedicineBarcodeLabel.Text = "";
            this.currentMedicineNameLabel.Text = "";
            this.firstAsyncTimeLabel.Text = App.Setting.FirstMedicineInfoAsyncTime.ToString();
            this.LastAsyncTimeLabel.Text = App.Setting.LastMedicineInfoAysncTime.ToString();
            this.medicineDataUpdateInfoLabel.Text = "";
            this.medicineKindCountLabel.Text = "";
            //this.statusLabel.Text = "";
            #endregion
        }
        public bool Init(AMDM_Stock stock)
        {
            this.stock = stock;
            localCurrentMedicineKindCount = App.medicineManager.GetMedicinesKindCount();
            this.medicineKindCountLabel.Text = this.localCurrentMedicineKindCount.ToString();
            #region 获取所有药品的绑定信息
            List<AMDM_Clip_data> bindings = App.bindingManager.GetStockAllBindedMedicine(stock);
            //Dictionary<long, GridMedicineBindingInfo_data> medicinesIdAndBindingInfoDic = new Dictionary<long,GridMedicineBindingInfo_data>();
            List<long> needGetMedicinesIdList = new List<long>();
            Dictionary<string, AMDM_Clip_data> bindingInfoLocationDic = new Dictionary<string, AMDM_Clip_data>();
            for (int i = 0; i < bindings.Count; i++)
            {
                AMDM_Clip_data current = bindings[i];
                string index = string.Format("{0}.{1}.{2}", current.StockIndex, current.FloorIndex, current.GridIndex );
                bindingInfoLocationDic.Add(index, current);
                if (needGetMedicinesIdList.Contains(current.MedicineId) == false)
                {
                    needGetMedicinesIdList.Add(current.MedicineId);
                }
            }
            #region 获取这些格子需要绑定的药品的实体对象
            List<AMDM_Medicine> medicines = App.medicineManager.GetMedicines(needGetMedicinesIdList);
            Dictionary<long, AMDM_Medicine> medicinesDic = new Dictionary<long, AMDM_Medicine>();
            for (int i = 0; i < medicines.Count; i++)
            {
                medicinesDic.Add(medicines[i].Id, medicines[i]);
            }
            #endregion

            #endregion
            #region 转换成可显示的格式
            float upPartXScallRate = this.upPartGridsShowerPanel.ClientRectangle.Width *1f/ stock.MaxFloorWidthMM;
            float upPartYScallRate = this.upPartGridsShowerPanel.ClientRectangle.Height *1f/ stock.MaxFloorsHeightMM;

            Dictionary<int, AMDM_Floor> floors = Utils.Z2ADictionary(stock.Floors);
            #region 上下不分的分开显示
            List<AMDM_Floor> upPartFloors = new List<AMDM_Floor>();
            List<AMDM_Floor> downPartFloors = new List<AMDM_Floor>();
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
            #region 显示上半部分的内容
            foreach (var item in upPartFloors)
            {
                AMDM_Floor currentFloor = item;
                addGrids2Panel(this.upPartGridsShowerPanel, currentFloor,
                    //ref currentGridIndexOfStock, 
                    bindingInfoLocationDic, medicinesDic, upPartXScallRate, upPartYScallRate);
            }
            #endregion

            #region 显示下半部分的内容

            float downPartXScallRate = this.downPartGridsShowerPanel.ClientRectangle.Width * 1f / stock.MaxFloorWidthMM;
            float downPartYScallRate = this.downPartGridsShowerPanel.ClientRectangle.Height * 1f / 400;
            float perDownpartFloorPadding = 5;
            float perDownPartFloorHeight = this.downPartGridsShowerPanel.ClientRectangle.Height * 1f / downPartFloors.Count - perDownpartFloorPadding/2;
            int currentDownPartFloorIndex = 0;
            foreach (var item in downPartFloors)
            {
                //currentGridIndexOfStock = ((0 - item.IndexOfStock) + 1) * 100 + 1;
                AMDM_Floor currentFloor = item;
                addGrids2DownPartPanel(this.downPartGridsShowerPanel,
                    currentFloor, 
                    //ref currentGridIndexOfStock,
                    bindingInfoLocationDic,
                    medicinesDic, 
                    downPartXScallRate, 
                    downPartYScallRate, 
                    perDownPartFloorHeight,
                    perDownpartFloorPadding,
                    currentDownPartFloorIndex++);
            }
            #endregion

            #endregion

            this.getMedicinesBW.WorkerSupportsCancellation = true;
            this.getMedicinesBW.WorkerReportsProgress = true;
            this.getMedicinesBW.DoWork += getMedicinesBW_DoWork;
            this.getMedicinesBW.ProgressChanged += getMedicinesBW_ProgressChanged;

            #region 初始化串口
            App.ICCardReaderAndCodeScanner2in1ReceivedData = this.processScanedMessage;
            #endregion
            return true;
        }

        void processScanedMessage(string msg)
        {
            App.ControlAnimationRenderingController.ClearAnimationControls();
            AMDM_Medicine medicine = App.medicineManager.GetMedicineByBarcode(msg);
            if (medicine != null)
            {
                this.currentScanedMedicine = medicine;
                this.currentMedicineNameLabel.Text = this.currentScanedMedicine.Name;
                this.currentMedicineBarcodeLabel.Text = this.currentScanedMedicine.Barcode;
                if (CheckCanContinueReBind(medicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.Speak("未找到药品数据");
                this.currentScanedMedicine = null;
                this.currentMedicineNameLabel.Text = "";
                this.currentMedicineBarcodeLabel.Text = "";
            }
        }

        void processMonitorDetectedError(object monitor, MonitorDetectedErrorTypeEnum err)
        {
            //获取到了故障 那就停用机器啥的.
        }

        void addGrids2DownPartPanel(Control destPanel, AMDM_Floor currentFloor,
            //ref int currentGridIndexOfStock,
            Dictionary<string, AMDM_Clip_data> bindingInfoLocationDic,
            Dictionary<long, AMDM_Medicine> medicinesDic,float xScallRate, float yScallRate,float perDownPartFloorHeight,
            float perDownPartFloorPadding, 
            int currentFloorI )
        {
            for (int j = 0; j < currentFloor.Grids.Count; j++)
            {
                //当前格子
                AMDM_Grid currentGrid = currentFloor.Grids[j];
                //当前格子的大序号
                //currentGrid.IndexOfStock = currentGridIndexOfStock++;
                //用于检测格子是否已经添加的索引
                string index = string.Format("{0}.{1}.{2}", stock.IndexOfMachine, currentFloor.IndexOfStock, currentGrid.IndexOfFloor);
                //从格子的绑定信息表中获取绑定信息
                AMDM_Clip_data bindingInfo = bindingInfoLocationDic.ContainsKey(index) ? bindingInfoLocationDic[index] : null;
                //绑定的药品
                AMDM_Medicine medicine = null;
                //如果绑定信息不为空并且药品信息表中包含这个绑定的药品的id,设置绑定的药品为目标药品
                if (bindingInfo != null && medicinesDic.ContainsKey(bindingInfo.MedicineId))
                {
                    medicine = medicinesDic[bindingInfo.MedicineId];
                }
                //格子中绑定的药品的信息表添加数据
                this.gridsBindedMedicineDic.Add(currentGrid, medicine);
                //格子显示器
                GridShower shower = new GridShower();
                //格子显示器的外观
                shower.BorderStyle = BorderStyle.FixedSingle;
                #region 计算格子可以添加多少个药品
                int maxCanLoadCount = 0;
                if (medicine != null)
                {
                    maxCanLoadCount = (int)Math.Floor(currentFloor.DepthMM / medicine.BoxLongMM);
                }
                #endregion
                //初始化格子显示器,作为药槽和药品绑定模式
                shower.Init(currentGrid,
                    medicine, maxCanLoadCount);
                //设置格子在panel中的位置
                shower.Location = new Point((int)Math.Round(currentGrid.LeftMM * xScallRate + App.Setting.HardwareSetting.Grid.GridWallWidthMM / 2),
                    (int)Math.Round(currentFloorI * 1f * perDownPartFloorHeight + perDownPartFloorPadding));
                //设置格子的显示宽度
                shower.Size = new System.Drawing.Size((int)Math.Round((currentGrid.RightMM - currentGrid.LeftMM) * xScallRate - App.Setting.HardwareSetting.Grid.GridWallWidthMM),
                    (int)Math.Floor(perDownPartFloorHeight - perDownPartFloorPadding)
                    );

                #region 添加对格子点击事件的绑定
                shower.Click += shower_Click;
                #endregion
                //在上半部分药槽显示区中添加这个可以接收绑定动作的药槽显示器
                destPanel.Controls.Add(shower);
            }
        }

        void addGrids2Panel(Control destPanel, AMDM_Floor currentFloor,
            //ref int currentGridIndexOfStock,
            Dictionary<string, AMDM_Clip_data> bindingInfoLocationDic,
            Dictionary<long, AMDM_Medicine> medicinesDic, float xScallRate, float yScallRate
            )
        {
            for (int j = 0; j < currentFloor.Grids.Count; j++)
            {
                //当前格子
                AMDM_Grid currentGrid = currentFloor.Grids[j];
                //当前格子的大序号
                //currentGrid.IndexOfStock = currentGridIndexOfStock++;
                //用于检测格子是否已经添加的索引
                string index = string.Format("{0}.{1}.{2}", stock.IndexOfMachine, currentFloor.IndexOfStock, currentGrid.IndexOfFloor);
                //从格子的绑定信息表中获取绑定信息
                AMDM_Clip_data bindingInfo = bindingInfoLocationDic.ContainsKey(index) ? bindingInfoLocationDic[index] : null;
                //绑定的药品
                AMDM_Medicine medicine = null;
                //如果绑定信息不为空并且药品信息表中包含这个绑定的药品的id,设置绑定的药品为目标药品
                if (bindingInfo != null && medicinesDic.ContainsKey(bindingInfo.MedicineId))
                {
                    medicine = medicinesDic[bindingInfo.MedicineId];
                }
                //格子中绑定的药品的信息表添加数据
                this.gridsBindedMedicineDic.Add(currentGrid, medicine);
                //格子显示器
                GridShower shower = new GridShower();
                //格子显示器的外观
                shower.BorderStyle = BorderStyle.FixedSingle;
                #region 计算格子可以添加多少个药品
                int maxCanLoadCount = 0;
                if (medicine != null)
                {
                    maxCanLoadCount = (int)Math.Floor(currentFloor.DepthMM / medicine.BoxLongMM);
                }
                #endregion
                //初始化格子显示器,作为药槽和药品绑定模式
                shower.Init(currentGrid,
                    medicine, maxCanLoadCount);
                //设置格子在panel中的位置
                shower.Location = new Point((int)Math.Round(xScallRate * (currentGrid.LeftMM  + App.Setting.HardwareSetting.Grid.GridWallWidthMM / 4)),
                    (int)Math.Round((stock.MaxFloorsHeightMM - currentFloor.TopMM) * yScallRate));
                //设置格子的显示宽度
                shower.Size = new System.Drawing.Size((int)Math.Round((currentGrid.RightMM - currentGrid.LeftMM) * xScallRate - App.Setting.HardwareSetting.Grid.GridWallWidthMM),
                    (int)Math.Round((currentFloor.TopMM - currentFloor.BottomMM) * yScallRate - App.Setting.HardwareSetting.Floor.FloorPanelHeightMM));

                #region 添加对格子点击事件的绑定
                shower.Click += shower_Click;
                #endregion
                //在上半部分药槽显示区中添加这个可以接收绑定动作的药槽显示器
                this.upPartGridsShowerPanel.Controls.Add(shower);
            }
        }
        
        #endregion

        #region 播放语音
        void Speak(string msg)
        {
            if (App.TTSSpeaker == null)
            {
                Utils.LogWarnning("没有初始化语音朗读引擎");
                return;
            }
            App.TTSSpeaker.Speak(msg);
        }
        #endregion

        #region 当格子被点击以后的事件
        void shower_Click(object sender, EventArgs e)
        {
            GridShower shower = sender as GridShower;
            AMDM_Medicine bindedMedicine = shower.BindedMedicine;
            #region 如果是鼠标右键
            MouseEventArgs me = e as MouseEventArgs;
            if (me != null && me.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (processMouseRightClick(shower) == false)
                {
                    return;
                }
            }
            #endregion
            #region 其他,鼠标左键
            #region 如果并没有选择药品,防止误操作做一个提示

            else if (this.currentScanedMedicine == null)
            {
                if (shower.BindedMedicine != null)
                {
                    MessageBox.Show(this,"为防止您的误操作,如需对药槽内的药品进行解绑或清空药槽,请长按屏幕或点击鼠标邮件以打开操作页面", "防误操作",  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(this,"请先扫描药品后再进行目标药槽选择", "请先扫码", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            #endregion
            #region 如果已经绑定了药品需要更换,做提示
            else if (bindedMedicine != null)
            {
                MessageBox.Show(this, string.Format("当前药槽已绑定药品为:\r\n\r\n{0}\r\n\r\n如需更换药品,右键点击该药槽或长按此药槽以进行解绑", bindedMedicine.Name), "已绑定药品", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            
            //#region 检查药盒的尺寸是否合适,如果不合适不能绑定到这个药槽.
            ////药槽可以装载的最大和最小宽度的药品
            //float minWidth = (shower.Grid.RightMM - shower.Grid.LeftMM) - App.Setting.DefaultHardwareSetting.GridWallWidthMM - App.Setting.DefaultHardwareSetting.MaxGridPaddingWidthMM;
            //float maxWidth = (shower.Grid.RightMM - shower.Grid.LeftMM) - App.Setting.DefaultHardwareSetting.GridWallWidthMM - App.Setting.DefaultHardwareSetting.MinGridPaddingWidthMM;

            //if (this.currentScanedMedicine.BoxWidthMM)
            //{
                
            //}
            //#endregion
            StringBuilder areYouSueBindMsg = new StringBuilder("确定将药品绑定到该药槽吗?");
            areYouSueBindMsg.AppendFormat("\r\n\r\n目标药槽号为:{0}", (shower.Grid.IndexOfStock+1).ToString().PadLeft(3, '0'));
            if (MessageBox.Show(this, areYouSueBindMsg.ToString(), "确认药槽信息:", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                //bool useOldBoxSizeInfo = false;
                //float boxWidthMM, boxHeightMM, boxLongMM;
                #region 药品已经存在药盒信息的话 直接显示 问是不是使用这个尺寸
                if (this.currentScanedMedicine.BoxHeightMM != 0
                    || this.currentScanedMedicine.BoxLongMM != 0
                    || this.currentScanedMedicine.BoxWidthMM != 0
                    )
                {
                    string hasBoxSizeMsg = string.Format(
                        "该药品已包含尺寸信息,请确认:\r\n\r\n长度(进深)mm:{0}\r\n宽度(左右)mm:{1}\r\n高度(厚度)mm:{2}\r\n\r\n是否使用此尺寸信息?",
                        this.currentScanedMedicine.BoxLongMM,
                        this.currentScanedMedicine.BoxWidthMM,
                        this.currentScanedMedicine.BoxHeightMM
                        );
                    //出个提示框  问是不是选这个尺寸
                    if (MessageBox.Show(hasBoxSizeMsg, "已获取到存在的药盒尺寸信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    {
                        //如果不用这个尺寸,输入新的尺寸,然后询问是不是覆盖
                        #region 输入药盒的信息 更新药盒的尺寸.

                        BoxSizeInfoInputResult inputBoxSizeRet = InputAndSaveMedicineBoxInfo(shower);
                        if (inputBoxSizeRet.Success == false)
                        {
                            return;
                        }
                        else
                        {
                            #region 获取该药品是否已经绑定到了药槽
                            //已经绑定了当前扫描药品的绑定信息集合
                            List<AMDM_Clip_data> bindedCurrentScanedMedicineInfos = App.bindingManager.GetBindedGridList(this.stock.IndexOfMachine, currentScanedMedicine.Id);

                            #endregion
                            #region 如果该药品已经在其它药槽内绑定,不能修改他的尺寸,需要将目标药槽与当前扫描的药品进行解绑以后再重新调整尺寸
                            if (bindedCurrentScanedMedicineInfos.Count > 0)
                            {
                                string resetSizeMustUnbindBindedGridMsg =
                                    string.Format("检测到{0}个药槽中已经绑定该药品\r\n更改药盒尺寸需要重新确认原已绑定该药品的药槽尺寸是否合适\r\n如需更改药盒尺寸,请将已绑定该药品的药槽清空并解绑", bindedCurrentScanedMedicineInfos.Count);
                                MessageBox.Show(resetSizeMustUnbindBindedGridMsg, "将绑定了该药品的药槽清空并解绑后继续", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            #endregion
                            #region 没有绑定到药槽的药品才可以修改尺寸
                            else
                            {
                                string resetSizeMsg = string.Format("请确认是否覆盖原药盒尺寸信息?");
                                if (MessageBox.Show(resetSizeMsg, "覆盖尺寸信息吗?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    if (App.medicineManager.ResetMedicineBoxSize(ref currentScanedMedicine, inputBoxSizeRet.DepthMM, inputBoxSizeRet.WidthMM, inputBoxSizeRet.HeightMM))
                                    {
                                        MessageBox.Show("药盒尺寸信息已更新");
                                    }
                                    else
                                    {
                                        MessageBox.Show("药盒尺寸更新失败,请与管理员联系");
                                        return;
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion
                #region 不存在药盒尺寸信息的时候 直接出来提示框要求用户输入药盒的尺寸
                else
                {
                    var inputSizeRet = InputAndSaveMedicineBoxInfo(shower);
                    if (inputSizeRet.Success == false)
                    {
                        return;
                    }
                }
                #endregion

                if (App.bindingManager.BindMedicine2Grid(this.currentScanedMedicine, shower.Grid))
                {
                    shower.BindedMedicine = this.currentScanedMedicine;
                    shower.Refresh();
                    this.currentScanedMedicine = null;
                    this.currentMedicineNameLabel.Text = "";
                    this.currentMedicineBarcodeLabel.Text = "";
                    MessageBox.Show("已完成该药品与药槽的绑定,请继续扫描药品条码进行下一药品的药槽绑定", "绑定完成!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            App.ControlAnimationRenderingController.ClearAnimationControls();
            #endregion
        }
        bool processMouseRightClick(GridShower shower)
        {
            if (shower.BindedMedicine == null)
            {
                ///点击右键但是点击的药槽内没有绑定药品,直接返回不需要处理
                return false;
            }
            //当鼠标右键按下的时候是要解除药品的绑定
            #region 对于药品从槽子里清空绑定的情况
            BindingActionsForm bform = new BindingActionsForm(shower, this.stock.Floors[shower.Grid.FloorIndex], false,this.Speak);
            if (bform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
            else
            {
                return false;
            }
            #endregion
            return true;
        }
        #endregion

        #region 输入药品尺寸信息
        class BoxSizeInfoInputResult
        {
            public float DepthMM { get; set; }
            public float WidthMM { get; set; }
            public float HeightMM { get; set; }
            public bool Success { get; set; }
        }
        /// <summary>
        /// 输入药盒的尺寸信息 返回结果是 输入的长宽深信息
        /// </summary>
        /// <param name="shower"></param>
        /// <returns></returns>
        BoxSizeInfoInputResult InputAndSaveMedicineBoxInfo(GridShower shower)
        {
            BoxSizeInfoInputResult ret = new BoxSizeInfoInputResult();
            #region 如果是特殊药槽 只需要长度信息
            Form sizeSettingForm = null;
            float boxdepth = 0;
            float boxwidth = 0;
            float boxheight = 0;
            DialogResult mbformRet = System.Windows.Forms.DialogResult.Cancel;
            if (shower.Grid.FloorIndex < 0)
            {
                MedicineSpicalSizeInfoInputForm mbform = new MedicineSpicalSizeInfoInputForm();
                sizeSettingForm = mbform;
                mbform.Init(shower.Grid, this.stock.Floors[shower.Grid.FloorIndex]);
                mbformRet = mbform.ShowDialog();
                boxdepth = mbform.DepthMM;
            }
            #endregion
            #region 如果是一般药槽 需要长宽高信息
            else
            {
                MedicineBoxInfoInputForm mbform = new MedicineBoxInfoInputForm();
                sizeSettingForm = mbform;
                mbform.Init(shower.Grid, this.stock.Floors[shower.Grid.FloorIndex]);
                mbformRet = mbform.ShowDialog();
                boxdepth = mbform.DepthMM;
                boxwidth = mbform.WidthMM;
                boxheight = mbform.HeightMM;
            }
            #endregion

            #region 展示和确认信息
            if (mbformRet == System.Windows.Forms.DialogResult.Cancel)
            {
                MessageBox.Show("必须输入药盒尺寸信息才可对该药品进行布药");
                ret.Success = false;
                return ret;
            }
            else
            {

                if (App.medicineManager.ResetMedicineBoxSize(ref currentScanedMedicine, boxdepth, boxwidth, boxheight))
                {
                    MessageBox.Show("已记录药盒尺寸信息");
                }
                else
                {
                    MessageBox.Show("药盒尺寸信息保存失败,请与管理员联系");
                    ret.Success = false;
                    return ret;
                }
            }
            #endregion
            ret.DepthMM = boxdepth;
            ret.HeightMM = boxheight;
            ret.WidthMM = boxwidth;
            ret.Success = true;
            return ret;
        }
        #endregion

        #region 多线程进行药品同步的处理
        void getMedicinesBW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProcessChangeParam param = e.UserState as ProcessChangeParam;
            //throw new NotImplementedException();
            medicineDataUpdateInfoLabel.Text = param.Message;
            this.dataGetAndWriteProcessbar.Value = e.ProgressPercentage;

            if (param.Step == 1)
            {
                if (param.FinishedThisStep)
                {
                    medicineDataUpdateInfoLabel.ForeColor = Color.SeaGreen;
                }
                else
                {
                    this.dataGetAndWriteProcessbar.Visible = true;
                }
            }
            if (param.Step == 2)
            {
                //更新显示的药品种类数量
                this.localCurrentMedicineKindCount = param.i + 1;
                this.medicineKindCountLabel.Text = this.localCurrentMedicineKindCount.ToString();
                //如果完成了关闭进度条 显示最后更新时间 
                if (param.FinishedThisStep)
                {
                    medicineDataUpdateInfoLabel.ForeColor = Color.ForestGreen;
                    this.dataGetAndWriteProcessbar.Visible = false;
                    string currentTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    this.LastAsyncTimeLabel.Text = currentTimeStr;
                    this.firstAsyncTimeLabel.Text = currentTimeStr;
                }
                else
                {
                    this.dataGetAndWriteProcessbar.Visible = true;
                }
            }
        }
        /// <summary>
        /// 当多线程的下载线程发生进度改变的时候传递此参数
        /// </summary>
        class ProcessChangeParam
        {
            public string Message { get; set; }
            /// <summary>
            /// 步骤,第一步骤是从HIS获取,第二个步骤是存入到本机数据库,取值为 1 2
            /// </summary>
            public int Step { get; set; }
            /// <summary>
            /// 本步骤完成
            /// </summary>
            public bool FinishedThisStep { get; set; }

            public int i { get; set; }

            public int total { get; set; }

            //public bool Canceled { get; set; }
        }
        #region 多线程中如果有信息更新 在本窗口中进行信息更新,以及使用数据处理器进行数据的保存
        void onGetedMedicinesFromHISServer(List<object> medicinesInfo, int page, int totalPage)
        {
            float percentF = (page * 1f + 1) / 1f / totalPage * 100;
            int percent = (int)Math.Round(percentF);
            ProcessChangeParam param = new ProcessChangeParam();
            param.i = page;
            param.total = totalPage;
            param.Step = 1;
            if ((page + 1) >= totalPage)
            {
                param.Message = "从HIS服务器获取全部药品完成";
                param.FinishedThisStep = true;
            }
            else
            {
                if (percent == 100)
                {
                    percent = 99;
                }
                param.Message = string.Format("从HIS服务器获取药品进度: {0}/{1}", page + 1, totalPage * 1f);
                param.FinishedThisStep = true;
            }


            this.getMedicinesBW.ReportProgress(percent, param);
        }
        void onAddedMedicine(AMDM_Medicine medicine, int i, int total)
        {
            //药品已经加入到了数据库中,显示在页面上吧
            float percentf = (i * 1f + 1) / total * 1f * 100;
            int percent = (int)Math.Round(percentf);
            ProcessChangeParam param = new ProcessChangeParam();
            param.i = i;
            param.total = total;
            param.Step = 2;
            if ((i + 1) >= total)
            {
                param.Message = "药品已全部保存到本机数据库";
                param.FinishedThisStep = true;

                DateTime now = DateTime.Now;
                App.Setting.LastMedicineInfoAysncTime = now;
                if (this.initMedicineInfoBtn.Enabled == false)
                {
                    App.Setting.FirstMedicineInfoAsyncTime = now;
                }
                else if (App.Setting.FirstMedicineInfoAsyncTime == null)
                {
                    App.Setting.FirstMedicineInfoAsyncTime = now;
                }
                App.Setting.Save();
            }
            else
            {
                if (percent == 100)
                {
                    percent = 99;
                }
                param.Message = string.Format("药品:[{0}] 已保存到数据库 当前进度:{1}/{2}", medicine.Name, i + 1, total);
                param.FinishedThisStep = false;

            }
            this.getMedicinesBW.ReportProgress(percent, param);
        }
        #endregion
        
        void getMedicinesBW_DoWork(object sender, DoWorkEventArgs e)
        {
            IHISServerConnector connector = e.Argument as IHISServerConnector;
            //初始化成功,开始获取数据
            //获取到了所有药品以后,清空本机的所有药品数据,然后重新添加进入新的药品信息.
            App.medicineManager.ReInitMedicinesInfo(connector,
                onGetedMedicinesFromHISServer,
                onAddedMedicine
                );
            this.initMedicineInfoBtn.Enabled = true;
        }
        #endregion
       
        #region 更新药品数据按钮
        private void updateMedicineInfoBtn_Click(object sender, EventArgs e)
        {
            if (this.localCurrentMedicineKindCount == 0)
            {
                MessageBox.Show("本机尚无药品数据,将执行初始化程序以从HIS服务器获取药品数据", "无初始药品数据",  MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                this.initMedicineInfoBtn_Click(sender, e);
            }
            else
            {
                bool supportModified = App.HISServerConnector.GetIsSupportModifiedMedicines();
                if (supportModified == false)
                {
                    MessageBox.Show("当前HIS系统不支持增量获取已更新的药品信息.", "不支持的操作",  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (App.Setting.LastMedicineInfoAysncTime == null)
                    {
                        MessageBox.Show("加载上次同步时间失败,通常可能因为更改了配置文件导致.\r\n请重新初始化药品数据或修改配置文件内的最后更新时间后重试", "加载时间信息失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #region 增量获取已经修改的药品信息
                    App.medicineManager.UpdateMedicinesInfo(App.HISServerConnector, App.Setting.LastMedicineInfoAysncTime.Value, DateTime.Now,
                        this.onGetedMedicinesFromHISServer, this.onAddedMedicine);
                    #endregion
                }
            }
        }
        #endregion
      
        #region 重新初始化药品数据按钮
        private void initMedicineInfoBtn_Click(object sender, EventArgs e)
        {
            #region 增加重新初始化数据的提示,如果已经初始化了数据,提示更新数据,如果需要强制更新 再执行下面的逻辑
            

            if (localCurrentMedicineKindCount>0)
            {
                if (MessageBox.Show("药品数据不为空\r\n该操作将删除全部药品数据\r\n\r\n是否仍要执行此操作?", "请谨慎操作!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            
            #endregion

                #region 启动线程
                this.initMedicineInfoBtn.Enabled = false;
                this.getMedicinesBW.RunWorkerAsync(App.HISServerConnector);
                
                #endregion
                this.dataGetAndWriteProcessbar.Visible = true;
            
        }        
        #endregion

        #region 模拟扫描药品条码
        private void simulatScanMedicineBarcodeBtn_Click(object sender, EventArgs e)
        {
            App.ControlAnimationRenderingController.ClearAnimationControls();
            AMDM_Medicine randomMedicine = App.medicineManager.GetRandomMedicine();
            if (randomMedicine != null)
            {
                this.currentScanedMedicine = randomMedicine;
                this.currentMedicineNameLabel.Text = this.currentScanedMedicine.Name;
                this.currentMedicineBarcodeLabel.Text = this.currentScanedMedicine.Barcode;
                if (CheckCanContinueReBind(randomMedicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.currentScanedMedicine = null;
                this.currentMedicineNameLabel.Text = "";
                this.currentMedicineBarcodeLabel.Text = "";
            }
        }

        /// <summary>
        /// 检查是否不能继续绑定 当绑定到多个药槽的时候
        /// </summary>
        /// <param name="medicine"></param>
        /// <returns></returns>
        bool CheckCanContinueReBind(AMDM_Medicine medicine)
        {
            #region 如果该药品已经绑定到其他的药槽了,出一个提示框 提示该药品已经绑定到几个药槽中了
            List<AMDM_Clip_data> bindingInfos = App.bindingManager.GetBindedGridList(this.stock.IndexOfMachine, medicine.Id);
            if (bindingInfos.Count > 0)
            {
                for (int i = 0; i < bindingInfos.Count; i++)
                {
                    for (int j = 0; j < this.upPartGridsShowerPanel.Controls.Count; j++)
                    {
                        GridShower currentGridShower = this.upPartGridsShowerPanel.Controls[j] as GridShower;
                        if (currentGridShower.Grid.FloorIndex == bindingInfos[i].FloorIndex && currentGridShower.Grid.IndexOfFloor == bindingInfos[i].GridIndex)
                        {
                            App.ControlAnimationRenderingController.AddAnimationControls(currentGridShower);
                            break;
                        }
                    }
                }
                if (MessageBox.Show(string.Format("当前药品已经绑定到了{0}个药槽中\r\n请确认是否为此药品再次增加药槽", bindingInfos.Count), "确认为此药品增加药槽吗?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    App.ControlAnimationRenderingController.ClearAnimationControls();
                    //不添加该药品的药槽
                    this.currentScanedMedicine = null;
                    this.currentMedicineNameLabel.Text = "";
                    this.currentMedicineBarcodeLabel.Text = "";
                    return false;
                }
            }
            #endregion
            return true;
        }

        private void simulatScanMedicine001BarcodeBtn_Click(object sender, EventArgs e)
        {
            App.ControlAnimationRenderingController.ClearAnimationControls();
            AMDM_Medicine randomMedicine = App.medicineManager.TestGetMedicineByIndex(0);
            if (randomMedicine != null)
            {
                this.currentScanedMedicine = randomMedicine;
                this.currentMedicineNameLabel.Text = this.currentScanedMedicine.Name;
                this.currentMedicineBarcodeLabel.Text = this.currentScanedMedicine.Barcode;
                if (CheckCanContinueReBind(randomMedicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.currentScanedMedicine = null;
                this.currentMedicineNameLabel.Text = "";
                this.currentMedicineBarcodeLabel.Text = "";
            }
        }

        private void simulatScanMedicine100BarcodeBtn_Click(object sender, EventArgs e)
        {
            App.ControlAnimationRenderingController.ClearAnimationControls();
            AMDM_Medicine randomMedicine = App.medicineManager.TestGetMedicineByIndex(99);
            if (randomMedicine != null)
            {
                this.currentScanedMedicine = randomMedicine;
                this.currentMedicineNameLabel.Text = this.currentScanedMedicine.Name;
                this.currentMedicineBarcodeLabel.Text = this.currentScanedMedicine.Barcode;
                if (CheckCanContinueReBind(randomMedicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.currentScanedMedicine = null;
                this.currentMedicineNameLabel.Text = "";
                this.currentMedicineBarcodeLabel.Text = "";
            }
        }
        #endregion

        #region 窗体关闭时,释放
        private void MedicineBindingManageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            App.ICCardReaderAndCodeScanner2in1ReceivedData = null;
            foreach (var item in this.upPartGridsShowerPanel.Controls)
            {
                Control ctrl = item as Control;
                ctrl.Click -= this.shower_Click;
            }
            this.getMedicinesBW.DoWork -= this.getMedicinesBW_DoWork;
            this.getMedicinesBW.ProgressChanged -= this.getMedicinesBW_ProgressChanged;
            this.getMedicinesBW.Dispose();
        }
        #endregion

        #region 最大化 最小化 关闭按钮的操作
        private void closeBtn_Click(object sender, EventArgs e)
        {
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

        private void upPartGridsShowerPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void downPartGridsShowerPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
