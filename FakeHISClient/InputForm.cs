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
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        public string InputValue { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            this.InputValue = this.textBox1.Text;
            if (this.InputValue == null || this.InputValue.Length<10)
            {
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
