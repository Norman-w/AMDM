using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;
using AMDM_Domain;

namespace AMDM_Server_SDK.Domain
{
    public class AccountUpdateResponse: NetResponse
    {
        public bool Success { get; set; }
        public Account UpdatedAccount { get; set; }
    }

    public class AccountUpdateRequest : BaseRequest<AccountUpdateResponse>
    {
        public Nullable<long> Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// 当需要修改密码的时候,如果不是超级管理员,需要提供旧密码
        /// </summary>
        public string OldPasswordMD5 { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public string Sex { get; set; }
        public Nullable<int> Age { get; set; }
        public string Mobile { get; set; }

        public override string GetApiName()
        {
            return "account.update";
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
            if (Id == null)
            {
                throw new ArgumentNullException("Id");
            }
            if (Name == null && UserName == null && Password == null && Department == null && Sex == null && Age == null)
            {
                throw new ArgumentException("至少需要更新一项内容");
            }
            List<string> sexList = new List<string>();
            sexList.Add("男");
            sexList.Add("女");
            sexList.Add("man");
            sexList.Add("women");
            sexList.Add("m");
            sexList.Add("w");
            sexList.Add("male");
            sexList.Add("famale");
            if (Sex!= null && string.IsNullOrEmpty(Sex) == false && sexList.Contains(Sex.ToLower()) == false)
            {
                throw new ArgumentException("无效的性别");
            }
            if (Age!= null && (Age > 100 || Age< 14))
            {
                throw new ArgumentException("无效的年龄信息");
            }
            if (string.IsNullOrEmpty(Password) == false && Id.Value!=1 && string.IsNullOrEmpty(OldPasswordMD5))
            {
                throw new ArgumentException("请使用原始密码做为凭据以修改新密码,或联系管理员帮助修改密码");
            }
            if (string.IsNullOrEmpty(Mobile) == false &&( Mobile.Trim().Length != 11 || Mobile.Trim().StartsWith("1") == false))
            {
                throw new ArgumentException("无效的手机号码");
            }
        }
    }
}
