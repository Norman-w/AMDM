using AMDM_Domain;
using AMDM.Manager;
using LibVLCSharp.Shared;
using MyCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EasyNamedPipe;
using AMDM_Server_SDK.Domain;

/*
 * 2021年11月12日13:39:11  构建取药主页面
 * 该页面的主要功能有
 * 展示取药机使用说明,展示当前状态,展示广告
 * 没有需要交互的部分.触发取药是通过扫描二维码
 * 
 * 逻辑顺序:
 * 提示如何使用
 * 扫描二维码
 * --标记当前扫描的二维码,标记当前为取药中,结束广告放映
 * 读取二维码是否为处方
 * --标记当前的处方
 *  根据处方状态确认是否能继续取药,如果没交费,不能取药
 *  读取本地保存的交付信息记录和远端的交付信息记录确认是否已经取药.如果已经取药.退回
 *  根据处方获取药品信息
 *  根据药品信息获取本机库存信息,如果没有足够的药品,进行提示,推送his药品不足但是有人取药
 *  如果有足够的库存,开始取药,记录交付单信息
 *  每完成一个药品的交付,增加一个交付单详情记录
 * 如果取药过程发生错误,一般是机械问题,在交付详单内记录错误,在交付单主单内记录交付单已经取消
 * 如果取药都没有发生错误,完成交付记录单主单的记录
 * 推送到HIS系统取药已经完成
 * 提示 
 * 广告植入 xxx药业提醒您:请核对药品与处方及取药单完全一致后 方可用药
 * --标记当前扫描的二维码为空,标记当前为没有取药
 * 继续等待取药.
 * 广告播放
 */

namespace AMDM
{
    public enum ShowingPageEnum
    {
        空闲播放广告中,//第一广告位,如果机器使用率低,则时间为最多,第二广告位为小票
        确认处方信息中,
        药机正在抓取和传送药品中,//第三广告位,时间为取药和传送时间
        等待用户取走药品中,//第四广告位,时间基本固定,如果时间长也只是长出来用户取走药品时候的慢动作时间.
        药品已经取走提醒用户确认药品中,//第五广告位,药品已经取走,这个广告只是一个提示,受众的时间不长或者是取药完成以后用户直接走开根本不会看或者机器比较忙碌的时候,下一个用户能看到.
        正在显示文字提示信息中,//正在显示文字框的提示信息中.
        上药中,
    }
    public partial class MedicineDeliveryForm : Form
    {
        #region 构造函数
        public MedicineDeliveryForm()
        {
            InitializeComponent();
            this.formAutoSizer = new FormAutoSizer(this);
            this.formAutoSizer.TurnOnAutoSize();
        }
        #endregion

        #region 全局变量
        #region 主控制状态指示器,指示当前显示的页面
        /// <summary>
        /// 当前屏幕上展示的页面是哪一个,如果是广告放映中 才可以扫描处方二维码.如果是正在确认处方信息的时候,扫描别的处方二维码直接切换处方显示
        /// </summary>
        ShowingPageEnum currentShowingPage = ShowingPageEnum.空闲播放广告中;
        #endregion
        #region 当前展示在最顶层的全屏播放视频的窗口
        /// <summary>
        /// 当前展示在最顶层的全屏播放视频的窗口
        /// </summary>
        FullScreenVideoShowerV3 currentTopmostFullScreenVideoShower = new FullScreenVideoShowerV3();
        #endregion
        #region 当前正在展示的药品信息确认中页面,如果正在显示,之前显示的那个页面需要 close, dispose 然后再重新初始化一个.
        /// <summary>
        /// 当前正在展示的药品信息确认中页面,如果正在显示,之前显示的那个页面需要 close, dispose 然后再重新初始化一个.
        /// </summary>
        FullScreenMedicineOrderShower currentFullScreenMedicineOrderShower = null;
        #endregion
        #region <<<视频,广告>>>视频文件列表和当前播放的视频
        #region 1广告位空闲时间的视频
        /// <summary>
        /// 空闲时间播放的广告视频文件的列表
        /// </summary>
        List<string> spareTimeADVideos = new List<string>();
        /// <summary>
        /// 当前正在播放的空闲时间广告的文件索引.
        /// </summary>
        int currentSpareTimeADVideoIndex = 0;
        #endregion
        #region 3广告位 正在抓取和传送中的时候.
        /// <summary>
        /// 正在取药中的时候,播放的广告视频文件的列表.
        /// </summary>
        List<string> medicinesGettingADVideos = new List<string>();
        /// <summary>
        /// 当前正在播放的药品获取中的视频的索引.
        /// </summary>
        int currentMedicinesGettingADVideoIndex = 0;
        #endregion
        #region 4广告位 药品已经出仓等待用户取药中的时候.
        /// <summary>
        /// 当药品已经出仓,等待用户取药中的时候,播放的视频列表
        /// </summary>
        List<string> medicinesWaitBeenTakeADVideos = new List<string>();
        /// <summary>
        /// 当前等待用户取药中的时候,播放的视频的索引
        /// </summary>
        int currentMedicinesWaitBeenTakeADVideoIndex = 0;
        #endregion
        #region 5广告位 药品已经被用户取走 提醒用户确认药品和小票的时候
        /// <summary>
        /// 当用户取走药品的时候提示用户核对药品的提示视频文件集合
        /// </summary>
        List<string> medicinesHasBeenTakedADVideos = new List<string>();
        /// <summary>
        /// 当用户取走了药品的时候,提示用户核对药品的当前播放的视频的索引
        /// </summary>
        int currentMedicinesHasBeenTakedADVideoIndex = 0;
        #endregion
        #endregion
        #region 默认的在闲暇广告时间的时候,在屏幕下方有一行文字 文字的内容
        //string noticeScanLabelStartedText = "正在取药中......";
        string noticeScanLabelNoticeDefaultText = "请在下方扫码器 扫描处方二维码开始取药↓";
        string noticeScanLabelNoticeNotUseText = "该设备正在维护中,请到其他取药机或人工窗口取药";
        string noticeScanLabelNoticeUVLampPreparing = "该药机即将开始紫外线杀菌,请远离!!!";
        string noticeScanLabelNoticeUVLampTurnOn = "该药机正在进行紫外线杀菌,请远离!!!";
        #endregion
        #region 窗体自动缩放控制组件
        FormAutoSizer formAutoSizer;
        #endregion
        #region 当前已经扫描了的二维码
        /// <summary>
        /// 当前已经扫描的二维码
        /// </summary>
        string currentScanedQRCode = null;
        #endregion
        #region 不使用的后台线程
        //BackgroundWorker getMedicinesBW = new BackgroundWorker();
        //BackgroundWorker playerControlBW = new BackgroundWorker();
        #endregion
        //#region 主要的取药控制器 也是控制PLC的
        //MedicinesGettingController medicinesGettingController = new MedicinesGettingController();
        //#endregion
        #region 媒体视频 videos播放器
        LibVLC vlc = null;
        MediaPlayer player = null;
        #endregion
        #endregion

