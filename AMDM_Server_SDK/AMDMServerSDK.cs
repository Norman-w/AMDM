using AMDM_Domain;
using AMDM_Server_SDK.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
/*
 * 2021年12月15日13:24:18
 */
namespace AMDM_Server_SDK
{
    /// <summary>
    /// 付药机后台管理系统的sdk
    /// </summary>
    public class AMDMServerSDK
    {
        #region 全局变量
        SQLDataTransmitter client;
        #endregion
        public AMDMServerSDK()
        {
            string ip = ConfigurationManager.AppSettings["server_side_sdk_ip"];
            string user = ConfigurationManager.AppSettings["server_side_sdk_user"];
            string pass = ConfigurationManager.AppSettings["server_side_sdk_pass"];
            string database = ConfigurationManager.AppSettings["server_side_sdk_database"];
            string port_str = ConfigurationManager.AppSettings["server_side_sdk_port"];
            client = new SQLDataTransmitter(ip, user, pass, database, Convert.ToInt32(port_str));
        }
        /// <summary>
        /// 2022年3月22日13:07:16  为了客户端和服务端装载一个电脑上,直接调用sdk的时候使用,而不使用web iis携带过来的参数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="database"></param>
        /// <param name="port"></param>
        public AMDMServerSDK(string ip, string user, string pass, string database, int port)
        {
            client = new SQLDataTransmitter(ip, user, pass, database, port);
        }

        public AccountsGetResponse DoAccountsGetRequest(AccountsGetRequest req, string session)
        {
            AccountsGetResponse rsp = req.AllocResponse();
            try
            {
                req.Validate();
            }
            catch (Exception paramErr)
            {

                rsp.ErrMsg = paramErr.Message;
                rsp.ErrCode = "400";
                return rsp;
            }
            rsp.Accounts = client.AccountsGet(req.Fields, req.Id, req.UserName, null, req.Name, req.Department);
            return rsp;
        }

        public LoginResponse DoLoginRequest(LoginRequest req, string session, Action<string> onNewSession)
        {
            LoginResponse rsp = req.AllocResponse();
            try
            {
                req.Validate();
            }
            catch (Exception paramErr)
            {

                rsp.ErrMsg = paramErr.Message;
                rsp.ErrCode = "400";
                return rsp;
            }
            List<Account> accounts = client.AccountsGet("*", req.Id, req.UserName, req.PasswordMD5,null,null);
            if (accounts.Count != 1)
            {
                rsp.ErrMsg = "用户名或密码不正确";
                rsp.ErrCode = "403";
                return rsp;
            }
            else
            {
                string newSession = Guid.NewGuid().ToString("N");
                //登录模式
                if (onNewSession != null)
                {
                    onNewSession(newSession);
                }
                rsp.Session = newSession;
                rsp.Account = accounts[0];
            }
            
            return rsp;
        }

        public AccountAddResponse DoAccountAddRequest(AccountAddRequest req, string session)
        {
            AccountAddResponse rsp = req.AllocResponse();
            try
            {
                req.Validate();
            }
            catch (Exception paramErr)
            {
                rsp.ErrMsg = paramErr.Message;
                rsp.ErrCode = "400";
                return rsp;
            }
           List<Account> hasedAccounts = client.AccountsGet("*", null, req.UserName, null, null, null);
           if (hasedAccounts!= null && hasedAccounts.Count>0)
           {
               rsp.ErrMsg = "已经存在的账户,不能重复添加";
               rsp.ErrCode = "400";
               return rsp;
           }
            rsp.NewAccount = client.AccountAdd(req.Department, req.Name, req.Sex, req.Age, req.UserName, req.Password);
            if (rsp.NewAccount == null)
            {
                rsp.ErrMsg = "向数据库中插入新的账户信息发生错误,账户体为空";
                rsp.ErrCode = "500";
                return rsp;
            }
            return rsp;
        }

        public AccountDeleteResponse DoAccountDeleteRequest(AccountDeleteRequest req, string session)
        {
            AccountDeleteResponse rsp = req.AllocResponse();
            try
            {
                req.Validate();
            }
            catch (Exception paramErr)
            {
                rsp.ErrMsg = paramErr.Message;
                rsp.ErrCode = "400";
                return rsp;
            }
            var acc = client.AccountsGet("*", req.Id, null, null, null, null);
            if (acc == null || acc.Count <1)
            {
                rsp.ErrMsg = "该账户不存在";
                rsp.ErrCode = "400";
                return rsp;
            }
            rsp.Success = client.AccountDelete(req.Id);
            return rsp;
        }

        public AccountUpdateResponse DoAccountUpdateRequest(AccountUpdateRequest req, string session)
        {
            AccountUpdateResponse rsp = req.AllocResponse();
            try
            {
                req.Validate();
            }
            catch (Exception paramErr)
            {
                rsp.ErrMsg = paramErr.Message;
                rsp.ErrCode = "400";
                return rsp;
            }
            //获取账号是否存在,并且检查md5等信息  这一步不需要去掉密码信息
            var acc = client.AccountsGet("*", req.Id, null, null, null, null, false);
            if (acc == null || acc.Count < 1)
            {
                rsp.ErrMsg = "该账户不存在";
                rsp.ErrCode = "400";
                return rsp;
            }
            #region 如果是更新密码的时候
            if (string.IsNullOrEmpty(req.OldPasswordMD5) == false)
            {
                if (acc.Count!=1)
                {
                    rsp.ErrMsg = "请明确要修改密码的账户";
                    rsp.ErrCode = "400";
                    return rsp;
                }
                Account destAccount = acc[0];
                if (destAccount.PasswordMD5!= null && destAccount.PasswordMD5.ToLower().Equals(req.OldPasswordMD5.ToLower()))
                {
                    //密码一致
                    destAccount.Password = req.Password;
                }
                else
                {
                    rsp.ErrMsg = "原始密码错误";
                    rsp.ErrCode = "403";
                    return rsp;
                }
            }
            #endregion
            rsp.Success = client.AccountUpdate(req.Id.Value, req.Name, req.UserName, req.Password, req.Department, req.Sex, req.Age, req.Mobile);

            acc.Clear();
            acc = client.AccountsGet("*", req.Id, null, null, null, null, true);
            if (acc.Count!= 1)
            {
                rsp.Success = false;
                rsp.ErrMsg = "数据冗余,找到多条的该账户信息";
                rsp.ErrCode = "500";
                return rsp;
            }
            rsp.UpdatedAccount = acc[0];
            return rsp;
        }
    }
}
