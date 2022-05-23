using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AMDM_Domain;
using MyCode;
using AMDM;
using AMDM.Manager;
/*
 * 上药 药品库存管理页面 2021年11月10日13:18:50 与布药管理页面基本相同 但是所处理的内容不同, 展示格子的方式也不同
 */

namespace AMDM
{
    public partial class MedicineInventoryManageForm : Form
    {
        #region 全局变量
        string pleaseScanMsg = "请先扫描药品,对应药槽的药品名称会闪烁提示您\r\n\r\n然后根据要投放的位置进行选择";
        AMDM_Stock stock = null;
        //List<GridShowerInfo> showers = new List<GridShowerInfo>();
        /// <summary>
        /// 当前已经扫描读取到的药品
        /// </summary>
        AMDM_Medicine currentScanedMedicine = null;
        long localCurrentMedicineKindCount = 0;
        /// <summary>
        /// 使用 0->1->2格式的索引 保存的格子显示器信息
        /// </summary>
        Dictionary<string, GridShower> gridShowersDic = new Dictionary<string, GridShower>();
        FormAutoSizer formAutoSizer = null;

        AMDM_InstockRecord currentInstockRecord = null;
        //MedicinesGettingController medicinesGettingControllerRef = null;
        #endregion

        #region 构造函数于初始化
        public MedicineInventoryManageForm()
        {
            InitializeComponent();
            this.formAutoSizer = new MyCode.FormAutoSizer(this);
            this.formAutoSizer.TurnOnAutoSize();
            //this.gridsShowerPanel.Dock = DockStyle.Fill;
            //this.gridsShowerPanel.Height += 100;
            //this.Height += 100;
            #region 默认显示的信息的清空
            this.fClearScanedMedicine();
            #endregion
        }
        public bool Init(AMDM_Stock stock)
        {
            this.stock = stock;
            localCurrentMedicineKindCount = App.medicineManager.GetMedicinesKindCount();
            this.medicineKindCountLabel.Text = this.localCurrentMedicineKindCount.ToString();

            ///当前药仓的数字序号
            this.currentStockIndexLabel.Text = string.Format("{0}仓",(stock.IndexOfMachine + 1).ToString().PadLeft(2, '0'));

            #region 显示空调状态
            App.ControlPanel.ReloadStatus();
            bool acWorking = false;
            float currentTemp = 0;
            if (App.ControlPanel.PeripheralsStatus != null && App.ControlPanel.PeripheralsStatus.WarehousesACStatus != null)
            {
                for (int i = 0; i < App.ControlPanel.PeripheralsStatus.WarehousesACStatus.Count; i++)
                {
                    var current = App.ControlPanel.PeripheralsStatus.WarehousesACStatus[i];
                    if (current.WarehouseIndexId == stock.IndexOfMachine)
                    {
                        acWorking = current.IsACWorking;
                        currentTemp = current.CurrentTemperature;
                        break;
                    }
                }
            }
            if (acWorking)
            {
                this.ACStatusLabel.ForeColor = Color.ForestGreen;
                this.ACStatusLabel.Text = "空调运行中";
            }
            else
            {
                this.ACStatusLabel.ForeColor = Color.Gray;
                this.ACStatusLabel.Text = "空调未启动";
            }
            this.currentStockTemperatureLabel.Text = string.Format("仓内当前{0}℃", currentTemp.ToString("f1"));
            #endregion
            

            initShow();
            #region 当前页面接收串口

            //Console.WriteLine("将注册码卡读头的回调函数,当前回调:{0}",App.ICCardReaderAndCodeScanner2in1ReceivedData);
            App.ICCardReaderAndCodeScanner2in1ReceivedData = this.processScanedMessage;
            #endregion
            return true;
        }

