using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMDM
{
    public partial class VisualKeyboard : Form
    {
        public VisualKeyboard()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="title"></param>
        /// <param name="isPasswordMode"></param>
        /// <returns></returns>
        public bool Init(string title,string defaultValue, bool isPasswordMode)
        {
            this.titleLabel.Text = title;
            this.textBox1.Text = defaultValue;
            if (isPasswordMode)
            {
                this.textBox1.PasswordChar = '*';
            }
            this.ActiveControl = this.textBox1;
            this.textBox1.Focus();
            return true;
        }

        /// <summary>
        /// 用户输入了的内容
        /// </summary>
        public string InputValue { get; set; }

        bool isShiftMode = false;
        bool isUpperMode = false;
        private void leftShift_Click(object sender, EventArgs e)
        {
            isShiftMode = !isShiftMode;
            leftShift.BackColor = isShiftMode ? Color.DarkSeaGreen : Color.LightGray;
            rightShift.BackColor = leftShift.BackColor;
            refreshKeyNames();
        }
        void refreshKeyNames()
        {
            //点击左边的shift的时候 跟右边的一样  判断现在是shift模式吗 如果是shift模式 换回小写如果是非shift模式,切换回大写
            for (int i = 0; i < this.Controls.Count; i++)
            {
                var current = this.Controls[i];
                if (current.Text.Length == 1)
                {
                    char cc = current.Text[0];
                    if ((cc >= 'a' && cc <= 'z') || (cc >= 'A' || cc <= 'Z'))
                    {
                        current.Text = (isShiftMode || isUpperMode) ? current.Text.ToUpper() : current.Text.ToLower();

                    }
                }
            }

            l1.Text = isShiftMode ? "~" : "`";
            n1.Text = isShiftMode ? "!" : "1";
            n2.Text = isShiftMode ? "@" : "2";
            n3.Text = isShiftMode ? "#" : "3";
            n4.Text = isShiftMode ? "$" : "4";
            n5.Text = isShiftMode ? "%" : "5";
            n6.Text = isShiftMode ? "^" : "6";
            n7.Text = isShiftMode ? @"&" : "7";
            n8.Text = isShiftMode ? "*" : "8";
            n9.Text = isShiftMode ? "(" : "9";
            n0.Text = isShiftMode ? ")" : "0";
            minuse.Text = isShiftMode ? "_" : "-";
            equal.Text = isShiftMode ? "+" : "=";
            leftRect.Text = isShiftMode ? "{" : "[";
            rightRect.Text = isShiftMode ? "}" : "]";
            rightIn.Text = isShiftMode ? "|" : "\\";
            sentence.Text = isShiftMode ? ":" : ";";
            quote.Text = isShiftMode ? "\"" : "'";
            douhao.Text = isShiftMode ? "<" : ",";
            juhao.Text = isShiftMode ? ">" : ".";
            leftIn.Text = isShiftMode ? "?" : "/";

            }

        private void caplock_Click(object sender, EventArgs e)
        {
            isUpperMode = !isUpperMode;
            refreshKeyNames();
        }

        private void rightShift_Click(object sender, EventArgs e)
        {
            isShiftMode = !isShiftMode;
            refreshKeyNames();
        }

        private void largeEnter_Click(object sender, EventArgs e)
        {
            this.InputValue = this.textBox1.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void esc_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void tab_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Close();
        }

        private void anyKey_Click(object sender, EventArgs e)
        {
            Control c = sender as Control;
            string content = c.Text;
            this.textBox1.AppendText(content);
            isShiftMode = false;
            refreshKeyNames();
            this.ActiveControl = this.textBox1;
            this.textBox1.Focus();
            this.textBox1.HideSelection = false;
            this.textBox1.SelectionLength = 0;
        }

        private void space_Click(object sender, EventArgs e)
        {
            this.textBox1.AppendText(" ");
        }

        private void backspace_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text!= null && this.textBox1.Text.Length>0)
            {
                this.textBox1.Text = this.textBox1.Text.Substring(0, this.textBox1.Text.Length - 1);
            }
        }
        //string allAbleKeys = "~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?`1234567890-=qwertyuiop[]\\asdfghjkl;'zxcvbnm,./";
        private void VisualKeyboard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Escape)
            {
                this.esc_Click(sender, e);
            }
            //else if (e.KeyChar == (int)Keys.Back)
            //{
            //    this.backspace_Click(sender, e);
            //}
            else if (e.KeyChar == (int)Keys.Enter)
            {
                this.largeEnter_Click(sender, e);
            }
            else if (e.KeyChar == (int)Keys.Tab)
            {
                this.tab_Click(sender, e);
            }
            //else if (e.KeyChar == (int)Keys.Space)
            //{
            //    this.space_Click(sender, e);
            //}
            //else if (allAbleKeys.Contains(e.KeyChar))
            //{
            //    this.textBox1.AppendText(e.KeyChar.ToString());
            //}
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                this.largeEnter_Click(sender, e);
            }
            else if(e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                this.tab_Click(sender, e);
            }
            else if(e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.esc_Click(sender, e);
            }
        }
    }
}
