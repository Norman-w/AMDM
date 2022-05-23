using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMDM_Domain;

/*
 * 2022年3月6日10:55:09 付药机客户端部署器工程文件,用于保存到硬盘以分阶段处理和为完成任务的保存以及读取.另可根据配置文件分析配置时刻的情况
 */
namespace 自动付药机客户端部署工具
{
    public class AMDMClientConfiggerProjectFile
    {
        /// <summary>
        /// 药机信息,里面包含sn,创建信息等
        /// </summary>
        public AMDM_Machine Machine { get; set; }
        /// <summary>
        /// 零部件信息,一些需要产生sn和一些其他记录信息的部件需要使用
        /// </summary>
        public List<PartsInfo> Parts { get; set; }
        /// <summary>
        /// 主应用程序的安装目录
        /// </summary>
        public string AMDMAppInstallPath { get; set; }
        /// <summary>
        /// 如果应用程序已经安装,记录应用程序的安装日期,可以判断是操作卸载还是什么
        /// </summary>
        public Nullable<DateTime> AMDMAppInstallTime { get; set; }
        /// <summary>
        /// 本地mysql服务器的安装路径
        /// </summary>
        public string LocalMysqlInstallPath { get; set; }
        /// <summary>
        /// 本地mysql服务器的安装时间
        /// </summary>
        public Nullable<DateTime> LocalMysqlInstallTime { get; set; }
        /// <summary>
        /// 日志服务器/看门狗 部署器的安装路径
        /// </summary>
        public string LogServerConfiggerInstallPath { get; set; }
        /// <summary>
        /// 日志服务器/看门狗  部署器的安装时间
        /// </summary>
        public Nullable<DateTime> LogServerConfigerInstallPath { get; set; }
    }
}
