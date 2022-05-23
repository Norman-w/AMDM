using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMDM.Forms
{
    public partial class YearsSelectForm : Form
    {
        DateTime now = DateTime.Now;
        /// <summary>
        /// 选中的年份是哪一年,如果没选中 就是null
        /// </summary>
        public Nullable<int> SelectedYear { get; set; }
        public YearsSelectForm()
        {
            InitializeComponent();
            //this.b1.Tag = now.AddYears(-1).Year;
            this.b2.Tag = now.AddYears(0).Year;
            this.b3.Tag = now.AddYears(1).Year;
            this.b4.Tag = now.AddYears(2).Year;

            //this.b1.Text = string.Format("{0}年", b1.Tag);
            this.b2.Text = string.Format("{0}年", b2.Tag);
            this.b3.Text = string.Format("{0}年", b3.Tag);
            this.b4.Text = string.Format("{0}年", b4.Tag);
        }

        private void YearsSelectForm_Deactivate(object sender, EventArgs e)
        {
            //if (this.SelectedYear == null)
            //{
            //    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //    this.Close();
            //}
        }

        //public Action<Nullable<int>> OnSelectedYear;
      

        private void b1_Click(object sender, EventArgs e)
        {
            this.SelectedYear = (int)((Button)sender).Tag;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            //if (this.OnSelectedYear!= null)
            //{
            //    this.OnSelectedYear(this.SelectedYear);
            //}
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void YearsSelectForm_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
