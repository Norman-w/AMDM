using AMDM.Forms;
using MyCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMDM
{
    public partial class ExpirationDataSelectForm : Form
    {
        void Speak(string text)
        {
            try
            {
                App.TTSSpeaker.Speak(text);
            }
            catch (Exception err)
            {
                Utils.LogError("在有效期选择页面中读TTS文本错误:", text, err.Message);
            }            
        }

        public DateTime SelectedDateTime { get; set; }
        public ExpirationDataSelectForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            //this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
            //this.dateTimePicker1.CustomFormat = "yyyy年MM月dd日";
        }
        Nullable<int> selectedYear = null;
        Nullable<int> selectedMonth = null;
        Nullable<int> selectedDay = null;
        private void submitBtn_Click(object sender, EventArgs e)
        {
            if (this.selectedYear == null || this.selectedMonth == null)
            {
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //看日期有没有选,如果没有选 默认就是目标年月中的1号
            this.SelectedDateTime = new DateTime(this.selectedYear.Value, this.selectedMonth.Value, this.selectedDay  == null? 1: this.selectedDay.Value);
            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            if (selectedYear == null && selectedMonth == null && App.Setting.ExpirationStrictControlSetting.Enable)
            {
                MessageBox.Show(this, "当前已开启严控药品有效期模式\r\n请至少选择有效的年份和月份", "严控药品有效期模式");
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void yearLabel_Click(object sender, EventArgs e)
        {
            YearsSelectForm yform = new YearsSelectForm();
            yform.WindowState = FormWindowState.Maximized;
            FormAutoSizer a = new FormAutoSizer(yform);
            a.TurnOnAutoSize();
            Speak("选择有效期的年份");
            if (yform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.selectedYear = yform.SelectedYear;
                fixDay();
                this.yearLabel.Text = string.Format("{0}年", yform.SelectedYear);
            }
        }

        private void monthLabel_Click(object sender, EventArgs e)
        {
            MonthSelectForm mform = new MonthSelectForm(this.selectedYear);
            mform.WindowState = FormWindowState.Maximized;
            FormAutoSizer a = new FormAutoSizer(mform);
            a.TurnOnAutoSize();
            Speak("选择有效期的月份");
            if (mform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.selectedMonth = mform.SelectedMonth;
                this.selectedYear = mform.BaseYear;
                fixDay();
                this.monthLabel.Text = string.Format("{0}月", mform.SelectedMonth);
                this.yearLabel.Text = string.Format("{0}年", mform.BaseYear);
                
            }
        }
        void fixDay()
        {
            //如果重新选择了有效期的月份但是已经选择了有效期的日的话,可能当月的最后一天不是本月的最后一天.
            //比如已经选择了3月的31日,但是后来改成了4月,4月只有30天.所以要强行的把日改成本月的最后一天
            #region 修正日期
            if (this.selectedYear != null && this.selectedMonth != null && this.selectedDay != null)
            {
                //检查这个月的最后一天是什么时候
                int days = DateTime.DaysInMonth(this.selectedYear.Value, this.selectedMonth.Value);
                if (this.selectedDay.Value > days)
                {
                    this.selectedDay = days;
                    this.dayLabel.Text = string.Format("{0}日", this.selectedDay.Value);
                }
            }
            #endregion
        }

        private void dayLabel_Click(object sender, EventArgs e)
        {
            DaySelectForm dForm = new DaySelectForm(this.selectedYear, this.selectedMonth);
            dForm.WindowState = FormWindowState.Maximized;
            FormAutoSizer a = new FormAutoSizer(dForm);
            a.TurnOnAutoSize();
            Speak("选择有效期(日期)");
            if (dForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.selectedDay = dForm.SelectDay;
                this.selectedMonth = dForm.BaseMonth;
                this.selectedYear = dForm.BaseYear;
                this.dayLabel.Text = string.Format("{0}日", dForm.SelectDay);
                this.monthLabel.Text = string.Format("{0}月", dForm.BaseMonth);
                this.yearLabel.Text = string.Format("{0}年", dForm.BaseYear);
            }
        }
    }
}
