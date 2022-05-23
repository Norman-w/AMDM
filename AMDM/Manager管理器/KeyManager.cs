using AMDM;
using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AMDM
{
    /// <summary>
    /// 钥匙控制器,一共分为两个步骤,第一步骤插入公钥安装钥匙
    /// 第二步骤生成随机信息,让控制器生成验证信息,本机校验控制器生成的验证信息(使用私钥的签名)
    /// </summary>
    public class KeyManager
    {
        #region 全局变量
        /// <summary>
        /// 临时钥匙插入和启动的时间间隔默认5分钟内有效
        /// </summary>
        int tempKeyValidTimeSpanMS = 3000000;
        /// <summary>
        /// 当前安装了的临时密码
        /// </summary>
        //RSACryptoServiceProvider currentInstalledTempKey = null;

        /// <summary>
        /// 当前安装了的临时钥匙
        /// </summary>
        KeyParam currentKey = null;
        string publicKey = "<RSAKeyValue><Modulus>xU4mWQFYZITiGOZrJpR4Vb0GSrGc+M3SuGDETaVTYcZgTkpNje4210H0yog3Im7PwOSiiFJlWztcLoTXgIXWAw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        /// <summary>
        /// 当前生成的临时密码,用于控制器进行签名校验
        /// </summary>
        string currentPass = null;
        #endregion
        public bool InsertKey(string msg, string machineSN)
        {
            ClearKey();
            if (string.IsNullOrEmpty(msg) || msg.Length < 64)
            {
                return false;
            }
            //从base64转换成string
            string fromBase64Json = null;
            try
            {
                fromBase64Json = DecodeBase64(msg);
            }
            catch (Exception err)
            {
                Utils.LogError("将数据转换为base64错误:", err.Message);
                return false;
            }
            //解析json到keyparam
            KeyParam param = null;
            try
            {
               param = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyParam>(fromBase64Json);
            }
            catch (Exception err)
            {
                Utils.LogError("解析json错误,在keymanager中", err.Message);
                return false;
            }
            //验证keyparam的时间距离现在是否相差5分钟以上
            DateTime createTime = DateTime.MinValue;
            try
            {
                if (param!= null && param.CreateTime!= null && param.CreateTime.Length == 6)
                {
                    string todayThisTime = string.Format("{0} {1}:{2}:{3}",DateTime.Now.ToString("yyyy-MM-dd"), param.CreateTime.Substring(0,2), param.CreateTime.Substring(2,2), param.CreateTime.Substring(4,2));
                    DateTime.TryParse(todayThisTime, out createTime);
                }
            }
            catch (Exception parseTimeErr)
            {
                Utils.LogError("解析时间错误", parseTimeErr.Message);
                return false;
            }
            if (createTime == DateTime.MinValue)
            {
                Utils.LogError("解析到的时间是最小时间错误");
                return false;
            }
            if (param == null || createTime.AddMilliseconds(this.tempKeyValidTimeSpanMS) < DateTime.Now)
            {
                Utils.LogError("临时钥匙时间过早", param.CreateTime);
                return false;
            }
            if (param.MachineSN != machineSN)
            {
                Utils.LogError("临时钥匙不属于此设备", param.MachineSN, machineSN);
                return false;
            }
            if (string.IsNullOrEmpty(param.PublicKey) == true)
            {
                Utils.LogError("临时钥匙特征信息无效");
                return false;
            }
            if (GetMd5(this.publicKey) != param.PublicKey)
            {
                Utils.LogError("当前设备的公钥与目标启动设备的公钥特征不匹配");
                return false;
            }
            ////安装公钥信息
            //this.currentInstalledTempKey = rsaPublic;
            //生成guid
            string guid = Guid.NewGuid().ToString("N");
            //生成guid的加密信息,使用公钥
            string passwordFull = guid.Substring(8,8);// string.Format("{0}{1}{2}{3}", guid.Substring(8,8),);
            //安装临时钥匙信息
            currentKey = param;
            currentPass = passwordFull;
            return true;
        }
        string GetMd5(string str)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(str);
                bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
                }
                str = sTemp.ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return str;
        }

        /// <summary>
        /// 获取当前的临时密码,用于语音播报
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPass()
        {
            if (this.currentKey == null)
            {
                return null;
            }
            return this.currentPass;
        }
        /// <summary>
        /// 检查当前是否有钥匙插入
        /// </summary>
        /// <returns></returns>
        public bool IsTempKeyInstalled()
        {
            return this.currentKey != null;
        }
        /// <summary>
        /// 校验临时钥匙启动器
        /// </summary>
        /// <param name="signed"></param>
        /// <returns></returns>
        public bool CheckKeyStarter(string signed)
        {
            //等待新的钥匙启动器插入
            //扫描二维码后新的钥匙启动器插入
            //验证签名,确认钥匙启动器是私钥发出
            if (IsTempKeyInstalled() == false)
            {
                return false;
            }
            //确认当前时间和钥匙启动器的有效时间是否一致
            #region 解析创建时间
            DateTime createTime = DateTime.MinValue;
            try
            {
                if (currentKey != null && currentKey.CreateTime != null && currentKey.CreateTime.Length == 6)
                {
                    string todayThisTime = string.Format("{0} {1}:{2}:{3}", DateTime.Now.ToString("yyyy-MM-dd"),
                        currentKey.CreateTime.Substring(0, 2), currentKey.CreateTime.Substring(2, 2), currentKey.CreateTime.Substring(4, 2));
                    DateTime.TryParse(todayThisTime, out createTime);
                }
            }
            catch (Exception parseTimeErr)
            {
                Utils.LogError("解析时间错误", parseTimeErr.Message);
                return false;
            }
            if (createTime == DateTime.MinValue)
            {
                Utils.LogError("解析到的时间是最小时间错误");
                return false;
            }
            #endregion
            if ((DateTime.Now - createTime).TotalMilliseconds > this.tempKeyValidTimeSpanMS)
            {
                Utils.LogError("钥匙启动器已失效");
                this.ClearKey();
                return false;
            }
            bool checkRet = false;
            try
            {
                checkRet = this.VerifySignedHash(this.currentPass,signed, this.publicKey);
            }
            catch (Exception checkErr)
            {
                Utils.LogError("校验钥匙启动器抛出异常:", checkErr.Message);
                ClearKey();
                return false;
            }
            //如果验证失败 清空临时钥匙信息
            if (checkRet != true)
            {
                Utils.LogError("校验钥匙启动器失败,内容不符");
                ClearKey();
                return false;
            }
            //如果验证成功 打开控制权限
            return true;
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="str_DataToVerify">当前保存的临时生成的8位数密码</param>
        /// <param name="str_SignedData">要验证的签名密文,就是控制器发过来的</param>
        /// <param name="str_Public_Key">公钥信息</param>
        /// <returns></returns>
        bool VerifySignedHash(string str_DataToVerify, string str_SignedData, string str_Public_Key)
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
        void ClearKey()
        {
            //if (this.currentInstalledTempKey!= null)
            //{
            //    this.currentInstalledTempKey = null;
            //}
            if (this.currentKey!= null)
            {
                this.currentKey = null;
                this.currentPass = null;
            }
        }
        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string EncodeBase64(string source)
        {
            return EncodeBase64(Encoding.UTF8, source);
        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="codeName">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string EncodeBase64(Encoding encoding, string source)
        {
            string encode = null;
            byte[] bytes = encoding.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }

    }
}

