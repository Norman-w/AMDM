using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/*
 * 故障管理器,警报管理器,通知消息管理器
 * 当发生卡药故障,其他机械故障,软件故障,药品有效期预警,药品库存预警等情况时,触发预警消息.
 * 预警消息产生后即刻像日志服务器推送,同时也推送到HIS服务器.但是由于检测是一直持续的,所以检测到了直接就推送显然是不对的.
 * 有新的检测报告出来以后应当检查预警管理器内是否已经触发了该消息的推送.如果已经推送了.就是一个已存在的问题,这个问题就不需要再次推送了.直到解决掉这个问题,才从故障预警管理器中消除.
 * 
 * 警报管理器内加载的警报每次系统重新启动后时空的.等检测器检测到以后上报.
 * 也就是说,如果任务之前推送过,再收到同样的检测结果后,不会重复推送,但是重启软件后,检测到故障以后还会再推送一次.
 */
namespace AMDM
{
    /// <summary>
    /// 警报管理器,故障管理器,已推送未处理的消息管理器,通知消息管理器.
    /// </summary>
    public class AlertManager
    {
        Dictionary<string, Alert> allAlertsDicByHash = new Dictionary<string, Alert>();
        /// <summary>
        /// 消息保存1天不再重新发送,如果消息过了这么长时间的,清除再重新发
        /// </summary>
        TimeSpan alertValidTimeSpan = new TimeSpan(24, 0, 0);
        
        void clearInvalidAlerts()
        {
            #region 每次要发消息之前清空之前已经失效的消息,保证后续能接着发
            try
            {
                List<string> needClearAlertList = new List<string>();
                foreach (var kv in allAlertsDicByHash)
                {
                    if (kv.Value == null || (DateTime.Now - kv.Value.CreateTime) > this.alertValidTimeSpan)
                    {
                        needClearAlertList.Add(kv.Key);
                    }
                }
                foreach (var i in needClearAlertList)
                {
                    if (this.allAlertsDicByHash.ContainsKey(i) == true)
                    {
                        this.allAlertsDicByHash.Remove(i);
                    }
                }
            }
            catch (Exception clearTimeoutAlertErr)
            {
                Utils.LogError("清空过期消息错误:", clearTimeoutAlertErr.Message);
            }
            #endregion
        }
        
