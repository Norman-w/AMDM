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
    public partial class SelectFloorHeightForm : Form
    {
        public float SelectedHeightMM { get; set; }
        float customHeightMM = 0;
        float heightMMPerStep = 0;
        float minFloorHeightMM = 0;
        float maxFloorHeightMM = 0;
        public SelectFloorHeightForm()
        {
            InitializeComponent();
            #region 重新计算
            float basicHeight = this.width95MMBtn.Height;
            this.width85MMBtn.Height = (int)Math.Floor(100f / 120 * basicHeight);
            this.width75MMBtn.Height = (int)Math.Floor(80f / 120 * basicHeight);
            this.width65MMBtn.Height = (int)Math.Floor(60f / 120 * basicHeight);

            #endregion
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gridWallWidth">隔档的宽度</param>
        /// <param name="defaultCustomFloorHeight">要显示在页面上的自定义宽度的值</param>
        /// <param name="heightMMPerStep">每次点击加减按钮的时候 值增加或者减少多少</param>
        /// <returns></returns>
        public bool Init(float defaultCustomFloorHeight, float heightMMPerStep, float minFloorHeightMM, float maxFloorHeightMM)
        {
            this.customHeightMM = defaultCustomFloorHeight;
            this.heightMMPerStep = heightMMPerStep;
            this.minFloorHeightMM = minFloorHeightMM;
            this.maxFloorHeightMM = maxFloorHeightMM;

            this.customSizeBtn.Text = string.Format("设为{0}毫米", this.customHeightMM);
            return true;
        }

        private void height120MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedHeightMM = 120;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void height100MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedHeightMM = 100;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void height80MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedHeightMM = 80;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void height60MMBtn_Click(object sender, EventArgs e)
        {
            this.SelectedHeightMM = 60;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void minus5MMBtn_Click(object sender, EventArgs e)
        {
            float destWidth = this.customHeightMM - heightMMPerStep;
            if (destWidth < this.minFloorHeightMM)
            {
                MessageBox.Show(string.Format("层最小高度{0}毫米", this.minFloorHeightMM));
                return;
            }
            this.customHeightMM = destWidth;
            this.customSizeBtn.Text = string.Format("设为{0}毫米", this.customHeightMM);
        }

        private void add5MMBtn_Click(object sender, EventArgs e)
        {
            float destWidth = this.customHeightMM + heightMMPerStep;
            if (destWidth> this.maxFloorHeightMM)
            {
                MessageBox.Show(string.Format("层最大高度{0}毫米",this.maxFloorHeightMM));
                return;
            }
            this.customHeightMM = destWidth;
            this.customSizeBtn.Text = string.Format("设为{0}毫米", this.customHeightMM);
        }

        private void customSizeBtn_Click(object sender, EventArgs e)
        {
            this.SelectedHeightMM = customHeightMM;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void SelectFloorHeightForm_Load(object sender, EventArgs e)
        {

        }
    }
}
