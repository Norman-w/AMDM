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
    public partial class MedicineCountForm : Form
    {
        public MedicineCountForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }
        #region 外部属性级全局变量
        /// <summary>
        /// 选择了想要增加多少盒药品
        /// </summary>
        public int WantAddCount { get; set; }
        #endregion
        #region 全局变量
        GridShower gridShowerRef = null;
        #endregion
        #region 初始化
        public bool Init(GridShower shower)
        {
            int padding = 5;
            int perButtonHeight = (int)Math.Floor(1f/shower.MaxLoadableCount * this.countContainerPanel.ClientRectangle.Height);
            int addCountBtnIndex = 0;
            //在panel中加入多个按钮,提示放多少个
            for (int i = 0; i < shower.MaxLoadableCount; i++)
            {
                Button button = new Button();
                button.Click += button_Click;
                button.Location = new Point(
                    this.countContainerPanel.ClientRectangle.Left+padding,
                    this.countContainerPanel.ClientRectangle.Height- (i+1)*(perButtonHeight) + (int)Math.Round(padding*1f)
                    );
                button.Size = new Size(
                    this.countContainerPanel.ClientRectangle.Width-2*padding ,
                    perButtonHeight- 2*padding
                    );
                if ((i+1)> shower.CurrentLoadedCount)
                {
                    addCountBtnIndex++;
                    button.Enabled = true;
                    button.Text = string.Format("添加 {0} 盒", addCountBtnIndex);
                    button.Tag = addCountBtnIndex;
                    if (i == shower.MaxLoadableCount -1)
                    {
                        button.Text = string.Format("添加{0}盒(加满)", addCountBtnIndex);
                    }
                }
                else
                {
                    button.Enabled = false;
                    button.Text = "已有药品";
                }
                this.countContainerPanel.Controls.Add(button);
            }
            return true;
        }

        #region 当点击了指定的数量以后
        void button_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Button currentClickedBtn = sender as Button;
            int wantAddCount = currentClickedBtn.Tag == null ? 0 : (int)currentClickedBtn.Tag;
            this.WantAddCount = wantAddCount;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        #endregion
        
        #endregion

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        #region 关闭时,解绑所有的回调函数
        private void MedicineCountForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var item in this.countContainerPanel.Controls)
            {
                Button btn = item as Button;
                btn.Click -= this.button_Click;
            }
        }
        #endregion
        
    }
}
