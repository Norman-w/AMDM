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
    public partial class ResetInventoryForm : Form
    {
        public ResetInventoryForm()
        {
            InitializeComponent();
        }
#region 全局变量
        /// <summary>
        /// 希望将库存设置为多少
        /// </summary>
        public int WantInventoryChangeTo { get; set; }
	#endregion
        #region 初始化
        public bool Init(int currentInventory, int maxAbleInventory)
        {
            string currentLabelText = string.Format("系统库存数量:{0}\r\n请设置实际库存数量:", currentInventory);
            this.label1.Text = currentLabelText;
            this.label2.Text = "";
            this.trackBar1.Maximum = maxAbleInventory;
            this.trackBar1.Minimum = 0;
            this.trackBar1.Value = currentInventory;
            return true;
        }
        #endregion

       

        #region 移动滑条
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            string setLabelText = string.Format("将数量设置为 {0}",  (sender as TrackBar).Value);
            this.label2.Text = setLabelText;
        }
        #endregion

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
            this.WantInventoryChangeTo = this.trackBar1.Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
