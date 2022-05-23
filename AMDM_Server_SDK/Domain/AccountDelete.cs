using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Server_SDK
{
    public class AccountDeleteResponse: NetResponse
    {
        public bool Success { get; set; }
    }
    /// <summary>
    /// 删除账户请求,需要给定id
    /// </summary>
    public  class AccountDeleteRequest : BaseRequest<AccountDeleteResponse>
    {
        public Nullable<long> Id { get; set; }

        public override string GetApiName()
        {

            return "account.delete";
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
        }
    }
}
