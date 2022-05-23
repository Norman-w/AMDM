using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace AMDM_Domain
{
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

}
