using AMDM_Domain;
using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Server_SDK.Domain
{
    public class AccountsGetResponse : NetResponse
    {
        public List<Account> Accounts { get; set; }
    }

    /// <summary>
    /// 搜索账号,也可用于使用id+密码或用户名+密码的形式校验登陆情况.获取到的信息中,密码字段始终不会被获取到.
    /// 如希望校验密码的正确性只能通过密码的md5进行对比.
    /// </summary>
    public class AccountsGetRequest:BaseRequest<AccountsGetResponse>
    {
        public AccountsGetRequest()
        {
            Fields = "*";
        }
        public string Fields { get; set; }
        /// <summary>
        /// 获取指定的账户
        /// </summary>
        public Nullable<long> Id { get; set; }
        /// <summary>
        /// 根据账号名搜索,使用like方式
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 使用名称搜索
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 搜索指定科师的账户
        /// </summary>
        public string Department { get; set; }

        public override string GetApiName()
        {
            return "accounts.get";
        }
        public override bool GetIsSessionRequired()
        {
            return true;
        }

        public override IDictionary<string, string> GetParameters()
        {
            return base.GetParametersDicByPublicFieldAuto(this);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(Fields) == true)
            {
                throw new ArgumentNullException("必须指定要查询的字段信息");
            }
        }
    }
}
