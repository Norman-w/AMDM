using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AMDM;

namespace AMDM
{
    public partial class MedicineSpicalSizeInfoInputForm : Form
    {
        public MedicineSpicalSizeInfoInputForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.label4.Text = "提示:\r\n必须准确输入尺寸信息以计算可容纳数量";
            this.label4.Font = new Font("宋体", 9);
        }


        public float DepthMM { get; set; }
        public float WidthMM { get; set; }
        public float HeightMM { get; set; }


        public bool Init(AMDM_Grid grid, AMDM_Floor floor)
        {
            
            //进深的实际可用长度 不做限制,多长的药都可以放???
            this.longMMNumericUpDown.Maximum = Convert.ToDecimal(200f);
            this.longMMNumericUpDown.Minimum = Convert.ToDecimal(5f);
            this.longMMNumericUpDown.Value = Convert.ToDecimal(50f);
            return true;
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
            this.DepthMM = Convert.ToSingle(this.longMMNumericUpDown.Value);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
