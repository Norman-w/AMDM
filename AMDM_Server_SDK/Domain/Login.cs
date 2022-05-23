using AMDM_Domain;
using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Server_SDK.Domain
{
    public class LoginResponse : NetResponse
    {
        public Account Account { get; set; }
        public string Session { get; set; }
    }

    /// <summary>
    /// 搜索账号,也可用于使用id+密码或用户名+密码的形式校验登陆情况.获取到的信息中,密码字段始终不会被获取到.
    /// 如希望校验密码的正确性只能通过密码的md5进行对比.
    /// </summary>
    public class LoginRequest : BaseRequest<LoginResponse>
    {
        /// <summary>
        /// 获取指定的账户
        /// </summary>
        public Nullable<long> Id { get; set; }
        /// <summary>
        /// 根据账号名搜索,使用like方式
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 指定密码时,属于账号登陆校验,如果指定密码,必须要指定账号或者id
        /// </summary>
        public string PasswordMD5 { get; set; }

        public override string GetApiName()
        {
            return "login";
        }
        public override bool GetIsSessionRequired()
        {
            return false;
        }

        public override IDictionary<string, string> GetParameters()
        {
            return base.GetParametersDicByPublicFieldAuto(this);
        }

        public override void Validate()
        {
            bool hasPass = (string.IsNullOrEmpty(PasswordMD5) == false && PasswordMD5.Trim().Length > 0);
            if (hasPass && string.IsNullOrEmpty(UserName) && Id == null)
            {
                throw new ArgumentNullException("需要校验密码时,必须指定ID或UserName");
            }
        }
    }
}
