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

namespace AMDM
{
    public partial class BindingActionsForm : Form
    {
        //public enum ActionsEnum { 
        //    /// <summary>
        //    /// 取消
        //    /// </summary>
        //    Cancel, 
        //    /// <summary>
        //    /// 解绑
        //    /// </summary>
        //    UnBind, 
        //    /// <summary>
        //    /// 强制清空药槽,以便重新进行绑定
        //    /// </summary>
        //    ForceClearGrid,
        //    /// <summary>
        //    /// 强制进行库存的矫正
        //    /// </summary>
        //    ForceResetInventory
        //}
        GridShower showerRef = null;

        List<AMDM_MedicineObject_data> currentLoadedMedicines = new List<AMDM_MedicineObject_data>();

        //public GridMedicineBindingInfo_data BindingInfo { get; set; }
        /// <summary>
        /// 执行操作完成以后 返回的动作
        /// </summary>
        //public ActionsEnum Action { get; set; }
        public BindingActionsForm(GridShower gridShower, AMDM_Floor floor, bool inventoryManageMode,Action<string> speakFunc)
        {
            InitializeComponent();
            //this.StartPosition = FormStartPosition.CenterParent;
            FormAutoSizer fas = new FormAutoSizer(this);
            //this.WindowState = FormWindowState.Maximized;
            fas.TurnOnAutoSize();
            this.speakFunc = speakFunc;
            this.Init(gridShower, floor, inventoryManageMode);
        }
        Action<string> speakFunc;
        void Speak(string msg)
        {
            if (this.speakFunc!= null)
            {
                this.speakFunc(msg);
            }
        }
        /// <summary>
        /// 初始化药品绑定信息管理页面,这个页面中包含 解除药品绑定 强制清空药槽 取消. 当库存管理模式的时候,由于护士没有权限解除药品的绑定,所以不显示解除药品绑定的按钮.
        /// </summary>
        /// <param name="gridShower"></param>
        /// <param name="floor"></param>
        /// <param name="inventoryManageMode"></param>
        /// <returns></returns>
        bool Init(GridShower gridShower,AMDM_Floor floor, bool inventoryManageMode)
        {
            this.showerRef = gridShower;
            this.indexOfStockLabel.Text = string.Format("{0}", gridShower.Grid.IndexOfStock+1).PadLeft(3,'0');
            this.indexInfoLabel.Text = string.Format("第{0}仓第{1}层第{2}槽",
                gridShower.Grid.StockIndex+1, 
                gridShower.Grid.FloorIndex +1, 
                gridShower.Grid.IndexOfFloor +1);
            if (gridShower.BindedMedicine!= null)
            {
                this.bindedMedicineMaxLoadableCountLabel.Text =gridShower.BindedMedicine.BoxLongMM == 0? "未录入药盒尺寸": string.Format("{0}盒余{1}毫米", 
                    (int)Math.Floor(floor.DepthMM/gridShower.BindedMedicine.BoxLongMM),
                    (float)Math.Floor(floor.DepthMM%gridShower.BindedMedicine.BoxLongMM)
                    );
                this.bindedMedicineNameLabel.Text = gridShower.BindedMedicine.Name;
                this.bindedMedicineBarcodeLabel.Text = gridShower.BindedMedicine.Barcode;
                this.bindedMedicineBoxSizeLabel.Text = string.Format("长:{0} 宽:{1} 高:{2}", gridShower.BindedMedicine.BoxLongMM, gridShower.BindedMedicine.BoxWidthMM, gridShower.BindedMedicine.BoxHeightMM);
            }
            #region 如果是库存管理模式,隐藏解除药品绑定的按钮
            if (inventoryManageMode)
            {
                this.button1.Visible = false;
            }
            #endregion

            #region 如果是已经卡药了.清空药槽的按钮高亮提示
            if (gridShower.BindingInfo!= null && gridShower.BindingInfo.Stuck)
            {
                this.setClearButtonText(true);
            }
            else
            {
            }
            #endregion

            #region 加载和显示装载的情况
            currentLoadedMedicines = App.bindingManager.GetMedicinesObject(gridShower.Grid.StockIndex, gridShower.Grid.FloorIndex, gridShower.Grid.IndexOfFloor);
            #endregion
            //this.panel1.Controls.Add(gridShower);
            //gridShower.Dock = DockStyle.Fill;
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var shower = this.showerRef;
            //GridMedicineBindingInfo_data bindingInfo = App.bindingManager.GetBindingInfo(shower.Grid);
            int currentInventory = App.bindingManager.GetMedicinesObjectCount(shower.Grid);
            if (currentInventory > 0)
            {
                MessageBox.Show(
                    string.Format("当前药槽中有药品尚未清空,库存:{0}\r\n请在上药管理页清空药品,或使用当前布药权限强制清空药槽后再解除绑定", currentInventory),
                    "不是空的药槽", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
                return;
            }
            else
            {
                if (App.bindingManager.UnBindMedicine(shower.Grid, shower.BindedMedicine) == true)
                {
                    shower.BindedMedicine = null;
                    shower.Refresh();
                    MessageBox.Show("已解除此药槽的药品绑定");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show("解除此药槽的药品绑定错误,请联系管理员!");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    return;
                }
            }
        }
        delegate void setClearButtonTextFunc(bool stuck);
        void setClearButtonText(bool stuck)
        {
            if (this.button4.InvokeRequired)
            {
                setClearButtonTextFunc fc = new setClearButtonTextFunc(setClearButtonText);
                this.button4.BeginInvoke(fc, stuck);
                return;
            }
            if (stuck)
            {
                this.button4.BackColor = Color.LightBlue;
                this.button4.Text = "当前药槽已卡药\r\n点此处清空药槽";
                this.button4.ForeColor = Color.Crimson;
            }
            else
            {
                this.button4.BackColor = Color.LightGray;
                this.button4.Text = "强制清空药槽";
                this.button4.ForeColor = Color.Crimson;
            }
        }
        FullScreenNoticeShower noticeShower = new FullScreenNoticeShower();
        private void button2_Click(object sender, EventArgs e)
        {
            var shower = this.showerRef;
            string msg = "危险操作!!\r\n此操作将清空该药槽内的药品数量信息\r\n这可能导致药品库存不准确\r\n\r\n如果强制清空该药槽,请确保将药槽内的药品全部取出\r\n\r\n确认强制清空该药槽吗?";
            this.Speak(msg);
            if (MessageBox.Show(this, msg, "确认强制清空药槽?", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                #region 使用机械手清空药槽
                var plc = App.medicinesGettingController.GetStocksPLC(shower.Grid.StockIndex);
                if (plc != null)
                {
                    noticeShower.Title = "请稍后";
                    noticeShower.Message = string.Format("正在清空{0}号药槽......", shower.Grid.IndexOfStock+1);
                    Speak(noticeShower.Message);
                    //noticeShower.TopMost = true;
                    noticeShower.Show(this);
                    plc.StartGridClear(shower.Grid, shower.MaxLoadableCount + 10, (times) =>
                    {
                        if (!this.ClearGrid(shower))
                        {
                            MessageBox.Show(this, "清空药槽发生错误");
                        }
                        else
                        {
                            shower.CurrentLoadedCount = 0;
                            shower.Refresh();
                            string finishMsg = string.Format("已完成药槽清空操作,共清出件数为:{0}。\r\n请再次确认药槽内药品是否已被清空", times);
                            bool ret = App.bindingManager.SetClipStucked(shower.Grid.StockIndex, shower.Grid.FloorIndex, shower.Grid.IndexOfFloor, false);
                            if (ret)
                            {
                                Utils.LogSuccess("清除弹夹卡药状态完成");
                                shower.BindingInfo.Stuck = false;
                                this.setClearButtonText(false);
                            }
                            else
                            {
                                Utils.LogWarnning("清除弹夹已经卡药失败");
                            }
                            shower.Refresh();
                            Speak(finishMsg);
                            if (noticeShower.IsDisposed == false)
                            {
                                MessageBox.Show(noticeShower, finishMsg, "完成");
                                noticeShower.Close();
                            }
                            else
                            {
                                MessageBox.Show(finishMsg, "完成");
                            }
                        }
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    });

                    //noticeShower.ShowDialog();
                    //this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    //this.Close();
                    return;
                }
                else
                {
                    string errmsg = "查询药仓控制器出现错误,清联系管理员";
                    MessageBox.Show(this, errmsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    return;
                }
                #endregion
            }
            else
            {
                //this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                //this.Close();
            }
            //this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //this.Action = ActionsEnum.Cancel;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void BindingActionsForm_Load(object sender, EventArgs e)
        {

        }
        #region 库存矫正
        private void resetInventoryBtn_Click(object sender, EventArgs e)
        {
            GridShower shower = this.showerRef;
            #region 如果选择的是库存矫正,那就弹出库存矫正的窗口
            ResetInventoryForm rform = new ResetInventoryForm();
            rform.Init(shower.CurrentLoadedCount, shower.MaxLoadableCount);
            DialogResult rformRet = rform.ShowDialog();
            if (rformRet == System.Windows.Forms.DialogResult.Cancel)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
                return;
            }
            else
            {
                if (shower.CurrentLoadedCount == rform.WantInventoryChangeTo)
                {
                    //没有改变 还是一样的
                    return;
                }
                if (App.inventoryManager.ForceResetInventory(shower.Grid, shower.BindedMedicine, string.Format("MIMF-FRSI-{0}", DateTime.Now.Ticks), shower.CurrentLoadedCount, rform.WantInventoryChangeTo) == false)
                {
                    string updateInventoryErrMsg = "更新药品库存失败,请联系管理员查看错误日志";
                    this.Speak(updateInventoryErrMsg);
                    MessageBox.Show(this, updateInventoryErrMsg); 
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    return;
                }
                else
                {
                    shower.CurrentLoadedCount = rform.WantInventoryChangeTo;
                    shower.Refresh();
                    string updatedInventoryMsg = "已更新药品库存";
                    this.Speak(updatedInventoryMsg);
                    MessageBox.Show(this, updatedInventoryMsg);
                }
            }
            #endregion
            //this.Action = ActionsEnum.ForceResetInventory;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }
        #endregion


        /// <summary>
        /// 清空药槽 如果发生错误 就标记为return  false
        /// </summary>
        /// <param name="shower"></param>
        /// <returns></returns>
        public bool ClearGrid(GridShower shower)
        {

            #region 清空药槽

            //创建一个出库记录
            AMDM_DeliveryRecord clearGridRecord = App.inventoryManager.CreateDeliveryRecord(string.Format("MBMF-CG-{0}", DateTime.Now.Ticks));
            if (clearGridRecord == null)
            {
                Utils.LogError("在药品绑定页执行清空药槽操作中,创建出库记录失败");
                return false;
            }
            //更新每一个可以发出的子弹的更新操作,变为已发出
            if (App.bindingManager.ZeroMedicineCount(shower.Grid, clearGridRecord.Id))
            {
                //记录一个出库记录的详情行
                var deliveryRecordDetail = App.inventoryManager.StartDeliveryOneMedicine(ref clearGridRecord, shower.BindedMedicine.Id,
                    shower.BindedMedicine.Name, shower.BindedMedicine.Barcode, this.currentLoadedMedicines.Count, shower.Grid.StockIndex, shower.Grid.FloorIndex, shower.Grid.IndexOfFloor);
                if (deliveryRecordDetail == null)
                {
                    Utils.LogError("在药品绑定操作中,药槽置空完成,启动出库单明细失败");
                    return false;
                }
                //把出库记录的详情行完成
                if (App.inventoryManager.EndDeliveryOneMedicine(deliveryRecordDetail, false, null) == false)
                {
                    Utils.LogError("在药品绑定操作中,药槽置空完成,完结出库单明细发生错误");
                    return false;
                }
                //完成整个出库记录
                if (
                App.inventoryManager.FinishDeliveryRecord(ref clearGridRecord, true, false, "清空药槽") == false)
                {
                    Utils.LogError("在药品绑定操作中,药槽置空完成,完结出库单发生错误");
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(this,"在药品绑定操作中,药槽清空失败,请与管理员联系");
                return false;
            }

            #endregion
        }

        private void medicinesObjectsShowerPanel_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = this.medicinesObjectsShowerPanel.ClientRectangle;

            int max = showerRef.MaxLoadableCount;

            int perObjectWidth = rect.Width / max;

            Pen borderPen = new Pen(Color.AliceBlue);
            borderPen.Width = 2;
            
            if (this.currentLoadedMedicines.Count>= max)
            {
                borderPen.Color = Color.DarkGreen;
            }
            else if(this.currentLoadedMedicines.Count == 0)
            {
                borderPen.Color = Color.DarkGray;
            }
            else
            {
                borderPen.Color = Color.DarkOrange;
            }
            Font infoFont = new Font("微软雅黑", 14);

            borderPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            for (int i = 0; i < max; i++)
            {
                Nullable<Color> objectBackColor = null;


                //计算每一个可以装载的药品实体的大小
                Rectangle currentRect = new Rectangle(i * perObjectWidth + 5, 5, perObjectWidth - 10, rect.Height - 10);
                
                //此位置装载的药品信息
                AMDM_MedicineObject_data currentLoadedMedicneObject = i < this.currentLoadedMedicines.Count ? this.currentLoadedMedicines[i] : null;

                if (currentLoadedMedicneObject != null)
                {
                    if (this.currentLoadedMedicines.Count >= max)
                    {
                        objectBackColor = Color.DarkGreen;
                    }
                    else
                    {
                        objectBackColor = Color.DarkOrange;
                    }
                }
                else
                {
                    //objectBackColor = null;
                }
                
                if (objectBackColor!= null)
                {
                    e.Graphics.FillRectangle(new SolidBrush(objectBackColor.Value), currentRect);
                }

                //绘制实体的边框
                e.Graphics.DrawRectangle(borderPen, new Rectangle(currentRect.X+1, currentRect.Y+1, currentRect.Width - 2, currentRect.Height - 2));

                Color infoTextColor = Color.DarkGray;
                string str = "未装载";
                if (currentLoadedMedicneObject!= null)
                {
                    str = string.Format("入库日期:\r\n{0}\r\n\r\n有效期至:\r\n{1}", currentLoadedMedicneObject.InStockTime.ToString("yyyy-MM-dd\r\nHH:mm:ss"), currentLoadedMedicneObject.ExpirationDate == null? "未知": currentLoadedMedicneObject.ExpirationDate.Value.ToString("yyyy年MM月dd日") );
                    infoTextColor = Color.LightGray;
                }
                //计算需要用多大的空间来渲染文字
                SizeF strNeedRect = e.Graphics.MeasureString(str, infoFont, perObjectWidth);
                float xSpan = currentRect.Width - strNeedRect.Width;
                float ySpan = currentRect.Height - strNeedRect.Height;


                e.Graphics.DrawString(str, infoFont, new SolidBrush(infoTextColor), new PointF(
                    currentRect.X + xSpan / 2, currentRect.Y + ySpan / 2
                    ));
            } 
        }
    }
}