        #region 初始化
        public bool Init(List<AMDM_Stock> stocks)
        {
            #region 初始化plc
            App.medicinesGettingController.OnAllMedicinesDeliveryFinished += medicinesGettingController_AllMedicinesDeliveryFinished;
            App.medicinesGettingController.OnMedicinesHasBeenTaked += medicinesGettingController_MedicinesHasBeenTaked;
            App.medicinesGettingController.OnMedicinesGettingError += medicinesGettingController_OnAMDM_MedicinesGettingError;
            #endregion

            //本类中接收码卡读头的消息并处理
            App.ICCardReaderAndCodeScanner2in1ReceivedData = processScannerMsg;
            List<int> debugPos = new List<int>();
            #region 初始化视频播放器
            try
            {
                #region 获取文件列表
                #region 没有文件夹自动创建
                debugPos.Add(1);
                App.Setting.AdvertVideosSetting.SpareTimeADVideosDir = System.IO.Path.GetFullPath(App.Setting.AdvertVideosSetting.SpareTimeADVideosDir);
                App.Setting.AdvertVideosSetting.MedicinesGettingADVideosDir = System.IO.Path.GetFullPath(App.Setting.AdvertVideosSetting.MedicinesGettingADVideosDir);
                App.Setting.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir = System.IO.Path.GetFullPath(App.Setting.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir);
                App.Setting.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir = System.IO.Path.GetFullPath(App.Setting.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir);
                if (System.IO.Directory.Exists(App.Setting.AdvertVideosSetting.SpareTimeADVideosDir) == false)
                {
                    Utils.LogInfo("空闲时间广告的文件夹不存在,正在创建");
                    System.IO.Directory.CreateDirectory(App.Setting.AdvertVideosSetting.SpareTimeADVideosDir);
                    Utils.LogFinished("空闲时间的广告文件夹不存在,已经创建", App.Setting.AdvertVideosSetting.SpareTimeADVideosDir);
                }
                if (System.IO.Directory.Exists(App.Setting.AdvertVideosSetting.MedicinesGettingADVideosDir) == false)
                {
                    Utils.LogInfo("正在取药中的广告视频文件夹,正在创建");
                    System.IO.Directory.CreateDirectory(App.Setting.AdvertVideosSetting.MedicinesGettingADVideosDir);
                    Utils.LogFinished("正在取药中的广告视频文件夹,已经创建", App.Setting.AdvertVideosSetting.MedicinesGettingADVideosDir);
                }
                if (System.IO.Directory.Exists(App.Setting.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir) == false)
                {
                    Utils.LogInfo("药品等待取走中的视频文件夹,正在创建");
                    System.IO.Directory.CreateDirectory(App.Setting.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir);
                    Utils.LogFinished("药品等待取走中的视频文件夹,已经创建", App.Setting.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir);
                }
                if (System.IO.Directory.Exists(App.Setting.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir) == false)
                {
                    Utils.LogInfo("药品已经被取走的视频文件夹,正在创建");
                    System.IO.Directory.CreateDirectory(App.Setting.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir);
                    Utils.LogFinished("药品已经被取走的视频文件夹,已经创建", App.Setting.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir);
                }
                #endregion
                debugPos.Add(2);
                #region 从文件夹中搜索视频文件
                //搜索空闲时间的视频
                this.spareTimeADVideos = new List<string>(System.IO.Directory.GetFiles(App.Setting.AdvertVideosSetting.SpareTimeADVideosDir));
                Utils.LogFinished("加载空闲时间播放的广告视频完成,文件数量:", this.spareTimeADVideos.Count);
                this.currentSpareTimeADVideoIndex = 0;
                //搜索正在取药中的视频
                this.medicinesGettingADVideos = new List<string>(System.IO.Directory.GetFiles(App.Setting.AdvertVideosSetting.MedicinesGettingADVideosDir));
                Utils.LogFinished("加载正在取药中的广告视频完成,文件数量:", this.medicinesGettingADVideos.Count);
                this.currentMedicinesGettingADVideoIndex = 0;
                //搜索等待用户取走药品中的视频
                this.medicinesWaitBeenTakeADVideos = new List<string>(System.IO.Directory.GetFiles(App.Setting.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir));
                Utils.LogFinished("加载药品已经出仓等待用户取走药品时候的广告视频文件完成,文件数量:", this.medicinesWaitBeenTakeADVideos.Count);
                this.currentMedicinesWaitBeenTakeADVideoIndex = 0;
                //搜索药品已经被取走时候的视频
                this.medicinesHasBeenTakedADVideos = new List<string>(System.IO.Directory.GetFiles(App.Setting.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir));
                Utils.LogFinished("加载药品已经被取走,提示用户确认药品时播放的广告视频完成,文件数量:", this.medicinesHasBeenTakedADVideos.Count);
                this.currentMedicinesHasBeenTakedADVideoIndex = 0;
                #endregion
                debugPos.Add(3);
                #endregion
                if (App.VLCHandler != null)
                {
                    this.vlc = App.VLCHandler;
                }
                else
                {
                    vlc = new LibVLC(
                    new string[] { 
                        //"--no-directx-hw-yuv", 
                        //" --no-audio",
                    //" --no-direct3d-hw-blending", 
                    //"  --transform-type=90", 
                    //" --no-avcodec-fast", 
                    //" --no-quiet" 
                }
                );
                }
                debugPos.Add(4);
                vlc.SetLog(vlcLogCallback);
                if (App.VLCPlayer != null)
                {
                    this.player = App.VLCPlayer;
                }
                else
                {
                    player = new MediaPlayer(vlc);
                }
                debugPos.Add(5);
                this.videoView1.MediaPlayer = player;
                player.EndReached += player_EndReached;

                if (this.spareTimeADVideos.Count >0)
                {
                    Utils.LogInfo("存在空闲时间广告视频播放文件,正在加载并播放第一个视频");
                    Media firstMedia = new Media(vlc, this.spareTimeADVideos[this.currentSpareTimeADVideoIndex]);
                    player.SetRate(App.Setting.AdvertVideosSetting.SpeedRate);
                    player.Play(firstMedia);
                    this.currentShowingPage = ShowingPageEnum.空闲播放广告中;
                }
                else
                {
                    string msg = string.Format("从路径{0}没有加载到任何的空闲时间播放的广告视频.",App.Setting.AdvertVideosSetting.SpareTimeADVideosDir);
                    Utils.LogWarnning(msg);
                    //MessageBox.Show(this, msg, "没有广告视频");
                }
            }
            catch (Exception initVideoPlayerErr)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("初始化广告视频播放器失败:{0}", initVideoPlayerErr.Message);
                Console.ResetColor();
                Utils.LogInfo("调试断点", debugPos);
            }
            #endregion

            #region 关注错误消息进行提示
            App.MonitorsManager.OnMonitorDetectedError += MonitorsManager_OnMonitorDetectedError;
            App.ControlPanel.OnControlPanelClearErrorOfMaintenanceStatus += ControlPanel_OnControlPanelClearErrorOfMaintenanceStatus;
            #endregion

            #region 关注紫外线灯杀菌状态变更
            App.UVLampManager.OnUVLampStatusChanged += UVLampManager_OnUVLampStatusChanged;
            #endregion

            this.setLabelTextByMaintenanceStatus(App.ControlPanel.MaintenanceStatus);
            //showAD();

            return true;
        }

        

        


        //void vlc_Log(object sender, LogEventArgs e)
        //{
        //    Utils.LogBug("vlc日志", e.Level.ToString(), e.Message, e.Module, e.SourceFile, e.SourceLine);
        //    //throw new NotImplementedException();
        //}
        #endregion

        #region 视频播放组件触发的事件


