using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FakeHISClient
{
    public partial class StockBasicInfoEditForm : Form
    {
        public StockBasicInfoEditForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        #region 初始化
        public bool Init(AMDM_Stock stock)
        {
            this.stockIdLabel.Text = stock.Id.ToString();
            this.stockSNLabel.Text = stock.SerialNumber;

            this.inndexOfMachineTextbox.Text = stock.IndexOfMachine.ToString();
            this.maxFloorWidthMMTextbox.Text = stock.MaxFloorWidthMM.ToString();
            this.xOffsetFromStartPointMMTextbox.Text = stock.XOffsetFromStartPointMM.ToString();
            this.yOffsetFromStartPointMMTextbox.Text = stock.YOffsetFromStartPointMM.ToString();
            this.centerDistanceBetweenTwoGrabbersTextbox.Text = stock.CenterDistanceBetweenTwoGrabbers.ToString();

            return false;
        }
        #endregion

        #region 返回值
        public int IndexOfMachine { get; set; }
        public float MaxFloorWidthMM { get; set; }
        public float XOffsetFromStartPointMM { get; set; }
        public float YOffsetFromStartPointMM { get; set; }
        public float CenterDistanceBetweenTwoGrabbers { get; set; }

        public bool NeedDeleteStock { get; set; }
        #endregion

        private void saveBtn_Click(object sender, EventArgs e)
        {
            int index = 0;
            float max = 0, xoffset = 0, yoffset = 0, grabbersWidth = 0;
            if (
                int.TryParse(this.inndexOfMachineTextbox.Text, out index) == false
                || float.TryParse(this.maxFloorWidthMMTextbox.Text, out max) == false
                || float.TryParse(this.xOffsetFromStartPointMMTextbox.Text, out xoffset) == false
                || float.TryParse(this.yOffsetFromStartPointMMTextbox.Text, out yoffset) == false
                || float.TryParse(this.centerDistanceBetweenTwoGrabbersTextbox.Text, out grabbersWidth) == false
                )
            {
                MessageBox.Show("参数有误请重新检查");
                return;
            }

            this.MaxFloorWidthMM = max;
            this.IndexOfMachine = index;
            this.XOffsetFromStartPointMM = xoffset;
            this.YOffsetFromStartPointMM = yoffset;
            this.CenterDistanceBetweenTwoGrabbers = grabbersWidth;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void removeThisStockBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("此操作不可恢复,仍要删除此药库吗?", "请再次确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,  MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
    }
}
