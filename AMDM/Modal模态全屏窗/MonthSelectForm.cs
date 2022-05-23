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
    public partial class MonthSelectForm : Form
    {
        //public Action<int?> OnSelectedMonth;
        DateTime now = DateTime.Now;
        /// <summary>
        /// 选中的年份是哪一年,如果没选中 就是null
        /// </summary>
        public Nullable<int> SelectedMonth { get; set; }
        public Nullable<int> BaseYear { get; set; }
        public MonthSelectForm(Nullable<int> year)
        {
            InitializeComponent();
            this.BaseYear = year;
            
        }

        private void YearsSelectForm_Deactivate(object sender, EventArgs e)
        {
        }

        private void b1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int month = Convert.ToInt32(btn.Text.Replace("月", ""));
            this.SelectedMonth = month;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            //if (this.OnSelectedMonth!= null)
            //{
            //    this.OnSelectedMonth(this.SelectedMonth);
            //}
        }

        private void MonthSelectForm_Load(object sender, EventArgs e)
        {
            if (this.BaseYear == null)
            {
                YearsSelectForm yform = new YearsSelectForm();
                var ret = yform.ShowDialog(this);
                if (ret == System.Windows.Forms.DialogResult.Cancel)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    return;
                }
                else
                {
                    this.BaseYear = yform.SelectedYear;
                }
            }
            if (this.BaseYear == null)
            {
                return;
            }
            int year = this.BaseYear.Value;
            DateTime nowMon = new DateTime(now.Year, now.Month, 1);
            DateTime after3yearsMon = nowMon.AddYears(3);
            foreach (Control item in this.Controls)
            {
                if (item is Button == false)
                {
                    continue;
                }
                int currentMon = Convert.ToInt32(item.Text.Replace("月", ""));
                DateTime current = new DateTime(year, currentMon, 1);
                if (current < nowMon || current > after3yearsMon)
                {
                    //大于或者小于的都让他不显示
                    item.Enabled = false;
                }
            }
        }

        private void MonthSelectForm_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
