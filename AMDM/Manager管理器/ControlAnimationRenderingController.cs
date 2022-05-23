using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/*
 * 控件动画渲染控制器.这里包含一个单独线程的计时器
 * 2021年11月7日13:50:12
 * 设计思路:设置FPS,每隔多久触发一次更新,比如这里设定为30帧
 * 触发的时候直接调用这里保存的控件列表的Refresh函数
 * 当控件接收到刷新函数的时候,从App中的本类对象中获取第多少帧.如果是小于第50帧的,显示白色,大于50帧的显示绿色.
 * 或则是控件接收到刷新函数的时候,获取帧对应的贝塞尔曲线值
 */
namespace AMDM.Manager
{
    public class ControlAnimationRenderingController
    {
        #region 公共全局变量
        /// <summary>
        /// 当前帧的索引序号 从0开始
        /// </summary>
        public int CurrentFrameIndex { get; set; }
        /// <summary>
        /// 当前帧所在运动轨迹中的百分比,线性的
        /// </summary>
        public float CurrentFramePercent { get; set; }
        /// <summary>
        /// 动画的fps
        /// </summary>
        public float FPS = 20;
        #endregion
        #region 全局变量
        /// <summary>
        /// 需要动态渲染的控件集合
        /// </summary>
        List<Control> animationControls = new List<Control>();
        BackgroundWorker bw = new BackgroundWorker();
        #endregion
        public ControlAnimationRenderingController()
        {
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }
        #region 公共函数
        /// <summary>
        /// 添加要动态渲染的控件,添加后如果多线程没有启动,启动他
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public bool AddAnimationControls(Control ctrl)
        {
            this.animationControls.Add(ctrl);
            if (this.bw.IsBusy == false)
            {
                this.bw.RunWorkerAsync();
            }
            return true;
        }
        /// <summary>
        /// 移除要动态渲染的控件,移除后如果没有需要动态渲染的组件了,关闭多线程.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public bool RemoveAnimationControls(Control ctrl)
        {
            if (this.animationControls.Contains(ctrl))
            {
                this.animationControls.Remove(ctrl);
                //if (this.animationControls.Count == 0 && this.bw.IsBusy)
                //{
                //    this.bw.CancelAsync();
                //}
                ctrl.Refresh();//最后再刷新一次,因为有可能他之前显示动画显示到了一半,所以要清空他的动画状态,如果在控件的渲染中没有获取到他自己是在动画中 那就直接渲染初始状态
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 清空需要动态渲染的组件,同时停止多线程.
        /// </summary>
        /// <returns></returns>
        public bool ClearAnimationControls()
        {
            List<Control> colone = new List<Control>(this.animationControls);
            
            this.animationControls.Clear();
            for (int i = 0; i < colone.Count; i++)
            {
                //最后再渲染一次 防止动画卡在了一半
                colone[i].Refresh();
            }
            //if (this.bw.IsBusy)
            //{
            //    this.bw.CancelAsync();
            //}
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 给一个控件,让本类告诉他 他是不是在动态渲染中.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public bool IsAnimating(Control ctrl)
        {
            return this.animationControls.Contains(ctrl);
        }
        /// <summary>
        /// 获取直线100时间相对应的sin后的0-100
        /// </summary>
        /// <param name="animateSpanMS"></param>
        /// <returns></returns>
        public float GetSinPercent()
            //(float animateSpanMS)
        {
            //直线化的0-100
            float tick0to100 = this.CurrentFramePercent;// *animateSpanMS / 1000;
            //用于计算平滑过渡的pi
            double pi = Math.PI * (1 - tick0to100);
            //通过sin曲线化计算过的平滑0-100
            double dot0to100_after_sin = 1 + Math.Sin(-pi);
            return Convert.ToSingle(dot0to100_after_sin);
        }
        #endregion
        #region 线程处理器

       
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrentFrameIndex = 0;
            CurrentFramePercent = 0;
            GC.Collect();
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < animationControls.Count; i++)
                {
                    animationControls[i].Refresh();
                }
                if (this.CurrentFramePercent >= 1)
                {
                    GC.Collect();
                }
            }
            catch (Exception refreshErr)
            {
                Utils.LogError("在控件动画渲染控制器中发生渲染的错误:", refreshErr.Message, refreshErr.StackTrace);

            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //return;
            //由于对精度的要求不高,所以直接用sleep,然后在bw_ProgressChanged的时候发送要更新页面的请求
            //int perFrameMS = 1000 / FPS;
            float perFrameMS = 1000 / FPS;
            DateTime start = DateTime.Now;
            while (this.bw.CancellationPending == false)
            {
                CurrentFrameIndex++;
                if (CurrentFrameIndex >= FPS)
                {
                    CurrentFrameIndex = 0;
                }
                this.CurrentFramePercent = CurrentFrameIndex / FPS;
                this.bw.ReportProgress(0, CurrentFrameIndex);
                Application.DoEvents();
                System.Threading.Thread.Sleep(Convert.ToInt32(perFrameMS));
            }
        }
        #endregion
    }
}
