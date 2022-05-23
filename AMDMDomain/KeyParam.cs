using System;
using System.Collections.Generic;
using System.Text;

/*
 * 2021年11月26日13:35:26 创建钥匙信息类.
 * 通过权限控制系统创建一个公钥私钥对,把公钥信息和当前时间,机器序列号都序列化成一个json,然后base64加密后
 * 生成二维码,扫描二维码后,解析出来这些信息,安装公钥信息
 * 安装后,生成一个guid,通过公钥加密
 * 加密后的guid转换成二维码显示在屏幕上方或者是直接输出成文本,作为临时密码,临时密码有效期是5分钟.
 * 5分钟之内,权限控制系统通过扫描的信息,进行私钥签名,私钥签名后的内容生成一个二维码
 * 扫描二维码以后,解析验证的签名是否与控制器生成的签名一致,一致后方可继续使用
 */
namespace AMDM_Domain
{
    /// <summary>
    /// 临时钥匙信息,用于打开机器的控制权
    /// </summary>
    public class KeyParam
    {
        /// <summary>
        /// 创建临时钥匙时使用的临时公钥
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 创建临时钥匙的时间,24小时的时分秒格式拼接  00:01:02秒就是  000102
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 创建的钥匙所属的机器的SN,如果不属于这个机器,属于非法生成
        /// </summary>
        public string MachineSN { get; set; }
    }
}
