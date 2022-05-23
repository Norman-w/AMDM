using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridAutoLayouter
{
    public partial class BoxWidthInputForm : Form
    {
        public string InputWidth { get; set; }

        public BoxWidthInputForm(string title, int currentIndex)
        {
            InitializeComponent();
            this.label1.Text = title;
            this.Text = string.Format("当前第{0}个药品", currentIndex + 1);
        }

        private void BarcodeInputForm_Load(object sender, EventArgs e)
        {
            this.textBox1.Focus();
        }

        private void 确定按钮_Click(object sender, EventArgs e)
        {
            this.InputWidth = this.textBox1.Text.Trim();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void 取消按钮_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.确定按钮_Click(sender, e);
            }
            else if(e.KeyCode == Keys.Escape)
            {
                if (this.textBox1.Text != null && this.textBox1.Text.Length>0)
                {
                    this.textBox1.Text = "";
                }
                else
                {
                    this.取消按钮_Click(sender, e);
                }
            }
        }

        private void backSpaceBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.InputWidth) == false)
            {
                this.InputWidth = this.InputWidth.Remove(this.InputWidth.Length - 1);
                this.textBox1.Text = this.InputWidth;
                this.textBox1.Select(this.InputWidth.Length, 0);
            }
            this.textBox1.Focus();
        }

        private void btnNumber_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            this.InputWidth = string.Format("{0}{1}", this.InputWidth, btn.Text);
            this.textBox1.Text = this.InputWidth;
            this.textBox1.Select(this.InputWidth.Length, 0);
            this.textBox1.Focus();
        }
    }
}
