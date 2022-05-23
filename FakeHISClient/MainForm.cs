using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sharp7;
using AForge.Video.DirectShow;
using System.IO;
using AForge.Video;
using AMDM_Domain;
using Newtonsoft.Json;
//using VlcPlayer;
using LibVLCSharp.Shared;
using System.Diagnostics;
using MyCode;
using System.Threading;
using System.Speech.Synthesis;
using AMDM;
using AMDM.Manager;
using System.IO.Ports;
//using Modbus.Device;

/*2021年10月28日11:15:35  已完成PLC的SHARP7库调用和PLC的基本通讯.具备读写寄存器的能力.但暂不清楚DBNumber,起始位置,结束位置,读取类型的相关详细信息.目前只是能把信息写进去,也能读出来.
 */
namespace FakeHISClient
{
    public partial class MainForm : Form
    {
        #region 变量
        S7Client s7client = new S7Client();
        VideoWork camera;
        LibVLC libVLC = null;
        MediaPlayer videoPlayer = null;
        #region 设备集合,使用的设备
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private int flag = 1;
        private string dirc = Application.StartupPath + "\\Images";

        void InitCameraDriver()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (!Directory.Exists(dirc))
                Directory.CreateDirectory(dirc);

            try
            {
                // 枚举所有视频输入设备  
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                foreach (FilterInfo device in videoDevices)
                {
                    tscbxCameras.Items.Add(device.Name);
                }

                tscbxCameras.SelectedIndex = 0;
            }
            catch (ApplicationException)
            {
                tscbxCameras.Items.Add("No local capture devices");
                videoDevices = null;
            }
        }
        /// <summary>  
        /// 连接开启摄像头  
        /// </summary>  
        private void CameraConn()
        {
            if (videoDevices == null)
            {
                MessageBox.Show("选择要连接的设备");
                return ;
            }
            videoSource = new VideoCaptureDevice(videoDevices[tscbxCameras.SelectedIndex].MonikerString);
            //videoSource.VideoResolution = videoSource.VideoCapabilities[2];//比较快3也还行
            //videoSource.VideoResolution = videoSource.VideoCapabilities[0];//也还行
            videoSource.VideoResolution = videoSource.VideoCapabilities[0];
            videoSource.NewFrame += videoSource_NewFrame;
            //videoPlayer.Size = videoSource.VideoResolution.FrameSize;
            //videoPlayer.VideoSource = videoSource;
            //videoPlayer.Start();
            videoSource.Start();
        }
        #endregion

        #endregion

