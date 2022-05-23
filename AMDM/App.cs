using AMDM_Domain;
using AMDM.Manager;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using EasyNamedPipe;
using System.Reflection;
using CCTVCapturer;
//using SpeechLib; 2021年11月24日10:38:06  即刻起 使用.net3.5以后 程序集内都包含的  system.speech的程序集
//因为之前的speechlib不稳定 是从.net2.0里面弄出来的  会在多次播放音频文件然后再启动的时候触发msvcrt的app异常 应用程序会直接崩溃

namespace AMDM
{
    /// <summary>
    /// 程序全局控制器,定义整个程序中都需要用到的数据和管理器
    /// </summary>
    public class App
    {
        #region 初始化app
        #region 私有函数,更新页面或者控制台,输出开始初始化结束初始化,全部完成等的页面更新函数

        #region 向控制台和指定的ReportStarted回调函数报告已经启动了某项任务
        /// <summary>
        /// 向控制台和指定的onstring回调函数报告已经启动了某项任务
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="objs"></param>
        static void ReportStarted(string msg, params object[] objs)
        {
            Utils.LogStarted(msg, objs);
            if (OnStartInitComponent != null)
            {
                OnStartInitComponent(msg);
            }
        }
        #endregion
        #region 向控制台和指定的ReportFinished回调函数报告已经完成了某项任务

        static void ReportFinished(string msg, params object[] objs)
        {
            Utils.LogFinished(msg, objs);
            if (OnInitedComponent != null)
            {
                OnInitedComponent(msg);
            }
        }

        #endregion
        #region 向控制台和指定的ReportAllDone回调函数报告已经完成了全部的初始化
        /// <summary>
        /// 报告给控制台和指定的回调函数,通知已经完成了所有的初始化任务
        /// </summary>
        static void ReportAllDone()
        {
            if (OnInitFinished != null)
            {
                OnInitFinished();
            }
        }
        #endregion

        #endregion

        #region 函数变量 本类中保存的,提供外部设置时候赋值的 当开始初始化组件,当初始化组件完成 当初始化所有组件完成的回调函数 以action类型定义
        static Action<string> OnStartInitComponent;
        static Action<string> OnInitedComponent;
        static Action OnInitFinished;
        #endregion

