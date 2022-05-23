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
    public partial class NumberInputForm : Form
    {
        public double InputValue { get; set; }
        bool enableDot = false;
        public NumberInputForm(string title, string formName, double defaultValue = 0, bool enableDot = false)
        {
            InitializeComponent();
            this.label1.Text = title;
            this.textBox1.Text = defaultValue.ToString();
            this.Text = formName;
            this.enableDot = enableDot;
        }

        private void BarcodeInputForm_Load(object sender, EventArgs e)
        {
            this.textBox1.Focus();
        }

        private void 确定按钮_Click(object sender, EventArgs e)
        {
            string input = this.textBox1.Text.Trim();
            if (this.enableDot)
            {
                if (isFloatDigitString(input) == false)
                {
                    MessageBox.Show(this, "请输入有效的内容");
                    return;
                }
            }
            else
            {
                if (isDigitString(input) == false)
                {
                    MessageBox.Show(this, "请输入有效的数字");
                    return;
                }
            }
            this.InputValue = Convert.ToDouble(input);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        bool isDigitString(string str)
        {
            if (str == null || str.Length < 1)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if ("0123456789".IndexOf(c) < 0)
                {
                    return false;
                }
            }
            return true;
        }
        bool isFloatDigitString(string str)
        {
            if (str == null || str.Length < 1)
            {
                return false;
            }
            if (str.Trim().StartsWith("."))
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if ("0123456789.".IndexOf(c) < 0)
                {
                    return false;
                }
            }
            return true;
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
    }
}
