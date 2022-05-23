using AMDM;
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
    public partial class BarcodeInputForm : Form
    {
        public string Barcode { get; set; }


        Action<string> oldOnReciveBarcodeFunction = null;
        public BarcodeInputForm(string title)
        {
            InitializeComponent();
            this.label1.Text = title;
            oldOnReciveBarcodeFunction = App.ICCardReaderAndCodeScanner2in1ReceivedData;
            App.ICCardReaderAndCodeScanner2in1ReceivedData = this.onReciveBarcode;
        }

        void onReciveBarcode(string barcode)
        {
            App.ICCardReaderAndCodeScanner2in1ReceivedData = this.oldOnReciveBarcodeFunction;
            this.Barcode = barcode;
            close();
        }
        delegate void closeFunc();
        void close()
        {
            if (this.InvokeRequired)
            {
                closeFunc cf = new closeFunc(close);
                this.BeginInvoke(cf);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void BarcodeInputForm_Load(object sender, EventArgs e)
        {
            this.textBox1.Focus();
        }

        private void 确定按钮_Click(object sender, EventArgs e)
        {
            this.Barcode = this.textBox1.Text.Trim();
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
    }
}