        /// <summary>
        /// 初始化全局变量,每完成一部都会触发一次回调函数
        /// </summary>
        /// <param name="onInitedComponent"></param>
        public static void Init(Action<string> onStartInitComponent, Action<string> onInitedComponent, Action onInitFinished,
            bool initDevices = true,
            bool initHisConnector = true,
            bool initVideoPlayer = true,
            bool initLogServerClient = true
            )
        {
            if (Setting != null)
            {
                Utils.LogInfo("已经进行过app对象的初始化");
                return;
            }
            Setting = new AMDMSetting();


            #region 设置回调函数

            OnStartInitComponent = onStartInitComponent;
            OnInitedComponent = onInitedComponent;
            OnInitFinished = onInitFinished;

            #endregion

            #region 加载设置文件

            string settingPath = Application.StartupPath + "\\setting.json";
            string settingFileContent = Setting.Load(settingPath);
            if (string.IsNullOrEmpty(settingFileContent))
            {
                MessageBox.Show("读取设置文件失败,将使用默认配置文件\r\n如需修改相关配置参数,可进入系统设置或编辑Setting.json文件", "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                App.Setting.Save();
            }
            ReportFinished("已完成设置加载");
            #endregion

            #region 设置重写工具的参数读取
            String[] args = System.Environment.GetCommandLineArgs();
            DevDebugSettingRewriter.GetSetting(args);
            if (DevDebugSettingRewriter.Setting != null)
            {
                App.Setting.PlcSetting_台达.MainPLC.TCPModeIP = DevDebugSettingRewriter.Setting.PlcIP;
                App.Setting.PlcSetting_台达.MainPLC.TCPModePort = DevDebugSettingRewriter.Setting.PlcPort;
                App.Setting.SqlConfig.IP = DevDebugSettingRewriter.Setting.AMDMClientIP;
                App.Setting.AMDMServerSDKSetting.IP = DevDebugSettingRewriter.Setting.AMDMServerIP;
                App.Setting.AMDMServerSDKSetting.Port = DevDebugSettingRewriter.Setting.AMDMServerSQLPort;
            }
            #endregion

            #region 在完成了设置加载以后,如果更换了版本的时候,有些字段可能没有值,需要重新初始化一下.不然就要删除设置文件,但是这里的代码只需要执行一次,setting文件中有了设置以后就可以注释掉,使用setting文件里面保存的设置
            ////Setting.PlcSetting_西门子 = new PLCSetting<MainPLCSetting西门子, StockPLCSetting西门子>();
            //Setting.PlcSetting_台达.MainPLC.PLCLib = PLCLibEnum.EasyModbusTCP;
            //Setting.PlcSetting_台达.MainPLC.TCPModeIP = "192.168.2.192";
            //Setting.PlcSetting_台达.MainPLC.TCPModePort = 20108;
            #endregion

            #region his系统通讯/连接器
            if (initHisConnector)
            {
                ReportStarted("初始化HIS系统连接器");
                bool loadHISDllRet = LoadHISConnectorDLLFile();
                if (loadHISDllRet == false)
                {
                    System.Threading.Thread.Sleep(2000);
                    Application.Exit();
                    return;
                }
            }
            #endregion


            #region 数据库服务器连接

            ReportStarted("初始化数据库连接器");
            sqlClient = new SQLDataTransmitter(Setting.SqlConfig.IP, Setting.SqlConfig.User, Setting.SqlConfig.Pass, Setting.SqlConfig.Database, Setting.SqlConfig.Port);
            ReportStarted("连接数据服务器");
            string sqlVersion = null;
            try
            {
                List<string> versions = sqlClient.MysqlSelectValue<string>("select version()");
                if (versions != null && versions.Count == 1)
                {
                    sqlVersion = versions[0];
                    Utils.LogSuccess("数据服务器版本:", sqlVersion);
                }
            }
            catch (Exception err)
            {
                string msg = "连接数据服务器错误";
                ReportFinished(msg, err.Message);
                System.Threading.Thread.Sleep(2000);
                Application.Exit();
                return;
            }
            Utils.LogFinished("数据服务器连接成功");

            #endregion
            ReportStarted("初始化用户管理器");
            UserManager = new UserManager(App.Setting.AMDMServerSDKSetting.IP, App.Setting.AMDMServerSDKSetting.User, App.Setting.AMDMServerSDKSetting.Pass, App.Setting.AMDMServerSDKSetting.Database, App.Setting.AMDMServerSDKSetting.Port);
            ReportStarted("初始化钥匙串");
            KeyManager = new KeyManager();
            ReportStarted("初始化药仓加载器");
            stockLoader = new AMDMStockLoader(sqlClient);
            #region 初始化machine对象
            ReportStarted("加载药机和药仓信息");
            try
            {
                App.machine = new AMDM_Machine();
                Newtonsoft.Json.JsonConvert.PopulateObject(settingFileContent, App.machine);
                App.machine.Stocks = stockLoader.LoadStocks();
            }
            catch (Exception err)
            {
                ReportFinished("加载药机或药仓信息失败", err.Message, App.machine.Stocks);
                System.Threading.Thread.Sleep(2000);
                Application.Exit();
            }
            ReportFinished("药机信息及药仓信息加载完成");

            #endregion

            ReportStarted("初始化药品信息绑定管理器(库存记录器)");
            bindingManager = new GridMedicineBiddingManager(sqlClient);
            ReportStarted("初始化库存管理器");
            inventoryManager = new InventoryManager(sqlClient);
            ReportStarted("初始化药品管理器");
            medicineManager = new MedicineManager(sqlClient);
            ReportStarted("初始化控件线型动画播放器");
            ControlAnimationRenderingController = new ControlAnimationRenderingController();


            if (initDevices)
            {
                #region 二维码和IC卡读头
                ReportStarted("初始化二合一码卡读头");
                ICCardReaderAndCodeScanner2in1 = new SerialPort(
                    Setting.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.portName, Setting.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.baudRate, Setting.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.parity,
                    Setting.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.dataBits
                    );
                try
                {
                    ICCardReaderAndCodeScanner2in1.Open();
                    string msg = string.Format("码卡读头串口已打开 @ {0}", ICCardReaderAndCodeScanner2in1.PortName);
                    ReportFinished(msg);
                }
                catch (Exception openScannerErr)
                {
                    string msg = string.Format("打开码卡读头串口失败:{0}", openScannerErr.Message);
                    ReportFinished(msg);
                }
                ICCardReaderAndCodeScanner2in1.DataReceived += ICCardReaderAndCodeScanner2in1_DataReceived;
                #endregion


                #region 药仓上的485数字显示屏
                //StocksGridNumber485ShowersManager = new Manager.StocksGridNumber485ShowersManager();

                //try
                //{
                //    if(StocksGridNumber485ShowersManager.Init(Setting.DevicesSetting.StocksGridNumber485ShowerSetting.portName,
                //    Setting.DevicesSetting.StocksGridNumber485ShowerSetting.baudRate,
                //    Setting.DevicesSetting.StocksGridNumber485ShowerSetting.parity,
                //    Setting.DevicesSetting.StocksGridNumber485ShowerSetting.dataBits))
                //    {
                //        StocksGridNumber485ShowersManager.Open();
                //        if (StocksGridNumber485ShowersManager.IsOpen)
                //        {
                //            ReportFinished("药仓上方药槽序号显示器端口已打开");
                //        }
                //        else
                //        {
                //            ReportFinished("药仓上方药槽序号显示器端 打开失败!!");
                //        }
                //    }
                //    else
                //    {
                //        ReportFinished("药仓上方药槽序号显示器端 初始化失败");
                //    }

                //}
                //catch (Exception err)
                //{
                //    string msg = string.Format("药仓上方药槽序号显示器端口 开启失败 :{0}", err.Message);
                //    ReportFinished(msg);
                //}
                #endregion



                #region 摄像头抓图组件 已经不再使用
                //ReportStarted("初始化摄像头");

                //if (string.IsNullOrEmpty(Setting.DevicesSetting.CameraSetting.CameraMonikerStringName) == true)
                //{
                //    CameraSnapshotCapturer = new CameraSnapshotCapturer(null);
                //    ReportFinished("未获取到设置中的摄像头设置,即将使用所有可用摄像头设备中的第1个设备的第1中编码方式启动摄像头");
                //}
                //else
                //{
                //    CameraSnapshotCapturer = new CameraSnapshotCapturer(Setting.DevicesSetting.CameraSetting.CameraMonikerStringName);
                //}
                #region 摄像头 2022年2月12日13:40:02 使用海康威视新的抓图方式
                ReportStarted("初始化图像采集组件");
                CameraSnapshotCapturer = new HIKVISION_CCTVCapturerV2("192.168.2.141", 8000, "admin", "yuntong8888");
                #endregion
                bool connectSnapCaptureRet = CameraSnapshotCapturer.Connect();
                ReportFinished(string.Format("初始化图像采集组件完成:{0}", connectSnapCaptureRet ? "已连接" : "连接失败"));
                //System.Threading.Thread.Sleep(5000);
                //CameraSnapshotCapturer.SetCaptureParams(4, "d:\\123.jpg");
                //CameraSnapshotCapturer.CaptureSync((img, path) => 
                //{

                //});
                #endregion

                #region 付药单打印机
                ReportStarted("初始化付药单打印机", Setting.DevicesSetting.Printer58MMSetting);
                DeliveryRecordPaperPrinter = new Manager.DeliveryRecordPaperPrinter();
                if (DeliveryRecordPaperPrinter != null)
                {
                    if (DeliveryRecordPaperPrinter.PrinterStatus != PrinterStatusEnum.空闲)
                    {
                        Utils.LogWarnning("当前打印机不在空闲中", DeliveryRecordPaperPrinter.PrinterStatus);
                    }
                    else
                    {
                        Utils.LogSuccess("打印机状态:", DeliveryRecordPaperPrinter.PrinterStatus);
                    }
                    ReportFinished("初始化付药单打印机完成");
                }
                else
                {
                    ReportFinished("初始化付药单打印机失败");
                }


                #endregion
            }

            if (initVideoPlayer)
            {

                #region 初始化视频播放组件
                ReportStarted("初始化视频组件");
                try
                {
                    VLCHandler = new LibVLC();
                    //VLCHandler.SetLog(vlcLogCallback);

                    if (VLCHandler != null)
                    {
                        //VLCPlayer = new MediaPlayer(VLCHandler);
                    }
                    //LibVLCSharp.Shared.LibVLC vlc = new LibVLCSharp.Shared.LibVLC();
                    //vlc.SetLog(vlcLogCallback);
                    //vlc.Dispose();
                }
                catch (Exception initVlcError)
                {
                    ReportFinished("初始化视频组件错误", initVlcError.Message, initVlcError.StackTrace);
                }
                ReportFinished("初始化视频组件完成");

                #region 语音朗读引擎
                #region 使用动态加载com组件的方法 也是使用的sapi.speech,不行 会出问题 作废
                //try
                //{
                //    Utils.LogInfo("尝试使用com方式初始化语音朗读引擎");
                //    Type type = Type.GetTypeFromProgID("SAPI.SpVoice");
                //    dynamic spVoice = Activator.CreateInstance(type);
                //    spVoice.Speak("欢迎使用!");
                //    Utils.LogInfo("已播放欢迎使用");
                //}
                //catch (Exception err)
                //{
                //    Utils.LogError(err.Message, err.StackTrace);
                //}
                #endregion
                #region 使用直接加载 speechlib.dll的方式  不行 会出问题 作废. 这个dll是从.net2.0过来的 虽然在win10里面没问题 但是win7会引发msvcrt.dll的文件异常
                //msvcrt出 0xc0000005的错误
                //try
                //{
                //    Utils.LogInfo("尝试使用dll引用speech的lib方式初始化语音朗读引擎");
                //    TTSSpeaker = new Speaker(App.Setting.TTSSpeakerVoice, false);
                //    List<string> voices = TTSSpeaker.GetVoices();
                //    if (voices.Count > 0)
                //    {
                //        foreach (var item in voices)
                //        {
                //            Utils.LogSuccess("获取到语音朗读引擎包:", item);
                //        }
                //    }
                //    //TTSSpeaker.Speak("语音引擎初始化完成");
                //    Console.ForegroundColor = ConsoleColor.DarkGreen;
                //    Console.WriteLine("初始化语音朗读引擎完成");
                //    Console.ResetColor();
                //}
                //catch (Exception err)
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.WriteLine("初始化语音引擎失败:{0}", err.Message);
                //    Console.ResetColor();
                //    //throw;
                //}
                #endregion
                #region 新的可用的 2021年11月24日11:03:10
                TTSSpeaker = new Manager.TTSSpeaker(App.Setting.TTSSpeakerVoice, false);
                if (TTSSpeaker != null)
                {
                    string currentVoice = TTSSpeaker.GetCurrentSelectedVoice();
                    Utils.LogInfo("当前使用的语音引擎:", currentVoice);
                    string oldVoice = App.Setting.TTSSpeakerVoice;
                    Utils.LogInfo("设置中的语音引擎:", oldVoice);
                    if (currentVoice != null && currentVoice != oldVoice)
                    {
                        App.Setting.TTSSpeakerVoice = currentVoice;
                        App.Setting.Save();
                        Utils.LogSuccess("已经更新所选择的语音朗读引擎设置,已保存设置", currentVoice, oldVoice);
                    }
                }
                try
                {
                    TTSSpeaker.Speak("欢迎使用");
                }
                catch (Exception speakWelcomeErr)
                {
                    Utils.LogError("播放欢迎使用的TTS错误:", speakWelcomeErr.Message);
                }
                ReportFinished("初始化语音朗读引擎完成");
                #endregion
                #endregion

                #endregion
            }

            #region 机械控制器
            ReportStarted("初始化机械控制器");
            try
            {
                medicinesGettingController = new MedicinesGettingController();
                medicinesGettingController.Init(App.machine.Stocks);
                var status = medicinesGettingController.MainPLCCommunicator.GetMedicineGettingStatus();
                if (status == null)
                {
                    ReportFinished("机械控制器连接失败");
                    System.Threading.Thread.Sleep(2000);
                    Application.Exit();
                    return;
                }
            }
            catch (Exception err)
            {
                string msg = "机械控制器初始化错误";
                ReportFinished(msg, err.Message);
                System.Threading.Thread.Sleep(2000);
                Application.Exit();
                return;
            }
            ReportFinished("机械控制器初始化完成");
            #endregion

            if (initLogServerClient)
            {
                #region 日志服务器(看门狗)管道消息连接器
                ReportStarted("初始化日志报告组件");
                try
                {
                    LogServerServicePipeClient = new PipeClient(App.Setting.LogServerServiceSetting.PipeServerLocation, App.Setting.LogServerServiceSetting.PipeServerName);
                    LogServerServicePipeClient.Connect(5000);
                }
                catch (Exception err)
                {
                    ReportFinished("初始化日志报告组件失败", err.Message);
                }
                ReportFinished("初始化日志报告组件完成");
                #endregion

                #region 警告信息管理器/错误管理器
                AlertManager = new AlertManager();
                #endregion
            }
            #region 状态监控器和日志报送器,使用管道连接器
            //ReportStarted("初始化状态检测器");
            //try
            //{
            //    StatusCheckAndReportor = new Manager.StatusCheckAndReportor();
            //    StatusCheckAndReportor.Start();
            //}
            //catch (Exception err)
            //{
            //    ReportFinished("初始化状态检测器失败:", err.Message);
            //    throw;
            //}
            //ReportFinished("初始化状态检测器完成");
            #endregion

            #region 控制面板
            ReportStarted("控制面板初始化");
            try
            {
                ControlPanel = new ControlPanel();
                ControlPanel.ReloadStatus();
                string statusString = ControlPanel.PeripheralsStatus.GetDescriptionString();
                ReportFinished(statusString);
            }
            catch (Exception controlPanelInitErr)
            {
                ReportFinished("初始化控制面板错误", controlPanelInitErr.Message);
            }
            #endregion

            #region 紫外线杀菌灯控制器
            ReportStarted("紫外线杀菌控制器初始化");
            UVLampManager = new UVLampManager();
            ReportFinished("紫外线杀菌控制器初始化完成");
            #endregion

            #region 控制面板对外接口服务器初始化
            ReportStarted("初始化控制面板接口服务器");
            try
            {
                ControlPanelInterfaceServer = new ControlPanelInterfaceServer();
                ControlPanelInterfaceServer.Start();
            }
            catch (Exception startCPISError)
            {
                ReportFinished("启动控制面板接口服务器失败", startCPISError.Message);
                System.Threading.Thread.Sleep(2000);
                Application.Exit();
            }
            ReportFinished("启动控制面板接口服务器成功");
            //System.Threading.Thread.Sleep(2000);

            #endregion

            #region 外部调试命令接收器
            DebugCommandServer = new DebugCommandServer();
            #endregion

            ReportStarted("监测器管理器初始化");
            MonitorsManager = new MonitersManager();
            ReportFinished("监测器管理器初始化完成");
            ReportFinished("管理器及外设全部初始化完成");
            ReportAllDone();
            App.Setting.Save();
        }
        #endregion
        #region 销毁APP
        public static void Dispose()
        {
            try
            {
                if (CameraSnapshotCapturer != null)
                {
                    CameraSnapshotCapturer.Disconnect();
                    CameraSnapshotCapturer.Dispose();
                }
            }
            catch (Exception err)
            {
                Utils.LogError("摄像头组件释放错误:", err.Message);
            }
            try
            {
                if (ICCardReaderAndCodeScanner2in1 != null)
                {
                    ICCardReaderAndCodeScanner2in1.DataReceived -= ICCardReaderAndCodeScanner2in1_DataReceived;
                }
            }
            catch (Exception err)
            {
                Utils.LogError("IC+QR读头组件释放错误:", err.Message);
            }
            try
            {
                if (TTSSpeaker != null)
                {
                    TTSSpeaker.Dispose();
                }
            }
            catch (Exception err)
            {
                Utils.LogError("TTS组件释放错误:", err.Message);
            }
            try
            {
                if (ControlPanelInterfaceServer != null)
                {
                    ControlPanelInterfaceServer.Stop();
                    ControlPanelInterfaceServer.Dispose();
                }
            }
            catch (Exception err)
            {
                Utils.LogError("控制面板接口服务器组件释放错误:", err.Message);
            }
            //try
            //{
            //    if (StatusCheckAndReportor != null)
            //    {
            //        StatusCheckAndReportor.Stop();
            //        StatusCheckAndReportor.Dispose();
            //    }
            //}
            //catch (Exception err)
            //{
            //    Utils.LogError("状态检查及报送组件释放错误:", err.Message);
            //}
            try
            {
                if (VLCHandler != null)
                {
                    VLCHandler.UnsetLog();
                    VLCHandler.Dispose();
                }
                if (VLCPlayer != null)
                {
                    VLCPlayer.Dispose();
                }
            }
            catch (Exception err)
            {
                Utils.LogError("VLC组件释放错误:", err.Message);
            }
            try
            {
                if (App.MonitorsManager != null)
                {
                    App.MonitorsManager.Dispose();
                }
            }
            catch (Exception err)
            {
                Utils.LogError("监测器组件释放错误:", err.Message);
            }
        }
        #endregion
        #region 全局app设置对象
        /// <summary>
        /// 全局app设置.
        /// </summary>
        public static AMDMSetting Setting;// = new Setting();
        //public static MedicinePLCSetting plcSetting = new MedicinePLCSetting();
        #endregion
        #region 组件变量和组件相关的处理函数
        #region sql客户端

        public static SQLDataTransmitter sqlClient;// = new SQLDataTransmitter(Setting.SqlConfig.IP,Setting.SqlConfig.User,Setting.SqlConfig.Pass,Setting.SqlConfig.Database, Setting.SqlConfig.Port);

        #endregion
        #region 药槽和药品的绑定管理器
        public static GridMedicineBiddingManager bindingManager;// = new GridMedicineBiddingManager(sqlClient);
        #endregion
        #region 药品库存管理器
        public static InventoryManager inventoryManager;// = new InventoryManager(sqlClient);
        #endregion
        #region 付药机对象
        public static AMDM_Machine machine;// = null;
        #endregion
        #region 药品信息管理器
        public static MedicineManager medicineManager;// = new MedicineManager(sqlClient);
        #endregion
        #region 药仓加载器
        public static AMDMStockLoader stockLoader;
        #endregion
        #region 控件的动画渲染控制器
        public static ControlAnimationRenderingController ControlAnimationRenderingController;// = new ControlAnimationRenderingController();
        #endregion
        #region HIS系统连接器,通讯器
        #region 加载HIS系统dll
        static bool LoadHISConnectorDLLFile()
        {
            #region 获取已经加载的程序集,如果已经加载的程序集里面已经有继承了  ihis...接口的,就使用已经加载的dll,否则从文件调用 (优先使用已加载的程序集能保证调试的时候使用最新代码)
            //这样的方式并不能获取到 因为没有调用dll中的相关方法,所以c#并不会加载这个程序集.
            //Assembly localAsm = Assembly.GetExecutingAssembly();
            //////var currentASMs = System.AppDomain.CurrentDomain.GetAssemblies();
            //////foreach (var item in currentASMs)
            //////{
            //////    if (item.FullName.ToLower().Contains("his") == false)
            //////    {
            //////        continue;
            //////    }
            //////    var types = item.GetTypes();
            //////    foreach (var type in types)
            //////    {
            //////        if (type.IsInterface) continue;//判断是否是接口
            //////        Type[] ins = type.GetInterfaces();
            //////        foreach (Type ty in ins)
            //////        {
            //////            if (ty == typeof(IHISServerConnector))
            //////            {
            //////                HISServerConnector = item.CreateInstance(ty.FullName) as IHISServerConnector;
            //////                return true;
            //////            }
            //////        }
            //////    }
            //////}
            #endregion
            try
            {
                //string dllFile = Application.StartupPath + "\\FakeHISServerConnector.dll";
                string dllFile = App.Setting.HISServerConnectorDllFilePath;
                if (dllFile == null)
                {
                    ReportFinished("HIS连接器文件位置未知");
                    return false;
                }
                dllFile = System.IO.Path.GetFullPath(dllFile);
                if (System.IO.File.Exists(dllFile) == false)
                {
                    ReportFinished("未能加载HIS系统连接器,不存在该文件", dllFile);
                    return false;
                }
                else
                {
                    //通过加载文件获取反序列化程序
                    Assembly asm = Assembly.LoadFile(dllFile);
                    //该程序集内的所有类
                    var types = asm.GetTypes();
                    //主类,继承实现了接口的类
                    Type mainType = null;
                    #region 找到继承接口的类,也就是入口类,主类
                    foreach (Type item in types)
                    {
                        if (item.IsInterface) continue;//判断是否是接口
                        Type[] ins = item.GetInterfaces();
                        foreach (Type ty in ins)
                        {
                            if (ty == typeof(IHISServerConnector))
                            {
                                mainType = item;
                                break;
                            }
                        }
                        if (mainType != null)
                        {
                            break;
                        }
                    }
                    #endregion
                    #region 创建主类的对象 赋值给全局接口
                    if (mainType != null)
                    {
                        //创建一个对象
                        var obj = asm.CreateInstance(mainType.FullName);
                        HISServerConnector = obj as IHISServerConnector;
                        if (App.HISServerConnector.Init())
                        {
                            ReportFinished("his系统连接组件初始化完成");
                            return true;
                        }
                        else
                        {
                            ReportFinished("his系统连接组件初始化失败", dllFile, asm, obj, mainType);
                            return false;
                        }
                    }
                    else
                    {
                        ReportFinished("his系统连接组件加载失败,未能加载有效的入口信息");
                        return false;
                    }
                    #endregion
                }
            }
            catch (Exception loadHisConnectorErr)
            {
                string msg = "加载his通讯模块失败";
                Utils.LogError(msg, loadHisConnectorErr.Message);
                ReportFinished(msg, loadHisConnectorErr.Message);
                return false;
            }
        }
        #endregion
        public static IHISServerConnector HISServerConnector;// = new FakeHISServerConnector();
        #endregion
        #region TTS文本朗读器
        /// <summary>
        /// TTS文本转语音的朗读器
        /// </summary>
        public static TTSSpeaker TTSSpeaker;
        //public static Speaker TTSSpeaker;// = new Speaker(true);
        #endregion
        #region 485数码屏显示器的控制器
        //public static StocksGridNumber485ShowersManager StocksGridNumber485ShowersManager;
        #endregion
        #region 码卡读头
        /// <summary>
        /// IC卡读卡器和条码二维码扫描一体设备
        /// </summary>
        static SerialPort ICCardReaderAndCodeScanner2in1;
        /// <summary>
        /// 当接收到了条码或者ic卡扫描到的完整数据(带换行的数据)的时候被调用
        /// </summary>
        public static Action<string> ICCardReaderAndCodeScanner2in1ReceivedData;

        #region 码卡读头的串口消息处理
        static StringBuilder receivedUnFullDataSB = new StringBuilder();
        static void ICCardReaderAndCodeScanner2in1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string currentScanedQRCode = null;
            string readed = ICCardReaderAndCodeScanner2in1.ReadExisting();
            //Console.WriteLine("本次接收内容:{0}", readed);
            if (readed == null)
            {
                string err = "无效的条码或二维码";
                Console.WriteLine(err);
                return;
            }
            receivedUnFullDataSB.Append(readed);
            //Console.WriteLine("已接收全部内容:{0}", receivedUnFullDataSB);
            if ((readed.Contains("\r") || readed.Contains("\n")) == false)
            {
                //Console.WriteLine("未结束,继续接收");
                return;
            }
            else
            {
                //读取完了,可以用了.
                currentScanedQRCode = receivedUnFullDataSB.ToString();
                //Console.WriteLine("收到结束符号,完整内容为:{0}", currentScanedQRCode);
                //重置待收取的内容
                receivedUnFullDataSB = new StringBuilder();
            }
            //将读取到的全部内容删掉换行符和空格等
            currentScanedQRCode = currentScanedQRCode.Replace("\r", "").Replace("\n", "").Trim();
            //string.Format("当前识别到的已扫描内容:{0}", currentScanedQRCode);
            if (ICCardReaderAndCodeScanner2in1ReceivedData != null)
            {
                ICCardReaderAndCodeScanner2in1ReceivedData(currentScanedQRCode);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("接收到条码或者ic卡内容为:{0},但是没有回调函数处理这个消息", currentScanedQRCode);
            }
        }
        #endregion
        #endregion
        #region 紫外线灯控制器
        /// <summary>
        /// 紫外线灯控制器
        /// </summary>
        public static UVLampManager UVLampManager;
        #endregion
        #region 抓图器