        public bool AlertHardwareError(string title, string message)
        {
            clearInvalidAlerts();
            getHash(AlertTypeEnum.HardwareError, title, message, App.UserManager.GetUsersMobile(App.Setting.MedicineAlertSetting.LowInventoryAndExpirationAlertReceiveUsers), null);
            List<string> receiverMobileList = App.UserManager.GetUsersMobile(App.Setting.MedicineAlertSetting.LowInventoryAndExpirationAlertReceiveUsers);
            string hash = getHash(AlertTypeEnum.HardwareError, title, message, receiverMobileList, null);
            if (this.allAlertsDicByHash.ContainsKey(hash))
            {
                //已经存在了这个消息
                return false;
            }
            bool pushErr2HisRet = false;
            try
            {
                pushErr2HisRet = App.HISServerConnector.PushMachineErrorMsg(message, App.Setting.Name
                , receiverMobileList);
            }
            catch (Exception hisErr)
            {
                Utils.LogWarnning("推送硬件故障消息到HIS系统发生错误",title,message, hisErr.Message);
            }    
            try
            {
                if (pushErr2HisRet)
                {
                    Utils.LogSuccess("已将硬件故障信息推送给his系统", message);
                    App.LogServerServicePipeClient.Log(LogLevel.Error, title, string.Format(
                    "{0}", message
                    ));
                }
                else
                {
                    //通知HIS失败的消息只记录到日志系统,不添加到字典,可以让他再次触发,再次触发的时候仍尝试推送到HIS系统
                    Utils.LogError("推送给硬件故障信息到his系统服务器失败,信息内容:", message);
                    App.LogServerServicePipeClient.Log(LogLevel.Error, title, string.Format(
                    "{0}[通知HIS失败]", message
                    ));
                    return false;
                }
            }
            catch (Exception sendPipeMsgErr)
            {
                Utils.LogError("AlertManager发送硬件故障管道消息时发生错误:", sendPipeMsgErr.Message);
                return false;
            }
            try
            {
                this.allAlertsDicByHash.Add(hash, new Alert() { CreateTime = DateTime.Now, Hash = hash, Message = message, SentoutTime = DateTime.Now, Title = title, UniqueId = null, ClearTime = null });
            }
            catch (Exception pushDicErr)
            {
                Utils.LogError("添加硬件故障消息到字典错误:", pushDicErr.Message);
                return false;
            }
            return true;
        }
        public bool AlertSortwarePartError(string title, string message)
        {
            clearInvalidAlerts();
            List<string> receiverMobileList = App.UserManager.GetUsersMobile(App.Setting.TroubleshootingPlanSetting.AlertReceiveUsers);
            string hash = getHash(AlertTypeEnum.SoftwarePartError, title, message, receiverMobileList, null);
            if (this.allAlertsDicByHash.ContainsKey(hash))
            {
                //已经存在了这个消息
                return false;
            }
            bool pushErr2HisRet = false;
            try
            {
                pushErr2HisRet = App.HISServerConnector.PushMachineErrorMsg(message, App.Setting.Name
               , receiverMobileList);
            }
            catch (Exception hisErr)
            {
                Utils.LogWarnning("推送软件组件故障消息到HIS系统失败",title,message, hisErr.Message);
            }   
            try
            {
                if (pushErr2HisRet)
                {
                    Utils.LogSuccess("已将软件故障信息推送给his系统", message);
                    App.LogServerServicePipeClient.Log(LogLevel.Error, title, string.Format(
                    "{0}", message
                    ));
                }
                else
                {
                    Utils.LogError("推送软件故障消息给his系统服务器失败,内容:", message);
                    App.LogServerServicePipeClient.Log(LogLevel.Error, title, string.Format(
                    "{0}[通知HIS失败]", message
                    ));
                    return false;
                }
            }
            catch (Exception sendPipeMsgErr)
            {
                Utils.LogError("AlertManager发送软件组件故障管道消息时发生错误:", sendPipeMsgErr.Message);
                return false;
            }
            try
            {
                this.allAlertsDicByHash.Add(hash, new Alert() { CreateTime = DateTime.Now, Hash = hash, Message = message, SentoutTime = DateTime.Now, Title = title, UniqueId = null, ClearTime = null });
            }
            catch (Exception pushDicError)
            {
                Utils.LogError("添加软件故障消息到字典错误:", pushDicError.Message);
                return false;
            }
            return true;
        }
        public bool AlertMedicineInventory(Dictionary<long, AMDM_MedicineInventory> medicinesInventory, string title, string message)
        {
            clearInvalidAlerts();
            List<string> receiverMobileList = App.UserManager.GetUsersMobile(App.Setting.MedicineAlertSetting.LowInventoryAndExpirationAlertReceiveUsers);
            StringBuilder uid = new StringBuilder();
            foreach (var m in medicinesInventory)
            {
                uid.Append(m.Key);
            }
            string hash = getHash(AlertTypeEnum.MedicineInventoryAlert, title, message, receiverMobileList, uid.ToString());
            if (this.allAlertsDicByHash.ContainsKey(hash))
            {
                //已经存在了这个消息
                return false;
            }
            bool log2HisRet = false;
            try
            {
                log2HisRet = App.HISServerConnector.NoticeInsufficientInventory(new List<AMDM_MedicineInventory>(medicinesInventory.Values), App.Setting.Name, ""
                , receiverMobileList);
            }
            catch (Exception hisErr)
            {
                Utils.LogWarnning("推送库存预警消息到HIS系统发生错误", title, message, hisErr.Message);
            }

            try
            {
                if (log2HisRet)
                {
                    Utils.LogSuccess("已将库存预警消息推送给his系统", title, message);
                    App.LogServerServicePipeClient.Log(LogLevel.Warnning, title, string.Format(
                    "{0}", message
                    ));
                }
                else
                {
                    Utils.LogError("推送给库存预警消息到his系统服务器失败,信息内容:", message);
                    App.LogServerServicePipeClient.Log(LogLevel.Warnning, title, string.Format(
                    "{0}[通知HIS失败]", message
                    ));
                    return false;
                }
            }
            catch (Exception sendPipeMsgErr)
            {
                Utils.LogError("AlertManager发送药品库存预警管道消息时发生错误:", sendPipeMsgErr.Message);
                return false;
            }
            try
            {
                this.allAlertsDicByHash.Add(hash, new Alert() { CreateTime = DateTime.Now, Hash = hash, Message = message, SentoutTime = DateTime.Now, Title = title, UniqueId = uid.ToString(), ClearTime = null });
            }
            catch (Exception pushDicErr)
            {
                Utils.LogError("添加药品库存预警消息到字典错误:", pushDicErr.Message);
                return false;
            }
            return true;
        }
        public bool AlertMedicineExpiration(Dictionary<long, AMDM_MedicineObject__Grid__Medicine> medicines, string title, string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("药品有效期预警:");
            clearInvalidAlerts();
            Console.WriteLine("清空过期消息");
            List<string> receiverMobileList = App.UserManager.GetUsersMobile(App.Setting.MedicineAlertSetting.LowInventoryAndExpirationAlertReceiveUsers);
            StringBuilder uid = new StringBuilder();
            foreach (var m in medicines)
            {
                uid.Append(m.Key);
            }
            string hash = getHash(AlertTypeEnum.MedicineExpirationAlert, title, null, receiverMobileList, uid.ToString());
            if (this.allAlertsDicByHash.ContainsKey(hash))
            {
                Console.WriteLine("消息已存在");
                //已经存在了这个消息
                return false;
            }
            bool pushWarnning2HisRet = false;
            try
            {
                pushWarnning2HisRet = App.HISServerConnector.NoticeMedicinesExpirationDateAlert(new List<AMDM_MedicineObject__Grid__Medicine>(medicines.Values), App.machine.Name, message,
                   receiverMobileList
                   );
            }
            catch (Exception hisErr)
            {
                Utils.LogWarnning("推送药品有效期预警信息到HIS系统时发生错误:",title, message, hisErr.Message);
            }
            Console.WriteLine("推送结果{0}", pushWarnning2HisRet);
            try
            {
                if (pushWarnning2HisRet)
                {
                    Utils.LogSuccess("推送药品有效期预警消息到HIS系统成功", title, message);
                    App.LogServerServicePipeClient.Log(LogLevel.Warnning, title, string.Format(
                    "{0}", message
                    ));
                }
                else
                {
                    Utils.LogFail("推送药品有效期预警消息到HIS系统失败", title, message);
                    App.LogServerServicePipeClient.Log(LogLevel.Warnning, title, string.Format(
                    "{0}[通知HIS失败]", message
                    ));
                    return false;
                }
            }
            catch (Exception sendPipeMsgErr)
            {
                Utils.LogError("AlertManager发送药品有效期预警管道消息时发生错误:", sendPipeMsgErr.Message);
                return false;
            }
            try
            {
                this.allAlertsDicByHash.Add(hash, new Alert() { CreateTime = DateTime.Now, Hash = hash, Message = message, SentoutTime = DateTime.Now, Title = title, UniqueId = uid.ToString(), ClearTime = null });
                Console.WriteLine("已经添加到词典,数量:{0}", this.allAlertsDicByHash.Count);
            }
            catch (Exception pushDicError)
            {
                Utils.LogError("添加药品有效期预警消息到字典错误:", pushDicError.Message);
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// 当故障被解决时,可以选择清空本类中的相关消息,避免消息过多的存储在内存中(当然也不会太多,也可以不清除,但是如果稳定运行了10年呢?会有多少的数据不好说,所以还是清除的比较好)
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="title"></param>
        ///// <param name="message"></param>
        ///// <param name="receiverMobileList"></param>
        ///// <param name="uniqueId"></param>
        ///// <returns></returns>
        //public bool ClearAlert(AlertTypeEnum type, string title, string message, List<string> receiverMobileList, object uniqueId = null)
        //{
        //    Nullable<int> hash = null;
        //    string uid = uniqueId == null ? null : uniqueId.ToString();
        //    int receiverHash = receiverMobileList == null ? 0 : receiverMobileList.GetHashCode();
        //    if (!contains(type, title, message, receiverHash, uid, out hash))
        //    {
        //        //不存在这个消息,没有办法清除
        //        return false;
        //    }
        //}
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
        /// 获取消息的hash值,确保消息的唯一性,不重复发送.(如果是有效期预警消息,通过uniqueId来检查)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="receiverHash"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        string getHash(AlertTypeEnum type, string title, string message, List<string> receiverMobiles, string uniqueId)
        {
            StringBuilder sb = new StringBuilder(type.ToString());
            switch (type)
            {
                case AlertTypeEnum.Unknow:
                case AlertTypeEnum.HardwareError:
                case AlertTypeEnum.SoftwarePartError:
                case AlertTypeEnum.MedicineInventoryAlert:
                case AlertTypeEnum.Other:
                default:
                    sb.Append(title);
                    sb.Append(message);
                    foreach (var m in receiverMobiles)
                    {
                        sb.AppendFormat("|{0}|", m);
                    }
                    sb.Append(uniqueId);
                    break;
                case AlertTypeEnum.MedicineExpirationAlert:
                    sb.Append(title);
                    foreach (var m in receiverMobiles)
                    {
                        sb.AppendFormat("|{0}|", m);
                    }
                    sb.Append(uniqueId);
                    break;
            }
            return GetMd5(sb.ToString());
        }
    }

    public enum AlertTypeEnum
    {
        Unknow,
        HardwareError,
        SoftwarePartError,
        MedicineInventoryAlert,
        MedicineExpirationAlert,
        Other,
    }
    /// <summary>
    /// 消息体
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// 消息的唯一ID,由数据库生成
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 消息的哈希值,根据消息内容生成
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// 消息的创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 消息的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 消息的送出时间,没送出的时候是null
        /// </summary>
        public Nullable<DateTime> SentoutTime { get; set; }
        /// <summary>
        /// 消息的清楚时间(问题的解决时间)
        /// </summary>
        public Nullable<DateTime> ClearTime { get; set; }
        /// <summary>
        /// 消息的唯一id,比如是药品有效期预警的时候,该ID是MedicineObject对象的ID
        /// </summary>
        public string UniqueId { get; set; }
    }
}
