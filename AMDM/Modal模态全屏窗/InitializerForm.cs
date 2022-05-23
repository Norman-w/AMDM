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
    public partial class InitializerForm : Form
    {
        #region 全局变量
        BackgroundWorker bw;
        #endregion
        public InitializerForm()
        {
            InitializeComponent();            
            bw = new BackgroundWorker();
           
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Action<string> started = (msg)=>
                {
                    bw.ReportProgress(0, msg);
                };
            Action<string> finished = (msg)=>
                {
                    bw.ReportProgress(1, msg);
                };
            Action allDone = ()=>
                {
                };
            App.Init(started, finished, allDone);
        }
        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string msg = e.UserState as string;
            if (e.ProgressPercentage == 0)
            {
                this.label1.Text = msg;
                this.label1.ForeColor = Color.DarkCyan;
            }
            else if(e.ProgressPercentage == 1)
            {
                this.label1.Text = msg;
                this.label1.ForeColor = Color.DarkGreen;
            }
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            //this.label1.Text = "欢迎使用!";
            //this.label1.Font = new Font("黑体", 80, FontStyle.Bold);
            //this.label1.ForeColor = Color.ForestGreen;
            //System.Threading.Thread.Sleep(1000);
            #region 显示取药页面
            AMDM_Stock index0Stock = null;
            #region 加载给定的药仓数据
            try
            {
                index0Stock = App.stockLoader.LoadStock(0);
            }
            catch (Exception loadStockErr)
            {
                MessageBox.Show(string.Format("读取药仓信息失败!{0}", loadStockErr.Message));
                return;
            }

            if (index0Stock == null)
            {
                MessageBox.Show("读取药仓信息失败!数据读取未发生错误,读取到的结果为空");
                return;
            }
            #endregion
            MedicineDeliveryForm mform = new MedicineDeliveryForm();
            mform.WindowState = FormWindowState.Maximized;
            App.ControlPanel.GetShowingPage = mform.GetShowingPage;
            mform.StartPosition = FormStartPosition.CenterParent;
            mform.Init(new List<AMDM_Stock>() { index0Stock });
            mform.FormClosing += mform_FormClosing;
            //mform.TopMost = true;

            this.label1.Text = "";
            //正在调试的时候 直接隐藏当前窗口,显示取药页面.如果是正式环境,为了在切换视频的时候不漏出后面的桌面图标,不能隐藏
            if (App.DebugCommandServer.Setting.HidInitialzerFormOnFinishedInit)
            {
                this.Hide();
            }
            if(mform.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                this.Close();
                App.Dispose();
            }
            #endregion
        }

        void mform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (App.ControlPanel != null)
            {
                App.ControlPanel.GetShowingPage = null;
            }
        }
    }
}
