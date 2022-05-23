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
    public partial class FullScreenMedicineOrderShower : Form
    {
        public FullScreenMedicineOrderShower()
        {
            InitializeComponent();
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }
        AMDM_Domain.AMDM_MedicineOrder order = null;
        /// <summary>
        /// 设置当前显示的付药单
        /// </summary>
        public AMDM_Domain.AMDM_MedicineOrder Order
        {
            get { return this.order; }
            set
            {
                this.order = value;
                this.showOrder(value);
            }
        }

        delegate void showOrderFunc(AMDM_Domain.AMDM_MedicineOrder order);
        /// <summary>
        /// 显示给定的order
        /// </summary>
        /// <param name="order"></param>
        void showOrder(AMDM_Domain.AMDM_MedicineOrder order)
        {
            if (this.InvokeRequired)
            {
                showOrderFunc fc = new showOrderFunc(showOrder);
                this.BeginInvoke(fc, new object[] { order });
                Utils.LogInfo("药单信息显示器的showOrder方法不在主线程中调用,使用invoke");
            }
            else
            {
                this.SuspendLayout();
                #region 清空
                this.medicineOrderDetailsContent.Controls.Clear();
                ///测试模式时显示的为虚拟信息
                if (App.DebugCommandServer.DebuggerConnected == false)
                {
                    this.personNameLabel.Text = "";
                    this.sixLabel.Text = "";
                    this.ageLabel.Text = "";
                    this.departmentLabel.Text = "";
                    this.diagnositicInfoLabel.Text = "";
                    this.visitTimeLabel.Text = "";
                    this.doctorNameLabel.Text = "";
                    this.totalMedicinesCountLabel.Text = "";
                }
                #endregion
                //如果给如的order不是空的那就显示order的信息
                if (order != null)
                {
                    if (App.DebugCommandServer.DebuggerConnected == false)
                    {
                        this.personNameLabel.Text = order.PatientName;
                        this.sixLabel.Text = order.PatientSex;
                        this.ageLabel.Text = order.PatientAge.ToString();
                        this.departmentLabel.Text = order.Department;
                        this.diagnositicInfoLabel.Text = order.DiagnositicInfo;
                        this.visitTimeLabel.Text = order.VisitTime.ToString("yyyy-MM-dd HH:mm:ss");
                        this.doctorNameLabel.Text = order.DoctorName;
                    }
                    this.totalMedicinesCountLabel.Text = order.TotalCount.ToString();
                    if (order.Details != null && order.Details.Count > 0)
                    {
                        for (int i = 0; i < order.Details.Count; i++)
                        {
                            AMDM_Domain.AMDM_MedicineOrderDetail detail = order.Details[i];
                            MedicineOrderDetailShower detailShower = new MedicineOrderDetailShower();
                            detailShower.Init(false, i + 1, detail.Name, detail.Count);
                            detailShower.Width = medicineOrderDetailsContent.ClientRectangle.Width;
                            medicineOrderDetailsContent.Controls.Add(detailShower);
                        }
                    }
                }
                this.ResumeLayout(false);
            }
        }


        #region 单独的线程
        BackgroundWorker bw;
        #endregion


        /// <summary>
        /// 延迟多少秒后关闭
        /// </summary>
        public int DelayCloseMS { get; set; }

        /// <summary>
        /// 当时间到了以后是否自动销毁自己
        /// </summary>
        public bool AutoDispose { get; set; }

        /// <summary>
        /// 当付药单显示超过了指定时间以后没有动作的话触发的函数,第二个参数是 是否超时
        /// </summary>
        public Action<FullScreenMedicineOrderShower,CompletedByEnum> OnClosed { get; set; }
        /// <summary>
        /// 显示提示内容并自动关闭 自动关闭的时候会有一个回调函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="delayCloseMS"></param>
        /// <param name="onTimeoutClosed"></param>
        /// <returns></returns>
        public bool Init(AMDM_Domain.AMDM_MedicineOrder order, 
            int delayCloseMS,
            Action<FullScreenMedicineOrderShower, CompletedByEnum> onClosed, 
            bool autoDispose = false)
        {
            if (delayCloseMS < 0 || delayCloseMS > 3600000)
            {
                Utils.LogError("错误,延迟关闭对话框的延迟时间不能小于0,不能大于一小时");
                return false;
            }
            this.DelayCloseMS = delayCloseMS;
            this.OnClosed = onClosed;
            this.AutoDispose = autoDispose;
            this.Order = order;
            if (bw.IsBusy == false)
            {
                this.bw.RunWorkerAsync();
            }
            else
            {
                this.start = DateTime.Now;
                Utils.LogInfo("当前正在显示付药单,重置时间");
                return false;
            }

            //this.Show();
            return true;
        }
        public enum CompletedByEnum { 
            /// <summary>
            /// 用户在指定的时间内没有进行交互
            /// </summary>
            TimeOut,
            /// <summary>
            /// 用户在指定的时间内进行了交互
            /// </summary>
            UserSubmit, 
            /// <summary>
            /// 用户在指定的时间内,强制关闭了窗口,比如按ALT+F4
            /// </summary>
            UserClose,
            /// <summary>
            /// 用户在指定的时间内进行了交互并且该窗口需要继续保持.通常不会用到.
            /// </summary>
            KeepShow };
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CompletedByEnum type = (CompletedByEnum)e.Result;
            //如果是超时取消了 看有没有timeout的回调函数 如果有就执行
            if (type == CompletedByEnum.TimeOut)
            {
                Utils.LogFinished("FullScreenMedicineOrderShower结束了定时,需要关闭");
                if (this.OnClosed != null)
                {
                    this.OnClosed(this,type);
                    Utils.LogFinished("已经执行回调函数OnTimeoutClosed");
                }
                //超时了没点就直接关闭
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
            #region 如果标记了自动释放
            if (AutoDispose)
            {
                if (this.Parent != null)
                {
                    this.Parent.Controls.Remove(this);
                    Utils.LogSuccess("当前FullScreenMedicineOrderShower的父窗体不为空,并且当前消息显示器已经标记了要在时间结束后销毁自己,已经从父窗体中移除了自身");
                }

                try
                {
                    this.Dispose();
                }
                catch (Exception disposeErr)
                {
                    Utils.LogError("FullScreenMedicineOrderShower释放自身错误:", disposeErr.Message);
                }
                Utils.LogSuccess("已是放FullScreenMedicineOrderShower自身");
            }
            #endregion
        }
        /// <summary>
        /// 开始时间 默认是构造该控件的时候的1分钟后.1分钟过了就关闭了.
        /// </summary>
        DateTime start = DateTime.Now.AddSeconds(1);
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.LogSuccess("bw_DoWork @ FullScreenMedicineOrderShower 开始了");
            start = DateTime.Now;
            while (true)
            {
                var now = DateTime.Now;
                var span = now- start;
                if (span.TotalMilliseconds > this.DelayCloseMS)
                {
                    e.Result = CompletedByEnum.TimeOut;
                    break;
                    //break;
                }
                if (this.Disposing || this.IsDisposed)
                {
                    e.Result = CompletedByEnum.UserClose;
                    break ;
                }
                if (this.bw.CancellationPending)
                {
                    e.Result = CompletedByEnum.UserSubmit;
                    break;
                }
                var remaining = (int)Math.Floor(App.Setting.UserInterfaceSetting.MedicineOrderAutoHideWhenNoActionMS - span.TotalMilliseconds);
                updateRemaingTimeShow(remaining);
                System.Threading.Thread.Sleep(117);
            }
            Utils.LogFinished("bw_DoWork @ FullScreenMedicineOrderShower 结束了", e.Result);
        }

        private void startMedicineGettingBtn_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                click_Func fc = new click_Func(startMedicineGettingBtn_Click);
                this.BeginInvoke(fc, sender, e);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bw.CancelAsync();
            this.Close();
        }

        delegate void setRemaingTime_Func(int remaningMS);
        setRemaingTime_Func fc = null;
        void updateRemaingTimeShow(int remaningMS)
        {
            if (this.startMedicineGettingBtn.InvokeRequired)
            {
                if (fc == null)
                {
                    fc = new setRemaingTime_Func(updateRemaingTimeShow);
                }
                if (this.Created)
                {
                    this.startMedicineGettingBtn.BeginInvoke(fc, remaningMS);
                }
                else
                {

                }
                return;
            }
            this.startMedicineGettingBtn.Text = string.Format("确认取药\r\n({0})", remaningMS / 1000);
        }
        #region 如果使用了自动取药测试控制器的话  显示以后间隔固定时间后 直接点击开始取药
        delegate void click_Func(object sender, EventArgs e);
        private void FullScreenMedicineOrderShower_Load(object sender, EventArgs e)
        {
            if (App.AutoMedicinesGettingTester != null
                //&&App.AutoMedicinesGettingTester.Working
                 && App.DebugCommandServer != null && App.DebugCommandServer.DebuggerConnected && App.DebugCommandServer.Setting.AutoClickStartMedicineGettingBtn
                )
            {
                System.Threading.ThreadPool.QueueUserWorkItem(
                    (res)=>
                    {
                        System.Threading.Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(1000, 5000 ));
                        this.startMedicineGettingBtn_Click(null, null);
                    }
                    );
                
            }
        }
        #endregion

        private void cancelMedicineGettingBtn_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                click_Func fc = new click_Func(cancelMedicineGettingBtn_Click);
                this.BeginInvoke(fc, sender, e);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }        
    }
}
