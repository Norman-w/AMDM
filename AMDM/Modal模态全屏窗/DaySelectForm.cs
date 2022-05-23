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
    public partial class DaySelectForm : Form
    {
        //public Action<int?> OnSelectedMonth;
        DateTime now = DateTime.Now;
        /// <summary>
        /// 选中的年份是哪一年,如果没选中 就是null
        /// </summary>
        public Nullable<int> SelectDay { get; set; }
        public Nullable<int> BaseYear{get;set;}
        public Nullable<int> BaseMonth { get; set; }
        public DaySelectForm(Nullable<int> year, Nullable<int> month)
        {
            InitializeComponent();
            this.BaseYear = year;
            this.BaseMonth = month;
        }


        private void YearsSelectForm_Deactivate(object sender, EventArgs e)
        {
        }

        private void b1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int day = Convert.ToInt32(btn.Text);
            this.SelectDay = day;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            //if (this.OnSelectedMonth!= null)
            //{
            //    this.OnSelectedMonth(this.SelectedMonth);
            //}
        }

        private void MonthSelectForm_Load(object sender, EventArgs e)
        {
            #region 如果没有月,先出来月的选择框
            if (this.BaseMonth == null)
            {
                MonthSelectForm mform = new MonthSelectForm(this.BaseYear);
                var ret = mform.ShowDialog(this);
                if (ret == System.Windows.Forms.DialogResult.Cancel)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    return;
                }
                else
                {
                    this.BaseMonth = mform.SelectedMonth;
                    this.BaseYear = mform.BaseYear;
                }
            }
            #endregion
            if (this.BaseMonth == null || this.BaseYear == null)
            {
                return;
            }
            int year = this.BaseYear.Value;
            int month = this.BaseMonth.Value;
            DateTime nowMon = new DateTime(now.Year, now.Month, now.Day);
            DateTime after3yearsDay = nowMon.AddYears(3);

            #region 添加这个月份中的所有天的按钮
            int dayCount = DateTime.DaysInMonth(year, month);
            int rowCount = dayCount > 30 ? 4 : 3;
            int currentDay = 1;
            int perLineHeight = this.panel2.Height / rowCount;
            int perColWidth = this.panel2.Width / 10;
            for (int i = 0; i < rowCount; i++)
            {
                int lineY = perLineHeight * i;
                for (int j = 0; j < 10; j++)
                {
                    #region 要不要让他显示出来 超过当前日期的或者是早于当前日期3年之前的不显示
                    DateTime thisTime = new DateTime(year, month, currentDay);
                    #endregion
                    int colX = perColWidth * j;
                    Point pos = new Point(colX + 5, lineY + 5);
                    Button btn = new Button();
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Tag = currentDay;
                    btn.Text = currentDay.ToString();
                    btn.Location = pos;
                    btn.Size = new System.Drawing.Size(perColWidth - 10, perLineHeight - 10);
                    btn.Click += this.b1_Click;
                    btn.Font = new System.Drawing.Font("微软雅黑", 30);
                    btn.Enabled = thisTime >= nowMon && thisTime <= after3yearsDay;
                    this.panel2.Controls.Add(btn);
                    currentDay++;
                    if (currentDay > dayCount)
                    {
                        break;
                    }
                }
            }
            #endregion
        }

        private void MonthSelectForm_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