        void initShow()
        {
            this.gridShowersDic.Clear();
            #region 获取所有药品的绑定信息
            List<AMDM_Clip> bindings = App.bindingManager.GetStockAllBindedMedicineWithMedicineObject(stock.IndexOfMachine);
            //Dictionary<long, GridMedicineBindingInfo_data> medicinesIdAndBindingInfoDic = new Dictionary<long,GridMedicineBindingInfo_data>();
            List<long> needGetMedicinesIdList = new List<long>();
            Dictionary<string, AMDM_Clip> bindingInfoLocationDic = new Dictionary<string, AMDM_Clip>();
            for (int i = 0; i < bindings.Count; i++)
            {
                AMDM_Clip current = bindings[i];
                string index = string.Format("{0}->{1}->{2}", current.StockIndex, current.FloorIndex, current.GridIndex);
                bindingInfoLocationDic.Add(index, current);
                if (needGetMedicinesIdList.Contains(current.MedicineId) == false)
                {
                    needGetMedicinesIdList.Add(current.MedicineId);
                }
            }
            #endregion

            #region 获取这些格子需要绑定的药品的实体对象
            List<AMDM_Medicine> medicines = App.medicineManager.GetMedicines(needGetMedicinesIdList);
            Dictionary<long, AMDM_Medicine> medicinesDic = new Dictionary<long, AMDM_Medicine>();
            for (int i = 0; i < medicines.Count; i++)
            {
                medicinesDic.Add(medicines[i].Id, medicines[i]);
            }
            #endregion
            #region 上层转换成可显示的格式
            //显示到panel的时候 缩放的比例
            float xScallRate = this.gridsInUpPartShowerPanel.ClientRectangle.Width * 1f / (stock.MaxFloorWidthMM - App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM);
            float yScallRate = this.gridsInUpPartShowerPanel.ClientRectangle.Height * 1f / stock.MaxFloorsHeightMM;

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
                    //currentGrid.IndexOfStock = currentGridIndexOfStock++;
                    string index = string.Format("{0}->{1}->{2}", stock.IndexOfMachine, currentFloor.IndexOfStock, currentGrid.IndexOfFloor);
                    AMDM_Clip bindingInfo = bindingInfoLocationDic.ContainsKey(index) ? bindingInfoLocationDic[index] : null;
                    AMDM_Medicine medicine = null;
                    if (bindingInfo != null && medicinesDic.ContainsKey(bindingInfo.MedicineId))
                    {
                        medicine = medicinesDic[bindingInfo.MedicineId];
                    }
                    //GridShowerInfo shower = new GridShowerInfo();
                    GridShower shower = new GridShower();
                    this.gridShowersDic.Add(index, shower);
                    //showers.Insert(0,shower);
                    //showers.Add(shower);
                    shower.BorderStyle = BorderStyle.FixedSingle;
                    #region 计算格子可以添加多少个药品
                    int maxCanLoadCount = 0;
                    if (medicine != null)
                    {
                        maxCanLoadCount = (int)Math.Floor(currentFloor.DepthMM / medicine.BoxLongMM);
                    }
                    #endregion
                    shower.Init(currentGrid,bindingInfo,
                        medicine, maxCanLoadCount, bindingInfo == null ? -1 : bindingInfo.MedicineObjects.Count);
                    shower.Location = new Point((int)Math.Round(
                        (currentGrid.LeftMM-(App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM)/2)  * xScallRate),
                        (int)Math.Round((stock.MaxFloorsHeightMM - currentFloor.TopMM) * yScallRate)
                        );
                    shower.Size = new System.Drawing.Size((int)Math.Round((currentGrid.RightMM - currentGrid.LeftMM -App.Setting.HardwareSetting.Grid.GridWallWidthMM) * xScallRate),
                        (int)Math.Round((currentFloor.TopMM - currentFloor.BottomMM) * yScallRate - App.Setting.HardwareSetting.Floor.FloorPanelHeightMM));

                    #region 添加对格子点击事件的绑定
                    shower.Click += shower_Click;
                    #endregion
                    this.gridsInUpPartShowerPanel.Controls.Add(shower);
                }
            }
            #endregion
            #region 下部分的层板的可显示格式
            //下部分空间中的每一个层板的显示高度是多少,包含了padding
            float perDownPartFloorHeight = this.gridsInDownPartShowerPanel.Height * 1f / downPartFloors.Count;
            float perDownPartFloorPadding = 5;
            int currentDownPartFloorIndex = 0;
            float downPartXScallRate = this.gridsInDownPartShowerPanel.ClientRectangle.Width * 1f / stock.MaxFloorWidthMM;
            //float downPartYScallRate = this.gridsInDownPartShowerPanel.ClientRectangle.Height * 1f / 40;
            foreach (var item in downPartFloors)
            {
                //currentGridIndexOfStock = (0 - item.IndexOfStock + 1) * 100 + 1;
                AMDM_Floor currentFloor = item;
                for (int j = 0; j < currentFloor.Grids.Count; j++)
                {
                    AMDM_Grid currentGrid = currentFloor.Grids[j];
                    //currentGrid.IndexOfStock = currentGridIndexOfStock++;
                    string index = string.Format("{0}->{1}->{2}", stock.IndexOfMachine, currentFloor.IndexOfStock, currentGrid.IndexOfFloor);
                    var bindingInfo = bindingInfoLocationDic.ContainsKey(index) ? bindingInfoLocationDic[index] : null;
                    AMDM_Medicine medicine = null;
                    if (bindingInfo != null && medicinesDic.ContainsKey(bindingInfo.MedicineId))
                    {
                        medicine = medicinesDic[bindingInfo.MedicineId];
                    }
                    //GridShowerInfo shower = new GridShowerInfo();
                    GridShower shower = new GridShower();
                    this.gridShowersDic.Add(index, shower);
                    //showers.Insert(0,shower);
                    //showers.Add(shower);
                    shower.BorderStyle = BorderStyle.FixedSingle;
                    #region 计算格子可以添加多少个药品
                    int maxCanLoadCount = 0;
                    if (medicine != null)
                    {
                        maxCanLoadCount = (int)Math.Floor(currentFloor.DepthMM / medicine.BoxLongMM);
                    }
                    #endregion
                    shower.Init(currentGrid,bindingInfo,
                        medicine, maxCanLoadCount, bindingInfo == null ? -1 : bindingInfo.MedicineObjects.Count);
                    shower.Location = new Point((int)Math.Round(currentGrid.LeftMM * downPartXScallRate + App.Setting.HardwareSetting.Grid.GridWallWidthMM / 2),
                        (int)Math.Round(perDownPartFloorHeight * currentDownPartFloorIndex + perDownPartFloorPadding / 2f));
                    shower.Size = new System.Drawing.Size((int)Math.Round((currentGrid.RightMM - currentGrid.LeftMM) * downPartXScallRate - App.Setting.HardwareSetting.Grid.GridWallWidthMM),
                        (int)Math.Round(perDownPartFloorHeight - perDownPartFloorPadding)
                        );

                    #region 添加对格子点击事件的绑定
                    shower.Click += shower_Click;
                    #endregion
                    this.gridsInDownPartShowerPanel.Controls.Add(shower);
                }
                currentDownPartFloorIndex++;
            }
            #endregion

        }
        
        #endregion

        void fClearScanedMedicine()
        {
            this.currentScanedMedicine = null;
            this.currentMedicineNameLabel.Text = "";
            this.currentMedicineBarcodeLabel.Text = "";
        }
        #region 当格子被点击以后的事件
        bool processShowerMouseRightClick(GridShower shower)
        {
            
                if (shower.BindedMedicine == null)
                {
                    ///点击右键但是点击的药槽内没有绑定药品,直接返回不需要处理
                    return false;
                }
                //当鼠标右键按下的时候是要解除药品的绑定
                #region 对于药品从槽子里清空的情况
                BindingActionsForm bform = new BindingActionsForm(shower, this.stock.Floors[shower.Grid.FloorIndex], true,this.Speak);
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
        void showGridNumberOnStock(int storkIndex, Nullable<int> number)
        {
            //MessageBox.Show(string.Format("在药仓{0}上显示号码:{1}", storkIndex+1, number == null? "清空": (number.Value+1).ToString()));
            App.medicinesGettingController.MainPLCCommunicator.Connect();
            App.medicinesGettingController.MainPLCCommunicator.SendShowGridNumberAt485ShowerOnStock(storkIndex, number);
        }
        bool processShowerMouseLeftClick(GridShower shower, AMDM_Medicine bindedMedicine)
        {
            #region 其他,鼠标左键
            #region 检测是否已经扫描了药品
            if (this.currentScanedMedicine == null)
            {
                
                this.Speak(pleaseScanMsg);
                MessageBox.Show(this, pleaseScanMsg, "请先扫描药品", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            #endregion
            #region 检查该药槽是否已经启用,也就是shower上有没有绑定药品
            if (bindedMedicine == null)
            {
                string unUseGridMsg = "该药槽未启用";
                this.Speak(unUseGridMsg);
                MessageBox.Show(this, unUseGridMsg);

                return false;
            }
            #endregion
            #region 检查药槽绑定的药品和当前扫描的药品是否一致
            if (bindedMedicine.Id != this.currentScanedMedicine.Id)
            {
                string canPutInThisGridMsg = "当前药品不应投放在此药槽!";
                this.Speak(canPutInThisGridMsg);
                MessageBox.Show(this, canPutInThisGridMsg, "错误的选择", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            #endregion
            #region 检查药槽是否已经满载
            //如果当前扫描的药品和目标药槽的药品一致的话,检查该药槽是否已满
            if (shower.CurrentLoadedCount >= shower.MaxLoadableCount)
            {
                string fullMsg = "当前药槽已满!\r\n请选择其他闪烁药名的药槽";
                this.Speak(fullMsg);
                MessageBox.Show(this, fullMsg, "请选择其他药槽", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            #endregion
            //弹出弹窗显示要放多少个
            #region 确认数量

            shower.Selected = true;
            //把别的格子都清掉,就显示这一个格子 让他变为选择状态 然后他的格子的背景就是多变的,药品的颜色就不变了.
            App.ControlAnimationRenderingController.ClearAnimationControls();
            App.ControlAnimationRenderingController.AddAnimationControls(shower);
            //数量选择的框框
            MedicineCountForm cform = new MedicineCountForm();
            cform.Init(shower);
            if (cform.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                fClearScanedMedicine();
                shower.Selected = false;
                App.ControlAnimationRenderingController.ClearAnimationControls();
                //不添加药品
                return false;
            }
            if (cform.WantAddCount <= 0)
            {
                string countInvalidMsg = "选择的要添加药品的数量不正确";
                this.Speak(countInvalidMsg);
                MessageBox.Show(this, countInvalidMsg);
                return false;
            }
            #endregion
            Nullable<DateTime> currentExpirationTime = null;
            //如果严控药品有效期模式已经开启,要输入药品有效期.
            if (App.Setting.ExpirationStrictControlSetting.Enable)
            {
                #region 确认到期日期还有多少个月
                this.Speak("请选择该批药品的有效期");
                ExpirationDataSelectForm eform = new ExpirationDataSelectForm();
                eform.WindowState = FormWindowState.Maximized;
                FormAutoSizer fas = new FormAutoSizer(eform);
                fas.TurnOnAutoSize();
                DialogResult eformRet = eform.ShowDialog();
                if (eformRet == System.Windows.Forms.DialogResult.OK)
                {
                    currentExpirationTime = eform.SelectedDateTime;
                    //剩余保质期多少天
                    int expDays = (int)Math.Floor((eform.SelectedDateTime - DateTime.Now).TotalDays);

                    //如果小于最小有效天数设定不能放入药仓
                    int clmed = bindedMedicine.CLMED == null ? App.Setting.ExpirationStrictControlSetting.DefaultCanLoadMinExpirationDays : bindedMedicine.CLMED.Value;
                    if (expDays < clmed)
                    {
                        string expTimeMsg = string.Format("该药品剩余保质期{0}天\r\n请勿装配再药机中\r\n如仍需继续,请联系管理员设定 [可装入药机的最少药品有效天数]", expDays);
                        string minTitle = string.Format("药品有效期最少{0}天才可放入药机", clmed);
                        Speak(minTitle);
                        MessageBox.Show(this, expTimeMsg, minTitle);
                        return false;
                    }

                    //保质期少于指定天数的不建议放在药机中.
                    int slmed = bindedMedicine.SLMED == null ? App.Setting.ExpirationStrictControlSetting.DefaultSuggestLoadMinExpirationDays : bindedMedicine.SLMED.Value;
                    if (expDays < slmed)
                    {
                        string expTimeMsg = string.Format("该药品剩余保质期{0}天\r\n不建议装配再药机中.\r\n请确认是否仍要装配在此药机中?", expDays);
                        string minTitle = string.Format("药品有效期建议{0}天以上", slmed);
                        Speak(expTimeMsg);
                        if (MessageBox.Show(this, expTimeMsg, minTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                        { return false; }
                    }

                    if (shower.CurrentLoadedCount > 0)
                    {
                        #region 检测之前装入的药品的保质期 跟现在选择的药品的保质期  哪个更短 如果后放的药很快就要过期了, 就要把之前的清出来以后排序后重新放入, 弹出提示框问是否清出来重新放
                        //如果后放的不是很快要过期了 就是提示一下最好好好排序, 不弹出是否清空药槽重新放
                        var medicinesObjs = App.bindingManager.GetMedicinesObject(shower.Grid.StockIndex, shower.Grid.FloorIndex, shower.Grid.IndexOfFloor);
                        if (medicinesObjs.Count != shower.CurrentLoadedCount)
                        {
                            string loadObjsErrMsg = string.Format("当前药槽已装载的药品数量应为{0}, 实际获取到的实物信息有{1}, 请联系管理员检查");
                            MessageBox.Show(this, loadObjsErrMsg);
                            Utils.LogBug(loadObjsErrMsg, shower.Grid);
                            return false;
                        }
                        //看一下最后一个有日期的药的的截止日期
                        Nullable<DateTime> lastMedicineObjectExpirationDate = null;
                        for (int i = medicinesObjs.Count - 1; i >= 0; i--)
                        {
                            if (medicinesObjs[i].ExpirationDate != null)
                            {
                                lastMedicineObjectExpirationDate = medicinesObjs[i].ExpirationDate;
                                break;
                            }
                        }
                        if (lastMedicineObjectExpirationDate != null)
                        {
                            if (eform.SelectedDateTime < lastMedicineObjectExpirationDate)
                            {
                                //刚刚放入的药品比之前的日期要早
                                //临期的药品 建议清空药槽以后 再重新排列好放入药品.
                                if (expDays < clmed)
                                {
                                    string expTimeMsg = string.Format("本次将装配的药品已临期\r\n此药槽中已经装载了日期较好的药品\r\n建议清空药槽后重新排列这些药品,按照临期的药品优先出库的顺序装载\r\n要执行清空药槽重新上药吗?\r\n\r\n如果您点击[确定],立刻执行药槽清空操作\r\n如果您点击[取消],将取消本次上药");
                                    string minTitle = string.Format("建议调整顺序重新上药");
                                    Speak(expTimeMsg);
                                    DialogResult ret = MessageBox.Show(this, expTimeMsg, minTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                    if (ret == System.Windows.Forms.DialogResult.Cancel)
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        //执行清空药槽操作
                                        //AMDM.BindingActionsForm bform = new BindingActionsForm(shower, null, true);
                                        processShowerMouseRightClick(shower);
                                        return false;
                                    }
                                }
                                else
                                {
                                    //不是临期的  提示一下是否仍要继续
                                    string expTimeMsg = string.Format("您本次将装配的药品比该药槽内的药品更早过有效期\r\n建议您装配在其他的药槽\r\n另外,您可以清空该药槽后重新排列并装配\r\n仍要按照不建议的顺序继续装配吗?");
                                    Speak(expTimeMsg);
                                    if (MessageBox.Show(this, expTimeMsg, "建议装配到其他药槽或重新排列后装配", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                //没事儿 可以正常放.
                            }
                        }
                        #endregion
                    }
                    else
                    {


                    }
                }

                else
                {
                    //用户没有输入有效期,跳过???
                    return false;
                }

                #endregion
            }
            
            bool changeCountRet = false;
            
            //在数据库内修改药品的数量,增加当前选择的数量 然后获取当前的数量是不是超过了最大的装载数量,虽然通常不会出问题 但是还是谨慎为妙
            try
            {
                //显示在药仓上
                string checkMsg = string.Format("您已选择将{0}盒药品放入{1}号药槽\r\n该药槽号已显示在药仓上方\r\n\r\n现在请将药品放入药仓\r\n\r\n完成后请点击[确定],如有变动请点击 [取消]"
                    ,
                    cform.WantAddCount, string.Format("{0}", (shower.Grid.IndexOfStock+1)).PadLeft(3, '0'));

                this.Speak(checkMsg);
                showGridNumberOnStock(shower.Grid.StockIndex, shower.Grid.IndexOfStock);
                if (MessageBox.Show(this,
                    checkMsg,
                    "放完药品后请点反馈", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Cancel)
                {
                    this.fClearScanedMedicine();
                    shower.Selected = false;
                    App.ControlAnimationRenderingController.ClearAnimationControls();
                    showGridNumberOnStock(shower.Grid.StockIndex,null);
                    return false;
                }
                //记录入库单明细到数据库
                AMDM_InstockRecordDetail recordDetailRet = App.inventoryManager.AddInstockDetail(ref this.currentInstockRecord, shower.Grid.StockIndex, shower.Grid.FloorIndex, shower.Grid.IndexOfFloor,
                    currentScanedMedicine.Id, currentScanedMedicine.Name, currentScanedMedicine.Barcode, cform.WantAddCount);
                if (recordDetailRet == null)
                {
                    string recordErrMsg = "记录上药明细单失败,请与管理员联系!";
                    this.Speak(recordErrMsg);
                    MessageBox.Show(this, recordErrMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                //记录当前库存到数据库

                changeCountRet = App.bindingManager.InOutMedicineCount(shower.Grid, bindedMedicine.Id, currentInstockRecord.Id, currentExpirationTime, cform.WantAddCount, shower.CurrentLoadedCount + cform.WantAddCount);
                
                //更新库存信息后,显示更新成功
                if (changeCountRet)
                {
                    string finishedMsg = "本次上药完成!";
                    this.Speak(finishedMsg);
                    //MessageBox.Show(this,finishedMsg, "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.fClearScanedMedicine();
                    shower.CurrentLoadedCount = shower.CurrentLoadedCount + cform.WantAddCount;
                    shower.Selected = false;
                    shower.Refresh();
                }
                else
                {
                    string updErrMsg = "数据更新失败,请与管理员联系!";
                    this.Speak(updErrMsg);
                    MessageBox.Show(this, updErrMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception err)
            {
                string errmsg = "更新库存信息错误,请与管理员联系";
                this.Speak(errmsg);
                MessageBox.Show(this, err.Message, errmsg, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                throw;
            }

            App.ControlAnimationRenderingController.ClearAnimationControls();
            showGridNumberOnStock(shower.Grid.StockIndex,null);
            #endregion
            return true;
        }
        void shower_Click(object sender, EventArgs e)
        {
            GridShower shower = sender as GridShower;

            AMDM_Medicine bindedMedicine = shower.BindedMedicine;

            #region 如果是鼠标右键
            MouseEventArgs me = e as MouseEventArgs;
            if (me != null && me.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (processShowerMouseRightClick(shower) == false)
                {
                    return;
                }

            }
            #endregion
            #region 如果是鼠标左键
            else if (!processShowerMouseLeftClick(shower,bindedMedicine))
            {
                return;
            }
            #endregion
        }

        #endregion

        #region 当前页面接收串口命令的函数
        delegate void ShowScanedMedicineFunc(AMDM_Medicine medicine);
        void showScanedMedicine(AMDM_Medicine medicine)
        {
            if (this.currentMedicineNameLabel.InvokeRequired)
            {
                ShowScanedMedicineFunc fc = new ShowScanedMedicineFunc(showScanedMedicine);
                this.currentMedicineNameLabel.Invoke(fc, medicine);
                return;
            }

            if (medicine != null)
            {
                this.currentScanedMedicine = medicine;
                this.currentMedicineNameLabel.Text = this.currentScanedMedicine.Name;
                this.currentMedicineBarcodeLabel.Text = this.currentScanedMedicine.Barcode;
                if (CheckMedicineBindingAndShowNeedInventoryGrids(medicine) == false)
                {
                    return;
                }
            }
            else
            {
                //MessageBox.Show("未找到药品数据");
                this.Speak("未找到药品数据");
                this.fClearScanedMedicine();
            }
        }
        void processScanedMessage(string msg)
        {
            Console.WriteLine("在上药页面接收到码卡读头的数据:{0}", msg);
            App.ControlAnimationRenderingController.ClearAnimationControls();
            AMDM_Medicine medicine = App.medicineManager.GetMedicineByBarcode(msg);
            showScanedMedicine(medicine);
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
                if (CheckMedicineBindingAndShowNeedInventoryGrids(randomMedicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.fClearScanedMedicine();
            }
        }

        /// <summary>
        /// 使用语音朗读引擎播放声音
        /// </summary>
        /// <param name="msg"></param>
        void Speak(string msg)
        {
            if (App.TTSSpeaker == null)
            {
                Utils.LogWarnning("语音朗读引擎没有初始化");
                return;
            }
            App.TTSSpeaker.Speak(msg);
        }

        #region 检查当前扫描的药品 在哪个槽子当中 如果没有 提示没有药槽需要此药品 如果有药槽已经绑定了该药品 闪烁显示药槽 具体哪个药槽缺药 显示空的那个药槽的缺药的部分
        bool CheckMedicineBindingAndShowNeedInventoryGrids(AMDM_Medicine medicine)
        {
            //获取该药品所有的绑定信息
            List<AMDM_Clip_data> bindingsInfoList = App.bindingManager.GetBindedGridList(this.stock.IndexOfMachine, medicine.Id);
            if (bindingsInfoList.Count == 0)
            {
                string notInThisStockMsg = "此药仓没有药槽需要此药品(此药品不在该药仓中)";

                this.Speak(notInThisStockMsg);
                MessageBox.Show(this, notInThisStockMsg, "此药仓没有药槽需要此药品", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ///当前药品绑定的药槽集合
            List<GridShower> currentMedicineGridShowers = new List<GridShower>();
            ///当前药品绑定的药槽没有库存的药槽集合
            List<GridShower> currentMedicineNotFullGridShowers = new List<GridShower>();
            //根据绑定信息,获取该窗口中绑定的格子,并让他们显示出来.
            for (int i = 0; i < bindingsInfoList.Count; i++)
            {
                AMDM_Clip_data currentBindingInfo = bindingsInfoList[i];
                string index = string.Format("{0}->{1}->{2}", currentBindingInfo.StockIndex, currentBindingInfo.FloorIndex, currentBindingInfo.GridIndex);
                if (this.gridShowersDic.ContainsKey(index) == true)
                {
                    GridShower gridShower = this.gridShowersDic[index];
                    currentMedicineGridShowers.Add(gridShower);
                }
            }
            for (int i = 0; i < currentMedicineGridShowers.Count; i++)
            {
                GridShower gridShower = currentMedicineGridShowers[i];
                App.ControlAnimationRenderingController.AddAnimationControls(gridShower);
                //计算当前最大装载量是否装载完毕 如果没有装载完毕 增加到没有完全装载的格子集合中
                AMDM_Floor currentFloor = this.stock.Floors[gridShower.Grid.FloorIndex];

                if (gridShower.CurrentLoadedCount < gridShower.MaxLoadableCount)
                {
                    currentMedicineNotFullGridShowers.Add(gridShower);
                }
            }
            //如果没有装满的药槽数量不为0那就是有药槽需要该药品,否则就是没有
            if (currentMedicineNotFullGridShowers.Count == 0)
            {
                string fullMsg = "此药仓没有药槽需要此药品(所有应装配该药品的药槽已满!)";
                this.Speak(fullMsg);
                MessageBox.Show(this, fullMsg, "此药仓没有药槽需要此药品", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        #endregion

        private void simulatScanMedicine001BarcodeBtn_Click(object sender, EventArgs e)
        {
            App.ControlAnimationRenderingController.ClearAnimationControls();
            AMDM_Medicine randomMedicine = App.medicineManager.TestGetMedicineByIndex(0);
            if (randomMedicine != null)
            {
                this.currentScanedMedicine = randomMedicine;
                this.currentMedicineNameLabel.Text = this.currentScanedMedicine.Name;
                this.currentMedicineBarcodeLabel.Text = this.currentScanedMedicine.Barcode;
                if (CheckMedicineBindingAndShowNeedInventoryGrids(randomMedicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.fClearScanedMedicine();
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
                if (CheckMedicineBindingAndShowNeedInventoryGrids(randomMedicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.fClearScanedMedicine();
            }
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

        private void MedicineInventoryManageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            #region 关闭窗口以后记录入库单的完结
            App.inventoryManager.FinishInstockRecord(ref this.currentInstockRecord, false);
            //MessageBox.Show(this, 
            //    Newtonsoft.Json.JsonConvert.SerializeObject(this.currentInstockRecord, new Newtonsoft.Json.JsonSerializerSettings() {  Formatting = Newtonsoft.Json.Formatting.Indented}),
            //    "已保存入库单");
            this.currentInstockRecord = null;
            #endregion
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("已关闭上药管理页面,开始析构");
            //var func = App.ICCardReaderAndCodeScanner2in1ReceivedData;
            //var tar = func.Target;
            //var met = func.Method;
            //Console.WriteLine("当前码卡读头回调函数:{0}", func);
            App.ICCardReaderAndCodeScanner2in1ReceivedData = null;

            unRegisterShowersCallback();

            Console.ResetColor();
            App.TTSSpeaker.Speak("上药记录已保存", false);
            App.medicinesGettingController.MainPLCCommunicator.Connect();
            App.medicinesGettingController.MainPLCCommunicator.SendLockerControlCommand(false);
            App.medicinesGettingController.MainPLCCommunicator.Disconnect();
        }

        void unRegisterShowersCallback()
        {
            foreach (var item in this.gridsInUpPartShowerPanel.Controls)
            {
                Control ctrl = item as Control;
                ctrl.Click -= this.shower_Click;
            }
            foreach (var item in this.gridsInDownPartShowerPanel.Controls)
            {
                Control ctrl = item as Control;
                ctrl.Click -= this.shower_Click;
            }
            Console.WriteLine("药槽点击事件已全部注销");
        }

        #region 开始上药的时候直接就创建一个入库单 然后每上一个药 记录一次上药明细,关闭窗口时,记录完结上药单
        private void MedicineInventoryManageForm_Load(object sender, EventArgs e)
        {
            if (this.currentInstockRecord == null)
            {
                AMDM_InstockRecord record = App.inventoryManager.CreateInstockRecord(0, this.stock.Id, 0, "上药");
                if (record != null)
                {
                    this.currentInstockRecord = record;
                }
                else
                {
                    MessageBox.Show(this, "创建出库单失败,数据连接错误");
                    return;
                }
            }
            App.Init(null, null, null);
            App.medicinesGettingController.MainPLCCommunicator.Connect();
            App.medicinesGettingController.MainPLCCommunicator.SendLockerControlCommand(true);
            this.Speak(pleaseScanMsg);
        }
        #endregion

        private void medicineBindingManageBtn_Click(object sender, EventArgs e)
        {
            MedicineBindingManageForm mform = new MedicineBindingManageForm();
            mform.Init(this.stock);
            mform.StartPosition = FormStartPosition.CenterParent;
            Speak("请先扫描药品,再点选目标药槽");
            mform.ShowDialog();
            //关闭了窗口以后,把接收串口数据的函数还设置回来
            App.ICCardReaderAndCodeScanner2in1ReceivedData = this.processScanedMessage;

            //更新显示

            this.unRegisterShowersCallback();
            this.gridsInUpPartShowerPanel.Controls.Clear();
            this.gridsInDownPartShowerPanel.Controls.Clear();
            this.fClearScanedMedicine();
            this.initShow();
        }

        private void rePrintDeliveryRecordPaperBtn_Click(object sender, EventArgs e)
        {

        }

        private void gridsInUpPartShowerPanel_Paint(object sender, PaintEventArgs e)
        {
            //定义一下这个方法  自动缩放控制器就会自动的判断是要自己绘图的 然后里面的GridShower就不会自动改变大小了
        }

        private void gridsInDownPartShowerPanel_Paint(object sender, PaintEventArgs e)
        {
            //定义一下这个方法  自动缩放控制器就会自动的判断是要自己绘图的 然后里面的GridShower就不会自动改变大小了
        }

        private void simulatScanMedicineDYSBarcodeBtn_Click(object sender, EventArgs e)
        {
            //模拟扫描地榆升白胶囊条码
            App.ControlAnimationRenderingController.ClearAnimationControls();
            AMDM_Medicine randomMedicine = App.medicineManager.TestGetMedicineDYS();
            if (randomMedicine != null)
            {
                this.currentScanedMedicine = randomMedicine;
                this.currentMedicineNameLabel.Text = this.currentScanedMedicine.Name;
                this.currentMedicineBarcodeLabel.Text = this.currentScanedMedicine.Barcode;
                if (CheckMedicineBindingAndShowNeedInventoryGrids(randomMedicine) == false)
                {
                    return;
                }
            }
            else
            {
                this.fClearScanedMedicine();
            }
        }
        
    }
}
