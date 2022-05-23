using MyCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Security.Cryptography;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keygen
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        public Main(string machineSN, string keyOutPutBaseDir)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            if (string.IsNullOrEmpty(machineSN) == true)
            {
                MessageBox.Show(this, "无效的序列号信息", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                this.Close();
                return;
            }
            this.sn = machineSN;
            this.设备sn.Text = sn;
            生成公钥私钥对_Click(null, null);
            this.填写设备的SerialNumber.Enabled = false;
            this.outPutBaseDir = keyOutPutBaseDir;
        }
        #region 全局变量
        string outPutBaseDir = "c:\\公钥私钥文件";
        string privateKey = null;
        string publicKey = null;
        /// <summary>
        /// 外部可访问的公钥信息
        /// </summary>
        public string PublicKey { get { return this.publicKey; } }
        string sn = null;
        DeviceInfo deviceInfo = null;
        PrivateKeyInfo privateKeyInfoData = null;
        string guidPublicFileName = null;
        string guidSignFileName = null;
        string deviceInfoJsonBase64 = null;
        string signedDeviceInfoJsonBase64 = null;
        byte[] encodedPublicKeyFileContent = null;
        byte[] encodedSignFileContent = null;
        GetGuidFileNameIntListResult publicKeyFile;
        GetGuidFileNameIntListResult signFile;
        #endregion

        private void 生成公钥私钥对_Click(object sender, EventArgs e)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            //using (StreamWriter writer = new StreamWriter("PrivateKey.xml", false))  //这个文件要保密...
            //{
            string str = rsa.ToXmlString(true);
            this.privateKey = str;
            this.私钥.Text = str;
            //writer.WriteLine(str);
            //}
            //using (StreamWriter writer = new StreamWriter("PublicKey.xml", false))
            //{
            string str2 = rsa.ToXmlString(false);
            this.publicKey = str2;
            this.公钥.Text = str2;
            //writer.WriteLine(str);
            //}
            //MessageBox.Show("文件已保存");
        }

        private void 填写设备的SerialNumber_Click(object sender, EventArgs e)
        {
            this.sn = 设备sn.Text;
            if (string.IsNullOrEmpty(this.sn))
            {
                MessageBox.Show("无效的设备序列号");
                return;
            }
            MessageBox.Show("已填写");
        }

        private void 获取设备的硬件信息_Click(object sender, EventArgs e)
        {
            Computer computer = new Computer();
            deviceInfo = new DeviceInfo();
            deviceInfo.CPUSN = computer.CpuID;
            deviceInfo.HarddiskSN = computer.DiskSN;
            deviceInfo.MAC = computer.MacAddress;
            this.设备硬件信息.Text = JsonConvert.SerializeObject(deviceInfo);
            //MessageBox.Show("已设置");
        }

        private void 生成私钥信息对象_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.privateKey) ||
                string.IsNullOrEmpty(this.publicKey) ||
                string.IsNullOrEmpty(this.sn) ||
                this.deviceInfo == null
                )
            {
                MessageBox.Show("无效的数据,请先执行先前的步骤");
                return;
            }
            privateKeyInfoData = new PrivateKeyInfo();
            privateKeyInfoData.DeviceInfoJSON = JsonConvert.SerializeObject(this.deviceInfo);
            privateKeyInfoData.PrivateKey = this.privateKey;
            privateKeyInfoData.PublicKey = this.publicKey;
            privateKeyInfoData.SN = this.sn;
            privateKeyInfoData.CreateTime = DateTime.Now;
            privateKeyInfoData.ExpirateTime = privateKeyInfoData.CreateTime.AddYears(1);

            this.私钥信息对象.Text = JsonConvert.SerializeObject(privateKeyInfoData);
            //MessageBox.Show("私钥信息数据已生成,需要保存到数据库,创建时间为当前时间,有效期1年");
        }

        private void 保存私钥信息到文件_Click(object sender, EventArgs e)
        {
            //这里正常应该是保存到数据库,目前为了方便直接保存到文件,使用设备的sn命名
            if (this.privateKeyInfoData == null)
            {
                MessageBox.Show("无效的私钥信息");
                return;
            }
            string json = JsonConvert.SerializeObject(this.privateKeyInfoData, Formatting.Indented);
            string path = string.Format("{0}\\{1}", this.outPutBaseDir, this.sn);
            if (System.IO.Directory.Exists(path) == false)
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string file = string.Format("{0}\\{1}",path, "秘钥信息.json");
            System.IO.File.WriteAllText(file, json);
            this.私钥保存文件位置.Text = file;
            //MessageBox.Show("私钥信息文件已保存到"+path);
        }

        private void 提取公钥准备安装_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.publicKey) == true)
            {
                MessageBox.Show("无效的公钥信息");
                return;
            }
            MessageBox.Show("已提取");
        }

        private void 使用公钥文件加密器加密公钥文件_Click(object sender, EventArgs e)
        {
            if (this.privateKeyInfoData == null)
	{
                MessageBox.Show("无效的私钥信息");
                return;
	}
            KeyInfo key = new KeyInfo();
            string privateKeyInfoJson = JsonConvert.SerializeObject(this.privateKeyInfoData);
            JsonConvert.PopulateObject(privateKeyInfoJson, key);
            string encodedPublicKey = Ciphertext.Encode<KeyInfo>(key);
            encodedPublicKeyFileContent = Encoding.UTF8.GetBytes(encodedPublicKey);
            this.加密后的公钥文件.Text = encodedPublicKey;
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void 生成guid作为公钥加密文件的文件名_Click(object sender, EventArgs e)
        {
            this.guidPublicFileName = Guid.NewGuid().ToString("N");
            this.公钥的目标文件名.Text = this.guidPublicFileName;
        }
        string Revease3(string original)
        {
            int length = original.Length;
            StringBuilder sb = new StringBuilder(length);
            for (int i = length - 1; i >= 0; i--)
                sb.Append(original[i]);
            return sb.ToString();
        }

        private void 将生成的公钥文件名转换成加密的byte数组_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.guidPublicFileName) == true)
            {
                MessageBox.Show("无效的公钥guid文件名");
                return;
            }
            GetGuidFileNameIntListResult ret = ConvertGuidName2IntList(this.guidPublicFileName);
            this.publicKeyFile = ret;
            this.公钥的目标文件名加密后的byte数组.Text = ret.StrBy10AndSpace;
            
        }
        class GetGuidFileNameIntListResult
        {
            public List<int> IntList { get; set; }
            /// <summary>
            /// 带空格和十进制显示的数字的字符串 用于显示
            /// </summary>
            public string StrBy10AndSpace { get; set; }
        }
        GetGuidFileNameIntListResult ConvertGuidName2IntList(string guid)
        {
            GetGuidFileNameIntListResult ret = new GetGuidFileNameIntListResult();
            //加密时,使用utf8转换字符串为byte
            //吧byte按位取反
            //把取反后的每一个byte倒叙排列成新的数组
            //用int数字形式记录这个数组

            byte[] bytes = Encoding.UTF8.GetBytes(guid);
            List<byte> z2abytes = new List<byte>();
            StringBuilder sb = new StringBuilder();
            List<int> intBytes = new List<int>();
            foreach (byte b in bytes)
            {
                byte newB = (byte)~b;
                #region 高低位反转
                string newB2 = Convert.ToString(newB, 2);
                newB2 = newB2.PadLeft(8, '0');
                newB2 = Revease3(newB2);
                newB = Convert.ToByte(newB2, 2);
                #endregion

                //sb.Append(newB.ToString("X2"));
                intBytes.Insert(0, (int)newB);

                sb.Append(newB.ToString());
                sb.Append(" ");
                z2abytes.Insert(0, newB);
            }

            //string encoded = Encoding.UTF8.GetString(z2abytes.ToArray());
            string encoded = sb.ToString();
            
            ret.StrBy10AndSpace = encoded;
            ret.IntList = intBytes;

            #region 尝试解密
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
            MessageBox.Show(decodedFromIntList, "解密后的数据");
            #endregion
            return ret;
        }

        private void 把设备信息保存到json然后base64_Click(object sender, EventArgs e)
        {
            if (this.deviceInfo == null)
            {
                MessageBox.Show("设备信息无效");
                return;
            }
            string json = JsonConvert.SerializeObject(this.deviceInfo);
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            this.deviceInfoJsonBase64 = base64;
            this.设备信息base64.Text = base64;
        }

        private void 使用私钥将这个base64签名_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(deviceInfoJsonBase64))
            {
                MessageBox.Show("无效的base64数据");
                return;
            }
            if (string.IsNullOrEmpty(this.privateKey) == true)
            {
                MessageBox.Show("无效的私钥");
                return;
            }
            signedDeviceInfoJsonBase64 = HashAndSign(deviceInfoJsonBase64, this.privateKey);
            this.encodedSignFileContent = Encoding.UTF8.GetBytes(signedDeviceInfoJsonBase64);
            this.设备信息base64的私钥签过的名.Text = signedDeviceInfoJsonBase64;
        }
        //对数据签名
        string HashAndSign(string str_DataToSign, string str_Private_Key)
        {
            UTF8Encoding ByteConverter = new UTF8Encoding();
            byte[] DataToSign = ByteConverter.GetBytes(str_DataToSign);
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                //RSAalg.ImportCspBlob(Convert.FromBase64String(str_Private_Key));
                RSAalg.FromXmlString(str_Private_Key);
                byte[] signedData = RSAalg.SignData(DataToSign, new SHA1CryptoServiceProvider());
                string str_SignedData = Convert.ToBase64String(signedData);
                return str_SignedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private void 生成guid作为设备签名信息文件的文件名_Click(object sender, EventArgs e)
        {
            this.guidSignFileName = Guid.NewGuid().ToString("N");
            this.设备签名信息文件名.Text = this.guidSignFileName;
        }

        private void 将生成的签名文件名转换成加密的byte数组_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.guidSignFileName) == true)
            {
                MessageBox.Show("无效的公钥guid文件名");
                return;
            }
            GetGuidFileNameIntListResult ret = ConvertGuidName2IntList(this.guidSignFileName);
            this.signFile = ret;
            this.签名文件目标文件名的加密后byte.Text = ret.StrBy10AndSpace;
        }

        private void 保存公钥资源文件和文件名文件_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sn))
            {
                MessageBox.Show("先输入设备序列号");
                return;
            }
            string path = string.Format("{0}\\{1}\\公钥", outPutBaseDir, this.sn);
            if (System.IO.Directory.Exists(path)==false)
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string publicKeyFile = string.Format("{0}\\{1}.bin", path, guidPublicFileName);
            string publicKeyFileNameFile = string.Format("{0}\\{1}", path, "文件名.cs");
            System.IO.File.WriteAllBytes(publicKeyFile, encodedPublicKeyFileContent);
            //写入调用的cs文件
            string publicKeyFileNameFileContent = createCSFileContent("PublicKeyFileName", this.publicKeyFile.IntList);
            System.IO.File.WriteAllText(publicKeyFileNameFile, publicKeyFileNameFileContent);
            MessageBox.Show("已保存");
        }
        string createCSFileContent(string className, List<int> datas)
        {
            StringBuilder csFileBuilder = new StringBuilder();
            csFileBuilder.Append(@"
using System.Collections.Generic;
using System.Text;

public class ");
            csFileBuilder.Append(className);
            csFileBuilder.Append("\r\n");
            csFileBuilder.Append(
@"
{
    public string Get()
    {
        List<int> ints = new List<int>();

");
            for (int i = 0; i < datas.Count; i++)
            {
                csFileBuilder.AppendFormat("\t\tints.Add({0});", datas[i]);
                csFileBuilder.Append("\r\n");
            }
            csFileBuilder.Append("\r\n");
            csFileBuilder.Append(
                @"
        StringBuilder sb = new StringBuilder();
        foreach (int i in ints)
        {
            char b = (char)i;
            sb.Append(b);
        }
        return sb.ToString();
    }
}
"
                );
            return csFileBuilder.ToString();
        }

        private void 保存签名资源文件和文件名文件_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sn))
            {
                MessageBox.Show("先输入设备序列号");
                return;
            }
            string path = string.Format("{0}\\{1}\\签名", outPutBaseDir, this.sn);
            if (System.IO.Directory.Exists(path) == false)
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string signFile = string.Format("{0}\\{1}.bin", path, guidSignFileName);
            string signFileNameFile = string.Format("{0}\\{1}", path, "文件名.cs");
            System.IO.File.WriteAllBytes(signFile, encodedSignFileContent);
            //写入调用的cs文件
            string signFileNameFileContent = createCSFileContent("SignFileName", this.signFile.IntList);
            System.IO.File.WriteAllText(signFileNameFile, signFileNameFileContent);
            MessageBox.Show("已保存");
        }

        private void 完成btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
    public class DeviceInfo
    {
        public string CPUSN { get; set; }
        public string HarddiskSN { get; set; }
        //public string MemorySN { get; set; }
        public string MAC { get; set; }
    }
    /// <summary>
    /// 私钥信息,保存到数据库,用于以后维护不同的机器时候使用
    /// </summary>
    public class KeyInfo
    {
        /// <summary>
        /// 硬件设备的序列号,比如系统安装在付药机上,这个值表示的是付药机的序列号
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 电脑的相关信息,包括cpu的序列号,硬盘序列号,网卡mac地址,使用deviceinfo类对象序列化而来
        /// </summary>
        public string DeviceInfoJSON { get; set; }
       
        /// <summary>
        /// 公钥xml
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime ExpirateTime { get; set; }
    }

    public class PrivateKeyInfo:KeyInfo
    {
        /// <summary>
        /// 私钥xml
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
