using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMDM
{
    public class FullScreenVideoShowerV3 : IDisposable
    {
        #region 全局变量
        LibVLC vlc;
        MediaPlayer player;
        Form shower;
        bool needForceClose = false;
        string videoFilePath = null;
        bool releasing = false;
        #endregion

        #region 构造函数
        public FullScreenVideoShowerV3()
        {
            if (App.VLCHandler != null)
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
            this.player.EndReached += player_EndReached;

            shower = new Form();
            shower.FormBorderStyle = FormBorderStyle.None;
            shower.WindowState = FormWindowState.Maximized;
            VideoView view = new VideoView();
            view.MediaPlayer = this.player;
            view.Dock = DockStyle.Fill;
            shower.Controls.Add(view);
        }
        #endregion

        #region 公共变量
        /// <summary>
        /// 视频播放完毕后是否自动关闭窗口
        /// </summary>
        public bool HideAfterFinished { get; set; }

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
                if (this.player != null && this.player.Media == null)
                {
                    Utils.LogStarted("全屏视频播放器  ##即将播放新视频:", value);
                    this.play();
                }
                else if (this.player != null && this.player.Media != null)
                {
                    Utils.LogStarted("全屏视频播放器  @@将更换视频:", value);
                    this.changeVideo(value);
                }
            }
        }

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
                    shower.Hide();
                }
            }
        }
        /// <summary>
        /// 当窗体关闭的时候发出的回调.如果是播放完毕自动关闭也是onclose,如果是外界干扰需要关闭 也会触发
        /// </summary>
        public Action OnHided { get; set; }
        #endregion

        #region 公共函数
        public void Show()
        {
            shower.Show();
        }
        delegate void hideFunc();
        public void Hide()
        {
            if (this.shower.InvokeRequired)
            {
                hideFunc fc = new hideFunc(Hide);
                this.shower.BeginInvoke(fc);
                return;
            }
            this.shower.Hide();
            #region 当有视频的时候 视频需要暂停的
            if (this.player != null && (this.player.IsPlaying || this.player.Media != null))
            {
                Utils.LogStarted("FullScreenVideoShowerV3 调用hide,需要停止视频,当前IsPlaying", this.player.IsPlaying);
                ThreadPool.QueueUserWorkItem((res) =>
                {
                    this.player.Stop();
                    Utils.LogFinished("FullScreenVideoShowerV3 在线程池内完成player.stop 当前IsPlaying", this.player.IsPlaying);
                });
            }
            #endregion
            if (this.OnHided != null)
            {
                this.OnHided();
            }
        }
        public void Dispose()
        {
            if (this.player != null)
            {
                releasePlayer();
            }
        }
        #endregion

        #region 私有函数
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
            #region 如果需要播放完了以后直接就关闭的话

            if (this.HideAfterFinished)
            {
                this.Hide();
                return;
            }
            #endregion
            #region 如果播放完毕了以后不需要关闭,需要继续重播的话,继续重新播放,但是要使用单独的线程来操控.这个要注意,不然会卡住的
            ThreadPool.QueueUserWorkItem(
                (res) =>
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

            #endregion
        }

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


        void changeVideo(string newPath)
        {
            ThreadPool.QueueUserWorkItem(
                (res) =>
                {
                    try
                    {
                        if (this.player != null)
                        {
                            if (this.player.Media != null)
                            {
                                this.player.Media.Dispose();
                            }
                            this.player.Play(new Media(vlc, newPath));
                        }
                    }
                    catch (Exception changeVideoErr)
                    {
                        Utils.LogError("在全屏视频播放组件v2中,更换视频失败", changeVideoErr.Message);
                    }
                    Utils.LogFinished("更换视频完成", newPath);
                }
            );
        }

        #endregion
    }
}
