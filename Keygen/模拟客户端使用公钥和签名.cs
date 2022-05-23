//using AMDM;
using MyCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
//using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keygen
{
    public partial class 模拟客户端使用公钥和签名 : Form
    {
        public 模拟客户端使用公钥和签名()
        {
            InitializeComponent();
        }

        private void 加载公钥资源文件_Click(object sender, EventArgs e)
        {

            加载到的公钥文件.Text = loadPublicKeyFile();
        }
        string loadPublicKeyFile()
        {
            PublicKeyFileName nameGetter = new PublicKeyFileName();
            string name = nameGetter.Get();
            List<int> intBytes = new List<int>();
            foreach (var item in name)
            {
                intBytes.Add((int)item);
            }
            //把这个int数组拉取出来
            List<int> inputIntBytes = new List<int>();
            inputIntBytes.AddRange(intBytes);
            List<byte> outputBytes = new List<byte>();
            //便利每一个byte,按位取反,得到byte
            foreach (int i in inputIntBytes)
            {
                byte b = (byte)i;
                b = (byte)~b;
                #region 高低位反转
                string newB2 = Convert.ToString(b, 2);
                newB2 = newB2.PadLeft(8, '0');
                newB2 = Revease3(newB2);
                b = Convert.ToByte(newB2, 2);
                #endregion

                outputBytes.Insert(0, b);
            }
            //取反后的byte数组反向排列,上面已经直接在foreach循环中直接排序完了
            //把这个byte数组使用utf8转换成string
            string decodedFromIntList = Encoding.UTF8.GetString(outputBytes.ToArray());

            Assembly assm = Assembly.GetExecutingAssembly();
            Stream istr = assm.GetManifestResourceStream(string.Format("Keygen.{0}.bin", decodedFromIntList));
            System.IO.StreamReader sr = new System.IO.StreamReader(istr);
            string str = sr.ReadLine();
            return str;
        }

        string Revease3(string original)
        {
            int length = original.Length;
            StringBuilder sb = new StringBuilder(length);
            for (int i = length - 1; i >= 0; i--)
                sb.Append(original[i]);
            return sb.ToString();
        }

        private void 加载签名资源文件_Click(object sender, EventArgs e)
        {
            加载到的签名文件.Text = loadSignFile();
        }
        string loadSignFile()
        {
            SignFileName nameGetter = new SignFileName();
            string name = nameGetter.Get();
            List<int> intBytes = new List<int>();
            foreach (var item in name)
            {
                intBytes.Add((int)item);
            }
            //把这个int数组拉取出来
            List<int> inputIntBytes = new List<int>();
            inputIntBytes.AddRange(intBytes);
            List<byte> outputBytes = new List<byte>();
            //便利每一个byte,按位取反,得到byte
            foreach (int i in inputIntBytes)
            {
                byte b = (byte)i;
                b = (byte)~b;
                #region 高低位反转
                string newB2 = Convert.ToString(b, 2);
                newB2 = newB2.PadLeft(8, '0');
                newB2 = Revease3(newB2);
                b = Convert.ToByte(newB2, 2);
                #endregion

                outputBytes.Insert(0, b);
            }
            //取反后的byte数组反向排列,上面已经直接在foreach循环中直接排序完了
            //把这个byte数组使用utf8转换成string
            string decodedFromIntList = Encoding.UTF8.GetString(outputBytes.ToArray());

            Assembly assm = Assembly.GetExecutingAssembly();
            Stream istr = assm.GetManifestResourceStream(string.Format("Keygen.{0}.bin", decodedFromIntList));
            if (istr == null)
            {
                MessageBox.Show("加载文件错误");
                return null;
            }
            System.IO.StreamReader sr = new System.IO.StreamReader(istr);
            string str = sr.ReadLine();
            return str;
        }

        delegate void setFileNameContentInUIThreadFunc(string publicKeyFile, string signFile);
        void setFileNameContentInUIThread(string publicKeyFile, string signFile)
        {
            if (this.InvokeRequired)
            {
                setFileNameContentInUIThreadFunc fc = new setFileNameContentInUIThreadFunc(setFileNameContentInUIThread);
                this.BeginInvoke(fc, publicKeyFile, signFile);
                return;
            }
            this.加载到的公钥文件.Text = publicKeyFile;
            this.加载到的签名文件.Text = signFile;
            DateTime now = DateTime.Now;

            Utils.LogFinished("公钥文件和签名文件全部加载完成,总用时", (now-startTime).TotalMilliseconds);
        }

        string publicKeyFile;
        KeyInfo key = null;
        string signFile;
        bool getPublicKeyThreadFinished;
        bool getSignThreadFinished;
        DateTime startTime = DateTime.MinValue;
        DeviceInfo deviceInfo = null;
        private void 异步解析公钥文件和资源文件_Click(object sender, EventArgs e)
        {
            this.加载到的公钥文件.Text = "";
            this.加载到的签名文件.Text = "";
            Console.Clear();
            Utils.LogSuccess("清空显示成功,准备开始工作");
            startTime = DateTime.Now;
            int timeOutSpanMS = 60000;
            //启动检查是否全部完成线程
            ThreadPool.QueueUserWorkItem((res) =>
            {
#region 获取设备信息
                Computer computer = new Computer();
            deviceInfo = new DeviceInfo();
            deviceInfo.CPUSN = computer.CpuID;
            deviceInfo.HarddiskSN = computer.DiskSN;
            deviceInfo.MAC = computer.MacAddress;
            string deviceInfoJson = Newtonsoft.Json.JsonConvert.SerializeObject(deviceInfo);
	#endregion
                Utils.LogStarted("进入主检测线程");
                DateTime lastDoTime = DateTime.Now;
                sleepRandom(0, 1000);
                Utils.LogStarted("主线程开始工作");
                while ((DateTime.Now - startTime).TotalMilliseconds < timeOutSpanMS)
                {
                    if ((lastDoTime - DateTime.Now).TotalMilliseconds > 1000)
                    {
                        MessageBox.Show("你干啥呢????");
                        Application.Exit();
                        return;
                    }
                    if (getPublicKeyThreadFinished && getSignThreadFinished && this.key!= null)
                    {
                        break;
                    }
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(157);
                    lastDoTime = DateTime.Now;
                }
                if (publicKeyFile == null || publicKeyFile == null)
                {
                    MessageBox.Show("错了不行啊你  解密不了");
                    Application.Exit();
                    return;
                }
                //this.setFileNameContentInUIThread(this.publicKeyFile, this.signFile);
                ThreadPool.QueueUserWorkItem(
                    (ree)=>
                    {
                        sleepRandom(0, 900);
                        while ((DateTime.Now - startTime).TotalMilliseconds < timeOutSpanMS)
                        {
                            if ((lastDoTime - DateTime.Now).TotalMilliseconds > 1000)
                            {
                                MessageBox.Show("你干啥呢????");
                                Application.Exit();
                                return;
                            }
                            if (getPublicKeyThreadFinished && getSignThreadFinished && this.key != null && this.deviceInfo != null && deviceInfoJson!= null)
                            {
                                break;
                            }
                            Application.DoEvents();
                            System.Threading.Thread.Sleep(157);
                            lastDoTime = DateTime.Now;
                        }
                    }
                    );
                if (getPublicKeyThreadFinished && getSignThreadFinished && this.key != null && this.deviceInfo != null && deviceInfoJson != null)
                {
                    string deviceInfoJsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(deviceInfoJson));
                    if (VerifySignedHash(deviceInfoJsonBase64, this.signFile, this.key.PublicKey))
                    {
                        Utils.LogSuccess("校验签名信息成功");
                        this.setFileNameContentInUIThread(Newtonsoft.Json.JsonConvert.SerializeObject(this.key), this.signFile);
                    }
                    else
                    {
                        //MessageBox.Show("校验签名信息失败");
                        //Application.Exit();
                        CrashApp();
                        return;
                    }
                }
            }
                );
            //启动等待解密公钥文件线程
            ThreadPool.QueueUserWorkItem((res)=>
                {
                    Utils.LogStarted("进入解密公钥文件线程");
                    ThreadPool.QueueUserWorkItem(
                        (res22)=>
                        {
                            sleepRandom(0, 1000);
                            Utils.LogStarted("在解密公钥文件线程中进入加载公钥文件线程");
                            this.publicKeyFile = this.loadPublicKeyFile();
                        }
                        );
                    ThreadPool.QueueUserWorkItem(
                        (res22) =>
                        {
                            sleepRandom(0, 1000);
                            Utils.LogStarted("在解密公钥文件线程中等待待解密的公钥文件被加载...");
                            DateTime lastDoTime = DateTime.Now;
                            while ((DateTime.Now - startTime).TotalMilliseconds < timeOutSpanMS)
                            {
                                if ((lastDoTime - DateTime.Now).TotalMilliseconds > 1000)
                                {
                                    MessageBox.Show("你干啥呢????");
                                    Application.Exit();
                                    return;
                                }
                                if (this.publicKeyFile != null)
                                {
                                    Utils.LogFinished("公钥加密文件已经加载");
                                    break;
                                }
                                Application.DoEvents();
                                System.Threading.Thread.Sleep(13);
                                lastDoTime = DateTime.Now;
                            }
                            ThreadPool.QueueUserWorkItem(
                                (rrr)=>
                                {
                                    Utils.LogStarted("解密已经被加密的公钥文件");
                                    sleepRandom(0, 100);
                                    DateTime lastDoTime2 = DateTime.Now;
                                    while ((DateTime.Now - startTime).TotalMilliseconds < timeOutSpanMS)
                                    {
                                        if ((lastDoTime2 - DateTime.Now).TotalMilliseconds > 1000)
                                        {
                                            MessageBox.Show("你干啥呢????");
                                            Application.Exit();
                                            return;
                                        }
                                        if (this.publicKeyFile != null)
                                        {
                                            Utils.LogFinished("已经加密的公钥文件已经被加载");
                                            break;
                                        }
                                        Application.DoEvents();
                                        System.Threading.Thread.Sleep(13);
                                        lastDoTime2 = DateTime.Now;
                                    }
                                    if (this.publicKeyFile!= null)
                                    {
                                        this.key = Ciphertext.Decode<KeyInfo>(this.publicKeyFile);
                                    }
                                }
                                );
                        }
                        );
                    
                    sleepRandom(0, 1000);
                    Utils.LogStarted("解密公钥文件线程开始检查或等待工作");
                    DateTime lastDoTime1 = DateTime.Now;
                    while ((DateTime.Now-startTime).TotalMilliseconds<timeOutSpanMS)
                    {
                        if ((lastDoTime1-DateTime.Now).TotalMilliseconds>1000)
                        {
                            MessageBox.Show("你干啥呢????");
                            Application.Exit();
                            return;
                        }
                        if (this.publicKeyFile!= null && this.key != null)
                        {
                            break;
                        }
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(13);
                        lastDoTime1 = DateTime.Now;
                    }
                    getPublicKeyThreadFinished = true;
                }
                );
            //启动等待解密签名文件线程
            ThreadPool.QueueUserWorkItem((res) =>
            {
                Utils.LogStarted("进入解密签名文件线程");
                ThreadPool.QueueUserWorkItem((res22) => {
                    Utils.LogStarted("在解密签名文件线程中进入加载签名文件线程");
                    sleepRandom(0,1000);
                    this.signFile = this.loadSignFile();
                });
                sleepRandom(0, 1000);
                Utils.LogStarted("解密签名文件线程开始检查或等待工作");
                DateTime lastDoTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < timeOutSpanMS)
                {
                    if ((lastDoTime - DateTime.Now).TotalMilliseconds > 1000)
                    {
                        MessageBox.Show("你干啥呢????");
                        Application.Exit();
                        return;
                    }
                    if (this.signFile != null)
                    {
                        break;
                    }
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(13);
                    lastDoTime = DateTime.Now;
                }
                getSignThreadFinished = true;
                
            }
                );
        }
        //验证签名
        public bool VerifySignedHash(string str_DataToVerify, string str_SignedData, string str_Public_Key)
        {
            byte[] SignedData = Convert.FromBase64String(str_SignedData);

            UTF8Encoding ByteConverter = new UTF8Encoding();
            byte[] DataToVerify = ByteConverter.GetBytes(str_DataToVerify);
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                //RSAalg.ImportCspBlob(Convert.FromBase64String(str_Public_Key));
                RSAalg.FromXmlString(str_Public_Key);

                return RSAalg.VerifyData(DataToVerify, new SHA1CryptoServiceProvider(), SignedData);

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }

        void sleepRandom(int min, int max)
        {
            Random rm = new Random(Guid.NewGuid().GetHashCode());
            int sleepTime = rm.Next(min, max);
            Thread.Sleep(sleepTime);
        }

        private void 校验公钥文件的有效期_Click(object sender, EventArgs e)
        {

        }

        private void 校验签名文件是否有效_Click(object sender, EventArgs e)
        {

        }

        private void 记录公钥已使用时间_Click(object sender, EventArgs e)
        {

        }

        #region 通过调用win32api给错误的结构体 干掉程序让他崩溃退出
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public string ullTotalPhys;//写成这样的结构会闪退程序
            //public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                //this.dwLength = 64;
            }
        }


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        //[SecurityCritical]
        //[HandleProcessCorruptedStateExceptions]
        public void CrashApp()
        {
            
            Utils.LogBug("签名不正确 正在干掉你");
            //sleepRandom(2000, 5000);
            MEMORYSTATUSEX sx = new MEMORYSTATUSEX();
            for (int i = 0; i < 1; i++)
            {
                GlobalMemoryStatusEx(sx);
            }
        }
        #endregion
    }
}
