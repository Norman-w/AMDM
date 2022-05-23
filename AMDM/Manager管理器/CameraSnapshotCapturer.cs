using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using CCTVCapturer;
/*2021年11月15日20:24:31
 * 摄像头拍照捕获器
 */
namespace AMDM.Manager
{
    public class CameraSnapshotCapturer :ISnapCapturer
    {
        public bool Connected { get; set; }
        #region 构造和释放
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraSnapshotCapturer(string deviceMonikerStringName)
        {
            this.deviceMonikerStringName = deviceMonikerStringName;
            try
            {
                // 枚举所有视频输入设备  
                usableCameraDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (usableCameraDevices.Count == 0)
                    throw new ApplicationException();

                foreach (FilterInfo device in usableCameraDevices)
                {
                    //tscbxCameras.Items.Add(device.Name);
                    camerasNamesList.Add(device.MonikerString);
                }

                if (usableCameraDevices != null && usableCameraDevices.Count > 0)
                {
                    string monikerString = usableCameraDevices[0].MonikerString;
                }
                else
                {
                    Console.WriteLine("没有检索到任何的可用设备,将放弃启动摄像头");
                }
            }
            catch (ApplicationException)
            {
                //tscbxCameras.Items.Add("No local capture devices");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("枚举可用设备失败,当前计算机未连接有效的视频捕获设备");
                Console.ResetColor();
                usableCameraDevices = null;
            }
        }
        public void Dispose()
        {
            if (this.currentUsingCamera != null)
            {
                this.currentUsingCamera.NewFrame -= videoSource_NewFrame;
                if (this.currentUsingCamera.IsRunning)
                {
                    this.currentUsingCamera.Stop();
                }
            }
        }
        #endregion
        #region 外部可调用的公共方法
        /// <summary>
        /// 开始视频捕获
        /// </summary>
        /// <param name="deviceName"></param>
        public bool Connect()
        {
            Console.WriteLine("即将启动:\r\n{0}\r\n摄像头", deviceMonikerStringName);
            if (usableCameraDevices == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("启动摄像头:尚未获取设备集合或当前计算机无有效设备,放弃连接");
                Console.ResetColor();
                return false;
            }
            if (currentUsingCamera != null)
            {
                currentUsingCamera.NewFrame -= videoSource_NewFrame;
            }
            try
            {
                if (this.usableCameraDevices.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("启动摄像头:当前无可用的摄像设备");
                    Console.ResetColor();
                    return false;
                }
                else
                {
                    string devicesJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.usableCameraDevices, Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine("启动摄像头:当前可用的摄像设备:{0}", devicesJson);
                }
                //videoSource = new VideoCaptureDevice(videoDevices[tscbxCameras.SelectedIndex].MonikerString);
                currentUsingCamera = new VideoCaptureDevice(deviceMonikerStringName);
                //videoSource.VideoResolution = videoSource.VideoCapabilities[2];//比较快3也还行
                //videoSource.VideoResolution = videoSource.VideoCapabilities[0];//也还行
                string VideoCapabilitiesJson = Newtonsoft.Json.JsonConvert.SerializeObject(currentUsingCamera.VideoCapabilities, Newtonsoft.Json.Formatting.Indented);
                Console.WriteLine("启动摄像头:摄像头可用采样率信息:{0}", VideoCapabilitiesJson);
                if (currentUsingCamera == null || currentUsingCamera.VideoCapabilities == null || currentUsingCamera.VideoCapabilities.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("启动摄像头:当前摄像头 {0} 无可用的采样率信息", this.currentUsingCamera == null ? "无摄像头" : this.currentUsingCamera.Source);
                    Console.ResetColor();
                    return false;
                }
                currentUsingCamera.VideoResolution = currentUsingCamera.VideoCapabilities[0];
                if (App.Setting.DevicesSetting.CameraSetting.FPS < 1 || App.Setting.DevicesSetting.CameraSetting.FPS > 120)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("App.Setting.DevicesSetting.CameraSetting.FPS的值过大或过小,将使用默认的10帧每秒");
                    Console.ResetColor();
                    App.Setting.DevicesSetting.CameraSetting.FPS = 10;
                }
                //默认先不添加回调函数,启动截图的时候再添加
                //currentUsingCamera.NewFrame += videoSource_NewFrame;
                //videoPlayer.Size = videoSource.VideoResolution.FrameSize;
                //videoPlayer.VideoSource = videoSource;
                //videoPlayer.Start();
                currentUsingCamera.Start();
                Connected = true;
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("启动摄像头:开启摄像头失败:{0}", err.Message);
                Console.ResetColor();
            }
            return true;
        }

