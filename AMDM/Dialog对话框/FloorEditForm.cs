using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AMDM
{
    public partial class FloorEditForm : Form
    {
        public FloorEditForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        #region 初始化
        public bool Init(AMDM_Floor floor)
        {
            if (floor == null)
            {
                return false;
            }
            this.stockIdLabel.Text = floor.StockId.ToString();
            this.indexOfStockLabel.Text = floor.IndexOfStock.ToString();
            this.floorIdLabel.Text = floor.Id.ToString();
            this.widthMMTextbox.Text = floor.WidthMM.ToString();
            this.topMMTextbox.Text = floor.TopMM.ToString();
            this.bottomMMTextbox.Text = floor.BottomMM.ToString();

            this.WidthMM = floor.WidthMM;
            this.TopMM = floor.TopMM;
            this.BottomMM = floor.BottomMM;
            return true;
        }
        #endregion
        public float WidthMM { get; set; }
        public float TopMM { get; set; }
        public float BottomMM { get; set; }
        /// <summary>
        /// 返回操作之后 是否需要删除层
        /// </summary>
        public bool NeedDeleteFloor { get; set; }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            float width = 0, top = 0, bottom = 0;
            if (float.TryParse(this.widthMMTextbox.Text, out width) == false
                || float.TryParse(this.topMMTextbox.Text, out top) == false
                || float.TryParse(this.bottomMMTextbox.Text, out bottom) == false
                )
            {
                MessageBox.Show("输入内容不正确");
            }
            this.WidthMM = width;
            this.TopMM = top;
            this.BottomMM = bottom;
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void removeThisFloorBtn_Click(object sender, EventArgs e)
        {
            this.NeedDeleteFloor = true;
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }
    }
}