        #region 视频播放组件的错误处理回调
        void vlcLogCallback(IntPtr data, LibVLCSharp.Shared.LogLevel logLevel, IntPtr logContext, string format, IntPtr args)
        {
            if (format != null)
            {
                #region 总出错的暂时先屏蔽
                if (format.Contains("Could not Create the D3D11 device. (hr=0x%lX)"))
                {
                    return;
                }
                if (format.Contains("SetThumbNailClip failed: 0x%0lx"))
                {
                    return;
                }
                if (format.Contains("Direct3D11 could not be opened"))
                {
                    return;
                }
                if (format.Contains("D3D11CreateDevice failed. (hr=0x%lX)"))
                {
                    return;
                }
                if (format.Contains("Failed to create device"))
                {
                    return;
                }
                #endregion
            }
            if (format != null && format.Contains("dropping"))
            {
                Console.WriteLine("丢帧!");
                return;
            }
            switch (logLevel)
            {
                case LibVLCSharp.Shared.LogLevel.Debug:
                    break;
                case LibVLCSharp.Shared.LogLevel.Error:
                    //Utils.LogBug("vlc的日志:", data, logLevel, logContext, format, args);
                    Utils.LogBug(string.Format("播放器错误:{0}", format));
                    break;
                case LibVLCSharp.Shared.LogLevel.Notice:
                    break;
                case LibVLCSharp.Shared.LogLevel.Warning:
                    break;
                default:
                    break;
            }
        }
        #endregion
       

        #region 闲暇时间,当视频播放结束的时候自动切换下一个
        void player_EndReached(object sender, EventArgs e)
        {
            string currentVideoFile = this.spareTimeADVideos[this.currentSpareTimeADVideoIndex];
            this.currentSpareTimeADVideoIndex++;
            if (this.currentSpareTimeADVideoIndex>= this.spareTimeADVideos.Count)
            {
                this.currentSpareTimeADVideoIndex = 0;
            }
            ThreadPool.QueueUserWorkItem((res) =>
            {
                try
                {
                    if (App.DebugCommandServer != null && App.DebugCommandServer.DebuggerConnected && App.DebugCommandServer.Setting != null && App.DebugCommandServer.Setting.LogVLCDebugInfo)
                    {
                        Utils.LogInfo("视频文件播放完毕,正在尝试释放当前播放器中加载的视频,上次播放的视频:", currentVideoFile);
                    }
                    this.player.Media.Dispose();
                }
                catch (Exception err)
                {
                    Utils.LogError("视频文件欧方完毕,尝试释放播放器中当前加载的视频失败,上次播放的视频是:", currentVideoFile, err.Message, err.StackTrace);
                }
                string nextVideoFile = this.spareTimeADVideos[this.currentSpareTimeADVideoIndex];
                //停止之前的视频的播放
                this.player.Stop();
                if (this.IsDisposed)//如果已经关闭窗口了 销毁
                {
                    Utils.LogSuccess("当前窗口已经关闭,取消绑定结束播放的事件");
                    this.player.EndReached -= player_EndReached;
                    if (App.VLCPlayer== null)
                    {
                        //不是全局给的而是本类中生成的
                        this.player.Dispose();
                    }
                    Utils.LogSuccess("已经执行完了播放器的释放");
                }
                else
                {
                    //开始播放新的视频内容
                    this.player.Play(new Media(vlc, nextVideoFile, Media.FromType.FromPath));
                    if (App.DebugCommandServer != null && App.DebugCommandServer.DebuggerConnected && App.DebugCommandServer.Setting != null && App.DebugCommandServer.Setting.LogVLCDebugInfo)
                    {
                        Utils.LogSuccess("已经开始播放新的视频文件:", nextVideoFile);
                    }
                }
            });
        }
        #endregion


        #endregion

        #region 切换页面相关方法
        /// <summary>
        /// 停止广告的放映.应当在扫描了付药单的时候直接停止,或者是取药机发生故障的时候停止.
        /// </summary>
        void stopAD()
        {
            //Utils.LogSuccess("准备直接在调用停止广告的方法的线程中停止广告放映");
            //if (this.currentShowingPage != ShowingPageEnum.空闲播放广告中)
            //{
            //    Utils.LogBug("不是正在播放空闲广告,不需要调用stopAD啊!!!!");
            //}
            //else
            //{
            //    this.player.Pause();
            //}
            if (this.player!= null)
            {
                if (this.player.IsPlaying == false)
                {
                    return;
                }
            }
            #region 使用线程池来操作试试
            Utils.LogInfo("准备开始在线程池内 暂停广告视频");
            ThreadPool.QueueUserWorkItem((res) =>
            {
                try
                {
                    //if (this.currentShowingPage != ShowingPageEnum.空闲播放广告中)
                    //{
                    //    Utils.LogBug("不是正在播放空闲广告,不需要调用stopAD啊!!!!");
                    //}
                    //else
                    //{
                        this.player.Pause();
                    //}
                }
                catch (Exception err)
                {
                    Utils.LogError("尝试在线程池内停止播放广告视频错误:", err.Message, err.StackTrace);
                }
            });
            #endregion
        }

        delegate void ShowADFunc();
        void showAD()
        {
            this.currentShowingPage = ShowingPageEnum.空闲播放广告中;
            Utils.LogInfo("准备开始在线程池内开始或者继续播放广告视频");

            //if (this.adVideoScreen.InvokeRequired)
            //{
            //    ShowADFunc fc = new ShowADFunc(showAD);
            //    this.adVideoScreen.BeginInvoke(fc);
            //}
            //else
            //{
            
                ////player.Play();
            //}

            #region 使用线程池来操作试试
            ThreadPool.QueueUserWorkItem((res) => {
                try
                {
                    this.player.Play();
                }
                catch (Exception err)
                {
                    Utils.LogError("尝试在线程池内开始播放广告视频错误:", err.Message,err.StackTrace);
                }
            });
            #endregion
        }
        //#region 显示管理员页面
        //delegate void ShowAdminFormFunc(Action onClose);
        //void showAdminForm(Action onClose)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        ShowAdminFormFunc fc = new ShowAdminFormFunc(showAdminForm);
        //        this.BeginInvoke(fc, new object[]{onClose});
        //        return;
        //    }
        //    AdminMenuForm aform = new AdminMenuForm();
        //    aform.OnClose = onClose;
        //    aform.Show();
        //}
        //#endregion
        delegate void ShowMedicineOrderFunc(AMDM_MedicineOrder order, Action<AMDM_MedicineOrder> onStartGetting);
        /// <summary>
        /// 显示当前扫描的付药单信息
        /// </summary>
        void showMedicineOrder(AMDM_MedicineOrder order, Action<AMDM_MedicineOrder> onStartGetting)
        {
            //if (this.InvokeRequired)
            //{
            //    ShowMedicineOrderFunc func = new ShowMedicineOrderFunc(showMedicineOrder);
            //    this.BeginInvoke(func, new object[] { order, onStartGetting });
            //    return;
            //}
            this.currentShowingPage = ShowingPageEnum.确认处方信息中;
            this.stopAD();
            #region 如果之前已经显示了付药单但是这个函数又调用了.说明这是新的跟刚才不一样的付药单需要显示了.之前的页面需要关闭 dispose 然后重新初始化
            if (currentFullScreenMedicineOrderShower != null)
            {
                currentFullScreenMedicineOrderShower.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                currentFullScreenMedicineOrderShower.Close();
                currentFullScreenMedicineOrderShower.Dispose();
            }
            #endregion
            //此处 之前设计的应该是在显示提示信息的时候,一扫码提示信息就没了.不这样处理了.要让屏幕显示完了提示信息的时候再能接受扫码.
            currentFullScreenMedicineOrderShower = new FullScreenMedicineOrderShower();
            currentFullScreenMedicineOrderShower.Init(order, App.Setting.UserInterfaceSetting.MedicineOrderAutoHideWhenNoActionMS, (sender,by) =>
            {
                if (by == FullScreenMedicineOrderShower.CompletedByEnum.UserClose)
                {

                }
                else
                {
                    Utils.LogInfo("显示取药单超过60秒没有动作以后自动关闭了");
                    this.showAD();
                }                
            }, true);
            if (this.InvokeRequired)
            {
                Utils.LogInfo("在显示付药单时,不在主线程");
            }
            showMedicineOrderInfoDialog(onStartGetting, order);
        }