        /// <summary>
        /// 使用摄像头的快照捕获器
        /// </summary>
        public static ISnapCapturer CameraSnapshotCapturer;

        #endregion
        #region 热敏小票打印机,打印付药单

        /// <summary>
        /// 付药单打印组件
        /// </summary>
        public static DeliveryRecordPaperPrinter DeliveryRecordPaperPrinter;

        #endregion
        #region 用户管理器

        /// <summary>
        /// 用户管理器,主要用于校验护士的密码
        /// </summary>
        public static UserManager UserManager;

        #endregion
        #region 测试模式时使用的自动取药控制器,改控制器只会在调试器初始化后才顺带初始化

        /// <summary>
        /// 自动取药测试器,只有在debug模式的时候才有效
        /// </summary>
        public static AutoMedicinesGettingTester AutoMedicinesGettingTester;

        #endregion
        #region 视频播放组件

        /// <summary>
        /// vlc的相关dll的句柄
        /// </summary>
        public static LibVLC VLCHandler;
        /// <summary>
        /// vlc的媒体播放器全局变量
        /// </summary>
        public static MediaPlayer VLCPlayer;
        #region 定义vlc播放组件的日志回调函数
        static void vlcLogCallback(IntPtr data, LogLevel logLevel, IntPtr logContext, string format, IntPtr args)
        {

        }
        #endregion
        #endregion
        #region 日志服务器连接管道的客户端