        /// <summary>
        /// 停止视频捕获
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (currentUsingCamera != null)
            {
                currentUsingCamera.Stop();
                currentUsingCamera.NewFrame -= this.videoSource_NewFrame;
                Connected = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        //public bool SetCaptureParams(Nullable<int> chalelIndex, string destFilePath)
        //{
        //    if (this.currentCaptureTaskFilePath != null)
        //    {
        //        Utils.LogWarnning("当前拍照任务正在进行中,接下来将强制替换为新目标,当前的目标文件路径为:", destFilePath);
        //    }
        //    if (chalelIndex != null)
        //    {
        //        Utils.LogWarnning("摄像头拍照器不支持通道,chanelIndex参数请为空,已忽略当前给定的值", chalelIndex);
        //    }
        //    this.currentCaptureTaskFilePath = destFilePath;
        //    return true;
        //}

        /// <summary>
        /// 异步捕获一张图片,存储到全局设置中的文件夹下,并以传入的入参为文件名命名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="onSnapshotCaptured">当图片被捕获到以后触发该函数,第一个参数是捕获到的图片,第二个是已经保存到的路径</param>
        public bool CaptureSync(Nullable<int> chalelIndex, string destFilePath, Action<Image, string> onSnapshotCaptured)
        {
            if (this.currentCaptureTaskFilePath != null)
            {
                Utils.LogWarnning("当前拍照任务正在进行中,接下来将强制替换为新目标,当前的目标文件路径为:", destFilePath);
            }
            if (chalelIndex != null)
            {
                Utils.LogWarnning("摄像头拍照器不支持通道,chanelIndex参数请为空,已忽略当前给定的值", chalelIndex);
            }
            this.currentCaptureTaskFilePath = destFilePath;


            if (string.IsNullOrEmpty(this.currentCaptureTaskFilePath) == true)
            {
                Utils.LogWarnning("未调用SetCaptureParams设置拍照图片位置,将不进行图片保存");
            }
            if (this.currentUsingCamera != null)
            {
                //重新注册关心的事件//currentUsingCamera.NewFrame += videoSource_NewFrame; 之前先减去他的事件绑定
                this.currentUsingCamera.NewFrame -= this.videoSource_NewFrame;
                this.currentUsingCamera.NewFrame += this.videoSource_NewFrame;
            }
            else
            {
                Utils.LogWarnning("未注册当前使用的摄像头的新帧处理函数,无法拍照");
                return false;
            }
            Console.WriteLine("已经发送拍照命令 当前摄像头是否已经工作中:{0}", currentUsingCamera.IsRunning);
            if (currentUsingCamera.IsRunning == false)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("CaptureSync:当前摄像设备未启动,无法异步拍照");
                Console.ResetColor();
                return false;
            }
            #region 获取硬盘剩余空间,剩余空间小于2g时候不能拍照
            string driverName = System.IO.Path.GetPathRoot(this.currentCaptureTaskFilePath);
            if (driverName!= null && driverName.Length == 1)
            {
                driverName = driverName.ToUpper();
                long sizeB = GetHardDiskSpace(driverName);
                if (sizeB < (App.Setting.DevicesSetting.CCTVCaptureSetting.MixRemainingHardDiskSpaceMB * 1024*1024))
                {
                    Utils.LogError("磁盘空间不足,当前剩余MB和最小要求MB", sizeB / (1024 * 1024), App.Setting.DevicesSetting.CCTVCaptureSetting.MixRemainingHardDiskSpaceMB);
                    App.AlertManager.AlertHardwareError("磁盘空间不足",string.Format("计算机磁盘空间不足,无法对交付记录进行抓图保存,当前磁盘剩余空间:{0}MB", sizeB / (1024 * 1024)));
                    return false;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CaptureSync:无效的驱动器盘符,无法获取驱动器的剩余空间大小不能拍照");
                Console.ResetColor();
                return false;
            }

            #endregion


            Console.WriteLine("当前准备拍照的图片名称:{0} 回调函数:{1}", currentCaptureTaskFilePath, onSnapshotCaptured);
            this.onSnapshotCaptured = onSnapshotCaptured;
            return true;
        }
        /// 获取指定驱动器的空间总大小(单位为B) 
        ///   
        ///  只需输入代表驱动器的字母即可 （大写） 
        ///    
        long GetHardDiskSpace(string str_HardDiskName)
        {
            long totalSize = new long();
            str_HardDiskName = str_HardDiskName + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalSize / (1024 * 1024 * 1024);
                }
            }
            return totalSize;
        }
        #endregion
        #region 全局变量
        string deviceMonikerStringName = null;
        /// <summary>
        /// 视频捕获设备集合
        /// </summary>
        FilterInfoCollection usableCameraDevices;
        /// <summary>
        /// 当前使用的视频捕获源
        /// </summary>
        VideoCaptureDevice currentUsingCamera;

        //string currentUsingCameraMonikerStringName = null;

        List<string> camerasNamesList = new List<string>();

        string currentCaptureTaskFilePath = null;
        //2022年1月6日14:17:01  新增的字段 如果是已经有了任务但是正在保存中的话 不重复保存.
        bool saving = false;

        /// <summary>
        /// 当异步拍照任务完成时,第一个参数是一张图片,第二个参数是保存的绝对位置和扩展名的全名称
        /// </summary>
        Action<Image, string> onSnapshotCaptured { get; set; }
        #endregion
        #region 私有方法
       
        /// <summary>
        /// 上一次接收到一帧的时候 是什么时候
        /// </summary>
        DateTime lastCaptureTime = DateTime.Now;
        void videoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            //Console.WriteLine("捕获到摄像头的新一帧, FPS:{0}", App.Setting.DevicesSetting.CameraSetting.FPS);
            //GC.Collect();
            float perFrameMS = 1000f / App.Setting.DevicesSetting.CameraSetting.FPS;
            if ((DateTime.Now - lastCaptureTime).Milliseconds < perFrameMS)
            {
                System.Threading.Thread.Sleep((int)perFrameMS);
                tryFreeFrame(eventArgs);
                return;
            }
            //Console.ForegroundColor = ConsoleColor.DarkMagenta;
            //Console.WriteLine("有效的使用帧");
            //Console.ResetColor();
            lastCaptureTime = DateTime.Now;
            //如果当前有保存任务,就保存图像然后把要保存的任务清空
            if (this.currentCaptureTaskFilePath != null && saving == false)
            {
                saving = true;
                //Console.ForegroundColor = ConsoleColor.Blue;
                //Console.WriteLine("当前要保存的文件名是:{0}", this.currentCaptureTaskFilePath);
                //Console.ResetColor();
                Bitmap bitShow = null;
                try
                {
                    if (System.IO.Directory.Exists(App.Setting.DevicesSetting.CameraSetting.SavingDictionary) == false)
                    {
                        System.IO.Directory.CreateDirectory(App.Setting.DevicesSetting.CameraSetting.SavingDictionary);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("截图保存文件夹{0}不存在,已经自动创建", App.Setting.DevicesSetting.CameraSetting.SavingDictionary);
                        Console.ResetColor();
                    }
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("目标文件:{0}", currentCaptureTaskFilePath);
                    //Console.ResetColor();
                    Bitmap bitSave = deepCopyBitmap(eventArgs.Frame);
                    bitShow = deepCopyBitmap(bitSave);
                    //eventArgs.Frame.Save(filePath);
                    if (this.currentCaptureTaskFilePath != null)
                    {
                        try
                        {
                            string dir = System.IO.Path.GetDirectoryName(this.currentCaptureTaskFilePath);
                            if (System.IO.Directory.Exists(dir) == false)
                            {
                                System.IO.Directory.CreateDirectory(dir);
                            }
                            bitSave.Save(this.currentCaptureTaskFilePath);
                        }
                        catch (Exception saveFileErr)
                        {
                            Console.WriteLine("在海康视频截图组件V2中发生了错误:");
                            Console.WriteLine(saveFileErr.Message);
                        }
                    }
                    bitSave.Dispose();
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine("截图已保存到:{0}", currentCaptureTaskFilePath);
                    //Console.ResetColor();
                }
                catch (Exception saveFileErr)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("保存图像文件失败:{0}", saveFileErr.Message);
                    Console.ResetColor();
                }
                //截完图以后 释放回调函数
                this.currentUsingCamera.NewFrame -= this.videoSource_NewFrame;
                
                if (this.onSnapshotCaptured != null)
                {
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine("已执行完毕尝试截图保存,有回调函数  即将执行");
                    //Console.ResetColor();
                    this.onSnapshotCaptured(bitShow, currentCaptureTaskFilePath);
                    try
                    {
                        if (bitShow!= null)
                        {
                            bitShow.Dispose();
                        }
                    }
                    catch (Exception disposeBitShowErr)
                    {
                        Utils.LogWarnning("在摄像头截图捕获器中尝试释放由帧克隆来的bit图像(用于显示的) 失败:", disposeBitShowErr.Message);
                    }
                }
                else
                {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    //Console.WriteLine("已执行完毕尝试截图保存,但是没有回调函数");
                    //Console.ResetColor();
                    Utils.LogBug("已经执行截图保存,没有回调函数,此次获取无用");
                    //如果没有回调函数 就白获取了
                }
                //清空任务
                this.currentCaptureTaskFilePath = null;
                this.saving = false;
            }
            else
            {
                //如果不需要截图,那就白获取了
            }
            Console.WriteLine("尝试是放当前帧");
            tryFreeFrame(eventArgs);
        }
        /// <summary>
        /// 深拷贝一张图片
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        Bitmap deepCopyBitmap(Bitmap src)
        {
            Bitmap dest = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, src);
                ms.Seek(0, SeekOrigin.Begin);
                dest = (Bitmap)bf.Deserialize(ms);
                ms.Close();
            }
            return dest;
        }
        void tryFreeFrame(AForge.Video.NewFrameEventArgs e)
        {
            if (e != null && e.Frame != null)
            {
                try
                {
                    e.Frame.Dispose();
                }
                catch (Exception disposeFrameErr)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("释放截图帧信息失败:{0}", disposeFrameErr.Message);
                    Console.ResetColor();
                }
            }
        }
        #endregion
    }
}
