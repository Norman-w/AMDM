using AMDM_Domain;
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
    public partial class MedicineBoxInfoInputForm : Form
    {
        public MedicineBoxInfoInputForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.label4.Text = "提示:\r\n如实际药盒尺寸不在此页面可设定的范围内\r\n即表明该药槽无法放置此药品或药槽参数设定有误!";
            this.label4.Font = new Font("宋体", 9);
        }


        public float DepthMM { get; set; }
        public float WidthMM { get; set; }
        public float HeightMM { get; set; }


        public bool Init(AMDM_Grid grid, AMDM_Floor floor)
        {
            float gridContentWidth = grid.RightMM - grid.LeftMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM;
            //药槽内可放的药品 最宽不能超过药盒+3毫米 最窄不能小于药槽-8毫米
            this.widthMMNumericUpDown.Maximum = Convert.ToDecimal(gridContentWidth - App.Setting.HardwareSetting.Grid.MinGridPaddingWidthMM);
            this.widthMMNumericUpDown.Minimum = Convert.ToDecimal(gridContentWidth - App.Setting.HardwareSetting.Grid.MaxGridPaddingWidthMM);
            this.widthMMNumericUpDown.Value = this.widthMMNumericUpDown.Minimum;

            ///层的可用实际高度为  层的上下间隙 减去上面一层的连接件的高度,减去上面一层的层板的高度.
            float floorContentHeight = floor.TopMM - floor.BottomMM - App.Setting.HardwareSetting.Stock.FloorFixingsHeightMM - App.Setting.HardwareSetting.Floor.FloorPanelHeightMM;
            this.heithtMMNumericUpDown.Maximum = Convert.ToDecimal(floorContentHeight);
            this.heithtMMNumericUpDown.Minimum = Convert.ToDecimal(App.Setting.HardwareSetting.Stock.MinPerMedicineHeightMM);
            this.heithtMMNumericUpDown.Value = this.heithtMMNumericUpDown.Minimum;

            //进深的实际可用长度 不做限制,多长的药都可以放???
            this.longMMNumericUpDown.Maximum = Convert.ToDecimal(App.Setting.HardwareSetting.Stock.MaxPerMedicineDepthMM);
            this.longMMNumericUpDown.Minimum = Convert.ToDecimal(App.Setting.HardwareSetting.Stock.MinPerMedicineDepthMM);
            this.longMMNumericUpDown.Value = this.longMMNumericUpDown.Minimum;
            return true;
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
            this.DepthMM = Convert.ToSingle(this.longMMNumericUpDown.Value);
            this.WidthMM =Convert.ToSingle( this.widthMMNumericUpDown.Value);
            this.HeightMM =Convert.ToSingle( this.heithtMMNumericUpDown.Value);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