        #region 构造函数和窗体初始化
        public MainForm()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            FormAutoSizer autoSizer = new FormAutoSizer(this);
            autoSizer.TurnOnAutoSize();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.closePlcBtn.Enabled = false;
            App.Init(null, null, null);
            App.Setting.PlcSetting_西门子.MainPLC.IPAddress = this.plcIPCombox.Text;
            #region 串口列表
            string[] serialPortsNames = System.IO.Ports.SerialPort.GetPortNames();
            if (serialPortsNames != null && serialPortsNames.Length > 0)
            {
                foreach (var item in serialPortsNames)
                {
                    this.qrCodeScanerComCombobox.Items.Add(item);
                }
            }
            if (serialPortsNames!= null && serialPortsNames.Length>0)
            {
                this.qrCodeScanerComCombobox.Text = serialPortsNames[0];
            }
            #endregion
        }
        /// <summary>
        /// 初始化,读取主程序配置文件
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            //App.TTSSpeaker.Speak("加载完成,欢迎使用");
            return true;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            //this.connectPlcBtn_Click(sender, e);
            Console.WriteLine("工具测试主窗:初始化摄像头");
            InitCameraDriver();
            Console.WriteLine("工具测试主窗:摄像头初始化完成");


            //if (App.ControlAnimationRenderingController!= null)
            //{
            //    GridShower gs = new GridShower();
            //    gs.Init(100, 1, 2, 10);
            //    gs.Dock = DockStyle.Fill;
            //    //gs.BorderStyle = BorderStyle.Fixed3D;
            //    this.animateControlContainerLabel.Controls.Add(gs);
            //    App.ControlAnimationRenderingController.AddAnimationControls(gs);
            //}
            

            #region 初始化视频播放器
            string file = "C:\\1.mov";
            if (System.IO.File.Exists(file))
            {
                libVLC = new LibVLC(new string[] { });
                libVLC.Log += libVLC_Log;
                videoPlayer = new MediaPlayer(libVLC);
                videoPlayer.Hwnd = this.videoPlayerPanel.Handle;
                FileStream mediaStream = new FileStream(file, FileMode.Open);
                    //new FileStream(Application.StartupPath + "\\4.MP4", FileMode.Open);
                Media media = new Media(libVLC, mediaStream);
                videoPlayer.Volume = 10;
                videoPlayer.Play(media);

                //VlcPlayerBase player = new VlcPlayerBase(Application.StartupPath + "\\plugins");
                //player.SetRenderWindow(this.videoPlayerPanel.Handle.ToInt32());
                //player.LoadFile("\\4.mp4");
                //player.Play();
            }
            else
            {
                string vlcErr = "vlc要播放的视频文件不存在";
                Utils.LogError(vlcErr, file);
                //MessageBox.Show("", file);
            }
            #endregion

            

            //this.medicineDeliveryBtn_Click(sender, e);
        }

        void libVLC_Log(object sender, LogEventArgs e)
        {
            //只要对player添加了这个回调函数  player就不会输出一堆信息在控制台上影响其他内容的观看.
            // 可以在此函数内不做任何处理  想要获取的时候 直接同eLevel来确定日志文件是什么类型的即可.
            //但是这个回调函数并不能解决初始读取dll的时候发生的错误提示

            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(e.Level);
            //Console.ResetColor();
            //throw new NotImplementedException();
        }
        
        #endregion

        #region 连接plc
        private void connectPlcBtn_Click(object sender, EventArgs e)
        {
            string ip = plcIPCombox.Text;
            if (string.IsNullOrEmpty(ip) == true)
            {
                MessageBox.Show("需要输入plc的ip地址");
                return;
            }

            int rack = 0; int slot = 0;
            if (int.TryParse(rackCombox.Text, out rack) == false)
            {
                MessageBox.Show("无效的机架号");
            }
            if (int.TryParse(slotCombox.Text, out slot) == false)
            {
                MessageBox.Show("无效的插槽号");
            }
            int connectRet = s7client.ConnectTo(ip, rack, slot);
            if (connectRet != 0)
            {
                MessageBox.Show(string.Format("连接PLC失败,错误号:{0}", connectRet));
                return;
            }
            //MessageBox.Show("连接PLC成功");
            this.Text = "AMDM-PLC已连接";
            this.connectPlcBtn.Enabled = false;
            this.closePlcBtn.Enabled = true;
        }
        #endregion

        #region 读取数据
        private void readBtn_Click(object sender, EventArgs e)
        {
            int dbNumber = 0;
            int start = 0;
            int end = 0;
            string typeStr = null;
            if (int.TryParse(readDBNumberTexbox.Text, out dbNumber) == false ||
                int.TryParse(readStartTextbox.Text, out start) == false || int.TryParse(readEndTextbox.Text, out end) == false || string.IsNullOrEmpty(readTypeCombox.Text) == true || readTypeCombox.Text.Trim().Length < 1)
            {
                MessageBox.Show("读取设置错误");
                return;
            }
            if (start>=end)
            {
                MessageBox.Show("开始位置应该小于结束位置");
                return;
            }
            typeStr = readTypeCombox.Text.Trim();
            Type type = typeof(bool);
            switch (typeStr.ToLower())
            {
                case "bool":
                    type = typeof(bool);
                    break;
                default:
                    break;
            }
            byte[] buffer = new byte[end - start];
            int readRet = s7client.DBRead(dbNumber, start, end - start, buffer);
            #region 要读取的数据类型
            //string readType = string.Format("{0}", readTypeCombox.SelectedItem);
            //if (readType == "Int64")
            //{
            //    long lvalue = S7.GetDIntAt(buffer, 0);
            //    MessageBox.Show("long内容:", lvalue.ToString());
            //}
            //else if (readType == "Int32")
            //{
            //    int intVluae = S7.GetIntAt(buffer, 0);
            //    MessageBox.Show("int内容:"+intVluae.ToString());
            //}
            #endregion
            if (readRet != 0)
            {
                MessageBox.Show(string.Format("读取数据错误:{0}", readRet));
                return;
            }
            StringBuilder readRetSB = new StringBuilder();
            StringBuilder x2StringSBFull = new StringBuilder();
            StringBuilder readBufferAsString = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                if (i>=buffer.Length)
                {
                    break;
                }
                if (readRetSB.Length>0)
                {
                    readRetSB.Append("\r\n");
                }
                readRetSB.AppendFormat("{0} ({1}):{2}", i,i+start, buffer[i]);
                string currentX2 = Convert.ToString(Convert.ToInt32(buffer[i]),2).PadLeft(8,'0');
                x2StringSBFull.Append(currentX2);
                readBufferAsString.Append(Convert.ToChar(buffer[i]));
            }
            this.readBufferTextbox.Text = readRetSB.ToString();

            #region 显示二进制和通过8位一组重新组合到10进制的信息

            //string a = "2";
            //string b = "165";
            //string ax2 = Convert.ToString(Convert.ToInt32(a), 2);
            //string bx2 = Convert.ToString(Convert.ToInt32(b), 2);

            //string full = ax2 + bx2;

            try
            {
                long int64 = Convert.ToInt64(x2StringSBFull.ToString(), 2);
                this.readBufferX2ToIntTextbox.Text = int64.ToString();
            }
            catch (Exception err)
            {
                this.readBufferX2ToIntTextbox.Text = err.Message;
            }
            this.readBufferX2Textbox.Text = x2StringSBFull.ToString();
            
            #endregion
        }
        #endregion

        #region 写入数据
        private void writeBtn_Click(object sender, EventArgs e)
        {
            int dbNumber = 0;
            int start = 0;
            int end = 0;
            string typeStr = null;
            if (int.TryParse(this.writeDBNumberTexbox.Text, out dbNumber) == false ||
                int.TryParse(this.writeStartTextbox.Text, out start) == false || int.TryParse(this.writeEndTextbox.Text, out end) == false 
                || string.IsNullOrEmpty(this.writeTypeCombox.Text) == true 
                || this.writeTypeCombox.Text.Trim().Length < 1)
            {
                MessageBox.Show("写设置错误");
                return;
            }
            if (string.IsNullOrEmpty(writeBufferPer8IntTextbox.Text) == true)
            {
                MessageBox.Show("无效的写入内容");
            }
            if (writeBufferPer8IntTextbox.Text.Trim().Length == 0)
            {
                MessageBox.Show("写入内容为空");
            }
            typeStr = readTypeCombox.Text.Trim();
            Type type = typeof(bool);
            switch (typeStr.ToLower())
            {
                case "bool":
                    type = typeof(bool);
                    break;
                default:
                    break;
            }
            string writeBufferStrPer8 = this.writeBufferPer8IntTextbox.Text;
            
            #region 转换要写入的内容到buffer
            string[] writeByfferStrPer8Arr = writeBufferStrPer8.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] writeBufferBytes = new byte[writeByfferStrPer8Arr.Length];
            for (int i = 0; i < writeByfferStrPer8Arr.Length; i++)
            {
                writeBufferBytes[i] = Convert.ToByte(writeByfferStrPer8Arr[i]);
            }
            #endregion
            int writeRet = s7client.DBWrite(dbNumber, start, writeBufferBytes.Length, writeBufferBytes);
            if (writeRet != 0)
            {
                MessageBox.Show("写取数据错误");
                return;
            }
            //MessageBox.Show("写入完成");
        }
         #endregion

        #region 写入后延迟100毫秒读取按钮
        private void writeAndReadBtn_Click(object sender, EventArgs e)
        {
            this.writeBtn_Click(sender, e);
            System.Threading.Thread.Sleep(100);
            this.readBtn_Click(sender, e);
        }
        #endregion

        #region 读操作相关设置变更
        private void readDBNumberTexbox_TextChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.writeDBNumberTexbox.Text = this.readDBNumberTexbox.Text;
            }
        }

        private void readStartTextbox_TextChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.writeStartTextbox.Text = this.readStartTextbox.Text;
            }
            int start, end;
            if (int.TryParse(this.readStartTextbox.Text, out start) && int.TryParse(this.readEndTextbox.Text, out end))
            {
                int count = end - start;
                this.readCountTextBox.Text = count.ToString();
            }
        }

        private void readEndTextbox_TextChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.writeEndTextbox.Text = this.readEndTextbox.Text;
            }
            int start, end;
            if (int.TryParse(this.readStartTextbox.Text,out start) && int.TryParse(this.readEndTextbox.Text, out end))
            {
                int count = end - start;
                this.readCountTextBox.Text = count.ToString();
            }
        }

        private void readTypeCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.writeTypeCombox.Text = this.readTypeCombox.Text;
            }
        }
        #endregion

        #region 写操作相关设置变更
        private void writeDBNumberTexbox_TextChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.readDBNumberTexbox.Text = this.writeDBNumberTexbox.Text;
            }
        }

        private void writeStartTextbox_TextChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.readStartTextbox.Text = this.writeStartTextbox.Text;
            }
        }

        private void writeEndTextbox_TextChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.readEndTextbox.Text = this.writeEndTextbox.Text;
            }
        }

        private void writeTypeCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.paramChangeLinkCheckbox.Checked)
            {
                this.readTypeCombox.Text = this.writeTypeCombox.Text;
            }
        }
        #endregion

        #region 关闭连接
        private void closePlcBtn_Click(object sender, EventArgs e)
        {
            if (this.s7client.Connected)
            {
                int closeRet = this.s7client.Disconnect();
                if (closeRet != 0)
                {
                    MessageBox.Show("断开连接错误");
                }
                else
                {
                    this.connectPlcBtn.Enabled = true;
                    this.closePlcBtn.Enabled = false;
                    this.Text = "AMDM-PLC未连接...";
                }
            }
        }
        #endregion

        #region plc连接设置变更
        private void rackCombox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void slotCombox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void plcIPCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            App.Setting.PlcSetting_西门子.MainPLC.IPAddress = plcIPCombox.Text;
        }
        #endregion

        #region 测试调用PLC上的函数
        private void callFuncOnPlcTestBtn_Click(object sender, EventArgs e)
        {
            //s7client.MBRead
        }
        #endregion

        #region 写数据的进制转换
        TextBox changingWriteTextbox = null;
        private void writeBufferTextbox_TextChanged(object sender, EventArgs e)
        {
            if (this.changingWriteTextbox != null)
            {
                return;
            }
            string text = writeBufferTextbox.Text;
            try
            {
                StringBuilder x2SB = new StringBuilder();
                StringBuilder stringPer8 = new StringBuilder();
                for (int i = 0; i < text.Length; i++)
                {
                    char current = text[i];
                    byte currentByte = Convert.ToByte(current);
                    string x2 = Convert.ToString(currentByte, 2);
                    x2 = x2.PadLeft(8, '0');
                    x2SB.Append(x2);
                    stringPer8.AppendFormat("{0} ", currentByte);
                }
                long lvalue = Convert.ToInt64(x2SB.ToString(), 2);
                this.changingWriteTextbox = writeBufferTextbox;
                this.writeBufferLongTextbox.Text = lvalue.ToString();
                this.writeBufferX2Textbox.Text = x2SB.ToString();
                this.writeBufferPer8IntTextbox.Text = stringPer8.ToString();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            this.changingWriteTextbox = null;
        }


        private void writeBufferX2Textbox_TextChanged(object sender, EventArgs e)
        {
            if (this.changingWriteTextbox != null)
            {
                return;
            }
            string text = this.writeBufferX2Textbox.Text;
            try
            {
                long lvalue = Convert.ToInt64(text, 2);
                int byteCount = text.Length / 8;
                int forCount = byteCount + (text.Length % 8 == 0 ? 0 : 1);
                StringBuilder stringTextSB = new StringBuilder();
                StringBuilder stringTextPer8 = new StringBuilder();
                for (int i = 0; i < forCount; i++)
                {
                    string current8bit = text.Substring(i * 8);
                    if (current8bit.Length>8)
                    {
                        current8bit = current8bit.Substring(0, 8);
                    }
                    int currentByte = Convert.ToInt32(current8bit, 2);
                    char currentChar = Convert.ToChar(currentByte);
                    stringTextSB.Append(currentChar);
                    stringTextPer8.AppendFormat("{0} ",currentByte);
                }
                #region 二进制分段值

                #endregion
                this.changingWriteTextbox = writeBufferX2Textbox;
                this.writeBufferLongTextbox.Text = lvalue.ToString();
                this.writeBufferTextbox.Text = stringTextSB.ToString();
                this.writeBufferPer8IntTextbox.Text = stringTextPer8.ToString();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            this.changingWriteTextbox = null;
        }

        private void writeBufferLongTextbox_TextChanged(object sender, EventArgs e)
        {
            if (this.changingWriteTextbox != null)
            {
                return;
            }
            string text = writeBufferLongTextbox.Text;
            try
            {
                long l = 0;
                if (long.TryParse(text, out l))
                {
                    string x2 = Convert.ToString(l, 2);
                    int byteCount = x2.Length / 8;
                    x2 = x2.PadLeft(8 * (byteCount+(x2.Length%8==0?0:1)), '0');
                    byteCount = x2.Length / 8;

                    #region 把二进制转换成string
                    int forCount = byteCount + (x2.Length % 8 == 0 ? 0 : 1);
                    StringBuilder stringTextSB = new StringBuilder();
                    StringBuilder stringTextPer8 = new StringBuilder();
                    for (int i = 0; i < forCount; i++)
                    {
                        string current8bit = x2.Substring(i * 8);
                        if (current8bit.Length > 8)
                        {
                            current8bit = current8bit.Substring(0, 8);
                        }
                        int currentByte = Convert.ToInt32(current8bit, 2);
                        char currentChar = Convert.ToChar(currentByte);
                        stringTextSB.Append(currentChar);
                        stringTextPer8.AppendFormat("{0} ", currentByte);
                    }
                    #endregion
                    this.changingWriteTextbox = writeBufferLongTextbox;
                    this.writeBufferX2Textbox.Text = x2;
                    this.writeBufferTextbox.Text = stringTextSB.ToString();
                    this.writeBufferPer8IntTextbox.Text = stringTextPer8.ToString();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            this.changingWriteTextbox = null;
        }
        #endregion

        private void viewCameraBtn_Click(object sender, EventArgs e)
        {
            if (App.CameraSnapshotCapturer == null)
            {
                App.CameraSnapshotCapturer = new CameraSnapshotCapturer(App.Setting.DevicesSetting.CameraSetting.CameraMonikerStringName);
            }
            App.CameraSnapshotCapturer.Disconnect();
            System.Threading.Thread.Sleep(1000);
            //Panel panel = this.cameraPanel;
            //if (camera == null)
            //{
            //    camera = new VideoWork(panel.Handle, 0, 0, panel.Width, panel.Height);
            //}
            //camera = new VideoWork(panel.Handle, 0, 0, panel.Width, panel.Height);
            if (cameraThread != null)
	{
                cameraThread.Abort();
	}
            cameraThread = new System.Threading.Thread(CameraThreadFunc);
            cameraThread.Start();
            
        }
        System.Threading.Thread cameraThread = null;

        DateTime lastCaptureTime = DateTime.Now;
        void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            GC.Collect();
            //return;
            try
            {
                
                if ((DateTime.Now-lastCaptureTime).Milliseconds<33)
                {
                    System.Threading.Thread.Sleep(11);
                    return;
                }
                lastCaptureTime = DateTime.Now;
                this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                this.pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            }
            catch (Exception)
            {
                
                throw;
            }
            
            //throw new NotImplementedException();
        }
        void CameraThreadFunc()
        {
            CameraConn();
        }

        private void stopCameraViewBtn_Click(object sender, EventArgs e)
        {
            //camera.Stop();
            //videoPlayer.SignalToStop();
            //videoPlayer.WaitForStop();
            if (videoSource!= null)
            {
                videoSource.NewFrame -= this.video_NewFrame;
                videoSource.Stop();
            }
        }

        private void firstStockManageFormBtn_Click(object sender, EventArgs e)
        {            
            AMDM_Stock index0Stock = null;
            #region 加载给定的药仓数据
            try
            {
                index0Stock = App.stockLoader.LoadStock(0);
            }
            catch (Exception loadStockErr)
            {
                MessageBox.Show(string.Format("读取药仓信息失败!{0}", loadStockErr.Message));
                return;
            }

            if (index0Stock == null)
            {
                MessageBox.Show("读取药仓信息失败!数据读取未发生错误,读取到的结果为空");
                return;
            }
            #endregion

            StockEditForm stockForm = new StockEditForm();
            AMDMHardwareInfoManager imanager = new AMDMHardwareInfoManager(App.sqlClient);

            stockForm.Init(App.Setting.PlcSetting_台达.StocksPLC[0], index0Stock, imanager, App.bindingManager);
            stockForm.ShowDialog();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            if (flag == 0)
            {
                string img = dirc + "/" + DateTime.Now.Ticks.ToString() + ".jpg";
                bitmap.Save(img);
                flag = 1;
            }
        }

        private void captureByCameraBtn_Click(object sender, EventArgs e)
        {
            flag = 0;
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
        }

        private void testSendMedicineGettingCMDBtn_Click(object sender, EventArgs e)
        {
            //MedicinePLCSetting setting = new MedicinePLCSetting();
            //setting.IPAddress = "192.168.50.40";
            //PLCCommunicator pc = new PLCCommunicator(setting, 310, -19, 34);
            //AMDMInfoManager am = new AMDMInfoManager();
            //AMDM_Grid destGrid = am.LoadStock(0).Floors[0].Grids[0];
            //float xpos = destGrid.LeftMM + (destGrid.RightMM - destGrid.LeftMM) /2;
            //pc.SendMedicineGettingInfoCommand((int)xpos, destGrid.FloorIndex, WhichGrabberEnum.Near, 1);

            //pc.SendStartMedicineGettingCommand();

            //System.Threading.Thread.Sleep(100);
            //this.plcIPCombox.Text = setting.IPAddress;
            //this.connectPlcBtn_Click(sender, e);
            //this.readBtn_Click(sender, e);
        }

        private void plcIPCombox_TextUpdate(object sender, EventArgs e)
        {
            App.Setting.PlcSetting_西门子.MainPLC.IPAddress = plcIPCombox.Text;
        }

        private void medicineBindingManageBtn_Click(object sender, EventArgs e)
        {
            AMDM_Stock index0Stock = null;
            #region 加载给定的药仓数据
            try
            {
                index0Stock = App.stockLoader.LoadStock(0);
            }
            catch (Exception loadStockErr)
            {
                MessageBox.Show(string.Format("读取药仓信息失败!{0}", loadStockErr.Message));
                return;
            }

            if (index0Stock == null)
            {
                MessageBox.Show("读取药仓信息失败!数据读取未发生错误,读取到的结果为空");
                return;
            }
            #endregion
            MedicineBindingManageForm mform = new MedicineBindingManageForm();
            mform.StartPosition = FormStartPosition.CenterParent;
            mform.Init(index0Stock);
            mform.ShowDialog();
        }

        #region 点击上药按钮后 打开上药窗口
        private void medicineInventoryMangeBtn_Click(object sender, EventArgs e)
        {
            AMDM_Stock index0Stock = null;
            #region 加载给定的药仓数据
            try
            {
                index0Stock = App.stockLoader.LoadStock(0);
            }
            catch (Exception loadStockErr)
            {
                MessageBox.Show(string.Format("读取药仓信息失败!{0}", loadStockErr.Message));
                return;
            }

            if (index0Stock == null)
            {
                MessageBox.Show("读取药仓信息失败!数据读取未发生错误,读取到的结果为空");
                return;
            }
            #endregion
            App.medicinesGettingController.Init(new List<AMDM_Stock>() { index0Stock });
            MedicineInventoryManageForm mform = new MedicineInventoryManageForm();
            mform.StartPosition = FormStartPosition.CenterParent;
            mform.Init(index0Stock);
            mform.ShowDialog();
        }
        #endregion

        #region 点击取药按钮后 进入取药页面
        private void medicineDeliveryBtn_Click(object sender, EventArgs e)
        {
            InitializerForm iform = new InitializerForm();
            iform.ShowDialog();
            //AMDM_Stock index0Stock = null;
            //#region 加载给定的药仓数据
            //try
            //{
            //    index0Stock = App.stockLoader.LoadStock(0);
            //}
            //catch (Exception loadStockErr)
            //{
            //    MessageBox.Show(string.Format("读取药仓信息失败!{0}", loadStockErr.Message));
            //    return;
            //}

            //if (index0Stock == null)
            //{
            //    MessageBox.Show("读取药仓信息失败!数据读取未发生错误,读取到的结果为空");
            //    return;
            //}
            //#endregion
            //MedicineDeliveryForm mform = new MedicineDeliveryForm();
            //mform.StartPosition = FormStartPosition.CenterParent;
            //mform.Init(new List<AMDM_Stock>() { index0Stock});
            //mform.ShowDialog();
        }
        #endregion

        #region 模拟打印付药单
        private void simulatePrintDeliveryRecoryPaperBtn_Click(object sender, EventArgs e)
        {
            if (App.inventoryManager  == null)
            {
                MessageBox.Show("未初始化库存管理器");
                return;
            }
            AMDM_DeliveryRecord record = App.inventoryManager.GetDeliveryRecordFull(222);
            if (record == null)
            {
                Utils.LogWarnning("根据付药单id222没有获取到付药单记录");
                return;
            }
            Utils.LogInfo("开始打印付药单:", record);
            App.DeliveryRecordPaperPrinter.Print(record);
        }
        #endregion

        #region 模拟显示处方信息
        private void simulateShowMedicineOrderBtn_Click(object sender, EventArgs e)
        {
            FullScreenMedicineOrderShower mform = new FullScreenMedicineOrderShower();
            mform.Show();
        }
        #endregion

        private void showAutoCloseFormBtn_Click(object sender, EventArgs e)
        {
            //Form form = new Form();
            //form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //form.Location = new Point(0, 0);
            //form.Size = Screen.PrimaryScreen.Bounds.Size;
            //NoticeShower ns = new NoticeShower();

            //form.Controls.Add(ns);
            //ns.Dock = DockStyle.Fill;
            

            //form.Show();
            //ns.ShowAndAutoClose("22222", "3432432", 2000, (s)=> {
            //    ns.Hide();
            //    form.Close();
            //    form.Dispose();
            //});

            FullScreenNoticeShower fs = new FullScreenNoticeShower();
            fs.ShowAndAutoClose("message content", "title",true, 2000, null);
        }

        private void showAutoCloseMedicineOrderFormBtn_Click(object sender, EventArgs e)
        {
            FullScreenMedicineOrderShower fs = new FullScreenMedicineOrderShower();
            //fs.Order = 
                AMDM_MedicineOrder order = new AMDM_MedicineOrder();
                fs.Init(order, 5000, (s,t) =>
                    {
                        order = null;
                        MessageBox.Show("超时没点");
                    }
                    );
                DialogResult ret = fs.ShowDialog();
                if (ret == System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("开始取药任务执行");
                }
        }
        private void showMedicinesGettingFullScreenFormBtn(object sender, EventArgs e)
        {
            FullScreenMedicinesGettingStatusForm fullScreenMedicinesGettingStatusForm = new FullScreenMedicinesGettingStatusForm();
            fullScreenMedicinesGettingStatusForm.ShowDialog();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process current = Process.GetCurrentProcess();
            this.stopCameraViewBtn_Click(sender, e);
            if (this.cameraThread!= null)
            {
                this.cameraThread.Abort();
                
            }

            //fullScreenMedicinesGettingStatusForm.Dispose();
            ProcessThreadCollection allThreads = current.Threads;
            if (App.CameraSnapshotCapturer != null)
            {
                App.CameraSnapshotCapturer.Disconnect();
            }
            App.Dispose();
        }
        #region 显示一个播放 正在取药中的视频的页面 该页面可以被外界触发. 也可以播放完毕后自动关闭 通过属性来设定
        
        #endregion
        private void showMedicinesGettingStatusVideoFormBtn_Click(object sender, EventArgs e)
        {
            FullScreenVideoShowerV3 shower = new FullScreenVideoShowerV3();
            shower.VideoFilePath = Application.StartupPath + "\\advideos\\medicinesgetting\\给钱的.mov";
            shower.HideAfterFinished = true;

            #region 定时在其他线程中进行关闭
            System.Threading.ThreadPool.QueueUserWorkItem((res) => {
                System.Threading.Thread.Sleep(3000);
                shower.NeedForceHide = true;
            });
            #endregion
            shower.Show();
            //DialogResult ret = shower.ShowDialog();
            //if (shower.InvokeRequired)
            //{
            //    Utils.LogInfo("在单独的线程中获取到了返回");
            //}
            //else
            //{
            //    Utils.LogInfo("在打开shower的线程中获取到了返回");
            //}
            string msg = "shower的showdialog收到了返回";
            //MessageBox.Show(msg);
            Utils.LogInfo(msg);
        }

        #region 多次连续的尝试播放TTS音频
        BackgroundWorker bww = null;
        string locker = "333";
        private void trySpeakTooMuchTTSBtn_Click(object sender, EventArgs e)
        {
            if (bww == null)
            {
                bww = new BackgroundWorker();
                bww.DoWork += bww_DoWork;
                bww.WorkerReportsProgress = true;
                bww.ProgressChanged += bww_ProgressChanged;
            }
            bww.RunWorkerAsync();
        }

        void bww_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string msg = e.UserState as string;
            speak3(msg);
            //speak(msg);
            //speak2(msg);
        }

        void bww_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();
            long times = 0;
            for (int i = 0; i < 100; i++)
            {
                Utils.LogInfo("使用ThreadPool.QueueUserWorkItem尝试播放", i);
                ThreadPool.QueueUserWorkItem(
                    (res) =>
                    {
                        lock (locker)
                        {
                            //Utils.LogInfo("将要播放 第", times);
                            string msg = string.Format("这是第{0}次播放音频文件", times++);
                            speak3(msg);
                        }
                    }
                        );
                //Utils.LogInfo("将要播放 第", times);
                //string msg = string.Format("这是第{0}次播放音频文件", times++);
                ////bww.ReportProgress(0, msg);
                //speak3(msg);
            }
        }
        delegate void SpeakFunc(string msg);
        /// <summary>
        /// 使用引用的    D:\源代码\FakeHISClient\FakeHISClient\obj\Debug\Interop.SpeechLib.dll播放文件 连续异步播放20次或者更多次后,再次播放会出现异常 msvcrt的文件就会导致程序溢出
        /// 放弃的方案
        /// </summary>
        /// <param name="msg"></param>
        void speak(string msg)
        {
            if (this.InvokeRequired)
            {
                SpeakFunc fc = new SpeakFunc(speak);
                this.BeginInvoke(fc, msg);
                Utils.LogInfo("需要转发到主线程上调用");
                return;
            }
            try
            {
                Utils.LogInfo("正在播放:", msg);
                App.TTSSpeaker.Speak(msg);
            }
            catch (Exception err)
            {
                Utils.LogError("播放音频文件出现错误", err.Message);
            }

            //System.Threading.Thread.Sleep(111);
        }
        SpeechSynthesizer speaker = new SpeechSynthesizer();
        /// <summary>
        /// 使用.net3.5以后具有的  system.speech的程序及  可以正确的释放和停止音频.每次音频结束以后直接把之前的SpeechSynthesizer dispose掉然后在播放文件 每次播放99次然后再重新开始 都正常
        /// 最终定稿方案
        /// </summary>
        /// <param name="msg"></param>
        void speak3(string msg)
        {
            //if (this.InvokeRequired)
            //{
            //    SpeakFunc fc = new SpeakFunc(speak3);
            //    this.BeginInvoke(fc, msg);
            //    //Utils.LogInfo("3需要转发到主线程上调用");
            //    return;
            //}
            try
            {
                #region 使用每次重新初始化的方法  没有问题
                if (speaker != null)
                {
                    speaker.Dispose();
                }
                speaker = new SpeechSynthesizer();
                #endregion
                #region 使用每次都cancel的方法  测试 2021年12月1日09:26:11 这样是行不通的
                //if (speaker!= null)
                //{
                //    speaker.SpeakAsyncCancelAll();
                //    //加上下面这一行 重新初始化一个播放器就可以了.
                //    //speaker = new SpeechSynthesizer();
                //}
                #endregion

                speaker.SpeakAsync(msg);
            }
            catch (Exception err)
            {
                Utils.LogError("播放音频文件出现错误", err.Message);
            }

            //System.Threading.Thread.Sleep(111);
        }

        #region sapi的回调函数
        //void sp_Word(int StreamNumber, object StreamPosition, int CharacterPosition, int Length)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_VoiceChange(int StreamNumber, object StreamPosition, SpeechLib.SpObjectToken VoiceObjectToken)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_Viseme(int StreamNumber, object StreamPosition, int Duration, SpeechLib.SpeechVisemeType NextVisemeId, SpeechLib.SpeechVisemeFeature Feature, SpeechLib.SpeechVisemeType CurrentVisemeId)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_StartStream(int StreamNumber, object StreamPosition)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_Sentence(int StreamNumber, object StreamPosition, int CharacterPosition, int Length)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_Phoneme(int StreamNumber, object StreamPosition, int Duration, short NextPhoneId, SpeechLib.SpeechVisemeFeature Feature, short CurrentPhoneId)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_EnginePrivate(int StreamNumber, int StreamPosition, object EngineData)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_Bookmark(int StreamNumber, object StreamPosition, string Bookmark, int BookmarkId)
        //{
        //    //throw new NotImplementedException();
        //}

        //void sp_AudioLevel(int StreamNumber, object StreamPosition, int AudioLevel)
        //{
        //    //throw new NotImplementedException();
        //}
        #endregion

        void sp_EndStream(int StreamNumber, object StreamPosition)
        {
            //throw new NotImplementedException();
        }
        //dynamic spVoiceRef = null;
        /// <summary>
        /// 使用动态初始化com组件的形式播放音频文件，在连续播放20次或者更多次以后，再次启动播放任务会发生msvcrt。dll的异常 应用程序崩溃
        /// </summary>
        /// <param name="msg"></param>
        void speak2(string msg)
        {
            ThreadPool.QueueUserWorkItem(
                (res) =>
                {
                    lock (locker)
                    {
                        //if (spVoiceRef == null)
                        //{
                        //    Utils.LogInfo("尝试使用com方式初始化语音朗读引擎");
                        //    Type type = Type.GetTypeFromProgID("SAPI.SpVoice");
                        //    dynamic spVoice = Activator.CreateInstance(type);
                        //    spVoice.Speak("欢迎使用!");
                        //    Utils.LogInfo("已播放欢迎使用");
                        //    spVoiceRef = spVoice;
                        //}
                        try
                        {
                            Utils.LogInfo("尝试使用com方式初始化语音朗读引擎");
                            Type type = Type.GetTypeFromProgID("SAPI.SpVoice");
                            foreach (var item in type.Assembly.GetTypes())
                            {
                                if (item.Name.ToLower().Contains("SAPI"))
                                {
                                    Utils.LogInfo(item.Name);
                                }
                            }
                            dynamic spVoice = Activator.CreateInstance(type);
                            //spVoice.Speak("欢迎使用!");
                            //Utils.LogInfo("已播放欢迎使用");
                            //spVoiceRef = spVoice;


                            Utils.LogSuccess("开始使用speak2播放");
                            int param = 1 | 2 | 128;
                            spVoice.Speak(msg, param);
                            Utils.LogFinished("使用speak2播放完毕", msg, param);
                            GC.Collect();
                        }
                        catch (Exception err)
                        {
                            Utils.LogError("使用speak2播放出现错误", err.Message);
                        }
                    }
                }
                );


        }
        #endregion

        private void secondStockManageFormBtn_Click(object sender, EventArgs e)
        {
            AMDM_Stock waitShowStock = null;
            #region 加载给定的药仓数据
            try
            {
                waitShowStock = App.stockLoader.LoadStock(1);
            }
            catch (Exception loadStockErr)
            {
                MessageBox.Show(string.Format("读取药仓信息失败!{0}", loadStockErr.Message));
                return;
            }
            AMDMHardwareInfoManager hardwareManager = new AMDMHardwareInfoManager(App.sqlClient);
            if (waitShowStock == null)
            {
                MessageBox.Show("读取药仓信息失败!数据读取未发生错误,读取到的结果为空");
                AMDM_Machine machine = new AMDM_Machine();
                AMDM_Stock_data insertedData = hardwareManager.CreateAndJoinStock(ref machine,
                    App.Setting.HardwareSetting.Stock.CenterDistanceBetweenTwoGrabbers, 
                    0, 
                    (int)App.Setting.HardwareSetting.Stock.MaxFloorsHeightMM, 
                    (int)App.Setting.HardwareSetting.Stock.MaxFloorWidthMM, 
                    Utils.GetRandomSN(6,6), App.Setting.HardwareSetting.Stock.XOffsetFromStartPointMM, App.Setting.HardwareSetting.Stock.YOffsetFromStartPointMM
                    );
                waitShowStock = App.stockLoader.LoadStock(1);
                if (waitShowStock == null)
                {
                    return;//开始没读到,自动创建也失败,真是失败失败
                }
            }
            #endregion

            StockEditForm stockForm = new StockEditForm();
            stockForm.Init(App.Setting.PlcSetting_台达.StocksPLC[0], waitShowStock, hardwareManager, App.bindingManager);
            stockForm.ShowDialog();
        }
        /// <summary>
        /// 模拟使用默认参数创建一个药仓
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        AMDM_Stock simulateCreateStockByDefaultSetting(int machineId)
        {
            AMDM_Stock stock = new AMDM_Stock()
            {
                CenterDistanceBetweenTwoGrabbers = App.Setting.HardwareSetting.Stock.CenterDistanceBetweenTwoGrabbers,
                MaxFloorsHeightMM = (int)App.Setting.HardwareSetting.Stock.MaxFloorsHeightMM,
                FirstLayoutMedicineUserId = 0,
                FirstLayoutTime = DateTime.Now,
                MachineId = machineId,
                IndexOfMachine = 0,
                Floors = new Dictionary<int, AMDM_Floor>(),
                MaxFloorWidthMM =
                    App.Setting.HardwareSetting.Stock.MaxFloorWidthMM,
                SerialNumber = Utils.GetRandomSN(6, 6),
                XOffsetFromStartPointMM = App.Setting.HardwareSetting.Stock.XOffsetFromStartPointMM,
                YOffsetFromStartPointMM = App.Setting.HardwareSetting.Stock.YOffsetFromStartPointMM
            };
            return stock;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ModbusConnectTest mctform = new ModbusConnectTest();
            mctform.StartPosition = FormStartPosition.CenterParent;
            mctform.ShowDialog();
        }

        private void 部署按钮_Click(object sender, EventArgs e)
        {
            自动付药机客户端部署工具.自动付药机客户端部署工具 f = new 自动付药机客户端部署工具.自动付药机客户端部署工具();
            f.ShowDialog();
        }
    }
    #region 累定义:设置
    //public class Setting
    //{
    //    public Setting()
    //    {
    //        PlcIPHistory = new List<string>();
    //    }
    //    public List<string> PlcIPHistory { get; set; }
    //    public string CurrentPlcIp { get; set; }
    //}
    #endregion
}
