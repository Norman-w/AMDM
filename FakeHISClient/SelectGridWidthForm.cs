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
    public partial class SelectGridWidthForm : Form
    {
        public float SelectedWidthMM { get; set; }
        /// <summary>
        /// 对于一个格子来说,他的左边挡板和右边挡板都取一半再加上格子的内径才是他的实际记录宽度.
        /// </summary>
        float totalWallWidthMM = 0;
        float customWidthMM = 0;
        float widthMMPerStep = 0;
        float minGridWidthMM = 0;
        float maxGridWidthMM = 0;
        public SelectGridWidthForm()
        {
            InitializeComponent();
            #region 重新计算宽度,按照窗体总宽度是最宽的目前是95毫米来做为单位,重新调整按钮的大小
            float basicWidth = this.width95MMBtn.Width;
            this.width85MMBtn.Width = (int)Math.Floor(85f / 95 * basicWidth);
            this.width75MMBtn.Width = (int)Math.Floor(75f / 95 * basicWidth);
            this.width65MMBtn.Width = (int)Math.Floor(65f / 95 * basicWidth);

            #endregion
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gridWallWidth">隔档的宽度</param>
        /// <param name="defaultCustomGridWidth">要显示在页面上的自定义宽度的值</param>
        /// <param name="widthMMPerStep">每次点击加减按钮的时候 值增加或者减少多少</param>
        /// <returns></returns>
        public bool Init(float gridWallWidth, float defaultCustomGridWidth, float widthMMPerStep, float minGridWidthMM, float maxGridWidthMM)
        {
            this.totalWallWidthMM = gridWallWidth;
            this.customWidthMM = defaultCustomGridWidth;
            this.widthMMPerStep = widthMMPerStep;
            this.minGridWidthMM = minGridWidthMM;
            this.maxGridWidthMM = maxGridWidthMM;

            this.customSizeBtn.Text = string.Format("设为  {0}  毫米", this.customWidthMM - this.totalWallWidthMM);
            return true;
        }

        private void width95MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedWidthMM = 95 + totalWallWidthMM;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void width85MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedWidthMM = 85 + totalWallWidthMM;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void width75MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedWidthMM = 75 + totalWallWidthMM;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void width65MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedWidthMM = 65 + totalWallWidthMM;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void minus5MMBtn_Click(object sender, EventArgs e)
        {
            float destWidth = this.customWidthMM - widthMMPerStep;
            if (destWidth < this.minGridWidthMM)
            {
                MessageBox.Show(string.Format("药槽内径最小宽度{0}毫米", this.minGridWidthMM - this.totalWallWidthMM));
                return;
            }
            this.customWidthMM = destWidth;
            this.customSizeBtn.Text = string.Format("设为{0}毫米", this.customWidthMM - this.totalWallWidthMM);
        }

        private void add5MMBtn_Click(object sender, EventArgs e)
        {
            float destWidth = this.customWidthMM + widthMMPerStep;
            if (destWidth> this.maxGridWidthMM)
            {
                MessageBox.Show(string.Format("药槽内径最大宽度{0}毫米",this.maxGridWidthMM - this.totalWallWidthMM));
                return;
            }
            this.customWidthMM = destWidth;
            this.customSizeBtn.Text = string.Format("设为  {0}  毫米", this.customWidthMM - this.totalWallWidthMM);
        }

        private void customSizeBtn_Click(object sender, EventArgs e)
        {
            this.SelectedWidthMM = customWidthMM;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
