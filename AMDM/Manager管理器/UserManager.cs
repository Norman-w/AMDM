using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMDM_Server_SDK;
using AMDM_Server_SDK.Domain;
using AMDM_Domain;
/*
 * 2021年11月26日09:50:33 构建的用户管理类,再此类中操作用户的登录等
 */
namespace AMDM
{
    /// <summary>
    /// 用户管理器
    /// </summary>
    public class UserManager
    {
        #region 全局变量
        AMDMServerSDK serverSDK = null;
        public Dictionary<long, Account> AccountsDic = new Dictionary<long, Account>();
        #endregion
        #region 构造函数
        public UserManager(string ip, string user, string pass, string database, int port)
        {
            this.serverSDK = new AMDMServerSDK(ip, user, pass, database, port);
            var accounts = serverSDK.DoAccountsGetRequest(new AccountsGetRequest(), null).Accounts;
            foreach (var a in accounts)
            {
                if (this.AccountsDic.ContainsKey(a.Id) == false)
                {
                    this.AccountsDic.Add(a.Id, a);
                }
            }
        }
        #endregion
        /// <summary>
        /// 使用用户名和密码登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public LoginResponse Login(string user, string pass)
        {
            string realPass = Utils.EncodePassword(pass);
            if (string.IsNullOrEmpty(realPass) == true)
            {
                LoginResponse errRsp = new LoginResponse();
                errRsp.ErrMsg = "无效的密码";
                errRsp.ErrCode = "403";
                return errRsp;
            }
            LoginRequest req = new LoginRequest();
            req.UserName = user;
            req.PasswordMD5 = Utils.GetMd5(pass).ToLower();

            LoginResponse rsp = serverSDK.DoLoginRequest(req, "client", null);
            return rsp;
        }

        /// <summary>
        /// 获取给定用户编号集合的用户的手机号码
        /// </summary>
        /// <param name="userIdList"></param>
        /// <returns></returns>
        public List<string> GetUsersMobile(List<int> userIdList)
        {
            if (userIdList == null || userIdList.Count <1)
            {
                return new List<string>();
            }
            List<string> mobiles = new List<string>();
            for (int i = 0; i < userIdList.Count; i++)
            {
                if (this.AccountsDic.ContainsKey(userIdList[i]) == true)
                {
                    mobiles.Add(this.AccountsDic[userIdList[i]].Mobile);
                }
            }
            return mobiles;
        }
    }
}
