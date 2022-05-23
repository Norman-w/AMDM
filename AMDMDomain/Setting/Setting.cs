using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace AMDM_Domain
{
    #region 设置类型的定义

    #region 广告视频
    /// <summary>
    /// 广告视频设置类
    /// </summary>
    public class AdvertVideosSettingClass
    {
        /// <summary>
        /// 播放速度的倍速.一般都是在测试的时候才会用到.
        /// </summary>
        public float SpeedRate { get; set; }
        /*
         * 后续可以加上一些设置,目前没有想到
         */
        /// <summary>
        /// 空闲时间播放的广告的总目录
        /// </summary>
        public string SpareTimeADVideosDir { get; set; }

        /// <summary>
        /// 正在取药中的广告视频的总目录
        /// </summary>
        public string MedicinesGettingADVideosDir { get; set; }

        /// <summary>
        /// 药品已经出仓等待用户取走药品时候播放的视频的文件夹
        /// </summary>
        public string MedicinesWaitBeenTakeADVideosDir { get; set; }

        /// <summary>
        /// 药品已经被取走,提示请核对药品时候的文件夹
        /// </summary>
        public string MedicinesHasBeenTakedAdVideosDir { get; set; }
    }
    #endregion
    #region 日志服务器(看门狗的设置)
    /// <summary>
    /// 日志服务器(看门狗的设置类)
    /// </summary>
    public class LogServerServiceSettingClass
    {
        /// <summary>
        /// 每一次心跳的间隔时间多久
        /// </summary>
        public int PerHeartbeatsDelayMS { get; set; }

        /// <summary>
        /// 管道服务器的地址 默认的是.
        /// </summary>
        public string PipeServerLocation { get; set; }

        /// <summary>
        /// 管道服务器的名称,默认是LogServerService
        /// </summary>
        public string PipeServerName { get; set; }
    }
    #endregion

    public class SqlConfigClass
    {
        public string IP { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string Database { get; set; }
        public int Port { get; set; }
    }
    /// <summary>
    /// 硬件基础信息设置类
    /// </summary>
    public class HardwareSettingClass
    {
        public HardwareSettingClass()
        {
            this.Stock = new StockSettingClass();
            this.Floor = new FloorSettingClass();
            this.Grid = new GridSettingClass();
        }
        public StockSettingClass Stock { get; set; }
        public FloorSettingClass Floor { get; set; }
        public GridSettingClass Grid { get; set; }
        public class StockSettingClass
        {
            public StockSettingClass()
            {
                XOffsetFromStartPointMM = -34;// -19;
                YOffsetFromStartPointMM = 34;
                FloorFixingsHeightMM = 20;
                StockFloorCount = 12;
                MaxPerMedicineDepthMM = 200;
                MinPerMedicineDepthMM = 30;
                MinPerMedicineHeightMM = 5;
                MaxFloorsHeightMM = 960;
                MaxFloorWidthMM = 975;
                CenterDistanceBetweenTwoGrabbers = 310;
            }
            /// <summary>
            /// 支持的安装所有层板的总高度,也就是机械手动作面可以动作的总高度了,太高了的话机械手就取不了了.这也就决定了能放多少层拍子,一层是80,12层正好是960
            /// </summary>
            public float MaxFloorsHeightMM { get; set; }
            /// <summary>
            /// 可以安装的层板的最宽的宽度是多少
            /// </summary>
            public float MaxFloorWidthMM { get; set; }
            public float XOffsetFromStartPointMM { get; set; }
            public float YOffsetFromStartPointMM { get; set; }
            public float CenterDistanceBetweenTwoGrabbers { get; set; }
            /// <summary>
            /// 在层板下房的层板固定件高度
            /// </summary>
            public float FloorFixingsHeightMM { get; set; }
            //float defaultStockFloorWidthMM = 12*80;
            /// <summary>
            /// 每一个药仓默认能放多少个层板
            /// </summary>
            public int StockFloorCount = 12;
            /// <summary>
            /// 可以放到药机内的药品的最长长度信息 这取决于机器的传送带等相关的参数 只能固定设置, 虽然长度很长的药品能够从层板上挑下来 但是不一定能被传送带和给药槽很好的接收
            /// </summary>
            public float MaxPerMedicineDepthMM { get; set; }
            /// <summary>
            /// 可以放到药机内的药品的最小长度信息 最下层的特殊药品层不做限制
            /// </summary>
            public float MinPerMedicineDepthMM { get; set; }
            /// <summary>
            /// 药盒的最小的尺寸 这个尺寸如果太小了 可能药品就非常轻,不适合放在上面的药槽中.
            /// </summary>
            public float MinPerMedicineHeightMM { get; set; }
        }

        public class FloorSettingClass
        {
            public FloorSettingClass()
            {

                DownPartFloorDepthMM = 500;
                //DefaultFloorDepthMM = 960;
                FloorHeightMM = 80;
                FloorWidthMM = 975;
                UpPartFloorDepthMM = 760;
                FloorPanelHeightMM = 3;
                FloorSlopeAngle = 27;
                MinGridPaddingHeighMM = 3;
                NewFloorDefaultHeightMM = 80;
                //PerFloorMaxHeightMM = 80;
                //PerFloorMinHeightMM = 50;
                //PerFloorMoveStepHeightMM = 5;
            }
            /// <summary>
            /// 层板的高度,用于计算一个药仓中可以放多少个层板
            /// </summary>
            public float FloorHeightMM { get; set; }
            /// <summary>
            /// 层板的宽度,用于计算可以放多少个格子
            /// </summary>
            public float FloorWidthMM { get; set; }
            /// <summary>
            /// 层板的进深尺寸,用于计算可以放多少盒药
            /// </summary>
            //public float DefaultFloorDepthMM { get; set; }
            /// <summary>
            /// 层板那个电木板的高度
            /// </summary>
            public float FloorPanelHeightMM { get; set; }
            /// <summary>
            /// 层板的倾斜角度/坡度 当前默认27°
            /// </summary>
            public float FloorSlopeAngle { get; set; }

            ///// <summary>
            ///// 每一个层板的最小高度是多少,包含框架连接件的高度.目前框架连接件的厚度是20mm,电板的厚度是3mm所以一个80mm的层板,会向下影响23mm的空间不可用.
            ///// </summary>
            //public float PerFloorMinHeightMM = 30;
            ///// <summary>
            ///// 每个层板的最大高度
            ///// </summary>
            //public float PerFloorMaxHeightMM = 200;
            ///// <summary>
            ///// 每个层板每次调整高度的时候能移动多少毫米
            ///// </summary>
            //public int PerFloorMoveStepHeightMM = 5;
            /// <summary>
            /// 新的层板的默认高度毫米
            /// </summary>
            public float NewFloorDefaultHeightMM = 80;
            /// <summary>
            /// 层板上方和药槽之间的最小间隙
            /// </summary>
            public float MinGridPaddingHeighMM { get; set; }

            public int UpPartFloorDepthMM { get; set; }

            /// <summary>
            /// 默认的下部分区域的层的深度
            /// </summary>
            public int DownPartFloorDepthMM { get; set; }
        }
        public class GridSettingClass
        {
            public GridSettingClass()
            {
                GridWallWidthMM = 10;
                GridWallFixtureFullWidthMM = 30;
                PerGridMinWidthMM = 60;
                PerGridMaxWidthMM = 170;
                PerGridMoveStepWidthMM = 5; NewGridDefaultWidth = 100;
                MaxGridPaddingWidthMM = 7;
                MinGridPaddingWidthMM = 2;
            }
            /// <summary>
            /// 槽之间的隔断铝条墙的宽度 药槽的边缘档条的宽度是多少.一个药槽两边都有档条,但是药槽实际被分派的宽度,可以视为0.5档条+内径+0.5档条.
            /// </summary>
            public float GridWallWidthMM { get; set; }
            /// <summary>
            /// 默认的安装在层板上的格子格栅固定件的总宽度,就是3d打印件那个30mm的
            /// </summary>
            public float GridWallFixtureFullWidthMM { get; set; }

            /// <summary>
            /// 每个格子的最小宽度(实际尺寸,包含两侧各半个墙板)
            /// </summary>
            public float PerGridMinWidthMM = 60;//这是外径,实际内径是50

            /// <summary>
            /// 每个格子的最大宽度毫米(实际尺寸,包含两侧各半个墙板)
            /// </summary>
            public float PerGridMaxWidthMM = 170;//这是外径,实际内径是160;
            /// <summary>
            /// 每个格子左右移动墙板的时候每一次能移动多少毫米的步伐
            /// </summary>
            public int PerGridMoveStepWidthMM = 5;
            /// <summary>
            /// 新的格子的默认宽度
            /// </summary>
            public float NewGridDefaultWidth = 100;
            #region 药槽和药盒之间的最小间隙和最大间隙信息
            /// <summary>
            /// 药槽和药盒之间的最大间隙
            /// </summary>
            public float MaxGridPaddingWidthMM { get; set; }
            /// <summary>
            /// 药槽和药盒之间的最小间隙
            /// </summary>
            public float MinGridPaddingWidthMM { get; set; }
            #endregion
        }
    }

    /// <summary>
    /// 付药机系统服务器的sdk设置(调用场景基于sdk的使用方,也就是sdkClient的设置 2022年3月22日13:01:47 目前是使用sql数据直连的方式,但是实际上应该client和server之间连接使用web,但是web的鉴权太复杂了所以暂时写这样的
    /// </summary>
    public class AMDMServerSDKSettingClass : SqlConfigClass
    {
    }

    /// <summary>
    /// 外设的设置
    /// </summary>
    public class DevicesSettingClass
    {
        #region 添加监控设置类
        /// <summary>
        /// 监控设置类
        /// </summary>
        public class CCTVCaptureSettingClass : SnapCapturerSettingClass
        {
            public string Ip { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
            public int Port { get; set; }

            /// <summary>
            /// 取药斗上方监控所属通道,
            /// </summary>
            public Nullable<int> CameraChannelOfMedicineBucket { get; set; }

            /// <summary>
            /// 交互区的监控所属通道
            /// </summary>
            public Nullable<int> CameraChannelOfInteractiveArea { get; set; }

            /// <summary>
            /// 取药机械手位置1的监控通道号
            /// </summary>
            public Nullable<int> CameraChannelOfGrabbersAreaPos1 { get; set; }
            /// <summary>
            /// 取药机械手位置2的监控通道号
            /// </summary>
            public Nullable<int> CameraChannelOfGrabbersAreaPos2 { get; set; }
        }
        #endregion
        /// <summary>
        /// 2022年2月16日13:39:43抓图器的设置
        /// </summary>
        public class SnapCapturerSettingClass
        {
            /// <summary>
            /// 保存目录
            /// </summary>
            public string SavingDictionary { get; set; }
            /// <summary>
            /// 磁盘最小剩余空间多少时候才可以拍照.
            /// </summary>
            public long MixRemainingHardDiskSpaceMB { get; set; }
        }
        public class CameraSettingClass : SnapCapturerSettingClass
        {
            /// <summary>
            /// 交付药品后拍照截图的文件保存目录
            /// </summary>
            //public string DeliveryRecordSnapshotSavePath { get; set; }
            /// <summary>
            /// 预览或者是处理时,每秒多少帧
            /// </summary>
            public float FPS { get; set; }
            /// <summary>
            /// 格式化的摄像头名称
            /// </summary>
            public string CameraMonikerStringName { get; set; }
        }
        /// <summary>
        /// 紫外线灯的相关设置
        /// </summary>
        public class UVLampSettingClass
        {
            /// <summary>
            /// 紫外线灯的定时打开时间
            /// </summary>
            public Nullable<DateTime> UVLampOnTime { get; set; }
            /// <summary>
            /// 紫外线灯的定时关闭时间
            /// </summary>
            public Nullable<DateTime> UVLampOffTime { get; set; }
            /// <summary>
            /// 提前多长时间进入到警告状态,进入到警告状态后,会播放提示音或者提示视频,提醒大家要远离该区域,避免紫外线伤害,以秒为单位
            /// </summary>
            public Nullable<double> HowEarlyToEnterTheWarningStateSS { get; set; }
        }
        public class ICCardReaderAndCodeScanner2in1SettingClass
        {
            //string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits
            public string portName { get; set; }
            public int baudRate { get; set; }
            public Parity parity { get; set; }
            public int dataBits { get; set; }
            public StopBits stopBits { get; set; }
        }
        public class Printer58MMSettingClass
        {
            public string PrinterName { get; set; }
            /// <summary>
            /// 纸张的宽度毫米数,固定为58
            /// </summary>
            public float PaperWidthMM { get; set; }

            /// <summary>
            /// 2022年1月13日19:08:13 是否保存备份图像
            /// </summary>
            public bool SaveBackupImage { get; set; }

            /// <summary>
            /// 2022年1月13日19:08:26 如果开启了保存备份图像,把文件保存在什么位置
            /// </summary>
            public string SaveBackupImageFileDir { get; set; }
        }

        public class StocksGridNumber485ShowerSettingClass
        {
            //string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits
            public string portName { get; set; }
            public int baudRate { get; set; }
            public Parity parity { get; set; }
            public int dataBits { get; set; }
            public StopBits stopBits { get; set; }
        }
        /// <summary>
        /// 摄像头设置
        /// </summary>
        public CameraSettingClass CameraSetting { get; set; }
        /// <summary>
        /// 紫外线灯相关设置
        /// </summary>
        public UVLampSettingClass UVLampSetting { get; set; }
        /// <summary>
        /// 监控拍照相关设置
        /// </summary>
        public CCTVCaptureSettingClass CCTVCaptureSetting { get; set; }
        public ICCardReaderAndCodeScanner2in1SettingClass ICCardReaderAndCodeScanner2in1Setting { get; set; }
        public Printer58MMSettingClass Printer58MMSetting { get; set; }
        public StocksGridNumber485ShowerSettingClass StocksGridNumber485ShowerSetting { get; set; }
    }

    /// <summary>
    /// 用户的交互设置
    /// </summary>
    public class UserInterfaceSettingClass
    {
        /// <summary>
        /// 当用户扫描了处方二维码但是没有进行点击取药的操作的话 多久自动消失那个提示框
        /// </summary>
        public int MedicineOrderAutoHideWhenNoActionMS { get; set; }

        /// <summary>
        /// 信息提示框 比如提示  该机药品库存不足之类的  提示完以后的默认关闭时间是多久
        /// </summary>
        public int NoticeShowerAutoHideMS { get; set; }
    }
    #region 控制面板组件设置
    /// <summary>
    /// 控制面板接口服务器的设置类
    /// </summary>
    public class ControlPanelInterfaceServerSettingClass
    {
        /// <summary>
        /// 开启一个控制面板的对外接口时,使用的httpserver的url是什么 要以/结尾
        /// </summary>
        public int HttpServerPort { get; set; }
    }
    #endregion

    #region 严控药品有效期模式设置
    //strict control expiration mode
    /// <summary>
    /// 严控有效期模式开关和相关的参数
    /// </summary>
    public class ExpirationStrictControlSettingClass
    {
        /// <summary>
        /// 严控有效期模式是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 默认的可以装入到药仓中的药品的最小有效期
        /// </summary>
        public int DefaultCanLoadMinExpirationDays { get; set; }
        /// <summary>
        /// 默认的建议装载到药仓中的药品的最小有效期
        /// </summary>
        public int DefaultSuggestLoadMinExpirationDays { get; set; }
        /// <summary>
        /// 默认的低于多少数量时候产生预警消息
        /// </summary>
        public int DefaultDaysThresholdOfExpirationAlert { get; set; }
    }
    #endregion

    #region 故障处置预案/方案类
    /// <summary>
    /// 故障处置方案设置
    /// </summary>
    public class TroubleshootingPlanSettingClass
    {
        /// <summary>
        /// 接收警告消息的用户集合
        /// </summary>
        public List<int> AlertReceiveUsers { get; set; }
        /// <summary>
        /// 在发生取药故障以后是否停用药机/卡药后进入故障状态
        /// </summary>
        public bool DisableAMDMWhenDeliveryFailed { get; set; }
    }
    #endregion

    #region 药品有效期及库存预警通知设置
    /// <summary>
    /// 药品有效期及库存预警通知设置
    /// </summary>
    public class MedicineAlertSettingClass
    {
        /// <summary>
        /// 药品库存预警及药品有效期预警消息接收人的ID集合
        /// </summary>
        public List<int> LowInventoryAndExpirationAlertReceiveUsers { get; set; }
    }
    #endregion

    #endregion

    /// <summary>
    /// 设置类
    /// </summary>
    public class Setting : AMDM_Machine_data
    {
        public Setting()
        {
            //this.LastMedicineInfoAysncTime
            //this.FirstMedicineInfoAsyncTime
            this.Name = "潮咖医疗付药机";
            this.TTSSpeakerVoice = null;
            this.AdvertVideosSetting = new AdvertVideosSettingClass();
            this.AdvertVideosSetting.SpeedRate = 5.0f;
            this.AdvertVideosSetting.SpareTimeADVideosDir =
                //Application.StartupPath.TrimEnd('\\') + 
                "\\ADVideos\\SpareTime";
            this.AdvertVideosSetting.MedicinesGettingADVideosDir =
                //Application.StartupPath.TrimEnd('\\') +
                "\\ADVideos\\MedicinesGetting";
            this.AdvertVideosSetting.MedicinesWaitBeenTakeADVideosDir =
                //Application.StartupPath.TrimEnd('\\') +
                "\\ADVideos\\MedicinesComming";
            this.AdvertVideosSetting.MedicinesHasBeenTakedAdVideosDir =
                //Application.StartupPath.TrimEnd('\\') +
                "\\ADVideos\\PleaseCheck";
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