        /// <summary>
        /// 连接到日志系统(看门狗系统)的管道客户端.
        /// </summary>
        public static PipeClient LogServerServicePipeClient;

        /// <summary>
        /// 错误/故障/警告消息管理器
        /// </summary>
        public static AlertManager AlertManager { get; set; }
        #endregion
        #region 控制面板

        /// <summary>
        /// 控制面板
        /// </summary>
        public static ControlPanel ControlPanel;


        /// <summary>
        /// 控制面板接口服务器.
        /// </summary>
        public static ControlPanelInterfaceServer ControlPanelInterfaceServer;

        #endregion
        #region 取药控制器

        /// <summary>
        /// 取药控制器,主要控制取药流程和plc的通讯控制
        /// </summary>
        public static MedicinesGettingController medicinesGettingController;

        #endregion
        #region 秘钥管理器

        /// <summary>
        /// 钥匙管理器.
        /// </summary>
        public static KeyManager KeyManager;

        #endregion
        #region 调试器
        /// <summary>
        /// 外部调试命令接收器 2022年1月26日13:06:43
        /// </summary>
        public static DebugCommandServer DebugCommandServer;
        /// <summary>
        /// 调试时候要输出的函数体集合,直接传递给utils.LogOutPutFunctions;
        /// </summary>
        public static List<LogOutputFuncDelegate> DebugOutputFunctions { get { return Utils.LogOutPutFunctions; } }
        #endregion
        #region 检测器管理器
        /// <summary>
        /// 监视器群的管理器,包括硬件监视器,软件监视器,药品监视器等.
        /// </summary>
        public static MonitersManager MonitorsManager;
        #endregion
        #endregion
    }
}