        void showMedicineOrderInfoDialog(Action<AMDM_MedicineOrder> onStartGetting, AMDM_MedicineOrder order)
        {
            DialogResult ret = currentFullScreenMedicineOrderShower.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                Utils.LogSuccess("点击了开始取药,马上调用startGetting的action");

                #region 向上迭代要播放的不同的药品公司的视频,每次取药的时候换一个公司的视频
                this.currentMedicinesGettingADVideoIndex++;
                if (this.currentMedicinesGettingADVideoIndex >= this.medicinesGettingADVideos.Count)
                {
                    this.currentMedicinesGettingADVideoIndex = 0;
                }
                string currentMedicinesGettingADFile = this.medicinesGettingADVideos[this.currentMedicinesGettingADVideoIndex];
                #endregion
                this.currentShowingPage = ShowingPageEnum.药机正在抓取和传送药品中;
                showFullScreenVideo(currentMedicinesGettingADFile, false, null);
                //开始取药
                onStartGetting(order);
            }
            else
            {
                //用户点击取消,恢复广告的播放
                Utils.LogInfo("用户点击取消,恢复播放广告");
                this.showAD();
            }
        }




        delegate void ShowMessageOnScreenFunc(string title, string msg, bool speakSync, int delayCloseMS);
        /// <summary>
        /// 在屏幕上显示提示信息,显示指定的时间
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="showTimeMS"></param>
        void showMessageOnScreen(string title, string msg,bool speakSync, int delayCloseMS)
        {
            if (this.InvokeRequired)
            {
                ShowMessageOnScreenFunc func = new ShowMessageOnScreenFunc(showMessageOnScreen);
                this.BeginInvoke(func, new object[] { title, msg ,speakSync, delayCloseMS });
                return;
            }
            //if (this.currentShowingPage == ShowingPageEnum.正在显示文字提示信息中)
            //{
            //    Utils.LogInfo("当前正在显示文字提示信息,等待上一提示结束后再扫描");
            //    return;
            //}
            this.currentShowingPage = ShowingPageEnum.正在显示文字提示信息中;
            if (this.currentTopmostFullScreenVideoShower != null)
            {
                //this.currentTopmostFullScreenVideoShower.Close();
                this.currentTopmostFullScreenVideoShower.Hide();
            }
            if (this.currentFullScreenMedicineOrderShower != null)
            {
                this.currentFullScreenMedicineOrderShower.Close();
                this.currentFullScreenMedicineOrderShower.Dispose();
            }
            this.stopAD();
            FullScreenNoticeShower ns = new FullScreenNoticeShower();
            ns.ShowAndAutoClose(msg, title,speakSync, delayCloseMS, (sender) =>
            {
                this.showAD();
            }, true);
        }
        #endregion

        #region 硬件触发的事件

        #region 登录成功后退出
        delegate void loginAndExitFunc();
        void loginAndExit()
        {
            if (this.InvokeRequired)
            {
                loginAndExitFunc fc = new loginAndExitFunc(loginAndExit);
                this.BeginInvoke(fc);
                return;
            }
            this.stopAD();
            this.currentShowingPage = ShowingPageEnum.上药中;
            LoginForm lform = new LoginForm();
            DialogResult ret = lform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                this.showAD();
                return;
            }
            LoginResponse rsp = App.UserManager.Login(lform.User, lform.Pass);
            if (rsp.IsError)
            {
                MessageBox.Show(this, rsp.ErrMsg, "错误");
                this.showAD();
                return;
            }
            this.Close();
        }
        #endregion

        #region 扫码读头事件
        delegate void ProcessScannerMsgFunc(string msg);
        /// <summary>
        /// 当扫码读头读取到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void processScannerMsg(string msg)
        {
            //if (this.InvokeRequired)
            //{
            //    ProcessScannerMsgFunc fc = new ProcessScannerMsgFunc(processScannerMsg);
            //    this.BeginInvoke(fc, new object[] { msg });
            //    Utils.LogInfo("当前不在主线程中,需要到主线程中执行扫码后的事件");
            //}
            //else
            //{
            //当前本类中扫描的条码
            //测试的时候显示的条码,后续去掉.
            //Utils.LogInfo("收到扫码器读头的消息", msg);

            #region 2022年4月3日12:04:10  如果扫描的是退出码,输入管理员账号和密码后退出
            if (msg!= null && msg.ToLower().StartsWith("exit:admin"))
            {
                loginAndExit();
                return;
            }
            #endregion

            //如果药机不是正常状态不让使用,但是可以执行此处之上的代码,可以退出程序
            if (App.ControlPanel.MaintenanceStatus!= MaintenanceStatusEnum.运行正常)
            {
                return;
            }

            #region string 转换为16进制,显示到屏幕让提示已经扫描的码
            //char[] values = msg.ToCharArray();
            //string newMSG = "";
            //foreach (char letter in values)
            //{
            //    // Get the integral value of the character.
            //    int value = Convert.ToInt32(letter);
            //    // Convert the decimal value to a hexadecimal value in string form.
            //    newMSG += String.Format("{0:X} ", value);
            //}
            //showMessageOnScreen("您扫描的内容是:", msg + "\r\n16进制内容是:" + newMSG, true, 3000);
            //return;
            #endregion
            

            #region 解析扫描的内容是否是超长文本,并且可以解析成钥匙信息
            //等待新的钥匙启动器插入

            //扫描二维码后新的钥匙启动器插入
            #endregion


            #region 解析fake his  server 扫描的二维码 并转换成一个付药单或者是处方id
            if (msg != null && msg.Contains("MedicineOrderID:") == true)
            {
                msg = msg.Replace("MedicineOrderID:", "");
                //Console.WriteLine("已替换为{0}", msg);
            }
            #endregion



            #region 检查可否扫码的状态.如果当前已经扫描的条码跟上次的一样的话,就不显示新的处方信息,如果不一样,关掉当前的处方信息页面,显示新的处方信息页面
            if (this.currentShowingPage == ShowingPageEnum.确认处方信息中)
            {
                if (currentScanedQRCode == msg)
                {
                    Utils.LogWarnning("上一次扫描的和这次扫描的是同一个二维码,取药只扫描一次就可以", msg);
                    return;
                }
                else
                {
                    Utils.LogWarnning("处方确认中,这时候扫描了新的处方二维码,确认另外的处方");
                    //扫描了新的二维码,刚才那个直接关闭就行了.
                    //代码往下走 直接走到 showMedicineOrder 那里 在showMedicineOrder 里面 直接进行上一次的页面的关闭或者替换
                }
            }
            else if (this.currentShowingPage != ShowingPageEnum.空闲播放广告中)
            {
                Utils.LogWarnning("当前不是在机器空闲中,不能扫描新的处方进行取药");
                return;
            }
            #endregion
            this.currentScanedQRCode = msg;


            #region 根据扫描的内容获取处方/付药单
            AMDM_MedicineOrder order = null;
            //获取处方对应的付药单
            try
            {
                order = App.HISServerConnector.GetMedicineOrderByPrescriptionId(this.currentScanedQRCode);
            }
            catch (Exception err)
            {
                Utils.LogError("获取取药单的时候发生了错误时;", err.Message,err.StackTrace);
                showMessageOnScreen("", "获取取药单发生错误",true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                try
                {
                    App.LogServerServicePipeClient.Log(LogLevel.Error, "获取取药单发生错误", string.Format("错误内容是{0}\r\n用户扫描的内容是:{1}", err.Message, msg));
                }
                catch (Exception sendPipeMsgErr)
                {
                    Utils.LogError("取药页面,发送管道消息错误:", sendPipeMsgErr.Message);
                }
                
                return;
            }
            #endregion
            //根据处方状态确认是否能继续取药,如果没交费,不能取药
            #region 对获取处方的结果是否为空检查,同时对处方能否取药进行一个检查
            if (order == null)
            {
                showMessageOnScreen("错误", "未查询到付药单", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                try
                {
                    App.LogServerServicePipeClient.Log(LogLevel.Info,
                            "根据扫描的内容没查询到付药单",
                            string.Format("客户扫描的内容是:{0}", msg)
                        );
                }
                catch (Exception sendPipeErrMsgErr)
                {
                    Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
                }
                this.currentScanedQRCode = null;
                return;
            }
            if (order.Details == null || order.Details.Count < 1)
            {
                showMessageOnScreen("错误的处方", "没有查询到需要取药的药品", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                try
                {
                    App.LogServerServicePipeClient.Log(LogLevel.Bug,"查到处方了但是处方的内容体为空,details为空或者是数量为0",string.Format("客户扫描的内容是:{0}", msg));
                }
                catch (Exception sendPipeErrMsgErr)
                {
                    Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
                }
                this.currentScanedQRCode = null;
                return;
            }
            if (order.Balance == false)
            {
                showMessageOnScreen("该处方未结清药款", "请结清药款后再进行取药", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                try
                {
                    App.LogServerServicePipeClient.Log(LogLevel.Info, "用户扫描了没有结清的付药单", string.Format("客户扫描的内容是:{0}", msg));
                }
                catch (Exception sendPipeErrMsgErr)
                {
                    Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
                }
                this.currentScanedQRCode = null;
                return;
            }
            if (order.Fulfilled == true)
            {
                showMessageOnScreen("错误", "当前处方已经取药,不能重复取药", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                try{App.LogServerServicePipeClient.Log(LogLevel.Info,
                    "用户扫描了已经取完药的码",
                    string.Format("客户扫描的内容是:{0}", msg)
                    );
                }
                catch (Exception sendPipeErrMsgErr)
                {
                    Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
                }
                this.currentScanedQRCode = null;
                return;
            }
            #endregion
            #region 转换药品id
            if (App.medicineManager.ConvertHISMedicineId2LocalMedicineID(ref order) == false)
            {
                showMessageOnScreen("错误", "获取药方所需药品在本药机中的对应药品数据错误", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
               try
               { 
                   App.LogServerServicePipeClient.Log(LogLevel.Bug, "获取药方所需药品在本药机中的对应药品数据错误,这是个bug",string.Format("客户扫描的内容是:{0}", msg));
               }
               catch (Exception sendPipeErrMsgErr)
               {
                   Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
               }
                this.currentScanedQRCode = null;
                return;
            }
            #endregion

            #region 读取本地保存的交付信息记录和远端的交付信息记录确认是否已经取药.如果已经取药.退回
            bool deliveriedInLocal = App.inventoryManager.GetIsDeliveriedByPrescriptionId(this.currentScanedQRCode, false);
            if (deliveriedInLocal)
            {
                showMessageOnScreen("不能重复取药", "当前处方已经在本药机完成取药", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                try
                {
                    App.LogServerServicePipeClient.Log(LogLevel.Info, "客户扫描了一个已经在本机完成取药的付药单",string.Format("客户扫描的内容是:{0}", msg));
                }
                catch (Exception sendPipeErrMsgErr)
                {
                    Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
                }
                return;
            }
            Console.WriteLine("转换detail中的所有的药品的从his来的id为本地的药品id");

            #endregion
            #region 根据药品信息获取本机库存信息,如果没有足够的药品,进行提示,推送his药品不足但是有人取药
            bool inventoryCanFulfill = App.inventoryManager.GetMedicinesInOrderCanDelivery(order);
            if (inventoryCanFulfill == false)
            {
                showMessageOnScreen("该机药品库存不足", "无法为您的处方付药,请您到人工窗口取药", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                try
                {App.LogServerServicePipeClient.Log(LogLevel.Warnning,
                    "用户扫码以后检测到处方的库存不足", string.Format("客户扫描的内容是:{0}", msg)
                    );
                }
                catch (Exception sendPipeErrMsgErr)
                {
                    Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
                }
                return;
            }
            #endregion

            #region 显示处方在屏幕上,如果用户点击了取药 就开始取药. 如果超时了 播放广告
            //在扫码线程上调用显示付药单
            showMedicineOrder(order, (needStartOrder) =>
            {
                #region 如果有足够的库存,开始取药,记录交付单信息 2022年2月12日14:32:51  并且同时拍一下交互区截图

                StartMedicinesGettingResult startOrderRet =
                    App.medicinesGettingController.StartMedicinesGetting(needStartOrder);
            
                if (startOrderRet.Success == false)
                {
                    #region 如果开始取药任务发生了错误,报告给his系统发生了错误
                    //如果开始取药发生了错误,报告给his系统发生了错误
                    if (startOrderRet.IsError)
                    {
                        string message = string.Format("客户扫描的内容是:{0}\r\nstartOrderRet的错误内容:{1}\r\nstartOrderRet的提示内容:{2}", msg, 
                                startOrderRet.ErrMsg, 
                                startOrderRet.Notice
                                );
                        showMessageOnScreen("启动取药任务失败", startOrderRet.ErrMsg , true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                        App.AlertManager.AlertHardwareError("启动取药任务失败", message);
                    }
                    #endregion
                    #region 如果开始取药没有发生错误,直接显示信息告诉用户为什么没有开始取药,比如该药机正在复位中等信息
                    else
                    {
                       //如果开始取药没有发生错误,直接显示信息告诉用户为什么没有开始取药,比如该机正在复位中等信息
                        showMessageOnScreen("未能开始取药", startOrderRet.Notice, true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
                        try{
                            var message = string.Format("客户扫描的内容是:{0}\r\nstartOrderRet的错误内容:{1}\r\nstartOrderRet的提示内容:{2}",
                                msg,
                                startOrderRet.ErrMsg,
                                startOrderRet.Notice
                                );
                                var level= LogLevel.Info;
                                var title = "启动取药任务失败,无错误";
                                App.LogServerServicePipeClient.Log(level, title, message);
                        }
                        catch (Exception sendPipeErrMsgErr)
                        {
                            Utils.LogError("取药页面,发送管道消息错误:", sendPipeErrMsgErr.Message);
                        }
                    }
                    #endregion
                    //notice("错误!启动取药任务失败,请重试");
                    return;
                }
                else
                {
                    if (App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfInteractiveArea!= null)
                    {
                        //开启取药任务成功,记录一下监控
                        string destPath = getCameraDestFilePath(startOrderRet.DeliveryRecordId, SnapshotLocationEnum.InteractiveArea, DateTime.Now, "jpg");
                        App.CameraSnapshotCapturer.CaptureSync(App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfInteractiveArea, destPath, (img, path) =>
                        {
                            Utils.LogFinished("取药任务启动成功,已完成交互区监控的抓图工作");
                        });
                        App.sqlClient.AddSnapshot(AMDM_Domain.SnapshotParentTypeEnum.DeliveryRecord, startOrderRet.DeliveryRecordId,
                            SnapshotTimePointEnum.BeforeAction,
                       AMDM_Domain.SnapshotLocationEnum.InteractiveArea
                       , DateTime.Now, "已经开始取药触发拍照", destPath);
                    }
                }
                #endregion
            }
                );
            #endregion
            //}
        }
        #endregion

        #region 当所有的药仓都完成了付药以后
        /// <summary>
        /// 获取文件的路径
        /// </summary>
        /// <returns></returns>
        string getCameraDestFilePath(long parentDeliveryRecordId, AMDM_Domain.SnapshotLocationEnum location, DateTime time, string imgType)
        {
            string filePath = string.Format("{0}\\{1}_{2}_{3}.{4}",
                App.Setting.DevicesSetting.CCTVCaptureSetting.SavingDictionary.TrimEnd('\\'),
                parentDeliveryRecordId,
                location.ToString(), time.Ticks,
                 imgType);
            return filePath;
        }
        void medicinesGettingController_AllMedicinesDeliveryFinished(AMDM_DeliveryRecord record)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("即将拍照");
            Console.ResetColor();
            if (App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfMedicineBucket!= null)
            {
                string destFilePath = getCameraDestFilePath(record.Id, SnapshotLocationEnum.MedicineBucket, DateTime.Now, "jpg");
                //App.CameraSnapshotCapturer.SetCaptureParams(2, destFilePath);
                bool startCaptureRet = App.CameraSnapshotCapturer.CaptureSync(App.Setting.DevicesSetting.CCTVCaptureSetting.CameraChannelOfMedicineBucket, destFilePath, (img, file) =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("已经完成拍照");
                    Console.ResetColor();
                    //这里将会是异步调用

                    //截图完成后保存截图的路径信息
                    App.inventoryManager.FinishedDeliveryRecordSnapshotCapture(ref record, file);
                    //updateSnapshotImage(img);


                    Utils.LogInfo("正在打印付药单");
                    //Utils.LogInfo("付药单的开始时间:", record.StartTime);
                    //Utils.LogInfo("付药单的结束时间:", record.EndTime);
                    if (App.DebugCommandServer.DebuggerConnected && App.DebugCommandServer.Setting.SkipDeliveryRecordPaperRealPrinting)
                    {
                        Utils.LogInfo("已完成取药斗拍照.调试手柄已插入,已设定调试时不进行真实打印");
                    }
                    else
                    {
                        App.DeliveryRecordPaperPrinter.Print(record);
                    }
                    Utils.LogSuccess("付药单打印任务完成");

                    #region 提示当前药品已经出仓

                    this.currentMedicinesWaitBeenTakeADVideoIndex++;
                    if (this.currentMedicinesWaitBeenTakeADVideoIndex >= this.medicinesWaitBeenTakeADVideos.Count)
                    {
                        this.currentMedicinesWaitBeenTakeADVideoIndex = 0;
                    }
                    this.currentShowingPage = ShowingPageEnum.等待用户取走药品中;
                    string currentVideo = this.medicinesWaitBeenTakeADVideos[this.currentMedicinesWaitBeenTakeADVideoIndex];
                    //等待播放完毕后不能播放空闲广告,等待被取走的命令触发的时候这个就会关闭.
                    Utils.LogFinished("已经触发了所有药仓取完药品的事件,当前显示的[药品正在等待被取走的视频],药品已经取走的视频播放处,会关闭关闭这个窗口");
                    showFullScreenVideo(currentVideo, false, null);
                    #endregion
                });
                //启动拍照任务完成后会返回一个即将要存放的目标文件路径

                //把这个记录保存到数据库
                App.sqlClient.AddSnapshot(AMDM_Domain.SnapshotParentTypeEnum.DeliveryRecord, record.Id,
                    SnapshotTimePointEnum.AfterAction,
                        AMDM_Domain.SnapshotLocationEnum.MedicineBucket
                        , DateTime.Now, string.Format("取药完成药品正在出仓信号触发拍照:{0}", (startCaptureRet ? "已触发" : "启动拍照失败")), destFilePath);
            }
        }
        //delegate void UpdateSnapShotFunc(Bitmap img, PictureBox pb);
        //void updateSnapshotImage(Bitmap img, PictureBox pb)
        /// <summary>
        /// 弹窗显示拍到的图片
        /// </summary>
        /// <param name="img"></param>
        void updateSnapshotImage(Bitmap img)
        {
            Form form = new Form();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Size = new Size(800, 600);
            PictureBox pb = new PictureBox();
            form.Controls.Add(pb);
            pb.Dock = DockStyle.Fill;
            pb.Image = img;
            form.ShowDialog();

            //if (pb.InvokeRequired)
            //{
            //    Console.WriteLine("需要异步更新截图图片");
            //    UpdateSnapShotFunc fc = new UpdateSnapShotFunc(updateSnapshotImage);
            //    pb.BeginInvoke(fc, new object[] { img, pb });
            //}
            //else
            //{
            //    Console.WriteLine("不需要异步更新图片");
            //    //显示已经截到的图片
            //    this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            //    this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //    this.pictureBox1.Visible = true;
            //    if (this.pictureBox1.Image != null)
            //    {
            //        this.pictureBox1.Image.Dispose();
            //    }
            //    this.pictureBox1.Image = img;
            //}
        }

        #endregion

        #region 取药控制器当收到药品被取走的信号 和取药控制器收到错误信号.
        void medicinesGettingController_MedicinesHasBeenTaked(AMDM_DeliveryRecord record)
        {

            this.currentMedicinesHasBeenTakedADVideoIndex++;
            if (this.currentMedicinesHasBeenTakedADVideoIndex >= this.medicinesHasBeenTakedADVideos.Count)
            {
                this.currentMedicinesHasBeenTakedADVideoIndex = 0;
            }
            this.currentShowingPage = ShowingPageEnum.药品已经取走提醒用户确认药品中;
            string currentVideo = this.medicinesHasBeenTakedADVideos[this.currentMedicinesHasBeenTakedADVideoIndex];
            showFullScreenVideo(currentVideo, true, () =>
            {
                this.showAD();

                #region 如果自动取药测试机启动了的话.启动下一次继续取药的任务
                if (App.AutoMedicinesGettingTester != null 
                    //&& App.AutoMedicinesGettingTester.Working
                    && App.DebugCommandServer != null && App.DebugCommandServer.DebuggerConnected
                    )
                {
                    ThreadPool.QueueUserWorkItem((re) =>
                    {
                        System.Threading.Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(2000, 15000));
                        string newOrderId = App.AutoMedicinesGettingTester.CreateNewOrder();
                        if (newOrderId == null)
                        {
                            MessageBox.Show("没有库存");
                            App.AutoMedicinesGettingTester.Working = false;
                            return;
                        }
                        processScannerMsg(newOrderId);
                    }
                        );
                }
                #endregion
            });//等待播放完成后开始播放广告
            //this.showAD();
            //string msg = "给钱的药业提醒您,请核对处方,付药单及您取到的药品,一致后方可用药";
            //notice(msg);
        }

        void medicinesGettingController_OnAMDM_MedicinesGettingError(AMDMMedicinesGettingErrorEnum error, Dictionary<int, List<StockMedicinesGettingErrorEnum>> subErrors)
        {
            StringBuilder errorFull = new StringBuilder(error.ToString());
            errorFull.AppendLine();
            if (subErrors!= null)
            {
                foreach (var item in subErrors)
                {
                    int index = item.Key;
                    errorFull.AppendFormat("{0}号仓的错误信息:", index+1);
                    errorFull.AppendLine();
                    List<StockMedicinesGettingErrorEnum> errors = item.Value;
                    foreach (var err in errors)
                    {
                        errorFull.Append(err.ToString());
                        errorFull.AppendLine();
                    }
                }
            }
            string errFullStr = errorFull.ToString();
            //推送给his系统错误消息的结果是否成功
            
            //置于故障状态
            if (App.Setting.TroubleshootingPlanSetting.DisableAMDMWhenDeliveryFailed)
            {
                App.ControlPanel.SetNewMaintenanceStatus(MaintenanceStatusEnum.硬件故障);
                this.setLabelTextByMaintenanceStatus(MaintenanceStatusEnum.硬件故障);
            }
            showMessageOnScreen("错误", "取药过程发生故障,请联系工作人员",true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
            App.AlertManager.AlertHardwareError("取药过程发生错误", errFullStr);
            //取药发生故障以后 就不能播放广告了 还是是说 取药发生了故障但是不要影响播放广告?
            //this.startAD();
        }
        #endregion
        #endregion

        #region 根据主状态的类型自动设置状态文本
        void setLabelTextByMaintenanceStatus(MaintenanceStatusEnum status)
        {
            switch (status)
            {
                case MaintenanceStatusEnum.运行正常:
                    this.setLabelText(this.noticeScanLabelNoticeDefaultText, Color.Black);
                    break;
                case MaintenanceStatusEnum.硬件故障:
                case MaintenanceStatusEnum.软件故障:
                case MaintenanceStatusEnum.系统维护中:
                    this.setLabelText(this.noticeScanLabelNoticeNotUseText, Color.DarkRed);
                    break;
                case MaintenanceStatusEnum.紫外线杀菌中:
                    this.setLabelText(this.noticeScanLabelNoticeUVLampTurnOn, Color.OrangeRed);
                    break;
                case MaintenanceStatusEnum.紫外线杀菌准备中:
                    this.setLabelText(this.noticeScanLabelNoticeUVLampPreparing, Color.Orange);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 故障触发
        delegate void setLabelTextFunc(string text, Color foreColor);
        void setLabelText(string text, Color foreColor)
        {
            if (this.label1.InvokeRequired)
            {
                setLabelTextFunc fc = new setLabelTextFunc(setLabelText);
                this.label1.BeginInvoke(fc, text, foreColor);
                return;
            }
            this.label1.Text = text;
            this.label1.ForeColor = foreColor;
        }
        void MonitorsManager_OnMonitorDetectedError(object monitor, MonitorDetectedErrorTypeEnum errType)
        {
            this.setLabelTextByMaintenanceStatus(App.ControlPanel.MaintenanceStatus); 
        }

        void ControlPanel_OnControlPanelClearErrorOfMaintenanceStatus()
        {
            this.setLabelTextByMaintenanceStatus(App.ControlPanel.MaintenanceStatus);
            this.showMessageOnScreen("提示", "该药机已恢使用,请扫描处方二维码开始取药", true, App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS);
        }
        #endregion

        #region 紫外线灯状态变化触发
        void UVLampManager_OnUVLampStatusChanged(Nullable<MaintenanceStatusEnum> preStatus)
        {
            if (App.ControlPanel.MaintenanceStatus == MaintenanceStatusEnum.紫外线杀菌中 || App.ControlPanel.MaintenanceStatus == MaintenanceStatusEnum.紫外线杀菌准备中)
            {
                this.setLabelTextByMaintenanceStatus(App.ControlPanel.MaintenanceStatus);
            }
            else if(preStatus!= null)
            {
                this.setLabelTextByMaintenanceStatus(preStatus.Value);
            }
        }
        #endregion

        #region 交互页面的显示
        delegate void ShowFullScreenVideoFunc(string file, bool closeAfterVideoFinished, Action onClose);

        delegate void noticeFunc(string msg);


        #region 记录日志信息并且显示信息在屏幕上的函数 现已不再使用 使用单独的ShowMessageOnScreen 和 App.LogServerServicePipe来显示和发送消息
        ///// <summary>
        ///// 显示并且记录提示信息到窗体以及log服务器  已经作废不再使用 2022年1月1日21:41:55
        ///// 
        ///// </summary>
        ///// <param name="title"></param>
        ///// <param name="msg"></param>
        ///// <param name="speakSync"></param>
        ///// <param name="delayMS"></param>
        ///// <param name="report2LogServer"></param>
        //void notice_bak(string title, string msg, bool speakSync, Nullable<int> delayMS, bool report2LogServer)
        //{
        //    showMessageOnScreen(msg, title,speakSync, delayMS == null ? App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS : delayMS.Value);

        //    #region 如果需要记录到日志服务器
        //    if (report2LogServer)
        //    {
        //        try
        //        {
        //            LogMsg logmsg = new LogMsg();
        //            logmsg.Title = title;
        //            logmsg.Msg = msg;
        //            App.LogServerServicePipeClient.Send(logmsg);
        //        }
        //        catch (Exception err)
        //        {
        //            Utils.LogError("在取药页面需要展示消息并记录到日志服务器,记录到日志服务器时发生错误:", err.Message);
        //        }
        //    }
        //    #endregion
        //}

        #endregion
        
        
        /// <summary>
        /// 全屏显示视频
        /// </summary>
        /// <param name="file">要显示的视频文件</param>
        /// <param name="closeAfterVideoFinished">播放完一次视频以后是否自动关闭</param>
        /// <param name="waitClose">是否等待视频关闭,如果不是,就是等待外部的信号关闭本类中的currentTopmostFullScreenVideoShower</param>
        void showFullScreenVideo(string file, bool closeAfterVideoFinished, Action onHide)
        {
            if (this.InvokeRequired)
            {
                ShowFullScreenVideoFunc fc = new ShowFullScreenVideoFunc(showFullScreenVideo);
                this.BeginInvoke(fc, new object[] { file, closeAfterVideoFinished, onHide });
                return;
            }
            //if (currentTopmostFullScreenVideoShower != null)
            //{
            //    currentTopmostFullScreenVideoShower.Close();
            //}
            //currentTopmostFullScreenVideoShower = new FullScreenVideoShowerV2();
            //this.currentTopmostFullScreenVideoShower.HideAfterFinished = closeAfterVideoFinished;
            //this.currentTopmostFullScreenVideoShower.VideoFilePath = file;
            //this.currentTopmostFullScreenVideoShower.OnHided = onClose;
            //this.currentTopmostFullScreenVideoShower.Show();

            #region 使用新版本 2021年12月25日11:11:33

            //if (currentTopmostFullScreenVideoShower== null)
            //{
            //    currentTopmostFullScreenVideoShower = new FullScreenVideoShowerV2();
            //}
            currentTopmostFullScreenVideoShower.HideAfterFinished = closeAfterVideoFinished;
            currentTopmostFullScreenVideoShower.OnHided = onHide;
            currentTopmostFullScreenVideoShower.Show();
            currentTopmostFullScreenVideoShower.VideoFilePath = file;
            #endregion
        }
        #endregion

        #region 页面事件
        #region 加载和关闭

        #region 窗体加载
        //VlcPlayerBase player = null;

        private void MedicineDeliveryForm_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region 正在关闭窗口的时候检测取药机是否正在工作 如果没有任务了才可以关闭
        private void MedicineDeliveryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (App.medicinesGettingController.MedicinesGetting)
            {
                MessageBox.Show(this, "当前有取药任务尚未完成\r\n请等待取药结束后再关闭窗口", "请稍等", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
        #endregion

        #region 窗体已关闭
        public void MedicineDeliveryForm_FormClosed(object sender, EventArgs e)
        {
            App.medicinesGettingController.OnAllMedicinesDeliveryFinished -= medicinesGettingController_AllMedicinesDeliveryFinished;
            App.medicinesGettingController.OnMedicinesHasBeenTaked -= medicinesGettingController_MedicinesHasBeenTaked;
            App.medicinesGettingController.OnMedicinesGettingError -= medicinesGettingController_OnAMDM_MedicinesGettingError;
            App.MonitorsManager.OnMonitorDetectedError -= this.MonitorsManager_OnMonitorDetectedError;
            App.ControlPanel.OnControlPanelClearErrorOfMaintenanceStatus -= this.ControlPanel_OnControlPanelClearErrorOfMaintenanceStatus;
            App.UVLampManager.OnUVLampStatusChanged -= this.UVLampManager_OnUVLampStatusChanged;

            App.ICCardReaderAndCodeScanner2in1ReceivedData = null;
            App.medicinesGettingController.Dispose();
            if (this.player != null)
            {
                //Utils.LogSuccess("窗体关闭的时候,player的hwnd为:", this.player == null ? IntPtr.Zero : this.player.Hwnd);
                //this.player.Hwnd = IntPtr.Zero;
                //Utils.LogSuccess("窗体关闭时,已经清空了player的hwnd");

                //player绑定到了某个控件的hwnd上的时候，窗体关闭就自动释放player，这里不能再调用stop或者是dispose
                try
                {
                    Utils.LogInfo("定义停止和释放播放器的线程回调任务");
                    ThreadPool.QueueUserWorkItem((res) =>
                    {
                        Utils.LogSuccess("在线程池中停止和释放播放器");
                        this.player.Stop();
                        this.player.EndReached -= this.player_EndReached;
                        if (App.VLCPlayer == null)
                        {
                            //是本类中生成的player
                            this.player.Dispose();
                        }
                        Utils.LogFinished("已经在线程池中释放和结束了player", res);
                    });
                }
                catch (Exception err)
                {
                    Utils.LogError("播放器停止播放和释放中发生了错误", err.Message, err.StackTrace);
                }
            }

            //this.player.Stop();
            //this.player.EndReached -= this.player_EndReached;
            //this.player.Dispose();
            //System.Threading.Thread.Sleep(1000);
            //this.player.Release();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
        #endregion

        #region 交互

        #region 点击上药按钮
        List<DateTime> clickAdminBtnTime = new List<DateTime>();
        bool checkClicks(double durationSeconds, int needClicks)
        {
            DateTime now = DateTime.Now;
            if (clickAdminBtnTime.Count > 0 && clickAdminBtnTime[clickAdminBtnTime.Count - 1].AddSeconds(durationSeconds) < now)
            {
                //太长时间没有点了
                clickAdminBtnTime.Clear();
            }
            clickAdminBtnTime.Add(now);
            if (clickAdminBtnTime.Count >= needClicks)
            {
                var last = clickAdminBtnTime[clickAdminBtnTime.Count - 1];
                var last3 = clickAdminBtnTime[clickAdminBtnTime.Count - needClicks];
                if ((last - last3).TotalSeconds <= durationSeconds)
                {
                    clickAdminBtnTime.Clear();
                    return true;
                }
                else
                {
                    clickAdminBtnTime.Clear();
                    return false;
                }
            }
            return false;
        }
        private void adminBtn_Click(object sender, EventArgs e)
        {
            if (checkClicks(2,3)== false)
            {
                return;
            }
            this.stopAD();
            this.currentShowingPage = ShowingPageEnum.上药中;
            LoginForm lform = new LoginForm();
            DialogResult ret = lform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                this.showAD();
                return;
            }
            LoginResponse rsp = App.UserManager.Login(lform.User, lform.Pass);
            if (rsp.IsError)
            {
                MessageBox.Show(this,rsp.ErrMsg,"错误");
                this.showAD();
                return;
            }
            AMDM_Stock index0Stock = null;
            #region 加载给定的药仓数据
            try
            {
                index0Stock = App.stockLoader.LoadStock(0);
            }
            catch (Exception loadStockErr)
            {
                MessageBox.Show(this, string.Format("读取药仓信息失败!{0}", loadStockErr.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.showAD();
                return;
            }

            if (index0Stock == null)
            {
                MessageBox.Show(this, "读取药仓信息失败!数据读取未发生错误,读取到的结果为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.showAD();
                return;
            }
            #endregion
            MedicineInventoryManageForm mform = new MedicineInventoryManageForm();
            mform.StartPosition = FormStartPosition.CenterParent;
            mform.Init(index0Stock);
            mform.ShowDialog();

            App.ICCardReaderAndCodeScanner2in1ReceivedData = this.processScannerMsg;

            this.showAD();
        }
        #endregion
        #region 点击这里是直接测试启动连续任务
        private void label1_Click(object sender, EventArgs e)
        {
            //if (App.AutoMedicinesGettingTester != null 
            //    //&& App.AutoMedicinesGettingTester.Working == false
            //    && App.DebugCommandServer != null && App.DebugCommandServer.DebuggerConnected
            //    )
            //{
            //    string orderIdStr = App.AutoMedicinesGettingTester.CreateNewOrder();
            //    processScannerMsg(orderIdStr);
            //}
            //else
            //{
            //    Utils.LogBug("没有开启功能或正在工作:working?", App.AutoMedicinesGettingTester);
            //}
        }
        #endregion
        #region 模拟扫描二维码
        private void simulateScanQRCodeBtn_Click(object sender, EventArgs e)
        {
            testScanBarcodeThread = new Thread(threadFunc);
            testScanBarcodeThread.Start();
        }
        void threadFunc()
        {
            processScannerMsg("747");
            if (testScanBarcodeThread != null)
            {
                //testScanBarcodeThread.Abort();
            }
        }
        System.Threading.Thread testScanBarcodeThread;
        #endregion
        #endregion
        #endregion

        #region 公共函数
        /// <summary>
        /// 获取当前正在显示的页面
        /// </summary>
        /// <returns></returns>
        public ShowingPageEnum GetShowingPage()
        {
            return this.currentShowingPage;
        }
        #endregion

        //双击的时候因为只会捕捉到一次click所以加上一次
        private void adminBtn_DoubleClick(object sender, EventArgs e)
        {
            clickAdminBtnTime.Add(DateTime.Now);
            if (clickAdminBtnTime.Count >=100)
            {
                clickAdminBtnTime.Clear();
            }
        }

        private void adminBtn_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
