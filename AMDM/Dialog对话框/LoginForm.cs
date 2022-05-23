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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        public string User { get; set; }
        public string Pass { get; set; }


        private void submitBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(userTextBox.Text) || string.IsNullOrEmpty(passTextBox.Text))
            {
                MessageBox.Show(this,"请输入用户名和密码", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.User = this.userTextBox.Text.Trim();
            this.Pass = this.passTextBox.Text.Trim();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void userTextBox_Enter(object sender, EventArgs e)
        {
            VisualKeyboard kb = new VisualKeyboard();
            kb.Init("请输入用户名:", this.userTextBox.Text, false);
             var ret = kb.ShowDialog();
             if (ret == System.Windows.Forms.DialogResult.Ignore)
             {
                 this.userTextBox.Text = kb.InputValue;
                 this.passTextBox_Enter(sender, e);
             }
             else if (ret == System.Windows.Forms.DialogResult.OK)
             {
                 this.userTextBox.Text = kb.InputValue;
                 this.passTextBox_Enter(sender, e);
             }
             else if (ret == System.Windows.Forms.DialogResult.Cancel)
             {
                 //
             }
        }

        private void passTextBox_Enter(object sender, EventArgs e)
        {
            VisualKeyboard kb = new VisualKeyboard();
            kb.Init("请输入密码:", this.passTextBox.Text, true);
            var ret = kb.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                this.passTextBox.Text = kb.InputValue;
                submitBtn_Click(sender, e);
            }
        }

        private void userTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void passTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
