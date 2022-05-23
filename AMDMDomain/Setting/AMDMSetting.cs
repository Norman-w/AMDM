using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace AMDM_Domain
{
    /// <summary>
    /// 设置类
    /// </summary>
    public class AMDMSetting : AMDM_Machine_data
    {
        public AMDMSetting()
        {
            //this.LastMedicineInfoAysncTime
            //this.FirstMedicineInfoAsyncTime
            this.Name = "潮咖医疗付药机";
            this.TTSSpeakerVoice = null;
            this.AdvertVideosSetting = new AdvertVideosSettingClass();
            this.AdvertVideosSetting.SpeedRate = 5.0f;
            this.AdvertVideosSetting.SpareTimeADVideosDir =
                //Application.StartupPath.TrimEnd('\\') + 
                "ADVideos\\SpareTime";
            this.AdvertVideosSetting.MedicinesGettingADVideosDir =
                //Application.StartupPath.TrimEnd('\\') +
                "ADVideos\\MedicinesGetting";
            this.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir =
                //Application.StartupPath.TrimEnd('\\') +
                "ADVideos\\MedicinesComming";
            this.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir =
                //Application.StartupPath.TrimEnd('\\') +
                "ADVideos\\PleaseCheck";
            #region 如果文件夹不存在,创建
            if (System.IO.Directory.Exists(this.AdvertVideosSetting.SpareTimeADVideosDir) == false)
            {
                System.IO.Directory.CreateDirectory(this.AdvertVideosSetting.SpareTimeADVideosDir);
            }
            if (System.IO.Directory.Exists(this.AdvertVideosSetting.MedicinesGettingADVideosDir) == false)
            {
                System.IO.Directory.CreateDirectory(this.AdvertVideosSetting.MedicinesGettingADVideosDir);
            }
            if (System.IO.Directory.Exists(this.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir) == false)
            {
                System.IO.Directory.CreateDirectory(this.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir);
            }
            if (System.IO.Directory.Exists(this.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir) == false)
            {
                System.IO.Directory.CreateDirectory(this.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir);
            }
            #endregion

            #region 默认sql设置
            this.SqlConfig = new SqlConfigClass();
            //this.SqlConfig.IP = "qpmysqlserver.mysql.zhangbei.rds.aliyuncs.com";
            this.SqlConfig.IP = "127.0.0.1";
            //this.SqlConfig.User = "enni";
            this.SqlConfig.User = "root";
            //this.SqlConfig.Pass = "Wsi8uokl";
            this.SqlConfig.Pass = "woshinidie";
            this.SqlConfig.Database = "amdm_local";
            //this.SqlConfig.Port = 3306;
            this.SqlConfig.Port = 10000;
            #endregion
            #region 默认服务器sql / sdk设置
            this.AMDMServerSDKSetting = new AMDMServerSDKSettingClass();
            this.AMDMServerSDKSetting.IP = "127.0.0.1";
            this.AMDMServerSDKSetting.Port = 10000;
            this.AMDMServerSDKSetting.Pass = this.SqlConfig.Pass;
            this.AMDMServerSDKSetting.User = this.SqlConfig.User;
            this.AMDMServerSDKSetting.Database = "server_public";
            #endregion

            #region 默认plc设置
            this.PlcSetting_西门子 = new PLCSetting<MainPLCSetting西门子, StockPLCSetting西门子>();


            this.PlcSetting_台达 = new PLCSetting<MainPLCSettingTD,StockPLCSettingTD>();
            this.PlcSetting_台达.StocksPLC.Add(0, new StockPLCSettingTD()
            {
                //IPAddress = "192.168.2.100",
                PLCDataIndex = new PLCDataIndexInfo(), IsMain = false
                //DBNumber = 1,
                //PerMMPulseCount = 80,
                //RackIndex = 0,
                //SlotIndex = 1
            });
            //this.PlcSetting = new MainPLCSetting();
            //this.PlcSetting.IPAddress = "192.168.2.100";

            #endregion

            #region 默认硬件设置
            this.HardwareSetting = new HardwareSettingClass();

            #endregion


            #region 外设
            this.DevicesSetting = new DevicesSettingClass();
            
            #region 详单打印机
            this.DevicesSetting.Printer58MMSetting = new DevicesSettingClass.Printer58MMSettingClass();
            this.DevicesSetting.Printer58MMSetting.PrinterName = "Zan 彩色图像打印机";
            this.DevicesSetting.Printer58MMSetting.PaperWidthMM = 58;
            this.DevicesSetting.Printer58MMSetting.SaveBackupImage = true;
            this.DevicesSetting.Printer58MMSetting.SaveBackupImageFileDir = "D:\\MedicineDeliveryRecordSnapshot\\";
            #endregion

            #region ic码卡读头

            this.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting = new DevicesSettingClass.ICCardReaderAndCodeScanner2in1SettingClass();
            this.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.dataBits = 8;
            this.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.parity = Parity.None;
            this.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.portName = "COM3";
            this.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.stopBits = StopBits.One;
            this.DevicesSetting.ICCardReaderAndCodeScanner2in1Setting.baudRate = 9600;

            #endregion

            #region 监控头
            this.DevicesSetting.CCTVCaptureSetting = new DevicesSettingClass.CCTVCaptureSettingClass();
            this.DevicesSetting.CCTVCaptureSetting.Ip = "192.168.2.141";
            this.DevicesSetting.CCTVCaptureSetting.User = "admin";
            this.DevicesSetting.CCTVCaptureSetting.Pass = "yuntong8888";
            this.DevicesSetting.CCTVCaptureSetting.CameraChannelOfMedicineBucket = 0;
            this.DevicesSetting.CCTVCaptureSetting.CameraChannelOfInteractiveArea =2;
            this.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos1 = 1;
            this.DevicesSetting.CCTVCaptureSetting.CameraChannelOfGrabbersAreaPos2 = 3;
            this.DevicesSetting.CCTVCaptureSetting.SavingDictionary = "D:\\MedicineDeliveryRecordSnapshot";
            this.DevicesSetting.CCTVCaptureSetting.MixRemainingHardDiskSpaceMB = 2048;
            #endregion
            #region 摄像头
            this.DevicesSetting.CameraSetting = new DevicesSettingClass.CameraSettingClass();
            this.DevicesSetting.CameraSetting.SavingDictionary = "D:\\MedicineDeliveryRecordSnapshot";
            this.DevicesSetting.CameraSetting.MixRemainingHardDiskSpaceMB = 2048;
            #endregion
            #region 药槽编号485显示器
            this.DevicesSetting.StocksGridNumber485ShowerSetting = new DevicesSettingClass.StocksGridNumber485ShowerSettingClass();
            this.DevicesSetting.StocksGridNumber485ShowerSetting.baudRate = 9600;
            this.DevicesSetting.StocksGridNumber485ShowerSetting.dataBits = 8;
            this.DevicesSetting.StocksGridNumber485ShowerSetting.parity = Parity.None;
            this.DevicesSetting.StocksGridNumber485ShowerSetting.portName = "COM1";
            this.DevicesSetting.StocksGridNumber485ShowerSetting.stopBits = StopBits.One;
            #endregion

            #region 紫外线灯设置
            this.DevicesSetting.UVLampSetting = new DevicesSettingClass.UVLampSettingClass();
            //在打开紫外线灯之前提前10分钟进入到警戒状态,不能取药,提示大家远离.
            this.DevicesSetting.UVLampSetting.HowEarlyToEnterTheWarningStateSS = 10 * 60;
            //每天从凌晨3点打开
            this.DevicesSetting.UVLampSetting.UVLampOnTime = new DateTime(1970, 1, 1, 3, 0, 0);
            //每天的凌晨4点关闭
            this.DevicesSetting.UVLampSetting.UVLampOffTime = new DateTime(1970, 1, 1, 4, 0, 0);
            #endregion
            #endregion

            #region 用户界面交互设置
            this.UserInterfaceSetting = new UserInterfaceSettingClass();
            this.UserInterfaceSetting.MedicineOrderAutoHideWhenNoActionMS = 60000;
            this.UserInterfaceSetting.NoticeShowerAutoHideMS = 5000;
            #endregion

            #region 日志服务器(看门狗)相关设置
            this.LogServerServiceSetting = new LogServerServiceSettingClass();
            this.LogServerServiceSetting.PerHeartbeatsDelayMS = 4000;
            this.LogServerServiceSetting.PipeServerLocation = ".";
            this.LogServerServiceSetting.PipeServerName = "LogServerService";
            #endregion

            #region 对外提供显示能力和控制能力的接口服务器设置
            this.ControlPanelInterfaceServerSetting = new ControlPanelInterfaceServerSettingClass();
            this.ControlPanelInterfaceServerSetting.HttpServerPort = 8080;
            #endregion

            
            //GetBestGridMode = GetMedicinesObjectSortModeEnum.ExpirationDateAsc;

            ExpirationStrictControlSetting = new ExpirationStrictControlSettingClass();
            ExpirationStrictControlSetting.DefaultCanLoadMinExpirationDays = 10;
            ExpirationStrictControlSetting.DefaultDaysThresholdOfExpirationAlert = 30;
            ExpirationStrictControlSetting.DefaultSuggestLoadMinExpirationDays = 90;
            ExpirationStrictControlSetting.Enable = true;

            //如果药品没有指定库存提醒阈值,默认药品数量小于3个的时候就提示库存不足
            DefaultCounttThredholdOfLowInventoryAlert = 3;

            //药机故障处置方案
            TroubleshootingPlanSetting = new TroubleshootingPlanSettingClass();
            TroubleshootingPlanSetting.DisableAMDMWhenDeliveryFailed = true;
            TroubleshootingPlanSetting.AlertReceiveUsers = new List<int>();

            //药品有效期及库存预警通知设置
            MedicineAlertSetting = new MedicineAlertSettingClass();
            MedicineAlertSetting.LowInventoryAndExpirationAlertReceiveUsers = new List<int>();

            TimeSignalGeneratorSetting = new TimeSignalGeneratorSettingClass();
            TimeSignalGeneratorSetting.HardwareDetectionPerTimeIntervalMS = 5000;
            TimeSignalGeneratorSetting.MedicineExpirationCheckingPerTimeIntervalMS = 1000 * 60 * 5;
            TimeSignalGeneratorSetting.SoftwarePartDetectionPerTimeIntervalMS = 8000;


            //2022年1月21日09:36:17 
            HISServerConnectorDllFilePath = @"D:\Visual Studio 2008\Projects\测试\FakeHISClient\FakeHISServerConnector\bin\Debug\FakeHISServerConnector.dll";
        }

        /// <summary>
        /// 配置的保存文件
        /// </summary>
        private string settingSavePath { get; set; }
        /// <summary>
        /// 是否正在调试中,正在调试的时候,全屏播放的组件都不是全屏,也不检查付药单是否已经交付
        /// </summary>
        /// //2022年3月13日15:49:36  这个字段被废除 使用DebugSetting是否为空以及个个字段的值来检测
        //public bool Debugging { get; set; }
        /// <summary>
        /// TTS语音朗读引擎的讲述人使用哪一个,默认为空,可以指定为lily
        /// </summary>
        public string TTSSpeakerVoice { get; set; }

        ///// <summary>
        ///// 当前药机的名称,由于继承了machine类,所以不需要了
        ///// </summary>
        //public string MachineName { get; set; }

        /// <summary>
        /// 广告宣传视频的路径,应当不带\\
        /// </summary>
        public AdvertVideosSettingClass AdvertVideosSetting { get; set; }
        /// <summary>
        /// 日志服务组件(看门狗)的相关设置
        /// </summary>
        public LogServerServiceSettingClass LogServerServiceSetting { get; set; }
        /// <summary>
        /// 药品信息从his最后的同步时间
        /// </summary>
        public Nullable<DateTime> LastMedicineInfoAysncTime { get; set; }
        /// <summary>
        /// 药品信息从his获取后第一次初始化本机药品库的时间
        /// </summary>
        public Nullable<DateTime> FirstMedicineInfoAsyncTime { get; set; }

        /// <summary>
        /// sql数据库连接设置
        /// </summary>
        public SqlConfigClass SqlConfig { get; set; }

        /// <summary>
        /// 2022年3月22日13:03:16 药机服务器sdk的设置
        /// </summary>
        public AMDMServerSDKSettingClass AMDMServerSDKSetting { get; set; }
        /// <summary>
        /// 机器的plc设置
        /// </summary>
        public PLCSetting<MainPLCSetting西门子,StockPLCSetting西门子> PlcSetting_西门子 { get; set; }
        public PLCSetting<MainPLCSettingTD,StockPLCSettingTD> PlcSetting_台达 { get; set; }

        /// <summary>
        /// 默认药机的硬件设置信息
        /// </summary>
        public HardwareSettingClass HardwareSetting { get; set; }

        /// <summary>
        /// 外设设备的相关设置
        /// </summary>
        public DevicesSettingClass DevicesSetting { get; set; }

        /// <summary>
        /// 用户界面交互设置.
        /// </summary>
        public UserInterfaceSettingClass UserInterfaceSetting { get; set; }

        /// <summary>
        /// 对外提供状态访问和控制能力的接口服务器设置
        /// </summary>
        public ControlPanelInterfaceServerSettingClass ControlPanelInterfaceServerSetting { get; set; }

        ///// <summary>
        ///// 获取最合适的格子的模式
        ///// </summary>
        //public GetMedicinesObjectSortModeEnum GetBestGridMode { get; set; }

        //public int _默认可装入药机的药品有效期最少多少天 { get; set; }

        //public int _默认可装入药机的药品建议有效期大于多少天 { get; set; }

        //public int _默认已装入药机的药品有效期小于多少天时提醒 { get; set; }

        /// <summary>
        /// 与his系统连接的连接器所在的dll文件
        /// </summary>
        public string HISServerConnectorDllFilePath { get; set; }

        /// <summary>
        /// 有效期严格控制设置
        /// </summary>
        public ExpirationStrictControlSettingClass ExpirationStrictControlSetting { get; set; }

        /// <summary>
        /// 药机故障处置方案设置
        /// </summary>
        public TroubleshootingPlanSettingClass TroubleshootingPlanSetting { get; set; }

        /// <summary>
        /// 药品预警设置
        /// </summary>
        public MedicineAlertSettingClass MedicineAlertSetting { get; set; }

        /// <summary>
        /// 药品的默认CTOLIA数值时多少.如果药品没有指定少于多少个报警的话,使用这个值.
        /// </summary>
        public int DefaultCounttThredholdOfLowInventoryAlert { get; set; }

        /// <summary>
        /// 时间信号产生器设置
        /// </summary>
        public TimeSignalGeneratorSettingClass TimeSignalGeneratorSetting { get; set; }
        #region 加载和保存函数
        public string Load(string filePathFull)
        {
            this.settingSavePath = filePathFull;
            if (System.IO.File.Exists(this.settingSavePath) == false)
            {
                return null;
            }

            string json = System.IO.File.ReadAllText(settingSavePath);
            JsonConvert.PopulateObject(json, this);
            return json;
        }
        public bool Save()
        {
            if (System.IO.File.Exists(settingSavePath) == false)
            {
                System.IO.File.Create(settingSavePath).Close();
            }
            System.IO.File.WriteAllText(settingSavePath,
                    JsonConvert.SerializeObject(this, new JsonSerializerSettings()
                    {
                        DateFormatString = "yyyy-MM-dd HH:mm:ss",
                        Formatting = Formatting.Indented
                    })
                );
            return true;
        }
        #endregion
    }
}
