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
    public partial class FullScreenNoticeShower : Form
    {
        #region 单独的线程
        BackgroundWorker bw;
        #endregion
        public FullScreenNoticeShower()
        {
            InitializeComponent();
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }
        /// <summary>
        /// 显示的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 显示的内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 延迟多少秒后关闭
        /// </summary>
        public int DelayCloseMS { get; set; }

        /// <summary>
        /// 当时间到了以后是否自动销毁自己
        /// </summary>
        public bool AutoDispose { get; set; }

        //public Action OnShown { get; set; }

        public Action<FullScreenNoticeShower> OnTimeoutClosed { get; set; }
        /// <summary>
        /// 显示提示内容并自动关闭 自动关闭的时候会有一个回调函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="delayCloseMS"></param>
        /// <param name="onTimeoutClosed"></param>
        /// <param name="autoDispose">关闭窗体的时候是否执行dispose函数销毁自己</param>
        /// <returns></returns>
        public bool ShowAndAutoClose(string msg, string title, bool speakSync, int delayCloseMS, Action<FullScreenNoticeShower> onTimeoutClosed, bool autoDispose = false)
        {
            if (delayCloseMS < 0 || delayCloseMS > 3600000)
            {
                Utils.LogError("错误,延迟关闭对话框的延迟时间不能小于0,不能大于一小时");
                return false;
            }
            this.Title = title;
            this.Message = msg;
            //this.titleLabel.Text = title;
            //this.messageLabel.Text = msg;
            this.DelayCloseMS = delayCloseMS;
            this.OnTimeoutClosed = onTimeoutClosed;
            this.AutoDispose = autoDispose;
            this.Show();
            ///同步的把说的内容都说完以后再进行延迟关闭
            if (speakSync)
            {
                string speakMsg = string.Format("{0}。{1}", title, msg);
                App.TTSSpeaker.Speak(speakMsg, false);
            }
            if (bw.IsBusy == false)
            {
                this.bw.RunWorkerAsync();
            }
            else
            {
                this.start = DateTime.Now;
                Utils.LogInfo("当前正在显示提示框,重置时间");
                return false;
            }
            return true;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Utils.LogFinished("NoticeShower结束了定时,需要关闭");
            if (this.OnTimeoutClosed != null)
            {
                this.OnTimeoutClosed(this);
                Utils.LogFinished("已经执行回调函数OnTimeoutClosed");
            }
            this.Close();
            if (AutoDispose)
            {
                if (this.Parent != null)
                {
                    this.Parent.Controls.Remove(this);
                    Utils.LogSuccess("当前notice shower 的父窗体不为空,并且当前消息显示器已经标记了要在时间结束后销毁自己,已经从父窗体中移除了自身");
                }
                
                try
                {
                    this.Dispose();
                }
                catch (Exception disposeErr)
                {
                    Utils.LogError("NoticeShower释放自身错误:", disposeErr.Message);
                }
                Utils.LogSuccess("已是放NoticeShower自身");
            }
        }
        /// <summary>
        /// 开始时间 默认是构造该控件的时候的1分钟后.1分钟过了就关闭了.
        /// </summary>
        DateTime start = DateTime.Now.AddSeconds(1);
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.LogSuccess("bw_DoWork @ FullScreenNoticeShower 开始了");
            start = DateTime.Now;
            while (!this.Disposing && !this.IsDisposed && !this.bw.CancellationPending)
            {
                if ((DateTime.Now - start).TotalMilliseconds > this.DelayCloseMS)
                {
                    break;
                }
            }
            Utils.LogFinished("bw_DoWork @ FullScreenNoticeShower 结束了");
        }

        private void FullScreenNoticeShower_Load(object sender, EventArgs e)
        {
            this.titleLabel.Text = Title;
            this.messageLabel.Text = Message;
        }
    }
}
