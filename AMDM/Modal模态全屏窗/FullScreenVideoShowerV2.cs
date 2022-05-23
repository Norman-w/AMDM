using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using System.Threading;

namespace AMDM
{
    public partial class FullScreenVideoShowerV2 : Form
    {
        LibVLC vlc;
        MediaPlayer player;
        public FullScreenVideoShowerV2()
        {
            InitializeComponent();
            //if (App.Setting.Debugging)
            //{
            //    this.WindowState = FormWindowState.Normal;
            //}
            //else
            //{
            //    this.WindowState = FormWindowState.Maximized;
            //}
            //调试的时候取消上面的注释;
           
            this.WindowState = FormWindowState.Maximized;
            if (App.VLCHandler!= null)
            {
                this.vlc = App.VLCHandler;
            }
            else
            {
                this.vlc = new LibVLC();
            }
            if (App.VLCPlayer != null)
            {
                this.player = App.VLCPlayer;
            }
            else
            {
                this.player = new MediaPlayer(vlc);
            }
            this.videoView1.MediaPlayer = this.player;
            this.player.EndReached += player_EndReached;
        }
        string videoFilePath = null;
        /// <summary>
        /// 要播放的视频文件的路径
        /// </summary>
        public string VideoFilePath
        {
            get
            {
                return videoFilePath;
            }
            set
            {
                this.videoFilePath = value;
                if (this.player!= null && this.player.Media == null)
                {
                    Utils.LogStarted("全屏视频播放器  ##即将播放新视频:", value);
                    this.changeVideo(value);
                }
                else if (this.player != null && this.player.Media!= null)
                {
                    Utils.LogStarted("全屏视频播放器  @@将更换视频:", value);
                    this.play();
                }
            }
        }
        /// <summary>
        /// 当窗体关闭的时候发出的回调.如果是播放完毕自动关闭也是onclose,如果是外界干扰需要关闭 也会触发
        /// </summary>
        public Action OnHided { get; set; }
        bool needForceClose = false;
        /// <summary>
        /// 由外部通知是否需要关闭
        /// </summary>
        public bool NeedForceHide
        {
            get { return this.needForceClose; }
            set
            {
                this.needForceClose = value;
                if (value)
                {
                    this.Hide();
                }
            }
        }
        /// <summary>
        /// 视频播放完毕后是否自动关闭窗口
        /// </summary>
        public bool HideAfterFinished { get; set; }
        //public enum CloseTriggerTypeEnum
        //{
        //    NONE=0,
        //    视频播放完毕后关闭,
        //    外部命令通知关闭,
        //};
        bool play()
        {
            if (videoFilePath == null)
            {
                Utils.LogError("全屏视频播放组件需要指定视频文件地址");
                return false;
            }
            if (System.IO.File.Exists(videoFilePath) == false)
            {
                Utils.LogError("全屏视频播放组件所指定的视频文件地址不存在");
                return false;
            }
            try
            {
                this.player.Play(new Media(vlc, videoFilePath));
                return true;
            }
            catch (Exception err)
            {
                Utils.LogError("尝试播放文件错误:", videoFilePath, err.Message, err.StackTrace);
                return false;
            }
        }
        void player_EndReached(object sender, EventArgs e)
        {
            if (this.HideAfterFinished)
            {
                this.Hide();
                return;
            }
            ThreadPool.QueueUserWorkItem(
                (res)=>
                {
                    try
                    {
                        if (releasing)
                        {
                            Utils.LogInfo("在重新播放视频时,发现窗体正在关闭或者已经释放,不再播放视频资源,等待释放");
                            return;
                        }
                        else
                        {
                            this.player.Stop();
                            this.player.Play();
                            Utils.LogInfo("全拼视频播放组件已重播");
                        }
                    }
                    catch (Exception err)
                    {
                        Utils.LogError("尝试重新播放视频时发生错误", err.Message);
                    }
                }
                );
        }
        bool releasing = false;
        void releasePlayer()
        {
            if (releasing)
            {
                return;
            }
            releasing = true;
            ThreadPool.QueueUserWorkItem((res) =>
                {
                    if (this != null && this.player != null)
                    {
                        try
                        {
                            this.player.Stop();
                            this.player.EndReached -= this.player_EndReached;
                            if (this.player.Media != null)
                            {
                                this.player.Media.Dispose();
                                this.player.Media = null;
                                Utils.LogFinished("已释放媒体文件");
                            }
                            if (App.VLCPlayer == null)
                            {
                                //那就是本类中生成的player 需要释放
                                this.player.Dispose();
                            }
                            
                            //this.vlc.Dispose();
                            //this.player.Dispose();
                            //this.vlc = null;
                            //this.player = null;
                            Utils.LogFinished("窗体已经关闭,在线程池有空闲时,停止了播放器的播放");
                        }
                        catch (Exception releaseErr)
                        {
                            Utils.LogError("在线程池中释放全屏播放器相关组件错误", releaseErr.Message, releaseErr.StackTrace);
                        }
                        
                    }
                }
            );
        }

        private void FullScreenVideoShower_Load(object sender, EventArgs e)
        {
            //if (this.videoFilePath == null)
            //{
            //    MessageBox.Show(this, "未指定要播放的视频文件", "全屏视频播放组件");
            //    return;
            //}
            //if (this.play() == false)
            //{
            //    Utils.LogError("全屏视频播放组件发生错误");
            //    return;
            //}
        }

        void changeVideo(string newPath)
        {
            try
            {
                if (this.player!= null)
                {
                    if (this.player.Media!= null)
                    {
                        this.player.Media.Dispose();
                    }
                    this.player.Play(new Media(vlc, newPath));
                }
            }
            catch (Exception changeVideoErr)
            {
                Utils.LogError("在全屏视频播放组件v2中,更换视频失败",changeVideoErr.Message);
            }
            Utils.LogFinished("更换视频完成",newPath);
        }

        private void videoView1_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                if (this.OnHided != null)
                {
                    this.OnHided();
                }
            }
        }

        private void FullScreenVideoShowerV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.player != null)
            {
                releasePlayer();
            }
        }
    }
}
